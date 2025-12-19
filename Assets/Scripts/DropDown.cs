using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropDown : MonoBehaviour
{
    public GameObject SinglePImg;
    public GameObject MultiplayerImg;
    public int value;

    public GameObject PlayBtn;
    public TMP_Text PBtnTxt;

    public GameObject ToggleGroup;

    public TMP_Text loadingQuoteTxt;
    private float changeInterval = 3f;
    public GameObject loadingPanel;

    private string[] quotes =
    {
        "“In the midst of chaos, there is also opportunity.”",
        "“Victorious warriors win first and then go to war, while defeated warriors go to war first and then seek to win.” ",
        "“Strategy without tactics is the slowest route to victory. Tactics without strategy is the noise before defeat.”",
        "“The supreme art of war is to subdue the enemy without fighting.”",
        "“Appear weak when you are strong, and strong when you are weak.”",
        "“If you know the enemy and know yourself, you need not fear the result of a hundred battles.”",
        "“He who wishes to fight must first count the cost.”"
    };

    private int lastIndex = -1;

    private void Start()
    {
        PlayBtn.GetComponent<Image>().color = Color.white;
        PBtnTxt.color = Color.white;
        StartCoroutine(ChangeQuoteRoutine());
    }

    public void HandleInputData(int val) 
    {
        value = val;
        if (val == 0) 
        {
            SinglePImg.SetActive(false);
            MultiplayerImg.SetActive(true);
            PlayBtn.GetComponent<Image>().color = Color.white;
            PBtnTxt.color = Color.gray;

            ToggleGroup.SetActive(false);
        }
        if (val == 1)
        {
            SinglePImg.SetActive(true);
            MultiplayerImg.SetActive(false);
           PlayBtn.GetComponent<Image>().color = Color.white;
            PBtnTxt.color = Color.white;
            //414141
            ToggleGroup.SetActive(true);
        }

    }

    public int Options() 
    {
        return value;
    }

    private IEnumerator ChangeQuoteRoutine()
    {
        while (true)
        {
            int newIndex;
            do
            {
                newIndex = Random.Range(0, quotes.Length);
            }
            while (newIndex == lastIndex);

            lastIndex = newIndex;
            loadingQuoteTxt.text = quotes[newIndex];

            yield return new WaitForSeconds(changeInterval);
        }
    }
}
