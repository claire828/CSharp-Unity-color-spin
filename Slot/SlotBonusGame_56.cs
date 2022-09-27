using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using HeavyDutyInspector;
using DG.Tweening;
using Common.Settings;
using Common.Audio;
using System.Collections.Generic;
using UniLinq;
using System;


public class SlotBonusGame_56 : BaseSlotBonusGame
{

    [Comment("BottomContain", CommentType.Info)]
    public GameObject BottomContain;
    [Comment("Bottom", CommentType.Info)]
    public BottomSideContent BottomUI;
    [Comment("下方長調", CommentType.Info)]
    public BonusInfo RoundInfo;
    [Comment("結算面版Prefab", CommentType.Info)]
    public GameObject EndGameObj;
   // [Comment("結算面版", CommentType.Info)]
    private BonusGameIncrease_56 EndGamePanel;
     [Comment("結算面版 JP - Super- mini", CommentType.Info)]
    public Transform[] JPTransformsPos;
     [Comment("RollerBG", CommentType.Info)]
     public GameObject RollerBG;
    private CheckStep CurrentStep { get; set; }
    private ColorSpinAnalysisAssist ResultStruct { get; set; }



    #region enum 執行條件 - 可對照流程圖更明確
    enum CheckStep
    { 
        StartByRemainAmount ,
        CheckFullfillResult,
        CheckFinalRound,
        CheckShouldWinThisTime,
        CheckWinIDByRandom,
        PerformanceWin,
        PerformanceLose,
        PerformanceEnding,
        Finish
    }
    #endregion



    #region 初始化
    protected override IEnumerator ImplementReadyShow(object[] showData)
    {
        this.AddExtraPanel();
        ResultStruct = new ColorSpinAnalysisAssist((Json)showData[0]);
        CurrentStep = CheckStep.StartByRemainAmount;
        this.BottomContain.GetComponent<CanvasGroup>().alpha = 1;
        this.RollerBG.gameObject.GetComponent<CanvasGroup>().alpha = 1;
        this.RoundInfo.gameObject.GetComponent<CanvasGroup>().alpha = 1;
        BottomUI.CreateItemsInContainer();
        RoundInfo.SetHowManyToWin(ResultStruct.LackAmountToJP);
        RoundInfo.SetRemainingAmount(ResultStruct.CurrentRemainingAmount);
        MainState.Roller.SetPotActivite(true);
        CoroutineV2.StartCoroutine(EnteringBonusGame());
        yield break;
    }

    private void AddExtraPanel()
    {
        this.EndGamePanel = this.gameObject.ExAddChild(EndGameObj).GetComponent<BonusGameIncrease_56>();
        this.EndGamePanel.gameObject.SetActive(false);
    }

    private IEnumerator EnteringBonusGame()
    {
        this.UpperTickerUI.SetSpeed(1);
        yield return new WaitForSeconds(3f); 
        CoroutineV2.StartCoroutine(ExecuteFirstStep());
    }


