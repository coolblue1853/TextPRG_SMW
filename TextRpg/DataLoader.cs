using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
namespace TextRpg
{

    internal class DataLoader
    {
        public static string job = "../../../Json/jobs.json";
        public static string item = "../../../Json/items.json";
        public static string sceneText = "../../../Json/sceneText.json";
        public static T LoadData<T>(string path)
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }
    
        public static string FormatText(string template, Dictionary<string, string> dict)
        {
            foreach (var pair in dict)
            {
                template = template.Replace("{" + pair.Key + "}", pair.Value);
            }
            return template;
        }
    }
}