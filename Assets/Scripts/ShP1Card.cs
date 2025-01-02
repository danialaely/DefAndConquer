using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

public class ShP1Card : MonoBehaviour, IPointerClickHandler
{

    public TMP_Text defenseText;
    public TMP_Text healthText;
    public Image outerBorder;
    public Image crdImg;

    private int defense = 5;
    public int health = 100;

    public bool isSelected = false;

    private string tagToSearch = "Player2";
    GameObject[] player2;

    public Image dice1;
    public Image dice2;

    public static int P1Power;
    public static int P2Power;

    public int SHealth;

    public GameManager gm;

    public CardShuffler shuffler;

    public GameObject PopUpCardP1;
    public TMP_Text popNameTxt;
    public TMP_Text popAttackTxt;
    public TMP_Text popHealthTxt;
    public TMP_Text popEnergyTxt;
    public Image popCardImg;
    UnityEngine.UI.Image popOuterBdr;

    public Animator popupAnim;
    public List<ShP1Card> allSHP1Cards;

    public bool canMove;

    // Start is called before the first frame update
    void Start()
    {

        defenseText.text = defense.ToString();
        healthText.text = health.ToString();

        player2 = GameObject.FindGameObjectsWithTag(tagToSearch);
        popOuterBdr = PopUpCardP1.transform.Find("OuterBorder").GetComponent<Image>();

        allSHP1Cards = new List<ShP1Card>(FindObjectsOfType<ShP1Card>());
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetCardHealth()
    {
        return health;
    }

    public int GetCardDefense()
    {
        return defense;
    }

    public void OnPtcClick() 
    {
        bool isP1Turn = ButtonTurn.GetPlayerTurn();

        if (!isP1Turn)
        {
            isSelected = !isSelected;
            if (isSelected)
            {
                //   if (gm.currentPhase == GamePhase.Attack) { }
                // if (gm != null) 
                // { 
                //dp != null && dp.adjCards.Contains(gameObject) && BoardSlot.GetCurrentEnergyP2() >= 2 && gm.currentPhase == GamePhase.Attack
                foreach (GameObject displayCardObject in player2)
                {
                    DisplayCard2 dp = displayCardObject.GetComponent<DisplayCard2>();
                    if (dp!=null && dp.adjCards.Contains(gameObject))  
                    {
                        if (gm.currentPhase == GamePhase.Attack) 
                        {
                            outerBorder.color = Color.red;
                            Debug.Log("Player2 Card's Attack:" + dp.GetCardAttack());
                            dice1.enabled = true;
                            dice2.enabled = true;
                            
                            shuffler.AttackSound();
                            //PopUpCardP1.SetActive(true);
                            popNameTxt.text = "StrongHold";
                            popHealthTxt.text = healthText.text;
                            //popCardImg.sprite = crdImg.sprite;
                            popOuterBdr.color = Color.red;
                            popupAnim.SetBool("Select", true);

                            P1Power = this.GetCardDefense();
                            P2Power = dp.GetCardAttack();

                            SHealth = this.GetCardHealth();

                            foreach (ShP1Card STCardP1 in allSHP1Cards)
                            {
                                UnityEngine.UI.Image shp1outerborder = STCardP1.transform.Find("OuterBorder").GetComponent<Image>();
                                shp1outerborder.color = Color.red;
                            }
                        }
                        /*  if (this.GetCardAttack() < dp.GetCardAttack()) 
                          {
                              //shuffler.DiscardSound();
                              DisCard = true; 
                          }*/

                    }
                }
                //}
            }

            if (!isSelected)
            {
                // PopUpCardP1.SetActive(false);
                popupAnim.SetBool("Select",false);
                //StartCoroutine(popupActiveFalse(0.6f));

                foreach (GameObject displayCardObject in player2)
                {
                    DisplayCard2 dp = displayCardObject.GetComponent<DisplayCard2>();
                    if (dp != null && dp.adjCards.Contains(gameObject))
                    {
                        outerBorder.color = Color.blue;   //MainCamera Blue Color: 314D79    Board Color: 292E48 
                        dice1.enabled = false;
                        dice2.enabled = false;

                        foreach (ShP1Card STCardP1 in allSHP1Cards)
                        {
                            UnityEngine.UI.Image shp1outerborder = STCardP1.transform.Find("OuterBorder").GetComponent<Image>();
                            shp1outerborder.color = Color.blue;
                        }
                    }
                }
            }

        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnPtcClick();
    }
    public int GetP1Power()
    {
        return P1Power;
    }

    public int GetP2Power()
    {
        return P2Power;
    }

    public int GetSHealth()
    {
        return SHealth;
    }

    public void SetSHealth(int health)
    {
        SHealth = health;
    }

    public void SetSelection(bool select) 
    {
        isSelected = select;
    }

    IEnumerator popupActiveFalse(float delay) 
    {
        yield return new WaitForSeconds(delay);
        PopUpCardP1.SetActive(false);
    }

}
