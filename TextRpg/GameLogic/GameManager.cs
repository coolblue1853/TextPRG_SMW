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
        // 플레이어 변수
        public  Player myPlayer;
        //상태 변수
        GameState state = GameState.None;
        int waitTime = 100;

        //게임 매니져
        public Shop shop = new Shop();
        public Inventory inventory = new Inventory();
        public Database database = new Database();
        public SaveManager saveManager = new SaveManager();

        //던전관련 변수
        public DungeonResultData dungeonResultData { get; set; } = new DungeonResultData();
        static bool? isRestore = null;

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
            database.SetData();
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
