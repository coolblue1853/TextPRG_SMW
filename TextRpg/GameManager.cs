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
        static StringBuilder sb = new StringBuilder();
        static int waitTime = 100; // 대기시간
        static int action;
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
                        CheckStat();
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
            // 중복 Set 을 막기 위한 state 변경
            ChangeState(GameState.None); ;

            Utils.UpdateStringBuilder(sb, "스파르타 던전에 오신 여러분 환영합니다\n");
            Utils.UpdateStringBuilder(sb, "원하시는 닉네임을 설정해 주세요 : ");
            Utils.ShowStringBuilder(sb);
            string nickName = Console.ReadLine();
            Utils.ClearStringBuilder(sb);

            Utils.UpdateStringBuilder(sb, "원하시는 직업을 설정해 주세요 : \n");
            Utils.UpdateStringBuilder(sb, "1. 전사");
            Utils.ShowStringBuilder(sb);
            int job = int.Parse(Console.ReadLine());
            Utils.ClearStringBuilder(sb);

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

            Utils.UpdateStringBuilder(sb, "스파르타 마을에 오신 여러분 환영합니다\n");
            Utils.UpdateStringBuilder(sb, "이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다\n");
            Utils.UpdateStringBuilder(sb, "1. 상태보기\n2. 인벤토리\n3. 상점\n");
            Utils.UpdateStringBuilder(sb, "원하시는 행동을 입력해 주세요\n>>");
            Utils.ShowStringBuilder(sb);
            action = int.Parse(Console.ReadLine());
            Utils.ClearStringBuilder(sb);

            switch (action)
            {
                case 1:
                    ChangeState(GameState.CheckStat);
                    break;
            }
        }

        static void CheckStat()
        {

            Utils.UpdateStringBuilder(sb, "당신에 대한 정보 입니다.\n");
            Utils.UpdateStringBuilder(sb, $"Lv. {myPlayer.GetLevel()}\n");
            Utils.UpdateStringBuilder(sb, $"{myPlayer.GetNickName()} ( {myPlayer.GetClass()} )\n");
            Utils.UpdateStringBuilder(sb, $"공격력 : {myPlayer.GetAttack()}\n");
            Utils.UpdateStringBuilder(sb, $"방어력 : {myPlayer.GetDefense()}\n");
            Utils.UpdateStringBuilder(sb, $"체  력 : {myPlayer.GetHP()}\n");
            Utils.UpdateStringBuilder(sb, $"Gold : {myPlayer.GetGold()}\n\n");
            Utils.UpdateStringBuilder(sb, "0. 나가기\n");
            Utils.ShowStringBuilder(sb);
            action = int.Parse(Console.ReadLine());
            Utils.ClearStringBuilder(sb);

            switch (action)
            {
                case 0:
                    ChangeState(GameState.Town);
                    break;
                default:
                    break;
            }
        }

        static void Inventory()
        {

            Utils.UpdateStringBuilder(sb, "당신에 대한 정보 입니다.\n");
            Utils.UpdateStringBuilder(sb, $"Lv. {myPlayer.GetLevel()}\n");
            Utils.UpdateStringBuilder(sb, $"{myPlayer.GetNickName()} ( {myPlayer.GetClass()} )\n");
            Utils.UpdateStringBuilder(sb, $"공격력 : {myPlayer.GetAttack()}\n");
            Utils.UpdateStringBuilder(sb, $"방어력 : {myPlayer.GetDefense()}\n");
            Utils.UpdateStringBuilder(sb, $"체  력 : {myPlayer.GetHP()}\n");
            Utils.UpdateStringBuilder(sb, $"Gold : {myPlayer.GetGold()}\n\n");
            Utils.UpdateStringBuilder(sb, "0. 나가기\n");
            Utils.ShowStringBuilder(sb);
            action = int.Parse(Console.ReadLine());
            Utils.ClearStringBuilder(sb);

            switch (action)
            {
                case 0:
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