    #endregion

  
    //不停運轉主邏輯區->命令模式 分為主要兩大區[確認條件類型&演出類型]
    private IEnumerator ExecuteFirstStep()
    {
        if (CurrentStep == CheckStep.Finish) yield break;

       // DebugEx.Log("CurrentStep:", CurrentStep);
        switch (CurrentStep)
        {
            #region ========確認條件區=======
            case CheckStep.StartByRemainAmount:
                CurrentStep = ResultStruct.IsNotFinishedYet ? CheckStep.CheckFullfillResult : CheckStep.PerformanceEnding;
                break;
            case CheckStep.CheckFullfillResult:
                CurrentStep = ResultStruct.IsFullFill ? CheckStep.PerformanceLose : CheckStep.CheckFinalRound;
                break;
             case CheckStep.CheckFinalRound:
                CurrentStep = ResultStruct.IsFinalRound ? CheckStep.CheckWinIDByRandom : CheckStep.CheckShouldWinThisTime;
                break;
             case CheckStep.CheckShouldWinThisTime:
                CurrentStep = ResultStruct.IsWin ? CheckStep.CheckWinIDByRandom : CheckStep.PerformanceLose;
                break;
             case CheckStep.CheckWinIDByRandom:
                ResultStruct.FetchRamdonInWaitList();
               // DebugEx.Log("有幾個中獎:", ResultStruct.FetchThisRound.Count());
                CurrentStep = CheckStep.PerformanceWin;
               
                break;
            #endregion



             #region ========演出區=======
             case CheckStep.PerformanceWin:
                CoroutineV2.StartCoroutine(PerformanceBlur());
                yield return new WaitForSeconds(0.1f);
                CoroutineV2.StartCoroutine(this.RoundInfo.AddLightingEffectDown(this.LightingEffect));
                ResultStruct.ReduceRound();
                this.RoundInfo.SetRemainingAmount(ResultStruct.CurrentRemainingAmount);
                yield return new WaitForSeconds(1.6f);
                yield return PerformanceWinResult();
                ResultStruct.ClearFetch();
                CurrentStep = CheckStep.StartByRemainAmount;
                break;

             case CheckStep.PerformanceLose:
                CoroutineV2.StartCoroutine(PerformanceBlur());
                yield return new WaitForSeconds(0.1f);
                CoroutineV2.StartCoroutine(this.RoundInfo.AddLightingEffectDown(this.LightingEffect));
                ResultStruct.ReduceRound();
                this.RoundInfo.SetRemainingAmount(ResultStruct.CurrentRemainingAmount);
                yield return new WaitForSeconds(1.6f);
                yield return PerformanceFailResult();
                CurrentStep = CheckStep.StartByRemainAmount;
                break;

             case CheckStep.PerformanceEnding:
                CurrentStep = CheckStep.Finish;
                yield return PlayEnding();
                this.Close();
                break;

             #endregion

             default:
                throw new System.NotImplementedException("ColorSpin Bonus狀態機: 未實做的Step");
        }

        if (CurrentStep != CheckStep.Finish) 
            CoroutineV2.StartCoroutine(ExecuteFirstStep());
    }



    #region 表演
    private GameObject LightingEffect { get { return this._controller.GetSlotEffect(12).GetChild(0).gameObject; } }

    private IEnumerator PerformanceBlur()
    {
       // DebugEx.Log("【演blur - ing 轉轉轉】");
        yield return BottomUI.PlayBlur(ResultStruct.BlurList); //除了已經演出火焰的以外, 所有bar調都要開始轉動
    }


    private IEnumerator PerformanceWinResult()
    {
       // DebugEx.Log("【演火焰】");
        //這段要把兩個結合
        ResultStruct.AddingFetchToResult();

        yield return this.BottomUI.PlayFire(ResultStruct.FetchThisRound);
        WaitFirstFireThenPlayLightEffect();
        yield return new WaitForSeconds(1.2f);

    }

    private void WaitFirstFireThenPlayLightEffect()
    {
        CoroutineV2.StartCoroutine(this.RoundInfo.AddLightingEffectAbove(this.LightingEffect));
        CoroutineV2.StartCoroutine(this.RoundInfo.AddLightingEffectDown(this.LightingEffect));
        ResultStruct.ResetRound();
        this.RoundInfo.SetHowManyToWin(ResultStruct.LackAmountToJP);
        this.RoundInfo.SetRemainingAmount(ResultStruct.CurrentRemainingAmount);
    }


    private IEnumerator PerformanceFailResult()
    {
        //DebugEx.Log("【演失敗】");
        yield return BottomUI.PlayRevelling(ResultStruct.BlurList);
        yield return new WaitForSeconds(0.6f);

    }

