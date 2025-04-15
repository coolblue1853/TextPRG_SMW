using System;
using System.Collections.Generic;

namespace TextRpg
{
    internal class GameManager
    {
        static void Main(string[] args)
        {
            List<Job> jobs = DataLoader.LoadJobs(DataLoader.job);

            foreach (var job in jobs)
            {
                Console.WriteLine($"[{job.Name}] HP: {job.MaxHP}, ATK: {job.Atk}, DEF: {job.Def}");
            }
        }
    }
}
