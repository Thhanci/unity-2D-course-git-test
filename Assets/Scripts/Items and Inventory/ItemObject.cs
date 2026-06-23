using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour //作用：挂在场景中的物品上。玩家碰到时，把物品加到背包，然后自身消失。
{
    //private SpriteRenderer sr;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ItemData itemData;
    //[SerializeField] private Vector2 velocity;

    //private void Start()
    //{
    //    sr = GetComponent<SpriteRenderer>();

    //    sr.sprite = itemData.icon;
    //}

    //private void OnValidate()  //Unity 编辑器中的校验函数。当你在 Inspector 面板修改脚本参数时，会自动执行。
    //{
    //    SetupVisuals();
    //}

    private void SetupVisuals()
    {
        if (itemData == null)
            return;

        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "Item object - " + itemData.itemName;
    }

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.M))
    //        rb.velocity = velocity;
    //}

    public void SetupItem(ItemData _itemData, Vector2 _velocity)
    { 
        itemData=_itemData;
        rb.velocity = _velocity;

        SetupVisuals();
    }


    public void PickupItem()
    {
        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }
}


/*

在 Unity 编辑器中，当脚本的 Inspector 参数被修改时自动调用的函数。

用途	例子
数值限制	防止血量负值、速度超范围
自动赋值	自动获取组件引用
数据验证	检查配置是否正确，不正确的自动修正
更新表现	修改数值时同步更新 UI 预览
 */


/*
    Item object - No data     layer 是item
    ItemTrigger                 layer是default，因为trigger是触发器
 */