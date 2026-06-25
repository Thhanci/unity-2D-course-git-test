using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;// 重要：必须引入此命名空间 //IpointerDownHandler

public class UI_ItemSlot : MonoBehaviour ,IPointerDownHandler   // UI展示   //Tip:UI排序，unity组件Grid Layout group
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemText;

    public InventoryItem item;

    //void Start()
    //{
    //    UpdateSlot();

    //}

    public void UpdateSlot(InventoryItem _newItem)
    {
        item = _newItem;

        itemImage.color = Color.white;

        if (item != null)
        {
            itemImage.sprite = item.data.icon;

            if (item.stackSize > 1)
            {
                itemText.text = item.stackSize.ToString();  //ToString()把任何数据转换成字符串（文字形式)

            }
            else
            {
                itemText.text = "";
            }
        }
    }

    public void ClearUpSlot()
    { 
        item=null;

        itemImage.sprite=null;
        itemImage.color = Color.clear;
        itemText.text = "";
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        //Debug.Log("Equiped new item+ " + item.data.itemName);

        if (Input.GetKey(KeyCode.LeftControl))
        {

            //ItemDrop itemDrop = FindObjectOfType<ItemDrop>();
            //itemDrop.DropItem(item.data);   //protected

            Inventory.instance.RemoveItem(item.data);
            return;
        }

        if (item != null && item.data != null && item.data.itemType == ItemType.Equipment)   //Tip:原 item.data.itemType == ItemType.Equipment     修改 item != null && item.data != null  && item.data.itemType == ItemType.Equipment      Debug.Log("Equiped new item+ " + item.data.itemName);
            Inventory.instance.Equipment(item.data);    //因为在UI_ItemSlot的鼠标触发事件中，没有判断当前点击的slot中的InventoryItem是否为null,没有判空，可能报错，解决方法，把条件改成上一行注释

    }



}



/*
if (eventData == null)
{
    Debug.Log("eventData == null");
}
else if (eventData.pointerPress != gameObject)
{
    Debug.Log("eventData.pointerPress != gameObject，点击的不是这个格子");
}
else if (item == null)
{
    Debug.Log("item == null，格子是空的");
}
else if (item.data == null)
{
    Debug.Log("item.data == null，物品数据丢失");
}
else
{
    Debug.Log("item.data.itemName: " + item.data.itemName);
    Debug.Log("item.data.itemType: " + item.data.itemType);

    if (item.data.itemType == ItemType.Material)
        Debug.Log("点击了材料: " + item.data.itemName);

    if (item.data.itemType == ItemType.Equipment)
        Debug.Log("Equiped new item: " + item.data.itemName);
}
*/

/*
     rect transform
    按shift alt移动文本中心点和文本位置

        IPointerDownHandler 是 Unity 事件系统（EventSystem）中的一个接口，用于检测和处理指针（鼠标、触摸等）在 UI 元素上按下的操作。
    当你在一个 UI 物体（如 Image、Button）上挂载一个实现了该接口的脚本时，当鼠标或手指在该物体上按下时，OnPointerDown 方法会被自动调用。

    throw 关键字
        throw 是 C# 中用于主动抛出异常的关键字。当程序执行到 throw 语句时，它会立即中断当前方法的正常流程，并向上层调用栈抛出一个异常对象。
        目的：throw 用于在代码中显式地标记一个错误或未实现的状态，而不是让程序在不确定的情况下继续运行。
        用法：throw 后面需要跟着一个继承自 System.Exception 的异常对象。


    System.NotImplementedException
        这是一个内置异常类型，专门用于表示“这个方法或功能尚未实现”。
        含义：字面意思是“未实现异常”。它是一个占位符，非常明确地告诉开发者（以及未来的你）：“嘿，这个方法的逻辑还没写，需要我来补充完整！”
    使用场景：通常用于以下几种情况：
        接口方法的默认实现：在实现接口时，如果你暂时不想写具体逻辑，先用 NotImplementedException 占位，这样代码就可以编译通过。
        作为待办事项（TODO）：这是一个比注释 // TODO 更“强有力”的提醒。调用这个方法会直接报错，迫使你或团队其他成员尽快实现它。
    标记过时或不应调用的方法：在某些情况下，也可以用它来标记一个虽然存在但当前不应被使用的方法。

    PointerEventData eventData 是什么
    这是 Unity 事件系统传递给你的“数据包”，里面装着本次点击/触摸操作的详细信息。

 */

/*
    Tip:
        Canvas 的“中心点”是它的局部坐标原点，但 Canvas 本身不受 3D 世界位置影响，它始终覆盖整个屏幕。
    所以即便 Canvas 的中心在场景中看上去在地图外面，它的内容依然会显示在屏幕上，因为它是 UI。
 
 */