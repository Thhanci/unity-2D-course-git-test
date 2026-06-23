using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour//背包管理器     作用：管理整个背包。添加/移除物品，存储所有拥有的物品。   //Tip:看后缀 ItemData->Data  ItemObject->object
{
    public static Inventory instance;

    public List<ItemData> startingItems;

    public List<InventoryItem> equipment;     //额外添加，左边是list stash右边是list inventory，里面的东西右边是equipment
    public Dictionary<ItemData_Equipment, InventoryItem> equipmentDictionary;

    //public List<ItemData> inventory=new List<ItemData>();
    public List<InventoryItem> inventory;  //存数量   //列表，按顺序存储物品，用于 UI 显示。
    public Dictionary<ItemData, InventoryItem> inventoryDictionary; //存 物品 数量(key,value)    //字典，键是物品数据，值是物品实例。用于快速查找。

    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;

    [Header("Inventory UI")]

    [SerializeField] private Transform inventorySlotParent;//inventorySlotParent   //Transform 和 GameObject 都可以表示物体，但用 Transform 是 Unity 开发中的常见习惯，因为几乎每个组件都挂着 .transform，获取方便，且能直接用 transform 来管理子物体位置。
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSlotParent;

    private UI_ItemSlot[] inventoryItemSlot;
    private UI_ItemSlot[] stashItemSlot;
    private UI_EquipmentSlot[] equipmentSlot;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()//在游戏开始时创建空的列表和字典，准备好装物品。
    {
        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();

        equipment = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemData_Equipment, InventoryItem>();

        inventoryItemSlot = inventorySlotParent.GetComponentsInChildren<UI_ItemSlot>();
        stashItemSlot = stashSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        equipmentSlot = equipmentSlotParent.GetComponentsInChildren<UI_EquipmentSlot>();

        AddStartingItems();

    }

    private void AddStartingItems()
    {
        for (int i = 0; i < startingItems.Count; i++)
        {
            AddItem(startingItems[i]);
        }
    }

    public void Equipment(ItemData _item)
    {
        ItemData_Equipment newEquipment = _item as ItemData_Equipment;  //父类转子类有条件的，你传进来的item其实是itemDataEquipment类型的，所以可以转
        InventoryItem newItem = new InventoryItem(newEquipment);//传入构造体   蓝图_item套上壳子InventoryItem,InventoryItem用于计数

        //ItemData_Equipment   //peek

        ItemData_Equipment oldEquipment = null;
        foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
        {
            if (item.Key.equipmentType == newEquipment.equipmentType)
                oldEquipment = item.Key;    //光删字典没用，得把蓝图也删了
        }

        if (oldEquipment != null) 
        {
            UnequipItem(oldEquipment);
            AddItem(oldEquipment);
        }

        equipment.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers();//ItemData_Equipment newEquipment  调用ItemData_Equipment里的函数AddModifier()

        RemoveItem(_item);  //因为格子不会更新没有装备的格子，格子里就会留下上一个物品的图像

        UpdateSlotUI();//装备完武器就更新UI槽
    }

    public void UnequipItem(ItemData_Equipment itemToRemove)
    {
        if (equipmentDictionary.TryGetValue(itemToRemove, out InventoryItem value))
        {
            //AddItem(itemToRemove);
            equipment.Remove(value);
            equipmentDictionary.Remove(itemToRemove);
            itemToRemove.RemoveModifiers();
        }
    }

    private void UpdateSlotUI()
    {
        for (int i = 0; i < equipmentSlot.Length; i++)
        {
            foreach (KeyValuePair<ItemData_Equipment, InventoryItem> item in equipmentDictionary)
            {
                if (item.Key.equipmentType == equipmentSlot[i].slotType)//保证每个槽位显示的都是正确的装备类型
                    equipmentSlot[i].UpdateSlot(item.Value);  //另每装备一次装备，都会调用更新UI函数，更新身上装备
            }
        }



        for (int i = 0; i < inventoryItemSlot.Length; i++)
        {
            inventoryItemSlot[i].ClearUpSlot();
        }
        for (int i = 0; i < stashItemSlot.Length; i++)
        {
            stashItemSlot[i].ClearUpSlot();
        }


        for (int i = 0;i<inventory.Count; i++)
        {
            inventoryItemSlot[i].UpdateSlot(inventory[i]);
        }

        for (int i = 0; i < stash.Count; i++)
        {
            stashItemSlot[i].UpdateSlot(stash[i]);
        }

    }

    public void AddItem(ItemData _item)
    {
        if (_item.itemType == ItemType.Equipment)
            AddToInventory(_item);
        else if (_item.itemType == ItemType.Material)
            AddToStash(_item);
       


        UpdateSlotUI();
    }

    private void AddToStash(ItemData _item)
    {
        if (stashDictionary.TryGetValue(_item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(_item);  //拾取起来的物品是ItemData，图纸信息，所以给他套个外壳InventoryItem
                                                               //_item 本身存的是物品的"蓝图"信息（名称、图标等），但 ItemData 不能记录数量，所以用 InventoryItem 来包装它，加一个 stackSize 字段记录数量。
            stash.Add(newItem); //添加入列表
            stashDictionary.Add(_item, newItem);    //添加入字典
        }
    }

    private void AddToInventory(ItemData _item)
    {
        //inventory.Add(_item);
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))//out类比return,但out可以返回多个值
        {
            value.AddStack();
        }
        else  //没找到，说明背包还没有这个物品。新建一个 InventoryItem 实例。
        {
            InventoryItem newItem = new InventoryItem(_item);//定义物品newItem   //创建一个新的 InventoryItem（背包物品）对象，并传入 _item（物品数据）作为参数。
            inventory.Add(newItem);
            inventoryDictionary.Add(_item, newItem);
        }
    }

    public void RemoveItem(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
        {
            if (value.stackSize <= 1)   //  public List<InventoryItem> inventoryItems;    //public Dictionary<ItemData, InventoryItem> inventoryDictionary; 
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(_item);
            }
            else
                value.RemoveStack();
        }

        if (stashDictionary.TryGetValue(_item, out InventoryItem stashValue))
        {
            if (stashValue.stackSize <= 1)
            { 
                stash.Remove(stashValue);
                stashDictionary.Remove(_item);

            }
            else
                stashValue.RemoveStack();
        }

        UpdateSlotUI();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        { 
            ItemData newItem = inventory[inventory.Count-1].data;//因为有inventoryItems[0]，所以-1  //数据对应的key  .data

            RemoveItem(newItem);
        }
    }


    public bool CanCraft(ItemData_Equipment _itemToCraft, List<InventoryItem> _requiredMaterials)
    { 
        List<InventoryItem> materialsToRemove = new List<InventoryItem>();

        for (int i = 0; i < _requiredMaterials.Count; i++)
        {
            if (stashDictionary.TryGetValue(_requiredMaterials[i].data, out InventoryItem stashValue))
            {
                //add this to used materials
                if (stashValue.stackSize < _requiredMaterials[i].stackSize)  //如果材料不够需求
                {
                    Debug.Log("not enough materials");
                    return false;
                }
                else
                { 
                    materialsToRemove.Add(stashValue);//如果材料够，就吧材料添加入移除列表
                }
            }
            else  //如果字典里没有，就是没有材料
            {
                Debug.Log("not enough materials");
                return false;
            }
        }

        for (int i = 0; i < materialsToRemove.Count; i++)   //清空需求材料区
        {
            RemoveItem(materialsToRemove[i].data);
        }
        AddItem(_itemToCraft);      //添加材料合成的 合成物
        Debug.Log("Here is your item " + _itemToCraft.name);

        return true;

    }

    public List<InventoryItem> GetEquipmentList() => equipment;  //GetEquipmentList() => equipment; = 一个返回 equipment 列表的简洁方法，外部只能读，不能改整个列表。

}


