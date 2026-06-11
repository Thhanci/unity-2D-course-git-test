using UnityEngine;

public class ItemData : ScriptableObject  //脚本化对象 //这是一种区别于生命周期的一种类型，一般用来存储一些固定数据
{
    public string itemName;
    public Sprite icon;
}


/*
 
ScriptableObject 是 Unity 中一种特殊的数据容器，用于存储大量数据，不依附于场景中的 GameObject。

 MonoBehaviour	演员（在场景中表演，每场戏都有自己的状态）
ScriptableObject	剧本（不表演，存着角色的设定，所有演员共用同一份剧本）
 */