using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject_Trigger : MonoBehaviour
{
    private ItemObject myItemObject => GetComponentInParent<ItemObject>();

    private void OnTriggerEnter2D(Collider2D collision)    //Tip：拾取物品
    {
        if (collision.GetComponent<Player>() != null)
        {
            if (collision.GetComponent<CharacterStats>().isDead)
                return;

            //Debug.Log("Picked up item " + itemData.itemName);
            Debug.Log("Picked up item " );
            myItemObject.PickupItem(); 
        }
    }


}
