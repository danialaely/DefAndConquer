using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class DisplayCard2 : MonoBehaviourPunCallbacks, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IOnEventCallback
{
    public List<Card> display = new List<Card>();
    public int displayId;
    public int carddid;

    public TMP_Text nameText;
    public TMP_Text attackText;
    public TMP_Text healthText;
    public TMP_Text energyText;
   // public TMP_Text defenseText;
    public Image crdImage;

    public CCardShuffler shuffler;

    private CanvasGroup canvasGroup;
    private Vector3 initialPosition;

    public bool isSelected = false;

    public string tagToSearch = "Player1";
    GameObject[] player1;

    public List<GameObject> adjCards = new List<GameObject>();

    public List<DisplayCard2> allDisplayCards; // Reference to all DisplayCard instances
    public UnityEngine.UI.Image outerBorder;

    public Image dice1;
    public Image dice2;

    private static bool DisCard;

    public List<BoardSlot> BoSlots; //Reference to all BoardSlots

    public GameManager gm;

    public static int P1Power;
    public static int P2Power;

    public bool canMove;

    public GameObject PopUpCardP2;
    public TMP_Text pop2NameTxt;
    public TMP_Text pop2AttackTxt;
    public TMP_Text pop2HealthTxt;
    public TMP_Text pop2EnergyTxt;
    public Image pop2CardImg;
    public UnityEngine.UI.Image popOuterBdr;

    public Animator popupanim2;

    public List<GameObject> movementAdjacentCards = new List<GameObject>();
    private string Movtag = "Player2";
    GameObject[] player2;

    public List<Transform> MoveBoardSlots = new List<Transform>();

    public GameObject discardpile;
    public Animator discaranimator;
    public AudioClip discardedClip;
    public AudioSource src;

    public List<ShP1Card> allSHP1Cards;

    List<DisplayCard> SelectedCardsP1;
    public DisplayCard dpcrd1;

    //public List<DisplayCard2> p2AdjacentToP1 = new List<DisplayCard2>();  //Copied from DisplayCard1
    //public List<DisplayCard2> selectedP2Cards = new List<DisplayCard2>(); //Copied from DisplayCard1

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        UpdateCardInformation();

        player1 = GameObject.FindGameObjectsWithTag(tagToSearch);
        player2 = GameObject.FindGameObjectsWithTag(Movtag);

        allSHP1Cards = new List<ShP1Card>(FindObjectsOfType<ShP1Card>());
        // Populate allDisplayCards with all instances of DisplayCard
        allDisplayCards = new List<DisplayCard2>(FindObjectsOfType<DisplayCard2>());
        outerBorder = this.transform.Find("OuterBorder").GetComponent<Image>();
        popOuterBdr = PopUpCardP2.transform.Find("OuterBorder").GetComponent<Image>();

        BoSlots = new List<BoardSlot>(FindObjectsOfType<BoardSlot>());

        SelectedCardsP1 = dpcrd1.SelectedP1Cards();

        dice1.enabled = false;
        dice2.enabled = false;

        DisCard = false;
        canMove = true;

        Scene cs = SceneManager.GetActiveScene();
        if (cs.name == "SampleScene") 
        {
            carddid = this.GetComponent<PhotonView>().ViewID;
            PhotonNetwork.AddCallbackTarget(this);
        }
    }

    public List<Transform> AdjacentBSlotsAvailable()
    {
        return MoveBoardSlots;
    }

    public IEnumerator CanMoveNow(float delay)
    {
        yield return new WaitForSeconds(delay);
        canMove = true;
    }

    public void UpdateCardInformation()
    {
        Scene cs = SceneManager.GetActiveScene();
        if (cs.name == "AI") 
        {
            Card cardd = display.Find(c => c.cardId == displayId);

            if (cardd != null)
            {
                nameText.text = cardd.cardName;
                attackText.text = cardd.cardAttack.ToString();
                healthText.text = cardd.cardHealth.ToString();
                energyText.text = cardd.cardEnergy.ToString();
            //  defenseText.text = cardd.cardDefence.ToString();
                crdImage.sprite = cardd.cardImage;
            }
            else
            {
                nameText.text = "Card Not Found";
                attackText.text = " ";
                healthText.text = " ";
                energyText.text = " ";
             // defenseText.text = " ";
                crdImage.sprite = null;
            }
        }

    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 5) 
        {
            object[] data = (object[])photonEvent.CustomData;
            int receivedDisplayId = (int)data[0];
            int receivedCardId = (int)data[1];

            if (this.carddid == receivedCardId)
            {
                this.displayId = receivedDisplayId;
                UpdateClientInfo(receivedDisplayId);
            }
        }
    }

    public void UpdateClientInfo(int dpID) 
    {
        displayId = dpID;
        Card cardd = display.Find(c => c.cardId == displayId);

        if (cardd != null)
        {
            nameText.text = cardd.cardName;
            attackText.text = cardd.cardAttack.ToString();
            healthText.text = cardd.cardHealth.ToString();
            energyText.text = cardd.cardEnergy.ToString();
            //  defenseText.text = cardd.cardDefence.ToString();
            crdImage.sprite = cardd.cardImage;
        }
        else
        {
            nameText.text = "Card Not Found";
            attackText.text = " ";
            healthText.text = " ";
            energyText.text = " ";
            // defenseText.text = " ";
            crdImage.sprite = null;
        }
    }

    [PunRPC]
    public void DisableCardBackRPC()
    {
        Image carddBackImage = transform.Find("Back").GetComponent<Image>();
        if (carddBackImage != null)
        {
            carddBackImage.enabled = false;
        }
    }

    [PunRPC]
    public void SyncCardDiscard(int cardViewID)
    {
        PhotonView cardView = PhotonView.Find(cardViewID);
        if (cardView != null)
        {
            Transform discarcard = cardView.transform;
            discarcard.SetParent(discardpile.transform);

            // Optionally, trigger any animations or effects on the Client side
            discaranimator.SetBool("isDiscard", true);
            DiscardSound();
        }
    }

    [PunRPC]
    public void SyncCardPlacement2(int boardSlotViewID)
    {
        PhotonView boardSlotPhotonView = PhotonView.Find(boardSlotViewID);
        if (boardSlotPhotonView != null)
        {
            Transform boardSlotTransform = boardSlotPhotonView.transform;
            transform.SetParent(boardSlotTransform);
            transform.localPosition = Vector3.zero;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            // photonView.RPC("DisableCardBack2RPC", RpcTarget.All);
            // Add any other code that needs to run on placement
        }
    }

    public void DiscardSound()
    {
        // src.clip = discardedClip;
        // src.Play();
        AudioManager.instance.PlaySFX("Discarded");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        initialPosition = transform.position;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // Determine if we are in the Play phase and the card is in Hand2
        bool isPlayPhase = gm.currentPhase == GamePhase.Play && transform.parent.name == "Hand2";
        bool isMovePhase = gm.currentPhase == GamePhase.Move && transform.parent.tag == "BSlot";

        foreach (BoardSlot bslot in BoSlots)
        {
            if (isPlayPhase && bslot.PreviouslyPlacedAvailableP2().Contains(bslot.transform))
            {
                bslot.GetComponent<Image>().color = Color.green;
            }

            if (isMovePhase)
            {
                CacheAndHighlightMoveOptions(bslot);
            }
        }
    }

    private void CacheAndHighlightMoveOptions(BoardSlot bslot)
    {
        List<GameObject> adjacentCards = new List<GameObject>();
        Dictionary<GameObject, DisplayCard2> displayCardCache = new Dictionary<GameObject, DisplayCard2>();
        Dictionary<Transform, Image> imageCache = new Dictionary<Transform, Image>();

        Vector3 currentPosition = transform.position;

        // Cache adjacent cards based on their distance and type
        foreach (GameObject p2 in player2)
        {
            float distance = Vector3.Distance(p2.transform.position, currentPosition);
            if (distance >= 400f || p2 == gameObject) continue;

            if (p2.name == "SHCardP2")
            {
                adjacentCards.Add(p2);
            }
            else
            {
                if (!displayCardCache.TryGetValue(p2, out DisplayCard2 displayCard2))
                {
                    displayCard2 = p2.GetComponent<DisplayCard2>();
                    displayCardCache[p2] = displayCard2;
                }

                if (displayCard2 != null && displayCard2.canMove)
                {
                    if (distance < 280f && p2 != this.gameObject)
                    { 
                        adjacentCards.Add(p2);
                    }
                }
            }
        }

        // Highlight slots based on cached adjacent cards
        HashSet<Transform> processedSlots = new HashSet<Transform>();

        foreach (GameObject gb in adjacentCards)
        {
            float dist = Vector3.Distance(bslot.transform.position, gb.transform.position);

            if (dist < 280f && processedSlots.Add(bslot.transform))
            {
                if (!imageCache.TryGetValue(bslot.transform, out Image image))
                {
                    image = bslot.GetComponent<Image>();
                    imageCache[bslot.transform] = image;
                }

                if (image != null)
                {
                    image.color = Color.green;
                    MoveBoardSlots.Add(bslot.transform);
                }
            }
        }
    }


    public void OnDrag(PointerEventData eventData)
    {
        int dragging = BoardSlot.GetCurrentEnergy();
        int dragging2 = BoardSlot.GetCurrentEnergyP2();
        bool isP1Turn = ButtonTurn.GetPlayerTurn();

        if (transform.parent != null && transform.parent.name == "Hand")
        {
            if (dragging >= 0)
            {
                transform.position = Input.mousePosition;
            }
        }

        if (transform.parent != null && transform.parent.name == "Hand2" && !isP1Turn && gm.currentPhase == GamePhase.Play)
        {
            if (dragging2 >= 0)
            {
                // transform.position = Input.mousePosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, Camera.main, out Vector2 localPos);
                transform.localPosition = localPos;
            }
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Card card = display.Find(c => c.cardId == displayId);

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        foreach (BoardSlot bslot in BoSlots)
        {
            for (int i = 0; i < bslot.transform.parent.childCount; i++)
            {
                Transform slot = bslot.transform.parent.GetChild(i);
                slot.GetComponent<Image>().color = Color.white;
            }
        }
        MoveBoardSlots.Clear();

        // If the card is not dropped on a slot, return it to the initial position.
        if (transform.parent == null || transform.parent.CompareTag("Hand"))
        {
            transform.position = initialPosition;

            int cardEnergy = GetCardEnergy();

        }

        foreach (GameObject p2 in player2)
        {
            float distance = Vector3.Distance(p2.transform.position, transform.position);

            if (distance < 280f)
            {
                UnityEngine.UI.Image p1outerborder = p2.transform.Find("OuterBorder").GetComponent<Image>();
               // p1outerborder.color = Color.black;

                //adjacentCards.Add(p1);
                movementAdjacentCards.Clear();
            }
        }
    }

    public int GetCardEnergy()
    {
        Card cardd = display.Find(c => c.cardId == displayId);
        if (cardd != null)
        {
            return cardd.cardEnergy;
        }
        return 0; // Return 0 if the card is not found.
    }

    public int GetCardAttack() 
    {
        Card cardd = display.Find(c => c.cardId == displayId);
        if (cardd != null)
        {
            return cardd.cardAttack;
        }
        return 0; // Return 0 if the card is not found.
    }

    public int GetCardHealth()
    {
        Card cardd = display.Find(c => c.cardId == displayId);
        if (cardd != null)
        {
            return cardd.cardHealth;
        }
        return 0; // Return 0 if the card is not found.
    }

    public int GetCardDefense() 
    {
        Card cardd = display.Find(c => c.cardId == displayId);
        if (cardd != null)
        {
            return cardd.cardDefence;
        }
        return 0;
    }

    public void OnPtClc() 
    {
        // isSelected = !isSelected;
        bool isP1Turn = ButtonTurn.GetPlayerTurn();
        Debug.Log("DISPLAY C2 PTC CALLED");
     /*   bool isZoom = Zoom.GetBool();
        if (isZoom)
        {
            // Vector3 offt1 = new Vector3(-400f, 0, 0);
            // dice1.transform.position = this.transform.position + offt1;
            // dice2.transform.position = this.transform.position - offt1;
        } */

        if (!isP1Turn)
        {
            isSelected = !isSelected;
            if (isSelected)
            {
              //  PopUpCardP2.SetActive(true);
                pop2NameTxt.text = nameText.text;
                pop2AttackTxt.text = attackText.text;
                pop2EnergyTxt.text = energyText.text;
                pop2HealthTxt.text = healthText.text;
                pop2CardImg.sprite = crdImage.sprite;
                popOuterBdr.color = Color.yellow;
                popupanim2.SetBool("Select",true);

                outerBorder.color = Color.white;

                foreach (GameObject p1 in player1)
                {
                    float distance = Vector3.Distance(p1.transform.position, transform.position);

                    if (distance < 280f)
                    {
                        UnityEngine.UI.Image p2outerborder = p1.transform.Find("OuterBorder").GetComponent<Image>();
                        p2outerborder.color = Color.blue;

                        adjCards.Add(p1);
                        if (p1.gameObject.name == "SHCardP1")
                        {
                            foreach (ShP1Card STCardP1 in allSHP1Cards)
                            {
                                UnityEngine.UI.Image shp1outerborder = STCardP1.transform.Find("OuterBorder").GetComponent<Image>();
                                shp1outerborder.color = Color.blue;
                            }
                        }
                    }
                }

                foreach (DisplayCard2 othercard in allDisplayCards)
                {
                    if (othercard != this && othercard.isSelected)
                    {
                        //othercard.isSelected = false;
                        othercard.OnPtClc();
                        othercard.adjCards.Clear();
                        othercard.outerBorder.color = Color.yellow;
                    }
                }

            }
            if (!isSelected)
            {
                // PopUpCardP2.SetActive(false);
                popupanim2.SetBool("Select",false);
               // StartCoroutine(PopUpActiveFalse(0.6f));
                outerBorder.color = Color.yellow;

                foreach (GameObject p1 in player1)
                {
                    float distance = Vector3.Distance(p1.transform.position, transform.position);

                    if (distance < 280f)
                    {
                        UnityEngine.UI.Image p1outerborder = p1.transform.Find("OuterBorder").GetComponent<Image>();
                        p1outerborder.color = Color.black;

                        adjCards.Clear();

                        if (p1.gameObject.name == "SHCardP1")
                        {
                            foreach (ShP1Card STCardP1 in allSHP1Cards)
                            {
                                UnityEngine.UI.Image shp1outerborder = STCardP1.transform.Find("OuterBorder").GetComponent<Image>();
                                shp1outerborder.color = Color.black;
                            }
                        }
                        // Check if DisplayCard component is attached
                        DisplayCard displayCard = p1.GetComponent<DisplayCard>();
                        if (displayCard != null)
                        {
                            bool selected = displayCard.GetSelected();
                            if (selected)
                            {
                                Debug.Log("IT WAS SELECTED BEFORE: " + p1.name);
                                selected = false;
                                displayCard.SetSelected(selected);
                                displayCard.OnPtcClk();
                                dice1.enabled = false;
                                dice2.enabled = false;
                                //dice1.gameObject.SetActive(false);
                                //dice2.gameObject.SetActive(false);
                            }
                        }
                        else
                        {
                            // Check if ShP1Card component is attached
                            ShP1Card shP1Card = p1.GetComponent<ShP1Card>();
                            if (shP1Card != null)
                            {
                                bool selection = shP1Card.isSelected;
                                if (selection)
                                {
                                    Debug.Log("IT WAS SELECTED BEFORE: " + p1.name);
                                   // selection = false;
                                   // shP1Card.SetSelection(selection);
                                    shP1Card.OnPtcClick();
                                    dice1.enabled = false;
                                    dice2.enabled = false;
                                }
                            }
                        }
                    }
                }

            }
        }

        if (isP1Turn)
        {
            foreach (GameObject displayCardObject in player1)
            {
                DisplayCard dp = displayCardObject.GetComponent<DisplayCard>();
                if (dp != null && dp.adjacentCards.Contains(gameObject))
                {
                    foreach (DisplayCard2 lastselected in allDisplayCards)
                    {
                        if (lastselected.isSelected)
                        {
                            // Deselect the last selected card
                            lastselected.isSelected = false;
                            lastselected.outerBorder.color = Color.blue;
                        }
                    }

                    // Toggle the selection status of the current card only once
                    isSelected = !isSelected;

                    // Break out of the loop once the card is selected/deselected
                    break;
                }
            }


            if (isSelected) //(3):ATTACKING PHASE
            {
                if (gm.currentPhase == GamePhase.Attack)
                {
                  //  PopUpCardP2.SetActive(true);
                    pop2NameTxt.text = nameText.text;
                    pop2AttackTxt.text = attackText.text;
                    pop2EnergyTxt.text = energyText.text;
                    pop2HealthTxt.text = healthText.text;
                    pop2CardImg.sprite = crdImage.sprite;
                    popOuterBdr.color = Color.red;
                    popupanim2.SetBool("Select",true);
                }

                int totalAttack = 0; // Variable to accumulate the total attack

                foreach (GameObject displayCardObject in player1)
                {
                    DisplayCard dp = displayCardObject.GetComponent<DisplayCard>();

                    // Check if the card is adjacent, there is enough energy, and it's the Attack phase
                    if (dp != null && dp.adjacentCards.Contains(gameObject) && BoardSlot.GetCurrentEnergy() >= 2 && gm.currentPhase == GamePhase.Attack)
                    {
                        // Set the border color to indicate selection
                        outerBorder.color = Color.red;

                        // Enable dice for attack calculation (or some visual feedback)
                        dice1.enabled = true;
                        dice2.enabled = true;

                        // Play the attack sound
                        shuffler.AttackSound();

                        // Accumulate the attack power
                        totalAttack += dp.GetCardAttack();

                        // Log individual attack for each selected card
                        Debug.Log("Player1 Card's Attack: " + dp.GetCardAttack());

                        // Optionally, you can store or compare P1Power and P2Power if needed
                        P1Power = totalAttack;  // P1Power is now the total attack from all selected cards
                        P2Power = this.GetCardHealth();  // Assuming this refers to the current game object’s health

                        // You can implement your discard or combat logic here
                        /* if (this.GetCardAttack() < dp.GetCardAttack()) 
                        {
                            //shuffler.DiscardSound();
                            DisCard = true; 
                        } */
                    }
                }

                // Log the total attack power after summing up all selected cards
                Debug.Log("Total Attack Power from all selected cards: " + totalAttack);
            }
            if (!isSelected)
            {
                // PopUpCardP2.SetActive(false);
                popupanim2.SetBool("Select",false);
               // StartCoroutine(PopUpActiveFalse(0.6f));

                foreach (GameObject displayCardObject in player1)
                {
                    DisplayCard dp = displayCardObject.GetComponent<DisplayCard>();
                    if (dp != null && dp.adjacentCards.Contains(gameObject))
                    {
                        outerBorder.color = Color.blue;   //MainCamera Blue Color: 314D79    Board Color: 292E48 
                        dice1.enabled = false;
                        dice2.enabled = false;
                    }
                }
            }
        }

        Debug.Log(isSelected);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnPtClc();
    }

    public void DisablePopUPAfterAttack() 
    {
        isSelected = false;
        popupanim2.SetBool("Select", false);
        dice1.enabled = false;
        dice2.enabled = false;

        foreach (GameObject displayCardObject in player1) 
        {
            DisplayCard dp = displayCardObject.GetComponent<DisplayCard>();
            if (dp != null && dp.adjacentCards.Contains(gameObject))
            {
                dp.OnPtcClk();
            }
        }
    }

    public bool GetDiscard() 
    {
        return DisCard;
    }

    public int GetP1Power() 
    {
        return P1Power;
    }

    public int GetP2Power() 
    {
        return P2Power;
    }

    public void SetSelected(bool select) 
    {
        isSelected = select;
    }

    IEnumerator PopUpActiveFalse(float delay) 
    {
        yield return new WaitForSeconds(delay);
       // PopUpCardP2.SetActive(false);
    }

    
}
