using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// ColorSpin BonusGame下方單獨的Bar
/// </summary>
public class BarInBottomSide : MonoBehaviour
{
   // public Animator BarEffect;

    public Image BarBG;
    public GameObject BlurEffect;
    public GameObject FireEffect;
    private Animator BlurAnimator;

    public int ID { get; private set; }
    public void Initial(int id)
    {
        this.ID = id;
        this.BlurEffect.SetActive(true);
        this.BlurAnimator = this.BlurEffect.GetComponentInChildren<Animator>();
        this.BlurAnimator.Play("Slot56_bs_BonusIdle");
        this.FireEffect.SetActive(false);
    }


    /// <summary>
    /// 播放一開始的快轉模糊
    /// </summary>
    public void ShowBeginingBlur()
    {
       this.BlurEffect.SetActive(true);
       this.BlurAnimator.Play("Slot56_bs_BonusRollWithFire1");
    }


    /// <summary>
    /// 聽牌(決定有無火焰)
    /// </summary>
    public void RevellingEffect()
    {
        
        this.BlurAnimator.Play("Slot56_bs_BonusStopWithoutFire");
    }


    /// <summary>
    /// 中獎的火焰
    /// </summary>
    public void ShowEffect()
    {
        this.FireEffect.SetActive(true);
        this.BlurEffect.SetActive(false);
    }




}
