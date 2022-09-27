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
public class ColorSpinAnalysisAssist : MonoBehaviour {

	public int DEFAULT_AMOUNT = 3; //每次重置的次數
    private Json Result { get; set; }
    public bool IsNotFinishedYet { get { return CurrentRemainingAmount > 0; } }
    public bool IsFinalRound { get { return CurrentRemainingAmount == 1; } }
    public bool IsWin { get { return RandomEx.GetBool(); } }

    public bool IsFullFill { get { return FinalResultFromServer.Count == ResultHasBeenPerformance.Count(); } }

    public int LackAmountToJP { get { return 10 - ResultHasBeenPerformance.Count(); } }

    public List<int> BlurList { get { return Enumerable.Range(1, 10).ToList().Except(ResultHasBeenPerformance).ToList(); } }
    /// <summary>
    /// 最終結果
    /// </summary>
    public List<int> FinalResultFromServer { get; private set; }
    /// <summary>
    /// 目前已表演的資料
    /// </summary>
    private List<int> ResultHasBeenPerformance { get; set; }
    /// <summary>
    /// 目前所剩回合數
    /// </summary>
    public int CurrentRemainingAmount { get; private set; } 
    /// <summary>
    /// 目前等後被Ramdon的資料
    /// </summary>
    private List<int> WaitForFetch { get { return FinalResultFromServer.Except(ResultHasBeenPerformance).ToList(); } }

    public List<int> FetchThisRound { get; set; }

    /// <summary>
    /// 盤面上的Bar列表ID
    /// </summary>
    public List<int> BarInGameIDList { get; private set; }

    public JPResultStruct JPResult { get; private set; }
    //index, result
    public Dictionary<int, JPResultStruct> EachJPMap { get; private set; }

    public bool IsWiningJP { get { return this.Result.ContainsKey("jp"); } }

    #region 塞資料
    public ColorSpinAnalysisAssist(Json bonusData)
    {
      
        Result = bonusData;
        InitialData();
        SetTickerBarData();
        if (IsWiningJP)
            SetResultWithWinJP();
        else
            SetResultWithoutWinJP();

        ClearFetch();
        ResetRound();
    }



    private void InitialData()
    {
        BarInGameIDList = new List<int>();
        FinalResultFromServer = new List<int>();
        ResultHasBeenPerformance = new List<int>();
        EachJPMap = new Dictionary<int, JPResultStruct>();
    }


    private void SetTickerBarData()
    {
        Result["s"].ToList().ForEach(x => BarInGameIDList.Add(x.Value.IntValue));
    }


    private void SetResultWithWinJP()
    {
        ///如果全中JP 要將index 1 - 10的資料都塞入, 因為server不會傳
        var jp = this.Result["jp"];
        JPResult = new JPResultStruct(jp[0].IntValue, jp[1].DoubleValue);
        Enumerable.Range(1, 10).ToList().ForEach(x => FinalResultFromServer.Add(x));
    }


    private void SetResultWithoutWinJP()
    {
        //先儲存有中的值
        this.Result["h"].ToList().ForEach(x => FinalResultFromServer.Add(x.Value.IntValue));
        //再看有沒有單一的Pot, 沒有pot連key也不會有要return
        if (!this.Result.ContainsKey("j")) return;
        var eachJ = this.Result["j"].ToList();
        eachJ.ForEach(x => EachJPMap.Add( Int16.Parse(x.Key), new JPResultStruct(x.Value[0].IntValue, x.Value[1].DoubleValue)));
       
    }

  

    #endregion



    public void ReduceRound()
    {
        CurrentRemainingAmount--;
        DebugEx.Log("回合數:", CurrentRemainingAmount);
    }

    public void ResetRound()
    {
        CurrentRemainingAmount = DEFAULT_AMOUNT;
        DebugEx.Log("回合數重置");
    }


    public List<int> FetchRamdonInWaitList()
    {
        ClearFetch();
        var wait = WaitForFetch;
        int max = wait.Count() > 3 ? 3 : wait.Count();
        int howManyShouldGet = RandomEx.GetInt(1, max);
        while (FetchThisRound.Count < howManyShouldGet)
        { 
            int index = RandomEx.GetInt(0, max-1);
            var temp = wait[index];
            if (!FetchThisRound.Contains(temp)) FetchThisRound.Add(temp);
        }
        return FetchThisRound;
       
    }

    public void AddingFetchToResult()
    {
        FetchThisRound.ForEach(x => ResultHasBeenPerformance.Add(x));
       
    }


    public void ClearFetch()
    {
        FetchThisRound = new List<int>();
    }











}





public class JPResultStruct
{
    public int JPType {get; private set;}
    public double JPAmount {get; private set;}

    public JPResultStruct(int jpType, double jpAmount)
    {
        this.JPType = jpType;
        this.JPAmount = jpAmount;
    }


}



