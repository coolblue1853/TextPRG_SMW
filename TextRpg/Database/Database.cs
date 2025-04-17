using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using TextRpg.Data;

namespace TextRpg
{
     class Database
    {
        DataLoader dataLoader;
        //DB
        public  List<JobData> jobs; // 직업 관련 
        public  List<ItemData> itemData; // 아이템 데이터 관련
        public  List<Item> items = new List<Item>(); // 아이템 관련
        public  List<DungeonData> dungeonData; // 던전관련
        public  List<Dictionary<string,string>> dungeonFormat; // 던전포맷팅 관련
        public  SceneTextData sceneDatas;

        //경로
         string job = "../../../Json/jobs.json";
         string item = "../../../Json/items.json";
         string scene = "../../../Json/sceneText.json";
         string dungeon = "../../../Json/dungeons.json";

        public void SetData()
        {
            dataLoader= new DataLoader();
            jobs = dataLoader.LoadData<List<JobData>>(job);
            itemData = dataLoader.LoadData<List<ItemData>>(item);
            sceneDatas = dataLoader.LoadData<SceneTextData>(scene);
            dungeonData = dataLoader.LoadData<List<DungeonData>>(dungeon);
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
            GameManager.gameLoop.shop.Init();
        }
    }
}