/*
    Tip: else后按tab可以快速出{ }

    TryGetValue：尝试从字典中根据键 _item 获取值。

    如果找到：返回 true，并把找到的值存入 value 变量

    如果没找到：返回 false

    out InventoryItem value：out 是输出参数。方法内部会把找到的值赋给 value，方法结束后可以在外面使用。




    关键方法/关键字用法总结
    方法/关键字	作用	例子
    TryGetValue	安全地从字典取值，不报错	dict.TryGetValue(key, out value)
    out	方法内部赋值，外部能用	out InventoryItem value
    Dictionary<TKey, TValue>	键值对集合，查找快	Dictionary<ItemData, InventoryItem>
    List<T>	有序列表	List<InventoryItem>
    static	属于类本身，不属于对象	public static Inventory instance



    ## 逐行解析

    ### 类定义与变量

    ```csharp
    public class Inventory : MonoBehaviour
    ```
    继承 MonoBehaviour，可以挂载到 GameObject 上。

    ```csharp
    public static Inventory instance;
    ```
    静态变量，保存唯一的实例。其他脚本通过 `Inventory.instance` 访问背包。

    ```csharp
    public List<InventoryItem> inventoryItems;
    ```
    列表，按顺序存储物品，用于 UI 显示。

    ```csharp
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;
    ```
    字典，键是物品数据，值是物品实例。用于快速查找。

    ---

    ### Awake 方法（单例初始化）

    ```csharp
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    ```
    - 如果 `instance` 是空，就把自己设为唯一实例
    - 如果已经有实例了，就销毁自己，保证只有一个背包管理器

    ---

    ### Start 方法（初始化容器）

    ```csharp
    private void Start()
    {
        inventoryItems = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();
    }
    ```
    在游戏开始时创建空的列表和字典，准备好装物品。

    ---

    ### AddItem 方法（添加物品）

    ```csharp
    public void AddItem(ItemData _item)
    ```
    公开方法，传入要添加的物品数据。

    ```csharp
    if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
    ```
    **TryGetValue**：尝试从字典中根据键 `_item` 获取值。
    - 如果找到：返回 `true`，并把找到的值存入 `value` 变量
    - 如果没找到：返回 `false`

    **out InventoryItem value**：`out` 是输出参数。方法内部会把找到的值赋给 `value`，方法结束后可以在外面使用。

    ```csharp
    {
        value.AddStack();
    }
    ```
    找到了，说明背包已有这个物品，调用 `AddStack()`，数量 +1。

    ```csharp
    else
    {
        InventoryItem newItem = new InventoryItem(_item);
    ```
    没找到，说明背包还没有这个物品。新建一个 `InventoryItem` 实例。

    ```csharp
        inventoryItems.Add(newItem);
        inventoryDictionary.Add(_item, newItem);
    }
    ```
    把新物品加到列表（给 UI 显示）和字典（给快速查找）。

    ---

    ### RemoveItem 方法（移除物品）

    ```csharp
    public void RemoveItem(ItemData _item)
    {
        if (inventoryDictionary.TryGetValue(_item, out InventoryItem value))
    ```
    同样用 TryGetValue 查找物品。

    ```csharp
        {
            if (value.stackSize <= 1)
            {
                inventoryItems.Remove(value);
                inventoryDictionary.Remove(_item);
            }
    ```
    如果数量只有 1 个，删除后就没东西了，直接从列表和字典中移除。

    ```csharp
            else
                value.RemoveStack();
        }
    }
    ```
    如果数量大于 1，只是减少数量，不移除整个物品。

    ---

    ## 关键方法/关键字用法总结

    | 方法/关键字 | 作用 | 例子 |
    |-------------|------|------|
    | `TryGetValue` | 安全地从字典取值，不报错 | `dict.TryGetValue(key, out value)` |
    | `out` | 方法内部赋值，外部能用 | `out InventoryItem value` |
    | `Dictionary<TKey, TValue>` | 键值对集合，查找快 | `Dictionary<ItemData, InventoryItem>` |
    | `List<T>` | 有序列表 | `List<InventoryItem>` |
    | `static` | 属于类本身，不属于对象 | `public static Inventory instance` |

    ---

    ## 流程图

    ```
    添加物品：
        ↓
    字典里有这个物品吗？
        ├── 有 → 数量 +1
        └── 没有 → 新建物品，加到列表和字典

    移除物品：
        ↓
    字典里有这个物品吗？
        ├── 没有 → 什么都不做
        └── 有 → 数量 > 1？
                    ├── 是 → 数量 -1
                    └── 否 → 从列表和字典中删除
    ```


    bool 是否存在 = 字典.TryGetValue(键, out 变量);

    int value = dict["不存在的键"];  // KeyNotFoundException 崩溃 所以用try TryGetValue

    out   作用  让方法可以返回多个值。普通方法只能 return 一个值，用 out 可以额外带出多个值。

    void 方法名(参数, out 类型 变量名)
    {
        变量名 = 值;  // 必须赋值
    }

    void SplitNumber(float num, out int integer, out float fraction)
    {
        integer = (int)num;           // 整数部分
        fraction = num - integer;     // 小数部分
    }

    TryGetValue 返回的 true/false 判断的是：字典里有没有这个键。

    out 出来的变量只是把对应的值取出来，不参与判断。

    字典是键值对（Key-Value Pair），键和值是绑定的

    值可以为 null 的情况（引用类型）
    如果值的类型是引用类型（如 string、object、自定义类），值可以是 null：


    Tip:这是 C# 中的 as 类型转换。尝试把 _item 转换为 ItemData_Equipment 类型，如果转换成功，就返回该类型对象；如果失败（_item 不是 ItemData_Equipment），就返回 null。
 
 
 */



