    private IEnumerator PlayEnding()
     {
        // DebugEx.Log("【==結束==】");
         this.RollerBG.gameObject.GetComponent<CanvasGroup>().DOFade(0f, 1f);
         this.RoundInfo.gameObject.GetComponent<CanvasGroup>().DOFade(0f, 1f);
         yield return new WaitForSeconds(1.5f);
         //出現火焰 速度加快 再開始演下方的聽牌 & 看要不要把playEnding也調整為協程
         yield return this.UpperTickerUI.ReadyForLastSence();
         yield return SetCorrectResultToBar();
         yield return this.UpperTickerUI.PlayEnding();
        //這邊要演從JP一個一個往下調
         yield return CheckJP();
         yield return this.BeforBonusAni();
         this.EndGamePanel.gameObject.SetActive(false);
         Close();
         this.UpperTickerUI.ResetDefaultTickerItems();
     }


    private IEnumerator SetCorrectResultToBar()
    {
        yield return this.UpperTickerUI.WaitForNextRound();
        this.ProcessFinalItemList(this.UpperTickerUI.Containers.First(con => !con.IsInFirst).ItemList);
        yield return UpperTickerUI.WaitForNextRound();
        this.UpperTickerUI.Containers.ForEach(x => this.ProcessFinalItemList(x.ItemList));
    }

    private void ProcessFinalItemList(List<IBasicTickerItem> itemList)
    { 
        itemList.ForEach(
            x =>{
                int id = this.ResultStruct.BarInGameIDList[x.Index];
                x.SetCorrectInfo(id, UpperTickerUI.GetBetByID(id));
            });
    }




    private IEnumerator CheckJP()
    {
        if (this.ResultStruct.IsWiningJP)
            yield return this.PerformanceJP();
        else
            yield return this.PerformancePot();
    }


    private IEnumerator PerformanceJP()
    {
       // DebugEx.Log("【演JP】");
        yield return new WaitForSeconds(2f);
        yield return CoroutineV2.Single(this._controller.OnJackpot(this.ResultStruct.JPResult.JPType, this.ResultStruct.JPResult.JPAmount, this._baseState)).Start();
        yield return PerformanceEmptyBottom();
        this.EndGamePanel.ShowEnd(GetJPAmount.ExFormatNumberWithComma());
        yield return new WaitForSeconds(2f);
    }

    private IEnumerator PerformanceEmptyBottom()
    {
        var result = this.ResultStruct.FinalResultFromServer.OrderByDescending(x => x);
        var ticker = this.UpperTickerUI.Containers.Find(x => x.IsInFirst);
        if (ticker == null)
        {
            yield return new WaitForSeconds(0.1f);
            ticker = this.UpperTickerUI.Containers.Find(x => x.IsInFirst);
        }
        //消失底部, 特效出現, 往上衝
        this.BottomContain.GetComponent<CanvasGroup>().DOFade(0f, 1.3f);
        yield return new WaitForSeconds(0.5f);
        ticker.ItemList.ForEach(x =>
            {
                if (result.Contains(x.Index))
                {
                
                    CoroutineV2.StartCoroutine(x.PlayEndingFireByBar(this._controller.GetSlotEffect(16).GetChild(0).gameObject));
                }
            });

        yield return new WaitForSeconds(3f);

    }

    private IEnumerator PerformancePot()
    {
        yield return PerformanceEmptyBottom();
        var result = this.ResultStruct.FinalResultFromServer.OrderByDescending(x => x);
        var bars = this.UpperTickerUI.Containers[0].ItemList;
        double total = 0;
        
        this.CreateEffectForEndedPanel(result.Count() * 1.6f + 3);
        foreach (int indexOfBar in result)
        {
            var bar = bars[indexOfBar];
            total += bar.IsNormalType ? bar.FinalBet : this.ResultStruct.EachJPMap[indexOfBar].JPAmount;

            var arrowEffect = this.AddArrowEffectToEndPanel(bar);
            if (!this.EndGamePanel.gameObject.activeInHierarchy) this.EndGamePanel.ShowEnd(total.ExFormatNumberWithComma());
            this.AddScaleEffectToEndPanel();
           // DebugEx.Log("【演Pot index:", indexOfBar, " amount:", bar.IsNormalType ? bar.FinalBet : this.ResultStruct.EachJPMap[indexOfBar].JPAmount, "  id:", bar.TargetID, "】");

            yield return new WaitForSeconds(0.8f);
            this.EndGamePanel.ShowEnd(total.ExFormatNumberWithComma());
            var exploreEffect = this.AddExploreEffectEndPanel();

            yield return new WaitForSeconds(1f);
            GameObject.Destroy(arrowEffect);
            GameObject.Destroy(exploreEffect);
        }
        yield return new WaitForSeconds(2f);

    }

