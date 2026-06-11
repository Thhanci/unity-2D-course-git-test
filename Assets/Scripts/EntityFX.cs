using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFX : MonoBehaviour
{

    private SpriteRenderer sr;   //例：sr是具体的电视（这个电视），SpriteRenderer只是"电视"这个词本身(类)

    [Header("Flash FX")]
    [SerializeField] private float flashDuration;
    [SerializeField] private Material hitMat;
    private Material originalMat;

    [Header("Ailment colors")]  //Ailment  n.疾病，小病；不安
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] chillColor;//Color 是 Unity 内置的结构体（struct），专门用来存储颜色。
    [SerializeField] private Color[] shockColor;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMat = sr.material;
    }

    public void MakeTransparent(bool _transparent)  //transprent
    {
        if (_transparent)
            sr.color = Color.clear;
        else
            sr.color = Color.white;  //白色，RGB 值为 (1, 1, 1)，完全不透明。
    }

    private IEnumerator FlashFX()
    {
        sr.material = hitMat;
        Color currentColor = sr.color;
        sr.color=Color.white;

        yield return new WaitForSeconds(flashDuration);

        sr.color = currentColor;
        sr.material = originalMat;
    }

    private void RedColorBlink()
    { 
        if(sr.color!=Color.white)   
            sr.color = Color.white;
        else
            sr.color = Color.red;
    }

    private void CancelColorChange()
    {
        CancelInvoke();   // 停止所有InvokeRepeating   CancelInvoke("方法")	取消指定方法的重复调用
        sr.color = Color.white;   //IsInvoking("方法")	检查方法是否在等待执行
    }

    public void IgniteFxFor(float _seconds)
    {
        InvokeRepeating("IgniteColorFx", 0, .3f); //InvokeRepeating("方法名", 开始延迟, 重复间隔);//立即开始（0秒后）   //每隔 0.3 秒调用一次 IgniteColorFx()   //会一直重复，直到用 CancelInvoke() 停止
        Invoke("CancelColorChange", _seconds);  //Invoke("方法名", 延迟秒数);等待 _seconds 秒后    //调用一次 CancelColorChange()    //然后结束
    }
    public void ChillFxFor(float _seconds)
    {
        InvokeRepeating("ChillColorFx", 0, .3f);
        Invoke("CancelColorChange", _seconds);  //_seconds秒后停止调用InvokeRepeating,CancelInvoke();  
    }

    public void ShockFxFor(float _seconds)
    {
        InvokeRepeating("ShockColorFx", 0, .3f); 
        Invoke("CancelColorChange", _seconds);  
    }



    private void IgniteColorFx()
    {
        if (sr.color != igniteColor[0])
            sr.color = igniteColor[0];
        else
            sr.color = igniteColor[1];
    }
    private void ChillColorFx()
    {
        if (sr.color != chillColor[0])
            sr.color =  chillColor[0];
        else            
            sr.color =  chillColor[1];
    }
    private void ShockColorFx()
    {
        if (sr.color != shockColor[0])
            sr.color =  shockColor[0];
        else
            sr.color = shockColor[1];
    }


}


/*
 方法	            作用	        执行次数
InvokeRepeating	  重复执行	   无限次，直到取消
Invoke	         执行一次	   单次
 */