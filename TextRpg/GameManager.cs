using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TextRpg
{
    interface IGameStateHandler
    {
        void Handle(GameLoop context); // 또는 필요한 매개변수를 더 넣어도 됨
    }
    class GameLoop
    {
        public  Player myPlayer;
        static GameState state = GameState.None;
        static int waitTime = 100;

        public DungeonResultData dungeonResultData { get; set; } = new DungeonResultData();
        static bool? isRestore = null;

        // 상태 핸들러 등록용
     //   private Dictionary<GameState, Action> stateHandlers;

        public void Run()
        {
            Init();
            InitializeStateHandlers();

            while (true)
            {
                if (stateHandlers.TryGetValue(state, out var handler))
                {
                    handler.Handle(this);
                }
                else
                {
                    Thread.Sleep(waitTime);
                }
            }
        }

        private Dictionary<GameState, IGameStateHandler> stateHandlers;

        private void InitializeStateHandlers()
        {
            stateHandlers = new Dictionary<GameState, IGameStateHandler>
    {
        { GameState.SetChar, new SetCharHandler() },
        { GameState.Town, new TownStateHandler() },
        { GameState.CheckStat, new CheckStatHandler() },
        { GameState.Inventory, new ShowInventoryHandler() },
        { GameState.Equip, new EquipInventoryHandler() },
        { GameState.Shop, new ShowShopHandler() },
        { GameState.Buy, new BuyShopHandler() },
        { GameState.Sell, new SellShopHandler() },
        { GameState.Dungeon, new ShowDungeonHandler() },
        { GameState.DungeonResult, new DungeonResultHandler() },
        { GameState.Restore, new GoRestoreHandler() },

    };
        }

        void Init()
        {
            myPlayer = new Player();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            Database.Instance.SetData();
            Player checkPlayer = SaveManager.Instance.LoadData();
            if (checkPlayer == null)
            {
                ChangeState(GameState.SetChar);
            }
            else
            {
                myPlayer = checkPlayer;
                ChangeState(GameState.Town);
            }
        }

        void OnProcessExit(object sender, EventArgs e)
        {
            try
            {
                if (myPlayer == null)
                {
                    return;
                }
                SaveManager.Instance.SaveData(myPlayer);

                Thread.Sleep(100);  // 저장 시간 확보
            }
            catch (Exception ex)
            {
                Console.WriteLine($"예외 발생: {ex.Message}");
            }
        }
        public void ChangeState(GameState changeState)
        {
            state = changeState;
        }
        
    }

    internal class GameManager
    {
       public static GameLoop gameLoop;
        static void Main(string[] args)
        {
            gameLoop = new GameLoop();
            gameLoop.Run();
        }
    }
}
