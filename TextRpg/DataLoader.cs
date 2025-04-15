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
        public static List<JobData> LoadJobs()
        {
            string json = File.ReadAllText(job);
            return JsonConvert.DeserializeObject<List<JobData>>(json);
        }
        public static List<ItemData> LoadItems()
        {
            string json = File.ReadAllText(item);
            return JsonConvert.DeserializeObject<List<ItemData>>(json);
        }
    }
}
