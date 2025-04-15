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
            jobs = DataLoader.LoadData<List<JobData>>(DataLoader.job);
            itemData = DataLoader.LoadData<List<ItemData>>(DataLoader.item);
            sceneDatas = DataLoader.LoadData<SceneTextData>(DataLoader.sceneText);

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
