using System;

[Serializable]  //让 Unity 在 Inspector 窗口中显示和保存自定义类的数据。
//作用：记录“我拥有多少个某物品”。把数据和数量绑定在一起。（背包里的物品）
public class InventoryItem   //Tip：构造函数
{
    public ItemData data;
    public int stackSize;

    public InventoryItem(ItemData _newItemData)//这是构造函数（Constructor）  //构造函数 = 创建对象时自动执行的方法，用来初始化对象。名字和类名一样，没有返回值。
    {
        data = _newItemData;
        AddStack();
        //TOOO: add to stack
    }

    public void AddStack() => stackSize++;
    public void RemoveStack() => stackSize--;


}

