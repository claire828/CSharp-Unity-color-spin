using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusGameIncrease_56 : MonoBehaviour 
{

    public GameObject IncreaseObj;
    public GameObject EndGameObj;
    public Text TotalWin;
    public Text TotalWinEffect;

    private enum InfoPanelType56
    { 
        INCREASE,
        END
    }

    public void ShowIncrease()
    {
        SwitchPanelStatus(InfoPanelType56.INCREASE);
    }


    public void ShowEnd(string total)
    {
        SwitchPanelStatus(InfoPanelType56.END);
        TotalWin.text = total;
        TotalWinEffect.text = total;
    }


    private void SwitchPanelStatus(InfoPanelType56 type)
    {
        IncreaseObj.SetActive(type == InfoPanelType56.INCREASE);
        EndGameObj.SetActive(type == InfoPanelType56.END);
        gameObject.SetActive(true);
    }

    

    
}
