using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EquipmentSlot : UI_ItemSlot //Tip:unity prefab unpack completely  可以解除与原始prefab的关联，使其转换为普通的游戏层级对象
{
    public EquipmentType slotType;  //对应  UI_ItemSlot.cs  line11   public InventoryItem item;

    private void OnValidate()
    {
        gameObject.name ="Equipment slot - "+slotType.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        //base.OnPointerDown(eventData);//删掉后，父类中 OnPointerDown 里的逻辑不会执行，只会执行子类里自己写的代码。
        Inventory.instance.UnequipItem(item.data as ItemData_Equipment);//父类转子类
        Inventory.instance.AddItem(item.data as ItemData_Equipment);
        ClearUpSlot();


        //这里有一个bug，点击空的物品栏格子会报错，
        //我的办法是在onpointerdown函数里加一个if当所点击的sprite等于null的时候return
        //Tip：详见UI_ItemSlot.cs      virtual OnPointerDown(PointerEventData eventData)
    }

}


/*
 
    override = 告诉编译器“我重写了父类方法”。删不删 base 是另一个问题：删了 = 完全替代，不删 = 先执行父类再加自己的。
    
    父类有 virtual 方法，你写了一个同名方法 → 会警告（隐藏了父类方法）	明确告诉编译器：“我知道父类有这个方法，我是在重写它，不是隐藏它”
    
 */