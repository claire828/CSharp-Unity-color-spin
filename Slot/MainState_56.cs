using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Settings;
using Common.Net;
using Common.Audio;
using Common.Resource;
using Common.Data;

public class MainState_56 : BaseMainState
{

    private FreeSpinState_56 FreeState { get { return (FreeSpinState_56)_controller.GetState(GameType.FREESPIN); } }
    public Roller_56 Roller { get { return ((Roller_56)RollerBase); } }
    public float DefaultTickerSpeed { get { return 0.4f; } }

    #region 基礎Symbol跟流程確認設定


    /// 設定遊戲 一進機台的時候執行一次
    public override void SetData(SlotController controller, BaseSlotSetting setting, BaseGameUI gameBase)
    {
        base.SetData(controller, setting, gameBase);
        UpperTickerUI.ProcessLevelBetData(CommonData.SlotInData["r"]);
        UpperTickerUI.ShouldMove(true);
        Roller.SetPotActivite(false);
        Roller.SetVisibleOfPinkBorder(false);
        Roller.IncreasePanel.gameObject.SetActive(false);
    }

    /// 每次Spin的流程
    protected override void CheckUseFunc()
    {
        StepAction = new IEnumerator[10]
        {
            CheckCanRun(),
            StopGetGameRunDataAni(),
            ShowChangAniBeforGet(),
            ShowResult(), 
            ShowAllRewardWin(),
            CheckBigWin(), 
            ShowChangAniAfterGet(),
            CheckFreeSpin(),
            CheckBonus(),
            CheckGetResoure(),
        };
    }

    #endregion



    #region 跑馬燈 Ticker
    private ColorSpinExecute _tickerInstance;
    public ColorSpinExecute UpperTickerUI{
        get{ return _tickerInstance ?? CreateSingletonTicker();}
    }

    private ColorSpinExecute CreateSingletonTicker()
    {
        var ticker = GameObject.Instantiate(Resources.Load("Slot/UI/BonusGame/Bonus56/UpperTickerUI")) as GameObject;
        _tickerInstance = ticker.GetComponent<ColorSpinExecute>();
        UpperTickerUI.SetParent(this._controller.CenterCanvas.transform);
        UpperTickerUI.Initial();
        UpperTickerUI.SetSpeed(DefaultTickerSpeed);
        UpperTickerUI.ShouldMove(true);
        return _tickerInstance;
    }

    #endregion


    private Json BsIncreaseData { get { return GameRunData["bp"]; } }

    protected override IEnumerator ShowChangAniBeforGet()
    {
      // DebugEx.Log("執行Bonus Increase");
        if (ShouldShowBsIncrease) {
            yield return CoroutineV2.StartCoroutine(ExecuteBonusIncrease());
        }
        
        yield return base.ShowChangAniBeforGet();
    }





    private IEnumerator ExecuteBonusIncrease()
    {
        int id = BsIncreaseData[0].IntValue;
        //更新bet到主map
        UpperTickerUI.AddToCurrentBet(id, BsIncreaseData[1].IntValue);
        //將bet更新到對應的item中
        UpperTickerUI.UpdateBetToCertainTargets(id);

        Transform increaseEffect = _controller.GetSlotEffect(10).GetChild(0);
        var effect = Roller.IncreasePanel.gameObject.ExAddChild(increaseEffect.gameObject);
        Roller.IncreasePanel.ShowIncrease();
        yield return new WaitForSeconds(0.6f);
        yield return this.UpperTickerUI.ReadyFoBonusIncrease();
        yield return this.UpperTickerUI.PerformanceIncrease(BsIncreaseData);
        yield return new WaitForSeconds(0.1f);
        UpperTickerUI.SetSpeed(DefaultTickerSpeed);
        UpperTickerUI.ShouldMove(true);
        yield return new WaitForSeconds(1.5f);
        Roller.IncreasePanel.gameObject.SetActive(false);
        GameObject.Destroy(effect);
       
    }

    private bool ShouldShowBsIncrease { get { return GameRunData.ContainsKey("bp"); } }








  
}


