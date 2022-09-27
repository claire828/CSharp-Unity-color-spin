using Common.Settings;
using DG.Tweening;
using HeavyDutyInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSpinTickerItem : MonoBehaviour, IBasicTickerItem
{
    public Text AmountTxt;
    public Image Img;

     [Comment("背景Sprites 共七組", CommentType.Info)]
    public Sprite[] BGSprites; //0123456

     [Comment("字體類型 共五組", CommentType.Info)]
     public Font[] FontStyles; //21345

    [Comment("兩個Pot的特效Prefab", CommentType.Info)]
    public GameObject[] PotEffects;

    [Comment("火焰特效", CommentType.Info)]
    public GameObject FireEffects;

 

    [Comment("特效的容器", CommentType.Info)]
    public GameObject EffectContainer;

    [Comment("結束的爆炸特效容氣", CommentType.Info)]
    public GameObject EndingEffectContainer;

    [Comment("倒過來", CommentType.Info)]
    public Text NumberTxt;


  


    Tween TweenMaxwin;

    #region Field
    public int Index { get; set; }
    public int TargetID { get; set; }
    private int StartIndexForJson { get; set; }
    private ColorSpinBonusReelStruct BarStruct { get; set; }

    private int _acculumateBet;
    public double FinalBet { 
        get
        {
          //  DebugEx.Log("id:", this.TargetID, " defalut:", BarStruct.GetAmountByBaseBet, " accu:", GetAmountByAcculumateBet, " tatal:", BarStruct.GetAmountByBaseBet + GetAmountByAcculumateBet, "  _acculumateBet:", _acculumateBet);
            return this.BarStruct.GetAmountByBaseBet + GetAmountByAcculumateBet; 
        } 
    }
    public double GetAmountByAcculumateBet { get { return _acculumateBet * BarStruct.GetBetPrice; } }
    public bool IsNormalType { get { return BarStruct.ColorIDinSetting <= 5; } }

    private bool IsFireBGType { get { return BarStruct.ColorIDinSetting == 5; } }

    #endregion


    private double _tempNumForTweenNeeded;

    public void AddIncreasingBet(int bet)
    {
        _acculumateBet = bet;
    }


    public void InsertItemIndex(int index)
    {
        this.Index = index;
    }


    public void AddStartIndex(int index)
    {

        StartIndexForJson = index;
    }


    public void UpdateItem()
    {
        this.TargetID = GetTargetIDbyIndex(Index + StartIndexForJson);
        BarStruct = new ColorSpinBonusReelStruct(TargetID);
        UpdateFont();
        Img.ExSetSprite(BGSprites[BarStruct.ColorIDinSetting-1],false);
    }


    private void UpdateFont()
    {
        EffectContainer.ExRemoveAllChildren();
        StyleFireEffectForPueple();
        if (IsNormalType)
            StyleFont();
        else
            AddingPotEffect();
    }

    private void StyleFont()
    {
        
        NumberTxt.font = FontStyles[BarStruct.ColorIDinSetting - 1];
        NumberTxt.text = FinalBet.ExTransferToBMK();
        this._tempNumForTweenNeeded = FinalBet;
    }

    private void StyleFireEffectForPueple()
    {
        this.FireEffects.SetActive(IsFireBGType);
    }

    private void AddingPotEffect()
    {
        NumberTxt.text = "";
        EffectContainer.ExAddChild(PotEffects[BarStruct.ColorIDinSetting - 6]);
    }

    public void DoNumbers(bool dotween)
    {
        if (!IsNormalType) return;

        KillTween();
        if (dotween)
        {
            //這邊可能是 1200- > 1400 但這樣都是1K  所以應該怎麼跑比較好?  要確認  
            DebugEx.Log("id:" + this.TargetID + "  原本:" + _tempNumForTweenNeeded + "  之後:" + FinalBet);

            TweenMaxwin = DOTween.To
           (
               () => _tempNumForTweenNeeded,
               (x) => NumberTxt.text = x.ExTransferToBMK(),
               FinalBet,
               5.0f
           ).SetAutoKill(false);

            TimerEx.Countdown(5f).OnCompleted((x) => { this._tempNumForTweenNeeded = FinalBet; });
        }
        else
        {
            NumberTxt.text = FinalBet.ExTransferToBMK();
        }
       
    }


    public void SetCorrectInfo(int id, int bet)
    {
        this.TargetID = id;
        this.AddIncreasingBet(bet);
        BarStruct = new ColorSpinBonusReelStruct(TargetID);
        UpdateFont();
        Img.ExSetSprite(BGSprites[BarStruct.ColorIDinSetting - 1], false);
    }



    public IEnumerator PlayEndingFireByBar(GameObject effects)
    {
        var effect = this.EndingEffectContainer.ExAddChild(effects);
        string stateName = IsNormalType ? "Slot56_bst_TotalwinNormal" : "Slot56_bst_TotalwinPot";
        effect.GetComponentInChildren<Animator>().Play(stateName);
        this.EndingEffectContainer.transform.SetAsLastSibling();
        yield return new WaitForSeconds(1.9f);
        this.EndingEffectContainer.transform.SetSiblingIndex(this.transform.childCount - 2);

        if (IsNormalType) yield break;
        DebugEx.Log("我要播ㄌ");
        if (PotEffect) PotEffect.LoopEffectPlay();
        
    }






    public void ClearEndEffect()
    {
        this.EndingEffectContainer.ExRemoveAllChildren();
        if (!IsNormalType&& PotEffect) PotEffect.LoopEffectStop();
    }


    private ColorSpinPotEffect PotEffect { get { return this.EffectContainer.GetComponentInChildren<ColorSpinPotEffect>(); } }



    #region weel資料
    private Json WeelList { get { return CSSettingsV3.Slot056.WheelClient[9999].SymbolList; } }

    private int GetTargetIDbyIndex(int index)
    {
        int total = WeelList.Count;
        if (index >= total)
        {
            int substitute = index-total;
            return WeelList[substitute].IntValue;
        }
        else
        {
            return WeelList[index].IntValue;
        }
    }
    #endregion


    void OnDestroy()
    {
        KillTween();
    }


    private void KillTween()
    {
        if (TweenMaxwin != null) TweenMaxwin.Kill();
    }
}


#region ReelStruct資料結構

struct ColorSpinBonusReelStruct
{
  

    public int TargetID { get; private set; }
 
    public double GetAmountByBaseBet { get { return GetBetPrice * PayInSetting; } }

    public double PayInSetting { get { return CSSettingsV3.Slot056.BonusInfo[TargetID].BonusPay; } }
    public double GetBetPrice { get { return SlotController.CommonData.GetTotalBet(56); } }
    public int ColorIDinSetting { get { return CSSettingsV3.Slot056.BonusInfo[TargetID].BonusColor; } }



    public ColorSpinBonusReelStruct( int targetID)
    {
        TargetID = targetID;
    }


}


#endregion