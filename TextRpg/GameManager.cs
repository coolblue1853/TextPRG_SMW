using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TextRpg
{
    internal class GameManager
    {
        static Player myPlayer;
        static GameState state = GameState.None;
        static int waitTime = 100; // 대기시간
        static int action;
        static void Main(string[] args)
        {
            Init();


         /*
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
                    default:
                        Thread.Sleep(waitTime);
                        break;
                }
            }
  */
        }

        static void Init()
        {
            Database.SetData();
            Item item = Item.Create(3);
            Inventory.AddItem(item);
            Inventory.ShowInventory();
            ChangeState(GameState.SetChar);
        }

        static void SetCharacter()
        {
            // 중복 Set 을 막기 위한 state 변경
            ChangeState(GameState.None); ;

            Utils.UpdateStringBuilder("스파르타 던전에 오신 여러분 환영합니다\n");
            Utils.UpdateStringBuilder("원하시는 닉네임을 설정해 주세요 : ");
            Utils.ShowStringBuilder();
            string nickName = Console.ReadLine();
            Utils.ClearStringBuilder();

            Utils.UpdateStringBuilder("원하시는 직업을 설정해 주세요 : \n");
            Utils.UpdateStringBuilder("1. 전사");
            Utils.ShowStringBuilder();
            int job = int.Parse(Console.ReadLine());
            Utils.ClearStringBuilder();

            switch (job)
            {
                case 1:
                    myPlayer = new Warrior();
                     myPlayer.SetInfo(nickName, Database.jobs[Utils.GetEnumIndex(PlayerType.Warrior)]);
                    break;
            }

            ChangeState(GameState.Town);
        }

        static void Town()
        {

            Utils.UpdateStringBuilder("스파르타 마을에 오신 여러분 환영합니다\n");
            Utils.UpdateStringBuilder("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다\n");
            Utils.UpdateStringBuilder("1. 상태보기\n2. 인벤토리\n3. 상점\n");
            Utils.UpdateStringBuilder("원하시는 행동을 입력해 주세요\n>>");
            Utils.ShowStringBuilder();
            action = int.Parse(Console.ReadLine());
            Utils.ClearStringBuilder();

            switch (action)
            {
                case 1:
                    ChangeState(GameState.CheckStat);
                    break;
                case 2:
                    ChangeState(GameState.Inventory);
                    break;
            }
        }

        static void CheckStat()
        {

            Utils.UpdateStringBuilder("당신에 대한 정보 입니다.\n");
            Utils.UpdateStringBuilder($"Lv. {myPlayer._level}\n");
            Utils.UpdateStringBuilder($"{myPlayer._nickName} ( {myPlayer._className} )\n");
            Utils.UpdateStringBuilder($"공격력 : {myPlayer._attack}\n");
            Utils.UpdateStringBuilder($"방어력 : {myPlayer._defense}\n");
            Utils.UpdateStringBuilder($"체  력 : {myPlayer._hp}\n");
            Utils.UpdateStringBuilder($"Gold : {myPlayer._gold}\n\n");
            Utils.UpdateStringBuilder("0. 나가기\n");
            Utils.ShowStringBuilder();
            action = int.Parse(Console.ReadLine());
            Utils.ClearStringBuilder();

            switch (action)
            {
                case 0:
                    ChangeState(GameState.Town);
                    break;
                default:
                    break;
            }
        }

        static void ShowInventory()
        {

            Utils.UpdateStringBuilder("[아이템 목록]\n");
            // 목록 보기

            Utils.UpdateStringBuilder("\n1. 장착 관리\n");
            Utils.UpdateStringBuilder("0. 나가기\n\n");
            Utils.UpdateStringBuilder("원하시는 행동을 입력해 주세요\n>>");
            Utils.ShowStringBuilder();
            action = int.Parse(Console.ReadLine());
            Utils.ClearStringBuilder();

            switch (action)
            {
                case 0:
                    ChangeState(GameState.Town);
                    break;
                case 1:
                    ChangeState(GameState.Town);
                    break;
                default:
                    break;
            }
        }


        static void ChangeState(GameState changeState)
        {
            state = changeState;
        }


    }
}
