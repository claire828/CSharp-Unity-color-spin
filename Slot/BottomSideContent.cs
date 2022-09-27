using HeavyDutyInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ColorSpin BonusGame的下方列表
/// </summary>
public class BottomSideContent : MonoBehaviour
{

    [Comment("單Bar物件引用", CommentType.Info)]
    public GameObject BarObj;


    [Comment("放Bar的容器", CommentType.Info)]
    public GameObject Container;

  

    private List<BarInBottomSide> ItemList { get; set; }
    private const int TotalAmount = 10;

  
    public void CreateItemsInContainer()
    {
        Container.ExRemoveAllChildren();
        ItemList = new List<BarInBottomSide>();
        Enumerable.Range(0, TotalAmount).ToList().ForEach(x =>
        {
            var bargameObject = Container.ExAddChild(BarObj).GetComponent<BarInBottomSide>();
            bargameObject.Initial(x);
            ItemList.Add(bargameObject);
        });
      
    }


    /// <summary>
    /// step1
    /// </summary>
    /// <param name="playList"></param>
    public IEnumerator PlayBlur(List<int> playList)
    {
        for (int i = 0; i < playList.Count; i++)
        {
            int index = playList[playList.Count-i-1] - 1;
            ItemList[index].ShowBeginingBlur();
            yield return new WaitForSeconds(0.2f);
        }
       
    }


    /// <summary>
    /// step2
    /// </summary>
    /// <param name="playList"></param>
    public IEnumerator PlayRevelling(List<int> playList)
    {
        for (int i = playList.Count-1; i >=0; i--)
        {
            int index = playList[i] - 1;
            ItemList[index].RevellingEffect();
            yield return new WaitForSeconds(0.2f);
        }

    }
   


    /// <summary>
    /// 演出聽牌火焰 step3
    /// </summary>
    /// <param name="playList"></param>
    public IEnumerator PlayFire(List<int> playList)
    {
       
        for (int i = 10; i>0; i--)
        {
            var elem = ItemList[i - 1];
            yield return new WaitForSeconds(0.2f);
            if (playList.Contains(i))
            {
                elem.ShowEffect();
            }
            else
            {
                elem.RevellingEffect();
            }
        }
     
    }





}
