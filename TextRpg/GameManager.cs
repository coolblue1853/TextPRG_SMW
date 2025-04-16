using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TextRpg
{
    internal class GameManager
    {
        public static Player myPlayer;
        static GameState state = GameState.None;
        static int waitTime = 100; // 대기시간
        static bool isShowError = false;
        static string shopText = "";
        //던전관련
        static   Dictionary<string, string> resultDict = new Dictionary<string, string>();
        static bool isDungeonSuccess = false;
        // 쉼터 관련
        static bool? isRestore = null;

        static void Main(string[] args)
        {
            Init();
            GameLoop();
        }
        static void GameLoop()
        {
            while (true) // 게임 루프
            {
                switch (state)
                {
                    case GameState.SetChar:
                        SetCharacter();
                        break;
                    case GameState.Town:
                        Town();
                        break;
                    case GameState.CheckStat:
                        CheckStat();
                        break;
                    case GameState.Inventory:
                        ShowInventory();
                        break;
                    case GameState.Equip:
                        ShowInventory(true);
                        break;
                    case GameState.Shop:
                        ShowShop();
                        break;
                    case GameState.Buy:
                        ShowShop(true);
                        break;
                    case GameState.Sell:
                        SellItem();
                        break;
                    case GameState.Dungeon:
                        GoDungeon();
                        break;
                    case GameState.DungeonResult:
                        ResultDungeon();
                        break;
                    case GameState.Restore:
                        GoRestore();
                        break;
                    default:
                        Thread.Sleep(waitTime);
                        break;
                }
            }
        }

        static void Init()
        {
            Database.SetData();
            ChangeState(GameState.SetChar);
        }

        static void SetCharacter() 
        {
            Utils.UpdateStringBuilder(Database.sceneDatas.Intro.intro_welcome, true, true);
            Utils.ReadLine(out string nickName);

  
            while (true)
            {
                string input = GetJobInput(isShowError);
                isShowError = false;
                if (int.TryParse(input, out int num))   // 예외처리 받아야함 tryCatch 로 잡아보면 좋을듯?
                {
                    switch (num)
                    {
                        case 1:
                            myPlayer = new Warrior();
                            myPlayer.SetInfo(nickName, Database.jobs[Utils.GetEnumIndex(PlayerType.Warrior)]);
                            ChangeState(GameState.Town);
                            return;
                        // TODO: 다른 직업도 추가
                        default:
                            isShowError = true;
                            break;
                    }
                }
                else
                    isShowError = true;
            }
        }
        static string GetJobInput(bool showError)
        {
            if (showError)
            {
                Utils.UpdateStringBuilder(Database.sceneDatas.Intro.choose_job,false, true);
                Utils.UpdateStringBuilder(Database.sceneDatas.Error.input_error, true);
            }
            else
            {
                Utils.UpdateStringBuilder(Database.sceneDatas.Intro.choose_job, true, true);
            }

            Utils.ReadLine(out string input);
            return input;
        }
        static void Town()
        {
            Utils.UpdateStringBuilder(Database.sceneDatas.Town.town_welcome);
            Utils.UpdateStringBuilder(Database.sceneDatas.Town.town_select, !isShowError, true);
            if (isShowError)
            {
                Utils.UpdateStringBuilder(Database.sceneDatas.Error.input_error, isShowError, false);
            }
            isShowError = false;

            Utils.ReadLine(out string action);
            if (int.TryParse(action, out int num)) // 예외처리
            {
                Utils.ClearStringBuilder();
                switch (num)
                {
                    case 1:
                        ChangeState(GameState.CheckStat);
                        break;
                    case 2:
                        ChangeState(GameState.Inventory);
                        break;
                    case 3:
                        ChangeState(GameState.Shop);
                        break;
                    case 4:
                        ChangeState(GameState.Dungeon);
                        break;
                    case 5:
                        ChangeState(GameState.Restore);
                        break;
                    default:
                        isShowError = true;
                        break;
                }
            }
            else
                isShowError = true;
        }
        static void CheckStat()
        {
            Utils.UpdateStringBuilder(DataLoader.FormatText(Database.sceneDatas.Stat.level_name, myPlayer.GetFormattedStats()),false,true);
            Utils.UpdateStringBuilder(DataLoader.FormatText(Database.sceneDatas.Stat.attack, myPlayer.GetFormattedStats()));
            CheckAdditionalStat(AdditionalStat.ATK);
            Utils.UpdateStringBuilder(DataLoader.FormatText(Database.sceneDatas.Stat.defense, myPlayer.GetFormattedStats()));
            CheckAdditionalStat(AdditionalStat.DEF);
            Utils.UpdateStringBuilder(DataLoader.FormatText(Database.sceneDatas.Stat.etc, myPlayer.GetFormattedStats()),!isShowError);

            if (isShowError)
            {
                Utils.UpdateStringBuilder(Database.sceneDatas.Error.input_error, isShowError, false);
            }

            isShowError = false;

            Utils.ReadLine(out string action);
            if (int.TryParse(action, out int num)) // 예외처리
            {
                Utils.ClearStringBuilder();
                switch (num)
                {
                    case 0:
                        ChangeState(GameState.Town);
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
        static void CheckAdditionalStat(AdditionalStat additionalStat)
        {
            if (myPlayer.GetAdditionalStat().TryGetValue(additionalStat, out int value))
            {
                if (value > 0)
                    Utils.UpdateStringBuilder($" (+ {value})\n");
                else
                    Utils.UpdateStringBuilder($" (- {Math.Abs(value)})\n");
            }
            else
                Utils.UpdateStringBuilder($"\n");
        }
        static void ShowInventory(bool isEqiup = false)
        {
            Utils.UpdateStringBuilder(Database.sceneDatas.Inventory.banner,false,true);
            Inventory.AddInventoryStringBuiler(isEqiup);

            if (!isEqiup)
            {
                Utils.UpdateStringBuilder(Database.sceneDatas.Inventory.equip_mode);
            }
  
            Utils.UpdateStringBuilder(Database.sceneDatas.ETC.base_etc,!isShowError);

            if (isShowError)
            {
                Utils.UpdateStringBuilder(Database.sceneDatas.Error.input_error, isShowError, false);
                isShowError = false;
            }

            Utils.ReadLine(out string action);
            if (int.TryParse(action, out int num)) // 예외처리
            {
                Utils.ClearStringBuilder();
                if (!isEqiup)
                {
                    switch (num)
                    {
                        case 0:
                            ChangeState(GameState.Town);
                            break;
                        case 1:
                            ChangeState(GameState.Equip);
                            break;
                        default:
                            isShowError = true;
                            break;
                    }
                }
                else
                {
                    if (num == 0)
                    {
                        ChangeState(GameState.Inventory);
                    }
                    else if (num <= Inventory.GetInventorySize())
                    {
                        Inventory.EquipItem(num);
                    }
                    else
                    {
                        isShowError = true;
                    }
      
                }
            }
            else
            {
                isShowError = true;
            }
        }
        public static void ShowShop(bool isBuy = false)
        {
            Utils.UpdateStringBuilder(DataLoader.FormatText(Database.sceneDatas.Shop.banner, myPlayer.GetFormattedStats()), false, true);
            Shop.AddShopStringBuiler(isBuy);
            if (isBuy == false)
            {
                Utils.UpdateStringBuilder(Database.sceneDatas.Shop.buy);
            }

            if (shopText != "")
            {
                Utils.UpdateStringBuilder(Database.sceneDatas.ETC.base_etc);
                Utils.UpdateStringBuilder(shopText, true, false);
                shopText = "";
            }
            else
            {
                Utils.UpdateStringBuilder(Database.sceneDatas.ETC.base_etc, !isShowError);
            }
        
            if (isShowError)
            {
                Utils.UpdateStringBuilder(Database.sceneDatas.Error.input_error, isShowError, false);
                isShowError = false;
            }
            
            Utils.ReadLine(out string action);
            if (int.TryParse(action, out int num)) // 예외처리
            {
                Utils.ClearStringBuilder();
                if (!isBuy)
                {

                    switch (num)
                    {
                        case 0:
                            ChangeState(GameState.Town);
                            break;
                        case 1:
                            ChangeState(GameState.Buy);
                            break;
                        case 2:
                            ChangeState(GameState.Sell);
                            break;
                        default:
                            isShowError = true;
                            break;
                    }
                }
                else
                {
                    if (num == 0)
                    {
                        ChangeState(GameState.Shop);
                    }
                    else if (num <= Shop.GetShopSize())
                    {
                        shopText = Shop.BuyItem(num);
                    }
                    else
                    {
                        isShowError = true;
                    }
                }
            }
            else
            {
                isShowError = true;
            }
        }
        public static void SellItem()
        {
            Utils.UpdateStringBuilder(DataLoader.FormatText(Database.sceneDatas.Shop.banner, myPlayer.GetFormattedStats()), false, true);
            Inventory.AddInventoryStringBuiler(true);
            Utils.UpdateStringBuilder(Database.sceneDatas.ETC.base_etc, !isShowError);

            if (isShowError)
            {
                Utils.UpdateStringBuilder(Database.sceneDatas.Error.input_error, isShowError, false);
                isShowError = false;
            }

            Utils.ReadLine(out string action);
            if (int.TryParse(action, out int num)) // 예외처리
            {

                if (num == 0)
                {
                    ChangeState(GameState.Shop);
                }
                else if (num <= Inventory.GetInventorySize())
                {
                    Shop.SellItem(Inventory.GetItem(num));
                }
                else
                {
                    isShowError = true;
                }
            }
            else
            {
                isShowError = true;
            }
        }

        public static void GoDungeon()
        {

            Utils.ClearStringBuilder();
            foreach(var value in Database.dungeonFormat)
            {
                Utils.UpdateStringBuilder(DataLoader.FormatText(Database.sceneDatas.Dungeon.banner, value), false, false);
            }
            Utils.UpdateStringBuilder(Database.sceneDatas.ETC.base_etc, !isShowError); 

            if(isShowError)
            {
                Utils.UpdateStringBuilder(Database.sceneDatas.Error.input_error, isShowError, false);
                isShowError = false;
            }
            
            Utils.ReadLine(out string action);
            
            if (int.TryParse(action, out int num)) // 예외처리
            {
                if (num == 0)
                {
                    ChangeState(GameState.Town);
                }
                else if (num <= Dungeon.GetDungeonCount())
                {
                    var result = Dungeon.IntoDungoen(myPlayer, num);
                    (bool isSuccess, int reduceHp, int resultGold, string name) = result;
                    isDungeonSuccess = isSuccess;

                    resultDict = new Dictionary<string, string>()
                        {
                            { "name" ,name },
                            { "beforHp", myPlayer._hp.ToString() },
                            { "afterHp", (myPlayer._hp - reduceHp).ToString() },
                            { "beforeGold", myPlayer._gold.ToString() },
                            { "afterGold", (myPlayer._gold + resultGold).ToString()},
                        };

                    myPlayer.ChangeHp(-reduceHp);
                    myPlayer.ChangeGold(resultGold);

                    ChangeState(GameState.DungeonResult);
                }
            }
            else
            {
                isShowError = true;
            }
        }
        static void ResultDungeon()
        {
            if (isDungeonSuccess)
            {
                myPlayer.ChangeExp(1);
                Utils.UpdateStringBuilder(DataLoader.FormatText(Database.sceneDatas.Dungeon.reuslt_succ, resultDict), false, true);
                Utils.UpdateStringBuilder(Database.sceneDatas.ETC.base_etc, !isShowError);
            }
            else
            {
                Utils.UpdateStringBuilder(DataLoader.FormatText(Database.sceneDatas.Dungeon.reuslt_fail, resultDict), false, true);
                Utils.UpdateStringBuilder(Database.sceneDatas.ETC.base_etc, !isShowError);
            }

            if (isShowError)
            {
                Utils.UpdateStringBuilder(Database.sceneDatas.Error.input_error, isShowError, false);
                isShowError = false;
            }

            Utils.ReadLine(out string action);
            if (int.TryParse(action, out int num)) // 예외처리
            {
                switch (num)
                {
                    case 0:
                        ChangeState(GameState.Dungeon);
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
        static void GoRestore()
        {
            Utils.UpdateStringBuilder(DataLoader.FormatText(Database.sceneDatas.Restore.banner, myPlayer.GetFormattedStats()), false, true);
            Utils.UpdateStringBuilder(Database.sceneDatas.ETC.base_etc, (!isShowError&& isRestore == null));
            if (isRestore != null)
            {
                if (isRestore == true)
                {
                    Utils.UpdateStringBuilder(Database.sceneDatas.Restore.succ, !isShowError);
                }
                else
                {
                    Utils.UpdateStringBuilder(Database.sceneDatas.Restore.fail, !isShowError);
                }
                isRestore = null;
            }

            if (isShowError)
            {
                Utils.UpdateStringBuilder(Database.sceneDatas.Error.input_error, isShowError, false);
            }

            isShowError = false;

            Utils.ReadLine(out string action);
            if (int.TryParse(action, out int num)) // 예외처리
            {
                switch (num)
                {
                    case 0:
                        ChangeState(GameState.Town);
                        break;
                    case 1:
                        isRestore = TryToRestore();
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
       static bool TryToRestore()
        {
            int nowGold = myPlayer._gold;
            if(nowGold >= 500)
            {
                myPlayer.ChangeGold(-500);
                myPlayer.SetHp(100);
                return true;
            }
            else
            {
                return false;
            }
        }
        static void ChangeState(GameState changeState)
        {
            state = changeState;
        }
    }
}
