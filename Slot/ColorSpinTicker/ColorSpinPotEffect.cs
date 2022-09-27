using System.Collections;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;
using UnityEngine.UI;

public class ColorSpinPotEffect : MonoBehaviour
{

    public Animator FireLoopingAni;

    public void LoopEffectPlay()
    {
        DebugEx.Log("LoopEffectPlay");
        FireLoopingAni.Play("Slot56_bst_TotalwinPotFireUp");
        FireLoopingAni.gameObject.SetActive(true);
        
    }


    public void LoopEffectStop()
    {
        DebugEx.Log("LoopEffectStop");
        FireLoopingAni.Play("Slot56_bst_TotalwinPotFireDefault");
        FireLoopingAni.gameObject.SetActive(false);
    }





}
