using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Dice : MonoBehaviourPunCallbacks
{

    // Array of dice sides sprites to load from Resources folder
    private Sprite[] diceSides;

    // Reference to sprite renderer to change sprites
   // private SpriteRenderer rend;
    private Image image;
    public Image otherdice;

    private static int finalSide=0;
    private static int finalSide2 = 0;

    public List<DisplayCard2> allDpCards;
    public List<DisplayCard> allDisplayCards;

    public GameObject discardpile;
    public GameObject discardpile2;

    public Animator discaranimator;
    public Animator animator2;

    public AudioSource src;
    public AudioClip diceClip;
    public AudioClip discardedClip;

    public BoardSlot bslot;

    public bool CanAttackP1;
    public bool CanAttackP2;

    public bool CounterAttackinP1;
    public bool CounterAttackinP2;

    //public ShP1Card StrongHoldCardP1;
    
    //public ShP2Card StrongHoldCardP2;
    public List<ShP2Card> allSHP2Cards;
    public List<ShP1Card> allSHP1Cards;

    public ButtonTurn btnTurn;

    public GameObject AtcFailedTxt;

  //  public TMP_Text SHoldHealthP1;
  //  float healthValStP1;
   
  //  public TMP_Text SHoldHealthP2;
  //  float healthValStP2;

    // Use this for initialization
    private void Start()
    {

        // Assign Renderer component
       // rend = GetComponent<SpriteRenderer>();
        image = GetComponent<Image>();

        // Load dice sides sprites to array from DiceSides subfolder of Resources folder
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");

        allDpCards = new List<DisplayCard2>(FindObjectsOfType<DisplayCard2>());
        allDisplayCards = new List<DisplayCard>(FindObjectsOfType<DisplayCard>());
        
        allSHP2Cards = new List<ShP2Card>(FindObjectsOfType<ShP2Card>());
        allSHP1Cards = new List<ShP1Card>(FindObjectsOfType<ShP1Card>());

        CanAttackP1 = true; 
        CanAttackP2 = true;

        CounterAttackinP1 = false;
        CounterAttackinP2 = false;
        AtcFailedTxt.SetActive(false);
      //  healthValStP1 = 100.0f;
      //  SHoldHealthP1.text = healthValStP1.ToString();

      //  healthValStP2 = 100.0f;
      //  SHoldHealthP2.text = healthValStP2.ToString();

 
    }

    // If you left click over the dice then RollTheDice coroutine is started
    public void OnMouseDown()
    {
        int val = BoardSlot.GetCurrentEnergy();
        if (val >= 2) 
        {
            DiceSound();
            StartCoroutine(RollAndDiscard());
        }
    }

    public void AiRollDice() 
    {
            DiceSound();
            StartCoroutine(RollAndDiscard());
    }

    private IEnumerator RollAndDiscard() 
    {
        yield return StartCoroutine(RollTheDice());
        yield return StartCoroutine(RollTheDice2());
        Discarded();
    }

    // Coroutine that rolls the dice
    private IEnumerator RollTheDice()
    {
        // Variable to contain random dice side number.
        // It needs to be assigned. Let it be 0 initially
        int randomDiceSide = 0;

        // Final side or value that dice reads in the end of coroutine
       // int finalSide = 0;

        // Loop to switch dice sides ramdomly
        // before final side appears. 20 itterations here.
        for (int i = 0; i <= 20; i++)
        {
            // Pick up random value from 0 to 5 (All inclusive)
            randomDiceSide = Random.Range(0, 5);

            // Set sprite to upper face of dice from array according to random value
            image.sprite = diceSides[randomDiceSide];
            
            // Pause before next itteration
            yield return new WaitForSeconds(0.05f);
        }

        // Assigning final side so you can use this value later in your game
        // for player movement for example
        finalSide = randomDiceSide + 1;

        // Show final dice value in Console
        Debug.Log("Our Dice:"+finalSide);
    }

    private IEnumerator RollTheDice2()
    {
        // Variable to contain random dice side number.
        // It needs to be assigned. Let it be 0 initially
        int randomDiceSide2 = 0;

        // Final side or value that dice reads in the end of coroutine
        //int finalSide2 = 0;

        // Loop to switch dice sides ramdomly
        // before final side appears. 20 itterations here.
        for (int i = 0; i <= 20; i++)
        {
            // Pick up random value from 0 to 5 (All inclusive)
            randomDiceSide2 = Random.Range(0, 5);

            // Set sprite to upper face of dice from array according to random value
            otherdice.sprite = diceSides[randomDiceSide2];

            // Pause before next itteration
            yield return new WaitForSeconds(0.05f);
        }

        // Assigning final side so you can use this value later in your game
        // for player movement for example
        finalSide2 = randomDiceSide2 + 1;

        // Show final dice value in Console
        Debug.Log("Opponent's Dice:"+finalSide2);
    }

    public int GetDice() 
    {
        return finalSide;
    }

    public int GetDice2() 
    {
        return finalSide2;
    }

    public IEnumerator CanAttackNowP1(float delay) 
    {
        yield return new WaitForSeconds(delay);
        CanAttackP1 = true;
    }
    public IEnumerator CanAttackNowP2(float delay)
    {
        yield return new WaitForSeconds(delay);
        CanAttackP2 = true;
    }
    public IEnumerator CountAttackP1(float del) 
    {
        yield return new WaitForSeconds(del);
        CounterAttackinP1 = false;
    }

    public IEnumerator CountAttackP2(float del)
    {
        yield return new WaitForSeconds(del);
        CounterAttackinP2 = false;
    }

    

    public void Discarded() 
    {
        //if (GetDice() > GetDice2()) {Debug.Log(" Dice Attack True");}
        Scene currentScene = SceneManager.GetActiveScene();

        bool isP1Turn = ButtonTurn.GetPlayerTurn();
        if (isP1Turn && !CounterAttackinP1)
        {//DisplayCard2
             //  bslot.SpendOnAttack();
            
            int val = BoardSlot.GetCurrentEnergy();
            if (val >= 2) 
            {
                val -= 2;
                BoardSlot.SetCurrentEnergy(val);
                bslot.energyText.text = val.ToString();
                Debug.Log("Value:"+val);
                
                UpdateP1CoinImages(val, bslot);

            }
            
            foreach (DisplayCard2 defenderCard in allDpCards)
                {
                    Debug.Log("Selected:"+defenderCard.isSelected);
                    if (defenderCard.isSelected)
                    {
                    if (CanAttackP1) 
                    {
                        Debug.Log("THIS IS CARD ATTACK:"+defenderCard.GetP1Power());
                        if ((GetDice() + defenderCard.GetP1Power()) > 2 * ((GetDice2()) + defenderCard.GetP2Power()))
                        {
                            Debug.Log("Attack More Than Twice");
                            //  Debug.Log("Discard Value:"+ defenderCard.GetDiscard());
                            //Destroy(defenderCard.gameObject);
                            //defenderCard.transform.position += new Vector3(600f,-300f,0f);
                            if (currentScene.name == "SampleScene")
                            {
                                if (defenderCard.photonView.Owner != PhotonNetwork.LocalPlayer) { defenderCard.photonView.RequestOwnership(); }
                            }

                            discaranimator.SetBool("isDiscard", true);
                            Transform discarcard = defenderCard.transform;
                            discarcard.SetParent(discardpile.transform);
                            if (currentScene.name == "AI")
                            {
                                btnTurn.CardPlacedToBoard.Remove(defenderCard.gameObject);
                            }
                            Debug.Log("POPup1:" + defenderCard.isSelected);
                            //defenderCard.OnPtClc(); //MADE CHANGES HERE
                            defenderCard.DisablePopUPAfterAttack();
                            Debug.Log("POPup2:" + defenderCard.isSelected);
                            DiscardSound();

                            //  healthValStP2 -= 2.5f;
                            //  SHoldHealthP2.text = healthValStP2.ToString();
                            // Notify other players about the discard action
                            photonView.RPC("SyncCardDiscard", RpcTarget.Others, defenderCard.photonView.ViewID);
                            break;
                        }
                        else if ((GetDice() + defenderCard.GetP1Power()) > ((GetDice2()) + defenderCard.GetP2Power()))
                        {
                            Debug.Log("ATTACKER Dice:" + GetDice() + "+" + "Attack:" + defenderCard.GetP1Power() + "=" + (GetDice() + defenderCard.GetP1Power()));
                            Debug.Log("DEFENSE Dice:" + GetDice2() + "+" + "Attack:" + defenderCard.GetP2Power() + "=" + (GetDice2() + defenderCard.GetP2Power()));

                            // Request ownership before moving the card
                            if (currentScene.name == "SampleScene")
                            {
                                if (defenderCard.photonView.Owner != PhotonNetwork.LocalPlayer) { defenderCard.photonView.RequestOwnership(); }
                            }

                            discaranimator.SetBool("isDiscard", true);
                            Transform discarcard = defenderCard.transform;
                            discarcard.SetParent(discardpile.transform);
                            if (currentScene.name == "AI")
                            {
                                btnTurn.CardPlacedToBoard.Remove(defenderCard.gameObject);
                            }
                            Debug.Log("POPup1:" + defenderCard.isSelected);
                            //defenderCard.OnPtClc(); //MADE CHANGES HERE
                            defenderCard.DisablePopUPAfterAttack();
                            Debug.Log("POPup2:" + defenderCard.isSelected);
                            DiscardSound();

                            // Notify other players about the discard action
                            photonView.RPC("SyncCardDiscard", RpcTarget.Others, defenderCard.photonView.ViewID);

                            CanAttackP1 = false;
                            StartCoroutine(CanAttackNowP1(10.0f));
                            break;
                        }
                        else if ((GetDice() + defenderCard.GetP1Power()) * 2 < ((GetDice2()) + defenderCard.GetP2Power()))   //APPLY DEFENSE MECHANISM HERE
                        {
                            Debug.Log("Defense More Than Twice");
                            Debug.Log("Hit");

                            int value = BoardSlot.GetCurrentEnergyP2();
                            value -= 1;
                            BoardSlot.SetCurrentEnergyP2(value); //czczczczczczc

                            UpdateP2CoinImages(value, bslot);

                            DiceSound();
                            StartCoroutine(RollAndDiscard());
                            CounterAttackinP1 = true;
                            CanAttackP1 = false;
                            StartCoroutine(CanAttackNowP1(10.0f));
                            foreach (DisplayCard atcCard in allDisplayCards)
                            {
                                if (atcCard.isSelected)
                                {
                                    StartCoroutine(delay());
                                }
                                IEnumerator delay()
                                {
                                    yield return new WaitForSeconds(3.0f);
                                    StartCoroutine(CountAttackP1(1.0f));
                                    Debug.Log("COUNTER ATTACK false hona chaahiye:" + CounterAttackinP1);
                                    if ((GetDice() + defenderCard.GetP1Power()) < ((GetDice2()) + defenderCard.GetP2Power()))
                                    {
                                        Debug.Log("Counter Attacked Succeeded");

                                        if (currentScene.name == "SampleScene")
                                        {
                                            if (atcCard.photonView.Owner != PhotonNetwork.LocalPlayer) { atcCard.photonView.RequestOwnership(); }
                                            // defenderCard.OnPtClc(); //MADE CHANGES HERE
                                        }
                                        animator2.SetBool("isDiscarded", true);
                                        Transform discarded = atcCard.transform;
                                        discarded.SetParent(discardpile2.transform);
                                        atcCard.OnPtcClk();
                                        DiscardSound();

                                        photonView.RPC("SyncCardDiscard", RpcTarget.Others, atcCard.photonView.ViewID);
                                        //   healthValStP1 -= 2.5f;
                                        //   SHoldHealthP1.text = healthValStP1.ToString();
                                    }
                                    else 
                                    { 
                                        Debug.Log("Counter Attacked Failed");
                                        AtcFailedTxt.SetActive(true);
                                        StartCoroutine(disableAtcTxt(1.0f));
                                    }
                                }
                            }
                            break;
                        }
                        else 
                        {
                            Debug.Log("Attack failed.");
                            AtcFailedTxt.SetActive(true);
                            StartCoroutine(disableAtcTxt(1.0f));
                            foreach (DisplayCard atcCard in allDisplayCards)
                            {
                                if (atcCard.isSelected)
                                {
                                    atcCard.OnPtcClk();
                                }
                            }
                        }
                    }
                    }
                }

            foreach (ShP2Card StrongholdCardP2 in allSHP2Cards) 
            {
                if (StrongholdCardP2.isSelected) 
                {
                    if (CanAttackP1) 
                    {
                        if ((GetDice() + StrongholdCardP2.GetP1Power()) > ((GetDice2()) + StrongholdCardP2.GetP2Power()))
                        {
                            Debug.Log("Attack Attack Attack");

                            Debug.Log("ATTACKER Dice:" + GetDice() + "+" + "Attack:" + StrongholdCardP2.GetP1Power() + "=" + (GetDice() + StrongholdCardP2.GetP1Power()));
                            Debug.Log("DEFENSE Dice:" + GetDice2() + "+" + "Attack:" + StrongholdCardP2.GetP2Power() + "=" + (GetDice2() + StrongholdCardP2.GetP2Power()));

                            int newHealth = StrongholdCardP2.GetCardHealth() - StrongholdCardP2.GetP1Power();

                            foreach (ShP2Card SHP2C in allSHP2Cards) 
                            {
                                SHP2C.SetSHealth(newHealth);
                                SHP2C.healthText.text = newHealth.ToString();
                                Debug.Log("New StrongHealth:"+newHealth);
                            }

                            CanAttackP1 = false;
                            StartCoroutine(CanAttackNowP1(10.0f));

                            if (StrongholdCardP2.GetCardHealth() <=0 ) 
                            {
                                Debug.Log("GAME OVER!, P1 Wins");
                             //discaranimator.SetBool("isDiscard", true);
                             // Transform discarcard = defenderCard.transform;
                             //  discarcard.SetParent(discardpile.transform);
                             //  DiscardSound();
                            }

                             //  healthValStP2 -= 2.5f;
                             //  SHoldHealthP2.text = healthValStP2.ToString();
                        }
                    }
                }
            }
                // discaranimator.SetBool("isDiscard", false);  make IEnumerator
                StartCoroutine(DiscardAnim(2.0f));
        }
            // isP1Turn = ButtonTurn.GetPlayerTurn() 
            // if(isP1Turn) then if(dp.isSelected == true) { Destroy(dp.gameobject) } 
            //defe.GetDiscard

            if (!isP1Turn && !CounterAttackinP2) 
            {
            //  bslot.SpendOnAttack2();
            int value = BoardSlot.GetCurrentEnergyP2();
            if (value >= 2)
            {
                value -= 2;
                BoardSlot.SetCurrentEnergyP2(value);
                bslot.energyTextP2.text = value.ToString();
                Debug.Log("Value:" + value);

                UpdateP2CoinImages(value, bslot);

            }



            Debug.Log("in dice attack is isp1turn false");
                //DisplayCard
                foreach (DisplayCard defcard in allDisplayCards)
                {
                if (defcard.isSelected)
                {
                    if (CanAttackP2) 
                    {
                        if ((GetDice() + defcard.Getp2Power()) > 2 * ((GetDice2()) + defcard.Getp1Power()))
                        {
                            Debug.Log("Attack is more than twice the defence");
                            if (currentScene.name == "SampleScene")
                            {
                                if (defcard.photonView.Owner != PhotonNetwork.LocalPlayer) { defcard.photonView.RequestOwnership(); }
                                defcard.OnPtcClk(); //MADE CHANGES HERE
                            }

                            animator2.SetBool("isDiscarded", true);
                            Transform discardCard = defcard.transform;
                            discardCard.SetParent(discardpile2.transform);
                            // discardCard.GetComponent<DisplayCard>().SetSelected(false);
                            DiscardSound();

                            photonView.RPC("SyncCardDiscard", RpcTarget.Others, defcard.photonView.ViewID);
                            //    healthValStP1 -= 2.5f;
                            //    SHoldHealthP1.text = healthValStP1.ToString();
                        }
                        else if ((GetDice() + defcard.Getp2Power()) > ((GetDice2()) + defcard.Getp1Power()))
                        {
                            Debug.Log("ATTACKER Dice:" + GetDice() + "+" + "Attack:" + defcard.Getp2Power() + "=" + (GetDice() + defcard.Getp2Power()));
                            Debug.Log("DEFENSE Dice:" + GetDice2() + "+" + "Attack:" + defcard.Getp1Power() + "=" + (GetDice2() + defcard.Getp1Power()));
                            // Debug.Log("Discard Value:" +defcard.GetDiscard());
                            // Destroy(defcard.gameObject);
                            if (currentScene.name == "SampleScene")
                            {
                                if (defcard.photonView.Owner != PhotonNetwork.LocalPlayer) { defcard.photonView.RequestOwnership(); }
                                defcard.OnPtcClk(); //MADE CHANGES HERE
                            }

                            animator2.SetBool("isDiscarded", true);
                            Transform discardCard = defcard.transform;
                            discardCard.SetParent(discardpile2.transform);
                            //discardCard.GetComponent<DisplayCard>().SetSelected(false);
                            DiscardSound();

                            //    healthValStP1 -= 2.5f;
                            //    SHoldHealthP1.text = healthValStP1.ToString();
                            // Notify other players about the discard action
                            photonView.RPC("SyncCardDiscard", RpcTarget.Others, defcard.photonView.ViewID);

                            CanAttackP2 = false;
                            StartCoroutine(CanAttackNowP2(10.0f));
                        }
                        else if ((GetDice() + defcard.Getp2Power()) * 2 < ((GetDice2()) + defcard.Getp1Power()))
                        {
                            Debug.Log("Defense More Than Twice");
                            Debug.Log("Hit");
                            DiceSound();

                            int val = BoardSlot.GetCurrentEnergy();
                            val -= 1;
                            BoardSlot.SetCurrentEnergy(val); //czczczczczczc

                            UpdateP1CoinImages(val, bslot);

                            StartCoroutine(RollAndDiscard());
                            CounterAttackinP2 = true;
                            CanAttackP2 = false;
                            StartCoroutine(CanAttackNowP2(10.0f));
                            foreach (DisplayCard2 atcCard in allDpCards)
                            {
                                if (atcCard.isSelected)
                                {
                                    StartCoroutine(delayed());
                                }
                                IEnumerator delayed()
                                {
                                    yield return new WaitForSeconds(3.0f);
                                    StartCoroutine(CountAttackP2(1.0f));
                                    Debug.Log("COUNTER ATTACK false hona chaahiye:" + CounterAttackinP2);
                                    if ((GetDice() + defcard.Getp2Power()) < ((GetDice2()) + defcard.Getp1Power()))
                                    {
                                        Debug.Log("Counter Attacked Succeeded");

                                        discaranimator.SetBool("isDiscard", true);
                                        Transform discards = atcCard.transform;
                                        discards.SetParent(discardpile.transform);
                                        if (currentScene.name == "SampleScene")
                                        {
                                            if (atcCard.photonView.Owner != PhotonNetwork.LocalPlayer) { atcCard.photonView.RequestOwnership(); }
                                            atcCard.OnPtClc();
                                        }
                                        Debug.Log("Discard Pile Name:" + discardpile.name);
                                        DiscardSound();

                                        // Notify other players about the discard action
                                        photonView.RPC("SyncCardDiscard", RpcTarget.Others, atcCard.photonView.ViewID);

                                        //   healthValStP2 -= 2.5f;
                                        //   SHoldHealthP2.text = healthValStP2.ToString();
                                    }
                                    else { Debug.Log("Counter Attacked Failed"); }
                                }
                            }
                        }
                        else 
                        {
                            Debug.Log("Ai Attack Failed");
                            AtcFailedTxt.SetActive(true);
                            StartCoroutine(disableAtcTxt(1.0f));
                        }
                    }
                }
            }

            foreach (ShP1Card StrongholdCardP1 in allSHP1Cards)
            { 
            if (StrongholdCardP1.isSelected)
            {
                if (CanAttackP2) 
                {
                    if ((GetDice() + StrongholdCardP1.GetP2Power()) > ((GetDice2()) + StrongholdCardP1.GetP1Power()))
                    {
                        Debug.Log("ATTACKER Dice:" + GetDice() + "+" + "Attack:" + StrongholdCardP1.GetP2Power() + "=" + (GetDice() + StrongholdCardP1.GetP2Power()));
                        Debug.Log("DEFENSE Dice:" + GetDice2() + "+" + "Attack:" + StrongholdCardP1.GetP1Power() + "=" + (GetDice2() + StrongholdCardP1.GetP1Power()));

                        int newHealthP1 = StrongholdCardP1.GetCardHealth() - StrongholdCardP1.GetP2Power();

                            foreach (ShP1Card SHP1C in allSHP1Cards)
                            { 
                                SHP1C.SetSHealth(newHealthP1);
                                SHP1C.healthText.text = newHealthP1.ToString();
                                Debug.Log("New StrongHealth:" + newHealthP1);
                            }

                        CanAttackP2 = false;
                        StartCoroutine(CanAttackNowP2(10.0f));

                        if (StrongholdCardP1.GetCardHealth() <=0) 
                        {
                            Debug.Log("GAME OVER, P2 WINS");
                        // Debug.Log("Discard Value:" +defcard.GetDiscard());
                        // Destroy(defcard.gameObject);
                        // animator2.SetBool("isDiscarded", true);
                        //Transform discardCard = defcard.transform;
                        //discardCard.SetParent(discardpile2.transform);
                        //DiscardSound();
                        }

                        //    healthValStP1 -= 2.5f;
                        //    SHoldHealthP1.text = healthValStP1.ToString();
                       
                    }
                }
            }
            }

                StartCoroutine(DiscardAnim2(2.0f));
            }
    }

    public void DiceSound() 
    {
        // src.clip = diceClip;
        // src.Play();
        AudioManager.instance.PlaySFX("Dice");
    }

    public void DiscardSound()
    {
        //src.clip = discardedClip;
        //src.Play();
        AudioManager.instance.PlaySFX("Discarded");
    }

    private IEnumerator DiscardAnim(float delay) 
    {
        yield return new WaitForSeconds(delay);
        discaranimator.SetBool("isDiscard", false);
    }

    private IEnumerator DiscardAnim2(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator2.SetBool("isDiscarded", false);
    }

    public void UpdateP1CoinImages(int val, BoardSlot bslot)
    {
        GameObject[] coinImages = new GameObject[] {
        bslot.coinP1img, bslot.coinP1img2, bslot.coinP1img3, bslot.coinP1img4,
        bslot.coinP1img5, bslot.coinP1img6, bslot.coinP1img7, bslot.coinP1img8
    };

        for (int i = 7; i >= val; i--)
        {
            coinImages[i].SetActive(false);
        }
    }

    public void UpdateP2CoinImages(int value, BoardSlot bslot)
    {
        GameObject[] coinP2Images = new GameObject[] {
        bslot.coinP2img8, bslot.coinP2img7, bslot.coinP2img6, bslot.coinP2img5,
        bslot.coinP2img4, bslot.coinP2img3, bslot.coinP2img2, bslot.coinP2img
    };

        for (int i = 7; i >= value; i--)
        {
            coinP2Images[i].SetActive(false);
        }
    }

    IEnumerator disableAtcTxt(float del) 
    {
        yield return new WaitForSeconds(del);
        AtcFailedTxt.SetActive(false);
    }
}
