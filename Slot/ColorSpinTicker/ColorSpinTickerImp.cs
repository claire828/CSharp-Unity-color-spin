using Common.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSpinTickerImp : BasicTickerImp
{
    private const int SpeedDefault = 1;
    private int SpeedMax = 1;
    private const int IncreaseSpeedMax = 110;

    
    
    public override IEnumerator PlayEnding()
    {
        DebugEx.Log("PerformanIncreasing");
        SpeedMax = 14;
        yield return CoroutineV2.StartCoroutine(PerformanIncreasing());
        yield return new WaitForSeconds(1f);
        SpeedMax = 40;
        yield return CoroutineV2.StartCoroutine(PerformanIncreasing());
        yield return new WaitForSeconds(2.6f);

        DebugEx.Log("PerformanDecreasing");
        yield return CoroutineV2.StartCoroutine(PerformanDecreasing());
        yield return new WaitForSeconds(1.4f);
       // DebugEx.Log("PerformanSlow");
        DebugEx.Log("===finish====");
      
    }


    private IEnumerator PerformanIncreasing()
    {
        while (Speed < SpeedMax)
        {
            yield return new WaitForEndOfFrame();
            Speed++;
        }

     /*   yield return new WaitForSeconds(1.5f);
        while (Speed >30)
        {
            yield return new WaitForEndOfFrame();
            Speed--;
        }
        */
    }


    private IEnumerator PerformanDecreasing()
    {
        while (Speed > SpeedDefault)
        {
            yield return new WaitForEndOfFrame();
            Speed--;
        }
        DebugEx.Log("===finish Decreasing====");
    }




    public override IEnumerator PlayBonusIncreasing()
    {
        DebugEx.Log("PlayBonusIncreasing");
        while (Speed < IncreaseSpeedMax)
        {
            yield return new WaitForEndOfFrame();
            Speed += 26;
        }

    }







 

}
