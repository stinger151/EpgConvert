using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;


namespace EpgConvert
{

    class epgDataFrame
    {

        private string _url;
        private DateTime _start;
        private DateTime _stop;
        private string _title;
        private string _desc;
        public string URL
        {
            set { _url = value; }
            get { return _url; }
        }
        public string title
        {
            set { _title = value; }
            get { return _title; }
        }
        public string desc
        {
            set { _desc = value; }
            get { return _desc; }
        }

        public DateTime start
        {
            set { _start = value; }
            get { return _start; }
        }
        public DateTime stop
        {
            set { _stop = value; }
            get { return _stop; }
        }


    }
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string pathtoepg = "";
                string pathtoxmltv = "";
                string vdrUNDPort = "";
                if (args.Length != 3)
                {
                    Console.WriteLine(@".exe path\epg.data path\xmltv.xml ip:port (ip:port wie in channels.m3u");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
                else
                {

                    if (!File.Exists(args[0]))
                        throw new System.ArgumentException("File epg.data not found at location " + args[0]);
                    if (!Directory.Exists(Path.GetDirectoryName(args[1])))
                        throw new System.ArgumentException("Location " + args[1] + " does not exist.");

                    pathtoepg = args[0];
                    pathtoxmltv = args[1];
                    vdrUNDPort = args[2];

                }
                List<epgDataFrame> EPGDATA = ParseVDRepg(pathtoepg, vdrUNDPort);

                CreateXMLTV(EPGDATA, pathtoxmltv);
            }
            catch (Exception e)
            {

            }
        }

        public static List<epgDataFrame> ParseVDRepg(string pathepg, string vdrPort)
        {
            try
            {
                TextReader tr = new StreamReader(pathepg, Encoding.GetEncoding("iso-8859-1"));
                string epgData;
                // read a line of text
                epgData = tr.ReadToEnd();

                List<List<string>> EpgChannels = new List<List<string>>();

                epgData = epgData.Substring(1, epgData.Length - 1);

                string[] epgstrings = Regex.Split(epgData, @"\nC");

                foreach (string data in epgstrings)
                {
                    List<string> EpgEvents = new List<string>(Regex.Split(data, "\nE"));
                    EpgChannels.Add(EpgEvents);
                }

                // close the stream
                tr.Close();

                List<epgDataFrame> goodFrames = new List<epgDataFrame>();

                foreach (List<string> allEpgEvent in EpgChannels)
                {
                    String ChannelName = "";
                    foreach (string eventString in allEpgEvent)
                    {
                        if (eventString.StartsWith(" S"))
                        {
                            string Channelid = eventString.Substring(1, eventString.IndexOf(" ", 1) - 1);
                            ChannelName = @"http://" + vdrPort + "/" + Channelid;

                        }
                        else
                        {
                            List<string> oneEpgEventData = new List<string>(Regex.Split(eventString, "\n"));

                            DateTime[] startStopTime = parseEpgTime(oneEpgEventData[0]);
                            //Console.WriteLine("channel Name: " + ChannelName);
                            //Console.WriteLine("start Time " + startStopTime[0]);
                            //Console.WriteLine("stop Time " + startStopTime[1]);
                            //Console.WriteLine("Title " + oneEpgEventData[1].Substring(2, oneEpgEventData[1].Length - 2));

                            epgDataFrame newframe = new epgDataFrame();
                            newframe.URL = ChannelName;
                            newframe.start = startStopTime[0];
                            newframe.stop = startStopTime[1];
                            newframe.title = oneEpgEventData[1].Substring(2, oneEpgEventData[1].Length - 2);

                            if (oneEpgEventData[2].StartsWith("D "))
                            {
                                newframe.desc = oneEpgEventData[2].Substring(2, oneEpgEventData[2].Length - 2);

                            }

                            goodFrames.Add(newframe);

                        }

                    }
                }
                return goodFrames;
            }
            catch (Exception e)
            {
                throw new System.Exception("ParseVDRepg", e);
            }
        }

        public static void CreateXMLTV(List<epgDataFrame> goodFrames, string pathxmltv)
        {
            try
            {
                //------------vdr parse Ende----
                XmlDocument doc = new XmlDocument();

                XmlNode myRoot;
                XmlAttribute myAttribute;
                //-----Tv start---
                myRoot = doc.CreateElement("tv");

                doc.AppendChild(myRoot);

                myAttribute = doc.CreateAttribute("date");
                myAttribute.InnerText = " +0100";


                myRoot.Attributes.Append(myAttribute);

                myAttribute = doc.CreateAttribute("generator-info-name");
                myAttribute.InnerText = "ce";

                myRoot.Attributes.Append(myAttribute);
                // -----Tv Ende---


                string _tmpurl = "";
                foreach (epgDataFrame frame in goodFrames)
                {
                    if (_tmpurl != frame.URL)
                    {
                        _tmpurl = frame.URL;
                        myRoot.AppendChild(createchannel(doc, _tmpurl));
                    }
                    // string date12 = frame.start.ToString("yyyyMMddHHMMss")+" +0100";
                    //TODO: mögliche system argument exception abfangen
                    myRoot.AppendChild(createProgram(doc, _tmpurl, frame.start.ToString("yyyyMMddHHMMss") + " +0100", frame.stop.ToString("yyyyMMddHHMMss") + " +0100", frame.title.ToString(), frame.desc));

                }
                //TODO: möglicher fehler beim Speichern falls Pfad nicht vorhanden oder keine Berechtigung
                doc.Save(pathxmltv);

            }
            catch (Exception e)
            {
                throw new System.Exception("CreateXMLTV", e);
            }

        }

        private static DateTime[] parseEpgTime(string Time)
        {
            try
            {
                DateTime[] startStop = new DateTime[2];

                //mögliche konvertierungsfehler abfangen falls die Daten nicht stimmen
                //TODO: Index out of bounds exception
                string estring2 = Time.Substring(2, Time.Length - 3);
                int timepos = estring2.IndexOf(" ");
                int dauerpos = estring2.IndexOf(" ", timepos + 1);
                int tableid = estring2.IndexOf(" ", dauerpos + 1);
                string time = estring2.Substring(timepos + 1, dauerpos - timepos - 1);
                string dauer = estring2.Substring(dauerpos + 1, tableid - dauerpos - 1);

                double unixtime = Convert.ToDouble(time);
                startStop[0] = UnixTimeStampToDateTime(unixtime);
                double seconds = Convert.ToDouble(dauer);
                startStop[1] = startStop[0].AddSeconds(seconds);

                return startStop;
            }
            catch (Exception e)
            {

                throw new System.Exception("parseEpgTime", e);
            }


        }

        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            try
            {

                // Unix timestamp is seconds past epoch
                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

                dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
                return dtDateTime;
            }
            catch (Exception e)
            {

                throw new System.Exception("UnixTimeStampToDateTime", e);
            }
        }

        static private XmlNode createProgram(XmlDocument doc, string url, string start, string stop, string title, string desc)
        {
            try
            {
                XmlNode myNode;

                XmlNode mySubNode;
                XmlAttribute myAttribute;
                //--programme start---
                myNode = doc.CreateElement("programme");
                myAttribute = doc.CreateAttribute("channel");
                myAttribute.InnerText = url;
                myNode.Attributes.Append(myAttribute);

                myAttribute = doc.CreateAttribute("start");
                myAttribute.InnerText = start;
                myNode.Attributes.Append(myAttribute);

                myAttribute = doc.CreateAttribute("stop");
                myAttribute.InnerText = stop;
                myNode.Attributes.Append(myAttribute);


                //--title 
                mySubNode = doc.CreateElement("title");
                mySubNode.InnerText = title;
                myNode.AppendChild(mySubNode);
                //--title  ende
                //--desc 
                mySubNode = doc.CreateElement("desc");
                mySubNode.InnerText = desc;
                myNode.AppendChild(mySubNode);
                //--desc  ende
                return myNode;
            }
            catch (Exception e)
            {
                throw new System.Exception("createProgram", e);
            }
        }

        static private XmlNode createchannel(XmlDocument doc, string url)
        {
            try
            {
                //--channel start---

                XmlNode myNode;

                XmlNode mySubNode;
                XmlAttribute myAttribute;
                myNode = doc.CreateElement("channel");
                myAttribute = doc.CreateAttribute("id");
                myAttribute.InnerText = url;
                myNode.Attributes.Append(myAttribute);

                //--chanel display-name
                mySubNode = doc.CreateElement("display-name");
                mySubNode.InnerText = url;
                myNode.AppendChild(mySubNode);
                //--chanel display-name ende


                //myRoot.AppendChild(myNode);
                return myNode;
                //--channel ende---
                //--programme start---
            }
            catch (Exception e)
            {
                throw new System.Exception("createChannel Exception", e);
            }
        }
    }
}