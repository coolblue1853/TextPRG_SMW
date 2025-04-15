using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
namespace TextRpg
{

    internal class DataLoader
    {
        public static string job = "../../../Data/jobs.json";
        public static List<Job> LoadJobs(string path)
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<Job>>(json);
        }
    }
}
