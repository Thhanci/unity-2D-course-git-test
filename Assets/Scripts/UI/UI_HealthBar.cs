using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //与slide有关，缺少就在下面slider按Alt+enter补齐此行

public class UI_HealthBar : MonoBehaviour
{
    private Entity entity;
    private CharacterStats myStats;
    private RectTransform myTransform;
    private Slider slider;

    private void Start()
    {
        myTransform = GetComponent<RectTransform>();
        entity = GetComponentInParent<Entity>();
        slider = GetComponentInChildren<Slider>();//tip：因InParent没找到导致报错
        myStats = GetComponentInParent<CharacterStats>();

        entity.onFlipped += FlipUI;
        myStats.onHealthChanged += UpdateHealthUI;  // 订阅事件：血量变化时，自动调用 UpdateHealthUI

        UpdateHealthUI();//在游戏开始前更新游戏AI

        //Debug.Log("health bar ui called");
    }

    //private void Update(){ UpdateHealthUI();}//血条实时更新会影响性能，改用委托（事件）

    private void UpdateHealthUI()
    {
        slider.maxValue = myStats.GetMaxHealthValue();
        slider.value = myStats.currentHealth;
    }

    private void FlipUI()=>myTransform.Rotate(0,180,0);//目前：玩家没有这个UI，  //Debug.Log("Entity is flipped");

    private void OnDisable()
    {
        entity.onFlipped -= FlipUI;
        myStats.onHealthChanged -= UpdateHealthUI;
    }
}