    private void AddScaleEffectToEndPanel()
    {
        var ani = this.EndGamePanel.EndGameObj.GetComponentInChildren<Animator>();
        ani.Play("Slot56_bst_BonusText");
    }

    private GameObject AddArrowEffectToEndPanel(IBasicTickerItem bar)
    {
        var effect = this._controller.GetSlotEffect(8).GetChild(0).gameObject;
        effect = this.gameObject.ExAddChild(effect);
        var anima = effect.GetComponentInChildren<Animator>();
        anima.Play(bar.Index.ToString());
        return effect;
    }

    private GameObject AddExploreEffectEndPanel()
    {
        var exploreEffect = this._controller.GetSlotEffect(17).GetChild(0).gameObject;
        exploreEffect = this.EndGamePanel.gameObject.ExAddChild(exploreEffect);
        return exploreEffect;
    }



    ///結算 BonusGame TOTAL WIN 
    private void CreateEffectForEndedPanel(float time)
    {
        Transform expandFire = this._controller.GetSlotEffect(9).GetChild(0);
        this.EndGamePanel.gameObject.ExAddChild(expandFire.gameObject);
        Transform underFire = this._controller.GetSlotEffect(11).GetChild(0);
        this.EndGamePanel.gameObject.ExAddChild(underFire.gameObject);
    }

    #endregion

    #region 結算的分數

  
    private double GetJPAmount { get { return ResultStruct.IsWiningJP ? UpperTickerUI.GetWholeBarWin + ResultStruct.JPResult.JPAmount : 0f; } }

    private double GetNormalAmountWithoutPot 
    {
        get 
        {
            double total = 0;
            var bars = UpperTickerUI.Containers[0].ItemList;
            ResultStruct.FinalResultFromServer.ForEach(x => total += bars[x].FinalBet);
            return total;
        }
    }

    private double GetNormalAmountWithPot
    {
        get
        {
            double total = GetNormalAmountWithoutPot;
            ResultStruct.EachJPMap.ToList().ForEach(x => total += x.Value.JPAmount);
            return total;
        }
    }



    #endregion

    private ColorSpinExecute UpperTickerUI { get { return MainState.UpperTickerUI; } }
    private MainState_56 MainState { get { return ((MainState_56)this._controller.GetState(GameType.MAIN)); } }


    protected override IEnumerator ImplementHide(object[] hideData)
    {
        //這邊要決定我要回去FS還是MAIN  決定pink border是否秀
        MainState.Roller.SetPotActivite(false);
        UpperTickerUI.SetParent(this._controller.CenterCanvas.transform);
        UpperTickerUI.SetSpeed(MainState.DefaultTickerSpeed);
        UpperTickerUI.ShouldMove(true);
        UpperTickerUI.Containers.ForEach(x => x.ItemList.ForEach(bar => bar.ClearEndEffect()));
        Destroy(this.EndGamePanel.gameObject);
        yield break;
    }



    private IEnumerator BeforBonusAni()
    {

        Transform moneyWaterfallEffect = _controller.GetSlotEffect(5).GetChild(0);
        var effect = this._controller.gameObject.ExAddChild(moneyWaterfallEffect.gameObject);
        yield return CoroutineV2.WaitTime(1.5f);
        GameObject.Destroy(effect);

    }


}




