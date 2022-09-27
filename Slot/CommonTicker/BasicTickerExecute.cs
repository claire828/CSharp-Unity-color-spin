using Common.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

//這個是跑馬燈執行器, 使用Obj的Interface控制運行
public class BasicTickerExecute:MonoBehaviour
{
    public GameObject TickerObj; 
    public GameObject TickerContainer;
    public List<ITicker> Containers { get; set; }
    protected float SingleObjWidth { get; private set; }
    protected Vector2 PrePos { get; private set; }
    protected virtual int StageWidth { get { return 800; } }

    protected int TotalWeelAmount;

    protected virtual int ItemWidth { get { return 72; } }

    protected virtual int ItemAmount { get { return 12; } }

    public virtual void Initial()
    {
        if (Containers != null) return;
        CreateItemsInContainer();
        InitialBeginingPosition();
    }
   

    private void CreateItemsInContainer()
    {
        Containers = new List<ITicker>();
        Enumerable.Range(0, 2).ToList().ForEach(x =>
        {
            var obj = TickerContainer.ExAddChild(TickerObj).GetComponent<ITicker>();
            obj.Initial(new BasicTickerStruct(ItemWidth, ItemAmount, x, StageWidth, TotalWeelAmount));
            obj.AdjustSpeed(0.4f);
            Containers.Add(obj);
            SingleObjWidth = obj.TickerWidth;
            obj.TerminalNotification += CallWhenReachTerminal;
        });
     
        
    }
    /// <summary>
    /// 設定預設位置
    /// </summary>
    private void InitialBeginingPosition()
    {
        int index = 1;
        Vector2 pos = Containers[index].Ticker.localPosition;
        pos.x = SingleObjWidth * -1;
        PrePos = pos;
        Containers[index].Ticker.localPosition = PrePos;
    }

 
    public void ShouldMove(bool bMove)
    {
        Containers.ForEach(x => x.SwitchMoving(bMove));
    }


    protected virtual void CallWhenReachTerminal(int index)
    {
        DebugEx.Log("收到CallBack");
    }


    void OnDisable() 
    {
        if (Containers == null) return;
        this.Containers.ForEach(x => x.TerminalNotification -= CallWhenReachTerminal);
    }


    public void SetSpeed(float speed)
    {
        Containers.ForEach(x => x.AdjustSpeed(speed));
    }


  




}
  