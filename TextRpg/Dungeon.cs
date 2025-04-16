using System;
using System.Collections.Generic;
using System.Text;
using TextRpg.Data;

namespace TextRpg
{
    internal class Dungeon
    {

        // 던전 리스트, (난이도, 해당 방어력 수치로 구성)
        static Random random = new Random();


        public static int GetDungeonCount()
        {
            return Database.dungeonData.Count;
        }

        public static (bool, int, int, string name) IntoDungoen(Player player, int dungeonNum)
        {
            var nowDungeon = Database.dungeonData[dungeonNum-1];
            GetPlayerData(player, out float playerAttack, out float playerDefense);

            // 방어력으로 결과 결정
            if (playerDefense < nowDungeon.Defense)
            {
                if(random.NextDouble() > nowDungeon.DefenseProbability) // 던전 성공
                {
                    return SuccessDungeon(player, nowDungeon);
                }
                else // 던전실패
                {
                    return FailDungeon(player, nowDungeon);
                }
            }
            else
            {
                //던전 성공
                return SuccessDungeon(player, nowDungeon);
            }
        }


        public static (bool, int, int, string name) SuccessDungeon(Player player,DungeonData dungeonData)
        {
            GetPlayerData(player, out float playerAttack, out float playerDefense);
            int defenseGap = (int)playerDefense - dungeonData.Defense;
            int reduceHp = Math.Max(0,random.Next(dungeonData.MinUseHp - defenseGap, dungeonData.MaxUseHp - defenseGap));
            float goldRatio = 1 + random.Next((int)playerAttack, (int)(playerAttack * dungeonData.GoldRatio + 1))/ 100.0f;
            int resultGold = (int)(dungeonData.Gold * goldRatio);
            return (true, reduceHp, resultGold, dungeonData.Name);
        }
        public static (bool, int,int, string name) FailDungeon(Player player, DungeonData dungeonData)
        {
            int reduceHp = player._hp / dungeonData.FailHealthDivied;
            return (false, reduceHp,0, dungeonData.Name);
        }
        public static void GetPlayerData(Player player, out float playerAttack, out float playerDefense)
        {
            playerAttack = player._attack.GetFinalValue().Value; 
            playerDefense = player._defense.GetFinalValue().Value;
        }

    }
}
