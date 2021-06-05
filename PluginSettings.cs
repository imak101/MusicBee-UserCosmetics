using System;
using System.IO;
using System.Xml;

namespace MusicBeePlugin
{
    public class PluginSettings
    {
        private XmlDocument _xmlDoc = new XmlDocument();
        private Plugin.MusicBeeApiInterface _mbAPI;
        private string _fileName = @"mb_Something1.xml"; // TODO: make dynamic
        private string _filePath;
        
        public PluginSettings(ref Plugin.MusicBeeApiInterface mbAPI)
        {
            _mbAPI = mbAPI;
            _filePath = _mbAPI.Setting_GetPersistentStoragePath.Invoke() + _fileName;
            
            if (!Exists()) FirstTimeSetup();
            else _xmlDoc.Load(_filePath);
            
        }

        private void FirstTimeSetup()
        {
            string[] keys = {"pfpPath", "username"};
            
            File.Create(_filePath).Close();
            var writer = File.AppendText(_filePath); writer.Write("<body></body>"); writer.Close();
            
            _xmlDoc.Load(_filePath);
            var dec = _xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
            _xmlDoc.InsertBefore(dec, _xmlDoc.DocumentElement);

            for (int i = 0; i < keys.Length; i++)
            {
                XmlElement key = _xmlDoc.CreateElement(keys[i]);
                XmlText d = _xmlDoc.CreateTextNode(string.Empty);
                key.AppendChild(d);
                _xmlDoc.DocumentElement.AppendChild(key);
            }
            
            _xmlDoc.Save(_filePath);
        }

        public string GetFromKey(string key)
        {
            XmlNodeList nodeList = _xmlDoc.DocumentElement.GetElementsByTagName(key); //TODO: could be made more dynamic

            if (nodeList.Count > 0)
            {
                return nodeList[0].ToString();
            }

            return null;
        }


        public bool Exists()
        {
            return File.Exists(_filePath);
        }
        
        
    }
}