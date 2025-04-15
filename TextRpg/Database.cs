using System;
using System.Collections.Generic;
using System.Text;

namespace TextRpg
{
     class Database
    {
        //DB
        public static List<Job> jobs; // 직업 관련 

        public static void SetData()
        {
            jobs = DataLoader.LoadJobs(DataLoader.job);
        }

    }
}