/*
 
类	比喻
ItemData	商品卡片（名字、图标）
InventoryItem	购物清单（卡片 + 数量）
Inventory	背包（管理所有清单）
ItemObject	地上的实物（触碰后变成清单）


ItemData (ScriptableObject)     (物品/类   物品的“蓝图”   例：一块泥土与图标对应)
    ↓ 被引用
InventoryItem (普通类)            （泥土/数量64 泥土有多少个）
    ↓ 被使用
Inventory (背包管理器)           （玩家背包E）
    ↓ 触发生成
ItemObject (场景物体)               （地上的掉落物泥土）
 */

/*
 
## 四个脚本的作用和逻辑

---

### 一、核心关系图

```
ItemData (ScriptableObject)     ← 物品的“蓝图”（名字、图标）
    ↓ 被引用
InventoryItem (普通类)           ← 物品的“拥有记录”（什么物品 + 多少个）
    ↓ 被使用
Inventory (背包管理器)           ← 背包本身（管理所有拥有的物品）
    ↓ 触发生成
ItemObject (场景物体)           ← 地上的物品（捡起来就变成背包里的数据）
```

---

### 二、逐段解释

#### 1. ItemData（数据模板）

```csharp
[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
}
```

**作用**：定义物品的固定信息（名字、图标）。

**怎么创建**：在 Project 窗口右键 → Create → Data → Item，会生成一个 `.asset` 文件，填好名字和图标。

**为什么用 ScriptableObject**：不依附于场景，所有地方共用同一份数据，改一个地方所有引用都跟着变。

**比喻**：商品目录卡，写着“药水”长什么样、叫什么。

---

#### 2. InventoryItem（背包里的记录）

```csharp
[Serializable]
public class InventoryItem
{
    public ItemData data;   // 这个物品是什么（引用上面的 ItemData）
    public int stackSize;   // 有多少个

    public InventoryItem(ItemData _newItemData)
    {
        data = _newItemData;
        AddStack();
    }

    public void AddStack() => stackSize++;
    public void RemoveStack() => stackSize--;
}
```

**作用**：记录“我拥有某个物品，以及拥有多少个”。

**为什么需要它**：ItemData 是模板，不能改数量。InventoryItem 加了一个 stackSize 来记录数量。

**构造函数**：创建时自动把数量设为 1。

**比喻**：购物清单上的一行——“药水 x3”。

---

#### 3. Inventory（背包管理器）

```csharp
public static Inventory instance;
public List<InventoryItem> inventoryItems;           // 给 UI 显示用
public Dictionary<ItemData, InventoryItem> inventoryDictionary; // 给快速查找用
```

**为什么用两个容器**：
- **List**：按顺序存，UI 显示时直接遍历
- **Dictionary**：查找某个物品有没有、有几个，O(1) 速度

**单例模式**：`public static Inventory instance`，任何地方都能用 `Inventory.instance.AddItem(...)` 访问。

**添加逻辑**：
1. 先查字典，看背包里有没有这个物品
2. 有 → 数量 +1
3. 没有 → 新建 InventoryItem，同时加到 List 和 Dictionary

**移除逻辑**：
1. 查字典，没有就结束
2. 有 → 数量 > 1？减数量；数量 = 1？直接从两个容器中删除

**比喻**：你的背包，知道里面有什么，也知道每个有多少。

---

#### 4. ItemObject（场景中的物品）

```csharp
[SerializeField] private ItemData itemData;

private void OnValidate()
{
    GetComponent<SpriteRenderer>().sprite = itemData.icon;
    gameObject.name = "Item object - " + itemData.itemName;
}

private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.GetComponent<Player>() != null)
    {
        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }
}
```

**作用**：挂在场景中的物品模型上。玩家碰到时，把物品加到背包，然后自己消失。

**OnValidate**：在 Inspector 中修改 itemData 时，自动更新图标和名字（方便策划配置时预览）。

**比喻**：地上的一瓶药水。玩家走过去碰一下，药水被捡起（加到背包），地上的模型消失。

---

### 三、完整流程

```
1. 在 Unity 中创建 ItemData（比如“药水”，设置名字和图标）
   ↓
2. 创建一个场景物体，挂上 ItemObject 脚本，把 ItemData 拖进去
   ↓
3. 玩家触碰该物体
   ↓
4. ItemObject 调用 Inventory.instance.AddItem(药水数据)
   ↓
5. Inventory 检查背包里是否有药水
   ├── 有 → 对应 InventoryItem 的 stackSize +1
   └── 没有 → 新建 InventoryItem，存到 List 和 Dictionary
   ↓
6. 捡起成功，ItemObject 销毁
```

---

### 四、以后怎么写这种东西？

#### 你需要记住的核心模式

| 角色 | 类 | 特点 |
|------|---|------|
| 数据模板 | `ScriptableObject` | 固定数据，可复用，不依附场景 |
| 运行时记录 | 普通类 + `[Serializable]` | 动态数据（如数量），要在 Inspector 显示 |
| 管理器 | 单例 `MonoBehaviour` | 全局唯一，管理所有实例 |
| 场景交互 | `MonoBehaviour` + 触发器 | 挂载在场景物体上，响应玩家交互 |

#### 写新物品系统时按这个顺序思考

1. **这个物品有什么固定属性？** → 写 `ItemData`（ScriptableObject）
2. **背包里需要记录什么动态信息？** → 写 `InventoryItem`（普通类，加 stackSize）
3. **谁来管理所有物品？** → 写 `Inventory`（单例 + List + Dictionary）
4. **场景中的物品怎么表现？** → 写 `ItemObject`（触发器 + 调用 Inventory）

---

### 五、简单记

| 类 | 比喻 | 核心代码 |
|---|------|---------|
| `ItemData` | 商品卡片 | `ScriptableObject` |
| `InventoryItem` | 购物清单 | `public ItemData data; public int stackSize;` |
| `Inventory` | 背包 | `static instance` + `Dictionary` + `List` |
| `ItemObject` | 地上的实物 | `OnTriggerEnter2D` → `Inventory.instance.AddItem` |


**口诀：数据用 SO，数量加个类，背包单例管，场景触发器。**
 */








