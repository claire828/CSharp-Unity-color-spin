using Common.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;



/// <summary>
/// 跑馬燈的基底物件
/// </summary>
public abstract class BasicTickerImp : MonoBehaviour, ITicker
{

    public GameObject TicketItem;


    #region Field

    public List<IBasicTickerItem> ItemList { get;  set; }
    protected BasicTickerStruct TickerStruct { get; private set; }
    protected float Speed { get; set; }
    private bool IsMoving { get; set; }

    private bool DoesReachTermination { get { return gameObject.transform.localPosition.x + Speed > this.TickerStruct.ContainerWidth; } }

 

    #endregion

    #region 創UI
    private void CreateItemsInContainer()
    {
        ItemList = new List<IBasicTickerItem>();
        Enumerable.Range(0, TickerStruct.Amouunt).ToList().ForEach(x => 
        {
            var item = gameObject.ExAddChild(TicketItem).GetComponent<IBasicTickerItem>();
            item.InsertItemIndex(x);
            item.AddIncreasingBet(0);
            item.AddStartIndex(TickerStruct.StartIndex);
            item.UpdateItem();
            ItemList.Add(item); 
        });

    }
    #endregion


    void Update()
    {
        if (!IsMoving) return;

        if (DoesReachTermination)
            ReachTerminalStation();
        else
            gameObject.transform.localPosition = new Vector2(transform.localPosition.x + Speed, transform.localPosition.y);

        
    }


    protected virtual void ReachTerminalStation()
    {
        float newX = transform.localPosition.x + Speed - this.TickerStruct.ContainerWidth;
        gameObject.transform.localPosition = new Vector2(-864 + newX, transform.localPosition.y);
        TerminalNotification.Invoke(this.TickerStruct.Index);
       // DebugEx.Log("抵達終點");
    }


    public virtual IEnumerator PlayEnding()
    {
        while (this.Speed > 0)
        {
            this.Speed--;
        }
        yield break;
    }


    public virtual IEnumerator PlayBonusIncreasing()
    {
        while (this.Speed < 50)
        {
            this.Speed++;
        }
        yield break;
    }

    #region Interface provided for TickerExecute
    public void Initial(BasicTickerStruct tickerStruct)
    {
        this.TickerStruct = tickerStruct;
        CreateItemsInContainer();
    }

    public int TickerWidth { get { return TickerStruct.ContainerWidth; } }

    public void AdjustSpeed(float speed) { this.Speed = speed; }

    public void SwitchMoving(bool bMove) { this.IsMoving = bMove; }

    public Transform Ticker { get { return this.gameObject.transform; } }

    public event Action<int> TerminalNotification;

    public bool IsInFirst { get { return this.gameObject.transform.localPosition.x >= 0; } }



    #endregion
}


#region 產出內部Item的定義結構

public struct BasicTickerStruct
{
    public int ItemWidth { get; private set; }
    public int Amouunt { get; private set; }
    public int ContainerWidth { get { return ItemWidth * Amouunt; } }

    public int Index { get; private set; }
    public int StartIndex { get; private set; }


    public int StageWidth { get; private set; }

    public BasicTickerStruct(int w, int amount, int index, int stageWidth, int startIndex)
    {
        this.ItemWidth = w;
        this.Amouunt = amount;
        this.Index = index;
        this.StageWidth = stageWidth;
        this.StartIndex = startIndex;
    }

}

#endregion
