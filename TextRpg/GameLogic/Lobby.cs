using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http.Headers;
using System.Text;

namespace TextRpg
{
    class TownStateHandler : IGameStateHandler
    {
        bool isShowError = false;
        public void Handle(GameLoop context)
        {
            Utils.UpdateStringBuilder(context.database.sceneDatas.Town.town_welcome);
            Utils.UpdateStringBuilder(context.database.sceneDatas.Town.town_select, !isShowError, true);
            if (isShowError)
            {
                Utils.UpdateStringBuilder(context.database.sceneDatas.Error.input_error, isShowError, false);
            }

            isShowError = false;
            Utils.ReadLine(out string action);

            if (int.TryParse(action, out int num))
            {
                Utils.ClearStringBuilder();
                switch (num)
                {
                    case 1:
                        context.ChangeState(GameState.CheckStat);
                        break;
                    case 2:
                        context.ChangeState(GameState.Inventory);
                        break;
                    case 3:
                        context.ChangeState(GameState.Shop);
                        break;
                    case 4:
                        context.ChangeState(GameState.Dungeon);
                        break;
                    case 5:
                        context.ChangeState(GameState.Restore);
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
    class CheckStatHandler : IGameStateHandler
    {
        bool isShowError = false;
        public void Handle(GameLoop context)
        {
            Player myPlayer = context.myPlayer;
            DataLoader dataLoader = new DataLoader();
            Utils.UpdateStringBuilder(dataLoader.FormatText(context.database.sceneDatas.Stat.level_name, myPlayer.GetFormattedStats()), false, true);
            Utils.UpdateStringBuilder(dataLoader.FormatText(context.database.sceneDatas.Stat.attack, myPlayer.GetFormattedStats()));
            CheckAdditionalStat(myPlayer, AdditionalStat.ATK);
            Utils.UpdateStringBuilder(dataLoader.FormatText(context.database.sceneDatas.Stat.defense, myPlayer.GetFormattedStats()));
            CheckAdditionalStat(myPlayer, AdditionalStat.DEF);
            Utils.UpdateStringBuilder(dataLoader.FormatText(context.database.sceneDatas.Stat.etc, myPlayer.GetFormattedStats()), !isShowError);

            if (isShowError)
            {
                Utils.UpdateStringBuilder(context.database.sceneDatas.Error.input_error, isShowError, false);
            }

            isShowError = false;

            Utils.ReadLine(out string action);
            if (int.TryParse(action, out int num)) // 예외처리
            {
                Utils.ClearStringBuilder();
                switch (num)
                {
                    case 0:
                        context.ChangeState(GameState.Town);
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
        void CheckAdditionalStat(Player player, AdditionalStat additionalStat)
        {
            if (player.GetAdditionalStat().TryGetValue(additionalStat, out int value))
            {
                if (value > 0)
                    Utils.UpdateStringBuilder($" (+ {value})\n");
                else
                    Utils.UpdateStringBuilder($" (- {Math.Abs(value)})\n");
            }
            else
                Utils.UpdateStringBuilder($"\n");
        }

    }
    class SetCharHandler : IGameStateHandler
    {
        bool isShowError = false;
        public void Handle(GameLoop context)
        {
            Utils.UpdateStringBuilder(context.database.sceneDatas.Intro.intro_welcome, true, true);
            Utils.ReadLine(out string nickName);


            while (true)
            {
                string input = GetJobInput(isShowError, context.database);
                isShowError = false;
                if (int.TryParse(input, out int num))   // 예외처리 받아야함 tryCatch 로 잡아보면 좋을듯?
                {
                    switch (num)
                    {
                        case 1:
                            context.myPlayer = new Warrior();
                            context.myPlayer.SetInfo(nickName, context.database.jobs[Utils.GetEnumIndex(PlayerType.Warrior)]);
                            context.ChangeState(GameState.Town);
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
        string GetJobInput(bool showError, Database database)
        {
            if (showError)
            {
                Utils.UpdateStringBuilder(database.sceneDatas.Intro.choose_job, false, true);
                Utils.UpdateStringBuilder(database.sceneDatas.Error.input_error, true);
            }
            else
            {
                Utils.UpdateStringBuilder(database.sceneDatas.Intro.choose_job, true, true);
            }

            Utils.ReadLine(out string input);
            return input;
        }
    }

    class GoRestoreHandler : IGameStateHandler
    {
        bool isShowError = false;
        bool? isRestore = null;
        public void Handle(GameLoop context)
        {
            Player myPlayer = context.myPlayer;
            DataLoader dataLoader = new DataLoader();

            Utils.UpdateStringBuilder(dataLoader.FormatText(context.database.sceneDatas.Restore.banner, myPlayer.GetFormattedStats()), false, true);
            Utils.UpdateStringBuilder(context.database.sceneDatas.ETC.base_etc, (!isShowError && isRestore == null));
            if (isRestore != null)
            {
                if (isRestore == true)
                {
                    Utils.UpdateStringBuilder(context.database.sceneDatas.Restore.succ, !isShowError);
                }
                else
                {
                    Utils.UpdateStringBuilder(context.database.sceneDatas.Restore.fail, !isShowError);
                }
                isRestore = null;
            }

            if (isShowError)
            {
                Utils.UpdateStringBuilder(context.database.sceneDatas.Error.input_error, isShowError, false);
            }

            isShowError = false;

            Utils.ReadLine(out string action);
            if (int.TryParse(action, out int num)) // 예외처리
            {
                switch (num)
                {
                    case 0:
                        context.ChangeState(GameState.Town);
                        break;
                    case 1:
                        isRestore = TryToRestore(myPlayer);
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

   
        bool TryToRestore(Player myPlayer)
        {
            int nowGold = myPlayer._gold;
            if (nowGold >= 500)
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
    }

}
