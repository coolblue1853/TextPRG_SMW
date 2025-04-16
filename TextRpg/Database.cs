using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using TextRpg.Data;

namespace TextRpg
{
     class Database
    {
        //DB
        public static List<JobData> jobs; // 직업 관련 
        public static List<ItemData> itemData; // 아이템 데이터 관련
        public static List<Item> items = new List<Item>(); // 아이템 관련
        public static List<DungeonData> dungeonData; // 던전관련
        public static List<Dictionary<string,string>> dungeonFormat; // 던전포맷팅 관련
        public static SceneTextData sceneDatas;

        public static void SetData()
        {
            jobs = DataLoader.LoadData<List<JobData>>(DataLoader.job);
            itemData = DataLoader.LoadData<List<ItemData>>(DataLoader.item);
            sceneDatas = DataLoader.LoadData<SceneTextData>(DataLoader.scene);
            dungeonData = DataLoader.LoadData<List<DungeonData>>(DataLoader.dungeon);
            dungeonFormat = new List<Dictionary<string, string>>();
            foreach (var value in dungeonData)
            {
               
                dungeonFormat.Add( new Dictionary<string, string>()
                {
                     {"idx", value.Idx.ToString()},
                    {"name", value.Name},
                    {"defense", value.Defense.ToString()}
                });
            }



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
