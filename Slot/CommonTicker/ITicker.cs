using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITicker
{

    void Initial(BasicTickerStruct tickerStruct);
    int TickerWidth { get; }
    void AdjustSpeed(float speed);
    void SwitchMoving(bool bMove);
    Transform Ticker { get; }

    event Action<int> TerminalNotification;
    IEnumerator PlayEnding();

    IEnumerator PlayBonusIncreasing();
    bool IsInFirst { get; }

 


    List<IBasicTickerItem> ItemList { get; set; }

}
