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
        static string action;
        static bool showErrorOnce = false;
        static void Main(string[] args)
        {
            Init();
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
            Utils.UpdateStringBuilder(Database.sceneDatas.Intro.welcome, true, true);
            Utils.ReadLine(out string nickName);

            bool isError = false;

            while (true)
            {
                string input = GetJobInput(isError);

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
                            isError = true;
                            break;
                    }
                }
                else
                {
                    isError = true;
                }
            }
        }


        static string GetJobInput(bool showError)
        {

            if (showError)
            {
                Utils.UpdateStringBuilder(Database.sceneDatas.Intro.choose_job, false, true);
                Utils.UpdateStringBuilder(Database.sceneDatas.Error.input_error, true);
            }
            else
                Utils.UpdateStringBuilder(Database.sceneDatas.Intro.choose_job, true, true);

            Utils.ReadLine(out string input);
            return input;
        }

        static void Town()
        {
            Utils.ClearStringBuilder();
            Utils.UpdateStringBuilder("스파르타 마을에 오신 여러분 환영합니다\n");
            Utils.UpdateStringBuilder("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다\n");
            Utils.UpdateStringBuilder("1. 상태보기\n2. 인벤토리\n3. 상점\n");
            Utils.UpdateStringBuilder("원하시는 행동을 입력해 주세요\n>>");
            Utils.ShowStringBuilder();
            action = Console.ReadLine();
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
                    default:
                        Utils.UpdateStringBuilder("! 잘못된 입력입니다 !\n");
                        break;
                }
            }
            else
            {
                Utils.ClearStringBuilder();
                Utils.UpdateStringBuilder("! 잘못된 입력입니다 !\n");
            }
        }

        static void CheckStat()
        {

            Utils.UpdateStringBuilder("당신에 대한 정보 입니다.\n");
            Utils.UpdateStringBuilder($"Lv. {myPlayer._level}\n");
            Utils.UpdateStringBuilder($"{myPlayer._nickName} ( {myPlayer._className} )\n");
            Utils.UpdateStringBuilder($"공격력 : {myPlayer._attack}");
            CheckAdditionalStat(AdditionalStat.ATK);
            Utils.UpdateStringBuilder($"방어력 : {myPlayer._defense}");
            CheckAdditionalStat(AdditionalStat.DEF);
            Utils.UpdateStringBuilder($"체  력 : {myPlayer._hp}\n");
            Utils.UpdateStringBuilder($"Gold : {myPlayer._gold}\n\n");
            Utils.UpdateStringBuilder("0. 나가기\n");
            Utils.ShowStringBuilder();
            action = Console.ReadLine();

            if (int.TryParse(action, out int num)) // 예외처리
            {
                Utils.ClearStringBuilder();
                switch (num)
                {
                    case 0:
                        ChangeState(GameState.Town);
                        break;
                    default:
                        Utils.UpdateStringBuilder("! 잘못된 입력입니다 !\n");
                        break;
                }
            }
            else
            {
                Utils.ClearStringBuilder();
                Utils.UpdateStringBuilder("! 잘못된 입력입니다 !\n");
            }
        }

        static void CheckAdditionalStat(AdditionalStat additionalStat)
        {
            if (myPlayer.GetAdditionalStat().TryGetValue(additionalStat.ToString(), out int value))
            {
                if(value > 0)
                    Utils.UpdateStringBuilder($" (+ {value})\n");
                else
                    Utils.UpdateStringBuilder($" (- {value})\n");
            }
            else
                Utils.UpdateStringBuilder($"\n");
        }

        static void ShowInventory(bool isEqiup = false)
        {

            Utils.UpdateStringBuilder("[아이템 목록]\n");
            if(isEqiup == false)
            {
                Inventory.AddInventoryStringBuiler();
                Utils.UpdateStringBuilder("\n1. 장착 관리\n");
                Utils.UpdateStringBuilder("0. 나가기\n\n");
            }
            else
            {
                Inventory.AddInventoryStringBuiler(true);
                Utils.UpdateStringBuilder("0. 나가기\n\n");
            }

            Utils.UpdateStringBuilder("원하시는 행동을 입력해 주세요\n>>");
            Utils.ShowStringBuilder();
            action = Console.ReadLine();
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
                            Utils.UpdateStringBuilder("! 잘못된 입력입니다 !\n");
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
                        Utils.UpdateStringBuilder("! 잘못된 입력입니다 !\n");
                }
            }
            else
            {
                Utils.ClearStringBuilder();
                Utils.UpdateStringBuilder("! 잘못된 입력입니다 !\n");
            }

        }

        public static void ShowShop(bool isBuy = false)
        {
            Utils.UpdateStringBuilder("[보유 골드]\n");
            Utils.UpdateStringBuilder($"{myPlayer._gold}\n\n");
            Utils.UpdateStringBuilder("[아이탬 목록]\n");

            if (isBuy == false)
            {
                Shop.AddShopStringBuiler();
                Utils.UpdateStringBuilder("\n1. 아이템 구매\n");
                Utils.UpdateStringBuilder("0. 나가기\n\n");
            }
            else
            {
                Shop.AddShopStringBuiler(true);
                Utils.UpdateStringBuilder("0. 나가기\n\n");
            }
 
            Utils.UpdateStringBuilder("원하시는 행동을 입력해 주세요\n>>");
            Utils.ShowStringBuilder();
            action = Console.ReadLine();
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
                        default:
                            Utils.UpdateStringBuilder("! 잘못된 입력입니다 !\n");
                            break;
                    }
                }
                else
                {
                    if (num == 0)
                        ChangeState(GameState.Shop);
                    else if (num <= Shop.GetShopSize())
                        Shop.BuyItem(num);
                    else
                        Utils.UpdateStringBuilder("! 잘못된 입력입니다 !\n");
                }
   
            }
            else
            {
                Utils.ClearStringBuilder();
                Utils.UpdateStringBuilder("! 잘못된 입력입니다 !\n");
            }
        }


        static void ChangeState(GameState changeState)
        {
            state = changeState;
        }
    }
}
