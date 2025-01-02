using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUpP1 : MonoBehaviour
{
    public TMP_Text popNameTxt;
    public TMP_Text popAttackTxt;
    public TMP_Text popHealthTxt;
    public TMP_Text popEnergyTxt;

    public List<DisplayCard> allDisplayCards; // Reference to all DisplayCard instances
    // Start is called before the first frame update
    void Start()
    {
        allDisplayCards = new List<DisplayCard>(FindObjectsOfType<DisplayCard>());
        
       // popNameTxt.text = "pop Card";
        //popAttackTxt.text = "5";
       // popHealthTxt.text = "100";
       // popEnergyTxt.text = "10";
    }

    // Update is called once per frame
    void Update()
    {
        foreach (DisplayCard otherCard in allDisplayCards)
        {
            if ( otherCard.isSelected)
            {
                popNameTxt.text = otherCard.nameText.text;
                popAttackTxt.text = otherCard.attackText.text;
            }
        }
    }
}
