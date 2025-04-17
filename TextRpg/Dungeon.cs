using System;
using System.Collections.Generic;
using System.Text;
using TextRpg.Data;

namespace TextRpg
{
    class DungeonResultHandler : IGameStateHandler
    {
        bool isShowError = false;
        public void Handle(GameLoop context)
        {
            Player myPlayer = context.myPlayer;
            DataLoader dataLoader = new DataLoader();
            if (context.dungeonResultData.IsSuccess)
            {
                myPlayer.ChangeExp(1);
                Utils.UpdateStringBuilder(dataLoader.FormatText(context.database.sceneDatas.Dungeon.reuslt_succ, context.dungeonResultData.ResultDict), false, true);
                Utils.UpdateStringBuilder(context.database.sceneDatas.ETC.base_etc, !isShowError);
            }
            else
            {
                Utils.UpdateStringBuilder(dataLoader.FormatText(context.database.sceneDatas.Dungeon.reuslt_fail, context.dungeonResultData.ResultDict), false, true);
                Utils.UpdateStringBuilder(context.database.sceneDatas.ETC.base_etc, !isShowError);
            }

            if (isShowError)
            {
                Utils.UpdateStringBuilder(context.database.sceneDatas.Error.input_error, isShowError, false);
                isShowError = false;
            }

            Utils.ReadLine(out string action);
            if (int.TryParse(action, out int num)) // 예외처리
            {
                switch (num)
                {
                    case 0:
                        context.ChangeState(GameState.Dungeon);
                        break;
                    default:
                        isShowError = true;
                        break;
                }
            }
            else
            {
                isShowError = true;
            }
        }
    }
    class ShowDungeonHandler : IGameStateHandler
    {
        bool isShowError = false;
        Dictionary<string, string> resultDict = new Dictionary<string, string>();
        public void Handle(GameLoop context)
        {
            Dungeon dungeon = new Dungeon();
            Player myPlayer = context.myPlayer;
            DataLoader dataLoader = new DataLoader();
            Utils.ClearStringBuilder();
            foreach (var value in context.database.dungeonFormat)
            {
                Utils.UpdateStringBuilder(dataLoader.FormatText(context.database.sceneDatas.Dungeon.banner, value), false, false);
            }
            Utils.UpdateStringBuilder(context.database.sceneDatas.ETC.base_etc, !isShowError);

            if (isShowError)
            {
                Utils.UpdateStringBuilder(context.database.sceneDatas.Error.input_error, isShowError, false);
                isShowError = false;
            }

            Utils.ReadLine(out string action);

            if (int.TryParse(action, out int num)) // 예외처리
            {
                if (num == 0)
                {
                    context.ChangeState(GameState.Town);
                }
                else if (num <= dungeon.GetDungeonCount())
                {
                    var result = dungeon.IntoDungoen(myPlayer, num);
                    (bool isSuccess, int reduceHp, int resultGold, string name) = result;
                   bool isDungeonSuccess = isSuccess;

                    resultDict = new Dictionary<string, string>()
                        {
                            { "name" ,name },
                            { "beforHp", myPlayer._hp.ToString() },
                            { "afterHp", (myPlayer._hp - reduceHp).ToString() },
                            { "beforeGold", myPlayer._gold.ToString() },
                            { "afterGold", (myPlayer._gold + resultGold).ToString()},
                        };

                    context.dungeonResultData.IsSuccess = isDungeonSuccess;
                    context.dungeonResultData.ResultDict = resultDict;

                    myPlayer.ChangeHp(-reduceHp);
                    myPlayer.ChangeGold(resultGold);

                    context.ChangeState(GameState.DungeonResult);
                }
            }
            else
            {
                isShowError = true;
            }
        }
    }
    public class DungeonResultData
    {
        public bool IsSuccess;
        public Dictionary<string, string> ResultDict;
    }
    internal class Dungeon
    {

        // 던전 리스트, (난이도, 해당 방어력 수치로 구성)
        Random random = new Random();


        public int GetDungeonCount()
        {
            return GameManager.gameLoop.database.dungeonData.Count;
        }

        public (bool, int, int, string name) IntoDungoen(Player player, int dungeonNum)
        {
            var nowDungeon = GameManager.gameLoop.database.dungeonData[dungeonNum - 1];
            GetPlayerData(player, out float playerAttack, out float playerDefense);

            // 방어력으로 결과 결정
            if (playerDefense < nowDungeon.Defense)
            {
                if (random.NextDouble() > nowDungeon.DefenseProbability) // 던전 성공
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


        public (bool, int, int, string name) SuccessDungeon(Player player, DungeonData dungeonData)
        {
            GetPlayerData(player, out float playerAttack, out float playerDefense);
            int defenseGap = (int)playerDefense - dungeonData.Defense;
            int reduceHp = Math.Max(0, random.Next(dungeonData.MinUseHp - defenseGap, dungeonData.MaxUseHp - defenseGap));
            float goldRatio = 1 + random.Next((int)playerAttack, (int)(playerAttack * dungeonData.GoldRatio + 1)) / 100.0f;
            int resultGold = (int)(dungeonData.Gold * goldRatio);
            return (true, reduceHp, resultGold, dungeonData.Name);
        }
        public (bool, int, int, string name) FailDungeon(Player player, DungeonData dungeonData)
        {
            int reduceHp = player._hp / dungeonData.FailHealthDivied;
            return (false, reduceHp, 0, dungeonData.Name);
        }
        public void GetPlayerData(Player player, out float playerAttack, out float playerDefense)
        {
            playerAttack = player._attack.GetFinalValue().Value;
            playerDefense = player._defense.GetFinalValue().Value;
        }

    }
}
