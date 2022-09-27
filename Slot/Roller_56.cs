using System.Collections;
using System.Collections.Generic;
using Common.Audio;
using Common.Settings;
using DG.Tweening;
using HeavyDutyInspector;
using UnityEngine;
using UnityEngine.UI;

public class Roller_56 : RollerType1
{
   
    //[Comment("BonusIncrease", CommentType.Info)]
    public BonusGameIncrease_56 IncreasePanel;

     private ColorSpinExecute UpperTickerUI { get { return MainState.UpperTickerUI; } }
     private MainState_56 MainState { get { return ((MainState_56)this._controller.GetState(GameType.MAIN)); } }


     public override void BetClickEvent()
     {
         base.BetClickEvent();
         this.UpperTickerUI.Containers.ForEach(x=>
             {
                 x.ItemList.ForEach(item => item.UpdateItem());
             }
         );
     }






 

}