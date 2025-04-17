using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
namespace TextRpg
{

    internal class DataLoader
    {
        public  T LoadData<T>(string path)
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }
    
        public  string FormatText(string template, Dictionary<string, string> dict)
        {
            foreach (var pair in dict)
            {
                template = template.Replace("{" + pair.Key + "}", pair.Value);
            }
            return template;
        }
    }
}