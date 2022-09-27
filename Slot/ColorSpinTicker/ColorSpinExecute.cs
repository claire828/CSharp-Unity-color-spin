using Common.Settings;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

//這邊控制基礎行動外的動作, 如演動畫
public class ColorSpinExecute : BasicTickerExecute
{

    public Dictionary<int, List<LevelBetStruct>> LevelBetMap { get; private set; }
    public int CurrentLevelBet { get { return SlotController.CommonData.GetBetLevel(56); } }
    private bool DetectShouldDoTweenEnding { get; set; }

    #region 初始化
    public override void Initial()
    {
        int total = CSSettingsV3.Slot056.WheelClient[9999].SymbolList.Count;
        TotalWeelAmount = RandomEx.GetInt(0, total - 1);
        base.Initial();
    }
   

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
        transform.SetSiblingIndex(0);
    }

    #endregion


    #region 演出BonusGame


    public IEnumerator PlayEnding()
    {
        float stopDuringtion = 1f;
        this.Containers.ForEach(x =>
        {
            x.SwitchMoving(false);
            int temp = 36;
            int startX = x.IsInFirst ? temp : x.TickerWidth * -1 + temp;
            x.Ticker.DOMoveX(startX, stopDuringtion).SetEase(Ease.OutQuart);
        });

        yield return new WaitForSeconds(stopDuringtion);

        this.Containers.ForEach(x =>
        {
            int temp = 0;
            int startX = x.IsInFirst ? temp : x.TickerWidth * -1 + temp;
            x.Ticker.DOMoveX(startX, 0.3f).SetEase(Ease.OutCubic);
        });
        
        yield return new WaitForSeconds(1f);
    }



    //BonusGame結算金額特效演出

    #endregion


    #region 演出BonusIncrease
    public IEnumerator ReadyFoBonusIncrease()
    {
        this.Containers.ForEach(x => CoroutineV2.StartCoroutine(x.PlayBonusIncreasing()));
        yield return new WaitForSeconds(0.3f);

    }


    public IEnumerator PerformanceIncrease(Json increaseData)
    {
        int id = increaseData[0].IntValue;
        var targets = GetSpecifyItems(id);
        int mutipier = increaseData[1].IntValue;
        if (targets.Count == 0)
        {
            yield return ReplaceWholeTickerItems(id);
            targets = GetSpecifyItems(id);
        }
        yield return MovingTickerForIncreaseResult(targets[0].Index);
        targets.ForEach(x => x.DoNumbers(true));
    }
    private IEnumerator MovingTickerForIncreaseResult(int barIndex)
    {
        float stopDuringtion = 0.2f;
        this.Containers.ForEach(x =>
        {
            x.SwitchMoving(false);
            int basicW = 72;
            var halfW = basicW / 2;
            var idWholeW = (barIndex + 4) * basicW;
            var distance = basicW + halfW + idWholeW;
            int startX = 0;
            if (barIndex + 1 >= 8)
            {
                var temp = x.TickerWidth * -1 + distance;
                startX = x.IsInFirst ? temp : temp - x.TickerWidth;
            }
            else
            {
                startX = x.IsInFirst ? distance : x.TickerWidth * -1 + distance;
            }
            x.Ticker.DOMoveX(startX, stopDuringtion);
        });
        yield return new WaitForSeconds(stopDuringtion);
    }

  

    private IEnumerator ReplaceWholeTickerItems(int id)
    {
        DebugEx.Log("不存在的ID 開始全盤改變");
        int index = 0;
        for (int i = 0; i < WeelList.Count; i++)
        {
            if (id == WeelList[i].IntValue)
            {
                index = i;
                break;
            }
        }
        yield return WaitForNextRound();
        DebugEx.Log("找到Index:", index, "  id:", index);
        //設定上新盤面
        Containers.First(con => !con.IsInFirst).ItemList.ForEach(x => {
            x.AddStartIndex(index);
            x.UpdateItem();
        });
        yield return WaitForNextRound();

        Containers.ForEach(x => x.ItemList.ForEach(y =>
        {
            y.AddStartIndex(index);
            y.UpdateItem();
        }));
    }


   


    #endregion

    

    #region (BS跟Increase共用)轉動時每Round是否抵達的判定
    public IEnumerator ReadyForLastSence()
    {
        this.Containers.ForEach(x => CoroutineV2.StartCoroutine(x.PlayEnding()));
        yield return new WaitForSeconds(4f);
    }


    public IEnumerator WaitForNextRound()
    {
        //這邊打開flag讓第一個抵達的呼叫CallWhenReachTerminal, 終止moving 然後再主控制器執行tween進行演出
        DetectShouldDoTweenEnding = true;
        while (DetectShouldDoTweenEnding)
        {
            yield return new WaitForEndOfFrame();
        }
    }


    /// <summary>
    /// 最後一個抵達了 準備開始演
    /// </summary>
    /// <param name="index"></param>
    protected override void CallWhenReachTerminal(int index)
    {
        if (DetectShouldDoTweenEnding)
        {
            DetectShouldDoTweenEnding = false;
            DebugEx.Log("最後一個抵達了 準備開始演:", index);
        }
    }

    #endregion


    #region LevelBet資料

    private Json WeelList { get { return CSSettingsV3.Slot056.WheelClient[9999].SymbolList; } }

    public void ProcessLevelBetData(Json wholeLevelBet)
    {
        wholeLevelBet.IsArray = true;
        LevelBetMap = new Dictionary<int, List<LevelBetStruct>>();
        Containers.ForEach(x=>x.ItemList.ForEach(y=>y.AddIncreasingBet(0)));
        foreach (var items in wholeLevelBet)
        {
            var list = new List<LevelBetStruct>();
            var whichLevel = Int16.Parse(items.Key);
            foreach(var item in items.Value)
            {
                DebugEx.Log("增加LevelBet 階級:", whichLevel, " id:", Int16.Parse(item.Key), " bet:", Int16.Parse(item.Value));
                list.Add(new LevelBetStruct(Int16.Parse(item.Key),Int16.Parse( item.Value)));
            }
            LevelBetMap.Add(whichLevel, list);
        }
    
    }


    public void AddToCurrentBet(int id, int bet)
    {
        if (!IsExistBetKey) LevelBetMap.Add(CurrentLevelBet, new List<LevelBetStruct>());
        if (!IsExistSpecifyBet(id)) 
            LevelBetMap[CurrentLevelBet].Add(new LevelBetStruct(id, bet));
        else
            GetSpecifyBetInfo(id).UpdataBet(bet);
    }

    private bool IsExistBetKey
    {
        get { return (this.LevelBetMap.ContainsKey(CurrentLevelBet)); }
    }


    private bool IsExistSpecifyBet(int id) { 
    {
        if (!IsExistBetKey) return false;
        return GetSpecifyBetInfo(id) != null;
    } }

    private LevelBetStruct GetSpecifyBetInfo(int id)
    {
        {
            if (!IsExistBetKey) return null;
            var info = LevelBetMap[CurrentLevelBet].ToList().FirstOrDefault(x => x.ID == id);
            return info;
        }
    }
   

    public int GetBetByID(int id)
    {
        if (!IsExistSpecifyBet(id)) return 0;
        return GetSpecifyBetInfo(id).LevelBet;
    }



    #endregion


    #region 對Ticker裡面的ItemBar進行處理


    /// <summary>
    /// 增加倍率 (目前只有火焰的會累加)
    /// </summary>
    /// <param name="id"></param>
    /// <param name="bet"></param>
    public void UpdateBetToCertainTargets(int id)
    {
        var bars = GetSpecifyItems(id);
        var lvBet = GetBetByID(id);
        if (bars.Count != 0)
        {
            bars.ForEach(bar => bar.AddIncreasingBet(lvBet));
        }
       
       
    }



    /// <summary>
    /// 取出指定ID的Bars
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private List<IBasicTickerItem> GetSpecifyItems(int id)
    {
        List<IBasicTickerItem> bars = new List<IBasicTickerItem>();
        this.Containers.ForEach(x =>
        {
            x.ItemList.Where(y => y.TargetID == id).ToList().ForEach(ele => bars.Add(ele));
        });
        return bars;
    }

    /// <summary>
    /// 更新回預設資料
    /// </summary>
    public void ResetDefaultTickerItems()
    {
        this.Containers.ForEach(x =>
        {
            x.ItemList.ForEach(y => y.UpdateItem());
        });
    }


    public double GetWholeBarWin
    {
        get
        { 
            double total = 0;
            Containers[0].ItemList.Where(x => x.Index != 0 && x.Index != 11).ToList().ForEach(y=>total+=y.FinalBet);
            return total;
        }
    }



    #endregion




}




public class LevelBetStruct
{
    public int ID { get; private set; }

    public int LevelBet { get; private set; }
    public LevelBetStruct(int id, int levelBet)
    {

        this.ID = id;
        UpdataBet(levelBet);
    }
    public void UpdataBet(int levelBet)
    {
        this.LevelBet = levelBet;
    }

}