/*

Grid Layout Group 是干什么的
一句话解释
自动把子物体排列成整齐的网格（几行几列的表格）。
 */




/*
 Tip:
 代码	意思
new InventoryItem(_item)	用 _item 的数据造一个新背包物品
InventoryItem newItem = ...	把这个新物品存到 newItem 变量里
 */

/*
 
    Tip:

    foreach 是 C# 中的一种循环语句，用来遍历集合（数组、列表、字典等）中的每一个元素。

    基本语法
    csharp
    foreach (类型 变量名 in 集合)
    {
        // 对每个元素执行的操作
    }
    示例
    遍历数组
    csharp
    int[] numbers = { 10, 20, 30, 40 };

    foreach (int num in numbers)
    {
        Debug.Log(num);  // 依次输出 10, 20, 30, 40
    }
    遍历 List
    csharp
    List<string> names = new List<string> { "张三", "李四", "王五" };

    foreach (string name in names)
    {
        Debug.Log(name);  // 依次输出 张三, 李四, 王五
    }
    遍历 Dictionary
    csharp
    Dictionary<string, int> scores = new Dictionary<string, int>();
    scores.Add("张三", 100);
    scores.Add("李四", 90);

    foreach (KeyValuePair<string, int> pair in scores)
    {
        Debug.Log(pair.Key + "的分数：" + pair.Value);
    }
 
 */



