using System.Collections;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;
using UnityEngine.UI;

public class ColorSpinFontBar : MonoBehaviour
{



    public void UpdateAmount(double total, Text colonFont)
    {

        var strArr = total.ExTransferToBMK().ToCharArray();
        int length = strArr.Length;
        Clear();
        Enumerable.Range(0, length).ToList().ForEach(x =>
        {
            Text colon = gameObject.ExAddChild(colonFont.gameObject).GetComponent<Text>();
            colon.text = strArr[x].ToString();
        });
    
    }



    public void Clear()
    {
        this.gameObject.ExRemoveAllChildren();
    
    }



}
