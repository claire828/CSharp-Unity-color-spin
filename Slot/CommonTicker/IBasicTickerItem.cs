using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBasicTickerItem {

    void InsertItemIndex(int index);

    void UpdateItem();

    void AddStartIndex(int index);

     void DoNumbers(bool dotween);

     void AddIncreasingBet(int bet);

     int TargetID { get; set; }

     int Index { get; set; }

     double FinalBet { get; }
     void SetCorrectInfo(int id, int bet);

     bool IsNormalType { get; }

     IEnumerator PlayEndingFireByBar(GameObject effects);

     void ClearEndEffect();
}
