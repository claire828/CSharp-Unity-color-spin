using HeavyDutyInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BonusInfo : MonoBehaviour {

    [Comment("剩餘多少spin次數機會", CommentType.Info)]
    public Text RemainingAmountTxt;

    [Comment("還要多少去贏", CommentType.Info)]
    public Text HowManyToWinTxt;

    [Comment("LastSpin", CommentType.Info)]
    public GameObject LastSpin;

    [Comment("RemainSpin", CommentType.Info)]
    public GameObject RemainSpin;


    [Comment("特效位置", CommentType.Info)]
    public GameObject EffectAboveContainer;

    [Comment("特效位置", CommentType.Info)]
    public GameObject EffectDownContainer;


    public void SetHowManyToWin(int amount)
    {
        HowManyToWinTxt.text = amount.ToString();
    }

    public void SetRemainingAmount(int amount)
    {
       // DebugEx.Log("amount:" + amount);
        RemainingAmountTxt.text = amount.ToString();
        RemainingAmountTxt.gameObject.SetActive(amount > 0);
        RemainSpin.SetActive(amount > 0);
        LastSpin.SetActive(amount == 0);
    }


    public IEnumerator AddLightingEffectAbove(GameObject effect)
    {
        var lighting = this.EffectAboveContainer.ExAddChild(effect);
        yield return new WaitForSeconds(0.5f);
        EffectAboveContainer.ExRemoveAllChildren();

    }


    public IEnumerator AddLightingEffectDown(GameObject effect)
    {
        if (LastSpin.activeInHierarchy) yield break;
        var lighting = this.EffectDownContainer.ExAddChild(effect);
        yield return new WaitForSeconds(0.5f);
        EffectDownContainer.ExRemoveAllChildren();
    }


}
