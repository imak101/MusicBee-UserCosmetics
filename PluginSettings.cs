using System;
using System.IO;
using System.Xml;
using MusicBeePlugin.Form.Popup;

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
            string[] keys = {"pfpPath", "username", "roundPfpCheck"};
            
            File.Create(_filePath).Close();
            var writer = File.AppendText(_filePath); writer.Write("<body></body>"); writer.Close();
            
            _xmlDoc.Load(_filePath);
            XmlDeclaration dec = _xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
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
        
        /// <returns>
        /// Null if key was not found.
        /// </returns>
        public string GetFromKey(string key)
        {
            XmlNodeList nodeList = _xmlDoc.DocumentElement.GetElementsByTagName(key); //TODO: could be made more dynamic ...?

            if (nodeList.Count > 0)
            {
                return nodeList[0].InnerText;
            }

            return null;
        }
        
        public void SetFromKey(string key, string value, bool safety = false) // safety indicates that the key may not exist already (due to versioning) and will have to be appended 
        {
            XmlNodeList nodeList = _xmlDoc.DocumentElement.GetElementsByTagName(key);

            if (nodeList.Count > 0)
            {
                nodeList[0].InnerText = value;
                _xmlDoc.Save(_filePath);
                return;
            }

            if (safety)
            {
                XmlElement newKey = _xmlDoc.CreateElement(key);
                XmlText emptyS = _xmlDoc.CreateTextNode(string.Empty);
                newKey.AppendChild(emptyS);
                _xmlDoc.DocumentElement.AppendChild(newKey);
                
                SetFromKey(key,value);
                return;
            }
            
            new Form_Popup($"Key '{key}' not found. This program's version may be outdated, please make sure that you are up to date with the GitHub repository.", "SetFromKey Error");
        }


        public bool Exists()
        {
            return File.Exists(_filePath);
        }
    }
}