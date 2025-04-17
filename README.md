# 스파르타코딩클럽 10기_19조 설민우 TextRPG 개인 프로젝트 입니다

# 던전크롤러 - TextRPG

스파르타 코딩클럽 10기, 개인 프로젝트 TextRPG를 제작해 보았습니다. SOLID 원칙을 지켜보는 것을 중점적으로 진행해 보았습니다.

## 📷 스크린샷

![화면 캡처 2025-04-17 165615](https://github.com/user-attachments/assets/b53141fe-bdb7-4673-9099-fa1608508bfc)

## 🕹️ 기능
<details>
<summary><input type="checkbox" checked disabled> 게임 루프와 StateHandler </summary>

```
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
}
```

- 인터페이스로 핸들러를 구현해두고 GameManager에서 Run() 작동시키면 사전에 초기화 해둔 Handler를 돌면서 State에 맞는 Handler를 작동시키도록 했습니다
- 과거의 State를 switch로 구분하는 것 보다 간결한 표현이 가능하고, 추가도 딕셔너리에 Hnadler만 추가해주면 됩니다.
- 외부로부터 데이터가 필요하다면 자신을 호출한 GameLoop의 데이터를 참조하여 해결합니다.

</details>
<details>
<summary><input type="checkbox" checked disabled> 캐릭터 생성 및 직업 선택</summary>

```
 public interface IAddable<T>
 {
     T Add(T other);
 }
 public class StatFloat : IAddable<StatFloat>
 {
     public float Value { get; }

     public StatFloat()
     {
         Value = 0f;
     }
     public StatFloat(float value)
     {
         Value = value;
     }

     public StatFloat Add(StatFloat other)
     {
         return new StatFloat(this.Value + other.Value);
     }
 }
 public abstract class Stat<T> where T : IAddable<T>, new()
 {
     public T _baseValue { get; protected set; }
     public T _addValue { get; protected set; }

     // 생성자에서 기본값을 설정
     public Stat(T value)
     {
         _baseValue = value;
         _addValue = new T(); 
     }

     // 최종 값을 구하는 프로퍼티
     public T FinalValue => _baseValue.Add(_addValue);

     // 기본값과 추가값을 반환하는 메서드
     public T GetBaseValue() => _baseValue;
     public T GetAddValue() => _addValue;
     public T GetFinalValue() => FinalValue;

     // 기본값 설정
     public void SetBaseValue(T value)
     {
         _baseValue = value;
     }

     // 추가값 설정
     public void SetAddValue(T value)
     {
         _addValue = _addValue.Add(value); // Add 연산을 호출
     }

 }
```
- Stat의 경우 int 일 수도, float 일 수도 있기 때문에, 제네릭 타입을 통해서 구현했습니다.
- 하지만 제네릭 타입의 경우 즉시 산술연산을 해줄 수 없기 때문에 별도의 StatInt, StatFloat 클래스를 IAddable 인터페이스를 추가하여 구현했습니다.
- Stat은 Player 클래스가 들고 있고, 각각의 직업이 Player를 상속받아 사용합니다.

</details>

  
<details>
<summary><input type="checkbox" checked disabled> 캐릭터 능력치 확인</summary>

```
  "Stat": {
    "level_name": "당신에 대한 정보 입니다.\nLv. {level}\n{nickName} ( {className} )\n",
    "attack": "공격력 : {attack}",
    "defense": "방어력 : {defense}",
    "etc": "체  력 : {hp}\nGold : {gold}\n\n0. 나가기\n"
  },
```
- 능력치는 별도의 Json 파일에 Player 관련 데이터에서 불러옵니다.
- UI/UX에 관련된 부분도 Json에서 불러오되, 파싱을 통해서 해당 부분에 원하는 수치를 넣어줍니다.

</details>

<details>
<summary><input type="checkbox" checked disabled> 인벤토리 및 아이템 장착 (중복 장착 금지) </summary>

```
   private  Dictionary<int, Item> invenDict = new Dictionary<int, Item>();
   private  Dictionary<ItemType, Item> equipDict = new Dictionary<ItemType, Item>();

   public  void AddItem(Item item)
   {
       invenDict.Add(item._id,item);
   }
   public  void DeleteItem(Item item, int num)
   {
       if (item._isEquip)
       {
           UnEquipItem(item);
       }
       invenDict.Remove(invenDict.ElementAt(num-1).Key);
   }
   public  int GetInventorySize() { return invenDict.Count; }
   public  void AddInventoryStringBuiler(bool isShowNum = false)
   {
       for (int i = 1; i < invenDict.Count + 1; i++)
       {
           Item item = invenDict.ElementAt(i - 1).Value;

           string equipText = "";
           if (item._isEquip)
               equipText = "[E]";

           if (isShowNum)
               equipText = ($"- {equipText} {i} ");
           else
               equipText = ($"- {equipText} ");

           // 패딩 과정
           string name = Utils.PadRight(equipText + item._name, 25);
           string effect = Utils.PadRight(Utils.AddEffectText(item), 20);
           string desc = Utils.PadRight(item._description, 55);
           string price = Utils.PadRight(((int)(item._price * 0.85f)).ToString(), 6);

           Utils.UpdateStringBuilder($"{name} | {effect} | {desc} | {price} G\n");
       }
       Utils.UpdateStringBuilder("\n\n");
   }

   // 토글 방식으로 작동
   public  void EquipItem(int index)
   {
       Item item = invenDict.ElementAt(index - 1).Value;
       //토글
       // 장착 해제에 따른 효과 반영
       if (equipDict.ContainsKey(item._itemType)) // 해당하는 타입이 있다면 장착해제 후 장착
       {
           if(equipDict[item._itemType]._id != item._id) // 그게 지금 아이템과 다른경우
           {
               var ChangeItem = equipDict[item._itemType];
               ChangeItem.TogleEquipState();
               ActiveItemEffect(ChangeItem);
               equipDict[item._itemType] = item;
           }
           else // 그게 지금 아이템과 같은경우
           {
               equipDict.Remove(item._itemType);
           }
       }
       else
       {
           equipDict.Add(item._itemType, item);
       }
       item.TogleEquipState();
       ActiveItemEffect(item);
   }
   //특정 아이템 장착 해제
   public  void UnEquipItem(Item item)
   {
       equipDict.Remove(item._itemType);
       item.TogleEquipState();
       ActiveItemEffect(item);
   }

   public  void ActiveItemEffect(Item item)
   {
       foreach (var value in item.GetEffect())
       {
           int effectPower = value.Value;
           if (!item._isEquip) // 효과 추가
               effectPower = -effectPower;

           GameManager.gameLoop.myPlayer.SetAddtionalStat(value.Key, effectPower);
       }
   }
```
- 딕셔너리에서는 기존 Database의 idx를 확인하여 item을 관리하는 방식과 다르게, 딕셔너리에서의 index를 기준으로 아이템을 얻거나 파괴합니다
- 기본적으로 장착은 토글 형식으로 구현하였고, Type을 딕셔너리에 넣어두어 중복 장착을 금지했습니다
- 상점에서 판매하는 경우에도 바로 장착이 해제 될 수 있도록 하였습니다.


</details>


<details>
<summary><input type="checkbox" checked disabled> 상점 구매, 판매  (판매시 장착 아이템 판매) </summary>

```
 public  void AddShopStringBuiler(bool isShowNum = false)
 {
     for (int i = 1; i < shopItems.Count+1; i++)
     {
         Item item = shopItems.ElementAt(i-1).Value.Item1;
         bool isBuy = shopItems.ElementAt(i-1).Value.Item2;


         string equipText = "";

         if (isShowNum)
             equipText = ($"- {i} ");
         else
             equipText = ($"- ");

         string name = Utils.PadRight(equipText + item._name, 25);
         string effect = Utils.PadRight(Utils.AddEffectText(item), 20);
         string desc = Utils.PadRight(item._description, 55);
         string price = Utils.PadRight(((int)(item._price)).ToString(), 6);

         Utils.UpdateStringBuilder($"{name} | {effect} | {desc} | ");


         if(isBuy)
             Utils.UpdateStringBuilder("[구매완료]\n");
         else
             Utils.UpdateStringBuilder($"{price}G\n");

     }
     Utils.UpdateStringBuilder("\n\n");
 }

 public string BuyItem(int index)
 {
     Database database = GameManager.gameLoop.database;
     Item item = shopItems.ElementAt(index - 1).Value.Item1;
     // 안 산 물건이라면
     if (shopItems[item._id].Item2 == false)
     {
         if(GameManager.gameLoop.myPlayer._gold >= item._price)
         {
             shopItems[item._id] = (item,true);
             GameManager.gameLoop.inventory.AddItem(item);
             GameManager.gameLoop.myPlayer.ChangeGold(-item._price);
             return database.sceneDatas.Shop.buy_Succ;
         }
         else
             return database.sceneDatas.Shop.buy_fail;
     }
     else
         return database.sceneDatas.Shop.buy_already;
 }

 public  void SellItem(int num)
 {
     Item sellItem = GameManager.gameLoop.inventory.GetItem(num);
     shopItems[sellItem._id] = (sellItem, false);
     int sellPrice = (int)(sellItem._price * 0.85f);
     GameManager.gameLoop.myPlayer.ChangeGold(sellPrice);
     GameManager.gameLoop.inventory.DeleteItem(sellItem, num);
 }
```
- 별도의 유틸리티에 패딩을 위한 함수를 작성해두어 UI가 일정하게 보여질 수 있도록 하였고
- 중복 구매를 막아서 이미 구매한 물건이라면 상점의 Dictionary<int, (Item, bool)> shopItems 에 bool 값으로 해당 정보를 저장해 두었습니다.
- 판매시에는 장착해제를 진행하여, 인벤토리에서 삭제하고 능력치도 반환하게 됩니다

</details>

<details>
<summary><input type="checkbox" checked disabled> 던전과 레벨 </summary>

```
[
  {
    "Idx": 1,
    "Name": "쉬운 던전",
    "Defense": 5,
    "Gold": 1000,
    "DefenseProbability": 0.4,
    "FailHealthDivied": 2,
    "MinUseHp": 20,
    "MaxUseHp": 35,
    "GoldRatio": 2
  },
  {
    "Idx": 2,
    "Name": "일반 던전",
    "Defense": 11,
    "Gold": 1700,
    "DefenseProbability": 0.4,
    "FailHealthDivied": 2,
    "MinUseHp": 20,
    "MaxUseHp": 35,
    "GoldRatio": 2
  },
  {
    "Idx": 3,
    "Name": "어려운 던전",
    "Defense": 17,
    "Gold": 2500,
    "DefenseProbability": 0.4,
    "FailHealthDivied": 2,
    "MinUseHp": 20,
    "MaxUseHp": 35,
    "GoldRatio": 2
  }
]
```
- 던전에 대한 정보 또한 별도의 Json 파일에 저장하여 불러와서 포맷팅을 해주는 방식으로 구현했습니다
- 이를 통해서 던전의 난이도와 이름, 추가를 유동적으로 쉽게 할수 있도록 구현했습니다.

</details>

<details>
<summary><input type="checkbox" checked disabled> 저장 / 불러오기 기능 (JSON 기반) </summary>

```
public interface ISave<T>
{
    void Save(T data);
    T Load();
}
    public class ItemSaveData
    {
        public int Id { get; set; }
        public bool IsEquipped { get; set; }
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
            GameManager.gameLoop.inventory.GetEquipDict().Clear();

            // 2) 각 DTO로부터 Item 인스턴스 생성
            foreach (var itemDto in data.Items)
            {
                var item = Item.Create(itemDto.Id);
                if (item == null)
                    continue;

                invenDict[itemDto.Id] = item;
                GameManager.gameLoop.shop.SetItemBuy(item._id);

                if (itemDto.IsEquipped)
                {
                    item.TogleEquipState();
                    GameManager.gameLoop.inventory.GetEquipDict()[item._itemType] = item;
                    GameManager.gameLoop.inventory.ActiveItemEffect(item);
                }
            }
            return invenDict;
        }
    }
```
- 저장 할 수 있음을 나타내는 인터페이스를 추가하였습니다
- 이를 기반으로 Json 파일을 읽어와 데이터를 저장 / 로드 합니다
- 저장의 경우는 ConvertToDTO() 함수를 통해 딕셔너리 값을 Data Transfer Object(데이터 전송 객체)로 변환 합니다.
- 로드의 경우는 ConvertToInventory() 함수를 통해 dto 값을 딕셔너리로 변환 합니다.

</details>

## 🛠️ 기술 스택

- C#
- .NET Core 3.1
- Newtonsoft.Json (데이터 직렬화/역직렬화)

## 🧙 사용법

1. 이 저장소를 클론하거나 다운로드합니다.
2. Visual Studio / Rider로 열고 실행합니다.
3. 콘솔에서 안내에 따라 게임을 진행합니다.
4. 플레이 도중 자동 저장됩니다. `player_save.json`, `inventory_save.json` 참고.

## 💾 저장 데이터

- 저장 경로: `bin/Debug/netcoreapp3.1/`
- `player_save.json`: 플레이어 정보
- `inventory_save.json`: 인벤토리 정보

## 🗂️ 프로젝트 구조
<details>
<summary><input type="checkbox" checked disabled> 펼쳐보기 </summary>

```
TextRpg/
├── Data/
│   ├── DungeonData.cs
│   ├── ItemData.cs
│   ├── JobData.cs
│   └── SceneData.cs
│
├── Database/
│   ├── Database.cs
│   └── DataLoader.cs
│
├── GameLogic/
│   ├── Dungeon.cs
│   ├── GameManager.cs
│   ├── Inventory.cs
│   ├── Item.cs
│   ├── Lobby.cs
│   ├── SaveManager.cs
│   └── Shop.cs
│
├── Json/
│   ├── dungeons.json
│   ├── items.json
│   ├── Jobs.json
│   └── sceneText.json
│
├── Player/
│   ├── Player.cs
│   └── Stats.cs
│
├── Utility/
│   ├── Define.cs
│   └── Utils.cs
```
</details>


## 🙋 개발자 정보

- 이름: SulMinWoo
- 연락처 : sataka1853@naver.com