/*
    Tip:删除字典，foreach里面不能用remove，得先保存，在外面删 
 
     为什么要用 itemToDelete = null
    因为在遍历字典的同时，不能直接修改字典。你正在用 foreach 遍历 equipmentDictionary，如果在遍历过程中直接调用 Remove，会报错：

    Collection was modified; enumeration operation may not execute.

    所以先找到要删的键（itemToDelete）存起来，等遍历结束再删。

    正常删除字典键值对
    csharp
    // 直接删除
    equipmentDictionary.Remove(键);
    csharp
    // 示例
    equipmentDictionary.Remove(equipmentToRemove);  // 直接删
    为什么你的代码要分两步？
    csharp
    // 第一步：先找
    ItemData_Equipment itemToDelete = null;
    foreach (var item in equipmentDictionary)
    {
        if (item.Key.equipmentType == newEquipment.equipmentType)
            itemToDelete = item.Key;  // 只记录，不删除
    }

    // 第二步：遍历结束后再删
    if (equipmentDictionary.TryGetValue(itemToDelete, out InventoryItem value))
    {
        equipment.Remove(value);
        equipmentDictionary.Remove(itemToDelete);
    }
    因为：

    你不能在 foreach 循环里删字典

    删字典需要键（item.Key），但循环里不能用 Remove

    所以先把键记下来，循环结束再删
 
 */



