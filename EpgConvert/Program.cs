using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Diagnostics;
using System.Data;


namespace EpgConvert
{

    class epgDataFrame
    {

        private string _url;
        private DateTime _start;
        private DateTime _stop;
        private string _title;
        private string _desc;
        private string _genre;
        public string genre
        {
            get { return this.getCategory(); }
            set { _genre = value; }
        }

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

        private string getCategory()
        {
            DataTable matchingTable = new DataTable();
            matchingTable.Columns.Add("xmltvCatagory", typeof(string));
            matchingTable.Columns.Add("epgCatagory", typeof(string));

            matchingTable.Rows.Add("Crime", "11");
            matchingTable.Rows.Add("Adventure", "12");
            matchingTable.Rows.Add("SciFi", "13");
            matchingTable.Rows.Add("Comedy", "14");
            matchingTable.Rows.Add("Soap", "15");
            matchingTable.Rows.Add("Romance", "16");
            matchingTable.Rows.Add("Drama", "17");
            matchingTable.Rows.Add("Adult", "18");
            matchingTable.Rows.Add("News", "20");
            matchingTable.Rows.Add("Wetter", "21");
            matchingTable.Rows.Add("Newsmagazine", "22");
            matchingTable.Rows.Add("Documentary", "23");
            matchingTable.Rows.Add("Interview", "24");
            matchingTable.Rows.Add("Game show", "30");
            matchingTable.Rows.Add("Game show", "31");
            matchingTable.Rows.Add("Game show", "32");
            matchingTable.Rows.Add("Talk", "33");
            matchingTable.Rows.Add("Sport", "40");
            matchingTable.Rows.Add("Sports event", "41");
            matchingTable.Rows.Add("Sports talk", "42");
            matchingTable.Rows.Add("Sports event", "43");
            matchingTable.Rows.Add("Sports event", "44");
            matchingTable.Rows.Add("Sports event", "45");
            matchingTable.Rows.Add("Sport", "46");
            matchingTable.Rows.Add("Sport", "47");
            matchingTable.Rows.Add("Sport", "48");
            matchingTable.Rows.Add("Sport", "49");
            matchingTable.Rows.Add("Sport", "4A");
            matchingTable.Rows.Add("Sport", "4B");
            matchingTable.Rows.Add("Childrens", "51");
            matchingTable.Rows.Add("Childrens", "52");
            matchingTable.Rows.Add("Childrens", "53");
            matchingTable.Rows.Add("Childrens", "54");
            matchingTable.Rows.Add("Childrens", "55");
            matchingTable.Rows.Add("music/ballet/dance (general)", "60");
            matchingTable.Rows.Add("rock/pop", "61");
            matchingTable.Rows.Add("serious music/classical music", "62");
            matchingTable.Rows.Add("folk/traditional music", "63");
            matchingTable.Rows.Add("jazz", "64");
            matchingTable.Rows.Add("musical/opera", "65");
            matchingTable.Rows.Add("ballet", "66");
            matchingTable.Rows.Add("arts/culture (without music,general)", "70");
            matchingTable.Rows.Add("performing arts ", "71");
            matchingTable.Rows.Add("fine arts", "72");
            matchingTable.Rows.Add("religion ", "73");
            matchingTable.Rows.Add("popular culture/traditional arts", "74");
            matchingTable.Rows.Add("literature", "75");
            matchingTable.Rows.Add("film/cinema", "76");
            matchingTable.Rows.Add("experimental film/video", "77");
            matchingTable.Rows.Add("broadcasting/press", "78");
            matchingTable.Rows.Add("social/political issues/economics (general)", "80");
            matchingTable.Rows.Add("documentary", "81");
            matchingTable.Rows.Add("economics/social advisory", "82");
            matchingTable.Rows.Add("remarkable people ", "83");
            matchingTable.Rows.Add("education/science/factual topics (general)", "90");
            matchingTable.Rows.Add("nature/animals/environment", "91");
            matchingTable.Rows.Add("technology/natural sciences", "92");
            matchingTable.Rows.Add("medicine/physiology/psychology", "93");
            matchingTable.Rows.Add("foreign countries/expeditions", "94");
            matchingTable.Rows.Add("social/spiritual sciences ", "95");
            matchingTable.Rows.Add("further education ", "96");
            matchingTable.Rows.Add("languages ", "97");
            matchingTable.Rows.Add("children", "50");

            string[] categories;
            if (this._genre != String.Empty && this._genre != null)
            {
                categories = Regex.Split(this._genre, " ");
            }
            else
            {
                return "";
            }
            DataRow[] erg;
            List<string> ret = new List<string>();

            foreach (var category in categories)
            {
                erg = matchingTable.Select("epgCatagory = '" + category + "'");
                if (erg.Length != 0)
                {

                    ret.Add(erg[0]["xmltvCatagory"] as string);
                }
                erg = null;
            }
            if (ret.Count != 0)
            {
                return ret.First();
            }
            else
                return " ";
            
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            //try
            {
                string pathtoepg = "";
                string pathtoxmltv = "";
                string vdrUNDPort = "";
                if (args.Length != 3)
                {
                    Console.WriteLine(@".exe path\epg.data path\xmltv.xml ip:port (ip:port wie in channels.m3u");
                    return;
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
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.InnerException);
            //}
            //finally
            //{
            //    Console.Read();
            //}
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

                            string epgqueryresult = oneEpgEventData.FirstOrDefault<string>(b => b.StartsWith("D "));
                            if (epgqueryresult != null)
                            {
                                newframe.desc = epgqueryresult.Substring(2, epgqueryresult.Length - 2);

                            }

                            epgqueryresult = oneEpgEventData.FirstOrDefault<string>(b => b.StartsWith("G "));
                            if (epgqueryresult != null)
                            {

                                newframe.genre = epgqueryresult.Substring(2, epgqueryresult.Length - 2);
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
            //try
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


                    myRoot.AppendChild(createProgram(doc, _tmpurl, frame.start.ToString("yyyyMMddHHmmss") + " +0100", frame.stop.ToString("yyyyMMddHHmmss") + " +0100", frame.title.ToString(), frame.desc, frame.genre));

                }
                //TODO: möglicher fehler beim Speichern falls Pfad nicht vorhanden oder keine Berechtigung
                doc.Save(pathxmltv);

            }

            //catch (Exception e)
            //{
            //    throw new System.Exception("CreateXMLTV", e);
            //}

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

        static private XmlNode createProgram(XmlDocument doc, string url, string start, string stop, string title, string desc, string genre)
        {
            //try
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
                if (genre != " ")
                {
                    mySubNode = doc.CreateElement("category");
                    mySubNode.InnerText = genre;
                    myNode.AppendChild(mySubNode);
                }
             
                return myNode;
            }
            //catch (Exception e)
            //{
            //    throw new System.Exception("createProgram", e);
            //}
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
