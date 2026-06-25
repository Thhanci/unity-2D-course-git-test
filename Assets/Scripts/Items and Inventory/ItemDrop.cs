using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private int possibleItemDrop;
    [SerializeField] private ItemData[] possibleDrop;
    private List<ItemData> dropList = new List<ItemData>();

    [SerializeField] private GameObject dropPrefab;
    //[SerializeField] private ItemData item;

    public virtual void GenerateDrop()
    { 
        for (int i = 0; i < possibleDrop.Length; i++)
        {
            if(Random.Range(0,100) <= possibleDrop[i].dropChance)  //按概率把东西加入掉落物列表里
                dropList.Add(possibleDrop[i]);
        }

        for (int i = 0; i < possibleItemDrop; i++)    // possibleItemDrop 允许的掉落物数量
        {
            ItemData randomItem = dropList[Random.Range(0, dropList.Count - 1)];//Tip_hold on repair:死去的骷髅掉落物为0  possibleDrop 里没有物品 

            dropList.Remove(randomItem);
            DropItem(randomItem);

        }

    }

    protected void DropItem(ItemData _itemData)
    { 
        GameObject newDrop=Instantiate(dropPrefab,transform.position,Quaternion.identity);  //transform.position  当前脚本挂载的那个游戏对象在世界空间中的位置。

        Vector2 randomVelocity = new Vector2(Random.Range(-5,5),Random.Range(15,20));

        newDrop.GetComponent<ItemObject>().SetupItem(_itemData,randomVelocity);

        //Debug.Log("drop item");
    }

}
