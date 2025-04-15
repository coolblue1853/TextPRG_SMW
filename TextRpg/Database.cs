using System;
using System.Collections.Generic;
using System.Text;

namespace TextRpg
{
     class Database
    {
        //DB
        public static List<JobData> jobs; // 직업 관련 
        public static List<ItemData> itemData; // 아이템 관련
        public static SceneTextData sceneDatas;
        public static List<Item> items = new List<Item>(); // 아이템 관련
        public static void SetData()
        {
            jobs = DataLoader.LoadJobs();
            itemData = DataLoader.LoadItems();
            sceneDatas = DataLoader.LoadSceneText();

            foreach (var value in itemData)
            {
                Item item = Item.Create(value.Id);
                if (item != null)
                    items.Add(item);
            }
            Shop.Init();
        }
    }
}
