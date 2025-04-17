using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Numerics;

namespace TextRpg
{
    public interface ISave<T>
    {
        void Save(T data);
        T Load();
    }
    public class PlayerSaveData
    {        //(string name, JobData job, Level level = Level.LV_1, int exp = 0, int gold = 1500)
        public int PlayerType { get; set; }
        public string NickName { get; set; }
        public string ClassName { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public float AttackBase { get; set; }
        public float AttackAdd { get; set; }
        public float DefenseBase { get; set; }
        public float DefenseAdd { get; set; }
        public int HP { get; set; }
        public int Gold { get; set; }
    }
    public class ItemSaveData
    {
        public int Id { get; set; }
        public bool IsEquipped { get; set; }
    }

    // 인벤토리 전체를 감싸는 DTO
    public class InventorySaveData
    {
        public List<ItemSaveData> Items { get; set; } = new List<ItemSaveData>();
    }

    public class PlayerSave : ISave<Player>
    {
        private const string SavePath = "player_save.json";

        public void Save(Player player)
        {
            var dto = ConvertToDTO(player);
            var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SavePath, json);
        }

        public Player Load()
        {
            if (!File.Exists(SavePath)) return null;

            var json = File.ReadAllText(SavePath);
            var dto = JsonSerializer.Deserialize<PlayerSaveData>(json);
            return ConvertToPlayer(dto);
        }

        private PlayerSaveData ConvertToDTO(Player player)
        {
            return new PlayerSaveData
            {
                PlayerType = (int)player._playerType,
                NickName = player._nickName,
                ClassName = player._className,
                Level = (int)player._level,
                Exp = player._exp,
                AttackBase = player._attack.GetBaseValue().Value,
                AttackAdd = player._attack.GetAddValue().Value,
                DefenseBase = player._defense.GetBaseValue().Value,
                DefenseAdd = player._defense.GetAddValue().Value,
                HP = player._hp,
                Gold = player._gold
            };
        }

        private Player ConvertToPlayer(PlayerSaveData data)
        {
            var playerType = (PlayerType)data.PlayerType;  // int → enum
            Player player;

            switch (playerType)
            {
                case PlayerType.Warrior:
                    player = new Warrior(data);  // Warrior 클래스 인스턴스화
                    break;
                default:
                    player = null;  // 기본 Player 클래스 인스턴스화
                    break;
            }
            return player;
        }
    }
    public class InventorySave : ISave<Dictionary<int, Item>>
    {
        private const string SavePath = "inventory_save.json";


        public void Save(Dictionary<int, Item> inventory)
        {
            var dto = ConvertToDTO(inventory);
            var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SavePath, json);
        }

        public Dictionary<int, Item> Load()
        {
            if (!File.Exists(SavePath)) return null;

            var json = File.ReadAllText(SavePath);
            var dto = JsonSerializer.Deserialize<InventorySaveData>(json);
            return ConvertToInventory(dto);
        }

        private InventorySaveData ConvertToDTO(Dictionary<int, Item> inventory)
        {
            var dto = new InventorySaveData();

            foreach (var value in inventory)
            {
                dto.Items.Add(new ItemSaveData
                {
                    Id = value.Key,
                    IsEquipped = value.Value._isEquip
                });
            }

            return dto;
        }

        private Dictionary<int, Item> ConvertToInventory(InventorySaveData data)
        {
            // 1) 기존 인벤토리/장착 컬렉션 초기화
            var invenDict = new Dictionary<int, Item>();
            Inventory.Instance.GetEquipDict().Clear();

            // 2) 각 DTO로부터 Item 인스턴스 생성
            foreach (var itemDto in data.Items)
            {
                var item = Item.Create(itemDto.Id);
                if (item == null)
                    continue;

                invenDict[itemDto.Id] = item;
                Shop.Instance.SetItemBuy(item._id);

                if (itemDto.IsEquipped)
                {
                    item.TogleEquipState();
                    Inventory.Instance.GetEquipDict()[item._itemType] = item;
                    Inventory.Instance.ActiveItemEffect(item);
                }
            }
            return invenDict;
        }
    }
    public class SaveManager
    {
        public static SaveManager Instance { get; private set; } = new SaveManager();
        ISave<Player> saveService = new PlayerSave();
         ISave<Dictionary<int, Item>> invenService = new InventorySave();
        public  void SaveData(Player myPlayer)
        {
            saveService.Save(myPlayer);
            invenService.Save(Inventory.Instance.GetInvenDict());
        }
        public  Player LoadData()
        {
            var player = saveService.Load();
            if (player == null)
                return null;

            GameManager.gameLoop.myPlayer = player;

            var invDict = invenService.Load();
            Inventory.Instance.SetInvenDict(invDict);

            return player;
        }
    }
}