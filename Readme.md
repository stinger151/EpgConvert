Quick and Dirty epg.data to xmltv.xml converter for streamdev Streams (streamurl = xmltvid)


Usage Example:
        EpgConvert.exe /video/epg.data /path/where/you/want/it/xmltv.xml localhost:3000
        
        mono /root/EpgConvert.exe  /video/epg.data /video/xmltv.xml 192.168.99.1:3000
        mythfilldatabase --file --sourceid 0 --xmlfile /video/xmltv.xml

