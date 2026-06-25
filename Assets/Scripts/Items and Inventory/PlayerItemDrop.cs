using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player's drop")]
    [SerializeField] private float chanceToLooseItems;
    [SerializeField] private float chanceToLooseMaterials;

    public override void GenerateDrop()
    {
        //base.GenerateDrop();

        //list of equipment

        //foreach item we gonna check if should loose item

        Inventory inventory = Inventory.instance;

        //List<InventoryItem> currentStash = inventory.GetStashList();
        //List<InventoryItem> currentEquipment = inventory.GetEquipmentList();
        
        List<InventoryItem> itemsToUnequip = new List<InventoryItem>();
        List<InventoryItem> materialsToLoose = new List<InventoryItem>();  //materialsToLoose


        foreach (InventoryItem item in inventory.GetEquipmentList())  //把当前装备按掉落概率加入掉落物列表
        {
            if (Random.Range(0, 100) <= chanceToLooseItems)
            {
                DropItem(item.data);
                itemsToUnequip.Add(item);
            }
        }

        for (int i = 0;i < itemsToUnequip.Count; i++)   //掉落物列表的蓝图转换
        {
            inventory.UnequipItem(itemsToUnequip[i].data as ItemData_Equipment);
        }

        foreach (InventoryItem item in inventory.GetStashList())
        {
            if (Random.Range(0, 100) <= chanceToLooseMaterials)
            {
                DropItem(item.data);
                materialsToLoose.Add(item);
            }
        }

        for (int i = 0; i < materialsToLoose.Count; i++)
        {
            inventory.RemoveItem(materialsToLoose[i].data);
        }

    }
}