/*
 
    for 用索引访问，foreach 直接拿元素。foreach 更简单但不能修改集合，for 更灵活但需要处理索引。
    Dictionary 没有索引器，所以不能直接用 for (int i = 0; i < dictionary.Count; i++)。
 */




/*
    Tip:UI_CraftSlot    合成

    玩家点击合成按钮
        ↓
    CanCraft(要合成的装备, 所需材料列表)
        ↓
    遍历所需材料列表，检查背包库存
        ├── 有材料且足够 → 继续检查
        ├── 有材料但不够 → 返回 false，合成失败
        └── 没有该材料 → 返回 false，合成失败
        ↓
    所有材料都够 → 扣除材料
        ↓
    给玩家新装备
        ↓
    返回 true，合成成功
 





 
    所需材料列表：[木材×3, 铁锭×2, 皮革×1]

    检查背包：
    ├── 木材 × 5 → ✅ 够，记下要扣 3 个
    ├── 铁锭 × 1 → ❌ 不够（需要2个）→ 返回 false，合成失败

    所需材料列表：[木材×3, 铁锭×2, 皮革×1]

    检查背包：
    ├── 木材 × 5 → ✅ 够
    ├── 铁锭 × 3 → ✅ 够
    └── 皮革 × 1 → ✅ 够
        ↓
    扣除材料：木材 -3，铁锭 -2，皮革 -1
        ↓
    添加新装备到背包
        ↓
    合成成功！


 */