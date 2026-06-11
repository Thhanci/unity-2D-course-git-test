using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Blackhole_HotKey_Controller : MonoBehaviour   //关联EventSystem  //当你第一次在场景中创建 UI 元素时（比如创建 Canvas、Button、Image），Unity 会自动在 Hierarchy 中生成 EventSystem。
{
    private SpriteRenderer sr;
    private KeyCode myHotKey;   // 存储需要按下的按键
    private TextMeshProUGUI myText;   // 显示按键文字的UI组件

    private Transform myEnemy;
    private Blackhole_Skill_Controller blackhole;

    public void SetupHotKey(KeyCode _myNewHotKey,Transform _myEnemy,Blackhole_Skill_Controller _myBlackHole)
    {
        sr= GetComponent<SpriteRenderer>();
        myText = GetComponentInChildren<TextMeshProUGUI>();   //myText 是一个变量，用来存储和控制 UI 上的文字

        myEnemy = _myEnemy;
        blackhole=_myBlackHole;  //blackhole技能控制脚本

        //  保存按键并显示
        myHotKey = _myNewHotKey;  //Q W E R T
        myText.text = _myNewHotKey.ToString();//myText.text 是文字组件上显示的"内容"
    }

    private void Update()
    {
        if(Input.GetKeyDown(myHotKey))
        {
            blackhole.AddEnemyTolist(myEnemy);
            //Debug.Log("HOT KEY IS " + myHotKey);

            myText.color = Color.clear;
            sr.color = Color.clear;
        }
    }

}


/*
 myText	整个显示屏（可以调亮度、颜色）
myText.text	屏幕上显示的文字内容
myText.color	文字的颜色
myText.fontSize	文字的大小




常用属性
属性	作用	示例
.text	文字内容	myText.text = "Hello"
.color	文字颜色	myText.color = Color.red
.fontSize	文字大小	myText.fontSize = 36
.font	字体	myText.font = myFont
.alignment	对齐方式	myText.alignment = TextAlignmentOptions.Center
.enabled	启用/禁用	myText.enabled = false
.gameObject	文字所在的物体	Destroy(myText.gameObject)
 */