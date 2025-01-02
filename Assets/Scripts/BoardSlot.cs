using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoardSlot : MonoBehaviourPunCallbacks, IDropHandler
{
    public TMP_Text energyText;
    public TMP_Text energyTextP2;
    // public GameObject hand;

    // Make currentEnergy static so that it's shared among all instances.
    private static int currentEnergy = 0;
    private static int currentEnergyP2 = 0;

    public HealthBar healthBar;

    public GameManager gmmm;
    // End Turn Button to move between Turns

    public AudioSource src;
    public AudioClip cardPlacementClip;

    public Color highlightColor;
    private Color originalColor;

    public GameObject coinP1img;
    public GameObject coinP1img2;
    public GameObject coinP1img3;
    public GameObject coinP1img4;
    public GameObject coinP1img5;
    public GameObject coinP1img6;
    public GameObject coinP1img7;
    public GameObject coinP1img8;

    public GameObject coinP2img;
    public GameObject coinP2img2;
    public GameObject coinP2img3;
    public GameObject coinP2img4;
    public GameObject coinP2img5;
    public GameObject coinP2img6;
    public GameObject coinP2img7;
    public GameObject coinP2img8;

    public static List<Transform> availableBSlotsAI = new List<Transform>();
    public static List<Transform> availableBSlotsAIMove = new List<Transform>();

    public static List<Transform> adjacentBSlotsPlayer1 = new List<Transform>(); //Player1 adjacent BoardSlots

    public static List<Transform> cardsPlacedInPreviousTurn = new List<Transform>(); //Keeping track of cards placed in previous turns
    public static List<Transform> cardsPlacedInPreviousTurnP2 = new List<Transform>(); //FOR P2 Keeping track of cards placed in previous turns

    private void Start()
    {
     //   UpdateMoveListP2();
        AdjacentBslotListP1();
        cardsPlacedInPreviousTurn.Clear();

        AiPlay();

        UpdatePreviousCardsList();
        Debug.Log("Previously Placed Cards P1:"+PreviouslyPlacedAvailable().Count);

        UpdatePreviousCardsListP2();

       // UpdateMovementPhaseList();
       // Debug.Log("Available Slots:"+availableBSlotsAI.Count);
    }
    #region Ai Logics
    public void AiPlay() 
    {
        availableBSlotsAI.Clear();

        foreach (Transform slot in transform.parent)
        {
            int rwindex = slot.GetSiblingIndex();

            // Check if any adjacent slot has a card tagged as "Player2"
            if ((rwindex > 0 && transform.parent.GetChild(rwindex - 1).childCount > 0 && transform.parent.GetChild(rwindex - 1).GetChild(0).CompareTag("Player2")) ||
                (rwindex > 12 && transform.parent.GetChild(rwindex - 13).childCount > 0 && transform.parent.GetChild(rwindex - 13).GetChild(0).CompareTag("Player2")) ||
                (rwindex > 13 && transform.parent.GetChild(rwindex - 14).childCount > 0 && transform.parent.GetChild(rwindex - 14).GetChild(0).CompareTag("Player2")) ||
                (rwindex > 14 && transform.parent.GetChild(rwindex - 15).childCount > 0 && transform.parent.GetChild(rwindex - 15).GetChild(0).CompareTag("Player2")) ||
                (rwindex < transform.parent.childCount - 1 && transform.parent.GetChild(rwindex + 1).childCount > 0 && transform.parent.GetChild(rwindex + 1).GetChild(0).CompareTag("Player2")) ||
                (rwindex < transform.parent.childCount - 13 && transform.parent.GetChild(rwindex + 13).childCount > 0 && transform.parent.GetChild(rwindex + 13).GetChild(0).CompareTag("Player2")) ||
                (rwindex < transform.parent.childCount - 14 && transform.parent.GetChild(rwindex + 14).childCount > 0 && transform.parent.GetChild(rwindex + 14).GetChild(0).CompareTag("Player2")) ||
                (rwindex < transform.parent.childCount - 15 && transform.parent.GetChild(rwindex + 15).childCount > 0 && transform.parent.GetChild(rwindex + 15).GetChild(0).CompareTag("Player2")))
            {
                // cardsPlacedInPreviousTurn.Add(slot);
                availableBSlotsAI.Add(transform.parent.GetChild(rwindex));
            }
        }
    }

    public List<Transform> Available()
    {
        return availableBSlotsAI;
    }

  /*  public void UpdateMoveListP2() 
    {
        availableBSlotsAIMove.Clear(); // Clear the list before updating it for the new turn

        // Iterate through all slots to find adjacent slots with a card tagged as "Player2"
        foreach (Transform slot in transform.parent)
        {
            int rwindex = slot.GetSiblingIndex();

            // Check if any adjacent slot has a card tagged as "Player2"
            if ((rwindex > 0 && transform.parent.GetChild(rwindex - 1).childCount > 0 && transform.parent.GetChild(rwindex - 1).GetChild(0).CompareTag("Player2")) ||
                (rwindex > 12 && transform.parent.GetChild(rwindex - 13).childCount > 0 && transform.parent.GetChild(rwindex - 13).GetChild(0).CompareTag("Player2")) ||
                (rwindex > 13 && transform.parent.GetChild(rwindex - 14).childCount > 0 && transform.parent.GetChild(rwindex - 14).GetChild(0).CompareTag("Player2")) ||
                (rwindex > 14 && transform.parent.GetChild(rwindex - 15).childCount > 0 && transform.parent.GetChild(rwindex - 15).GetChild(0).CompareTag("Player2")) ||
                (rwindex < transform.parent.childCount - 1 && transform.parent.GetChild(rwindex + 1).childCount > 0 && transform.parent.GetChild(rwindex + 1).GetChild(0).CompareTag("Player2")) ||
                (rwindex < transform.parent.childCount - 13 && transform.parent.GetChild(rwindex + 13).childCount > 0 && transform.parent.GetChild(rwindex + 13).GetChild(0).CompareTag("Player2")) ||
                (rwindex < transform.parent.childCount - 14 && transform.parent.GetChild(rwindex + 14).childCount > 0 && transform.parent.GetChild(rwindex + 14).GetChild(0).CompareTag("Player2")) ||
                (rwindex < transform.parent.childCount - 15 && transform.parent.GetChild(rwindex + 15).childCount > 0 && transform.parent.GetChild(rwindex + 15).GetChild(0).CompareTag("Player2")))
            {
               // availableBSlotsAIMove.Add(slot);
            }
        }
        //  else { if (availableBSlotsAIMove.Contains(gameObject.transform)) { availableBSlotsAIMove.Remove(transform.parent.GetChild(rwindex)); }}
    }*/

  /*  public List<Transform> MoveAvailable() 
    {
        return availableBSlotsAIMove;
    }*/

    public void AdjacentBslotListP1()  //Adjacent P1 BSlot List
    {
        adjacentBSlotsPlayer1.Clear(); // Clear the list before updating it for the new turn

        // Iterate through all slots to find adjacent slots with a card tagged as "Player1"
        foreach (Transform slot in transform.parent)
        {
            int rwindex = slot.GetSiblingIndex();

            // Check if any adjacent slot has a card tagged as "Player1"
            if ((rwindex > 0 && transform.parent.GetChild(rwindex - 1).childCount > 0 && transform.parent.GetChild(rwindex - 1).GetChild(0).CompareTag("Player1")) ||
                (rwindex > 12 && transform.parent.GetChild(rwindex - 13).childCount > 0 && transform.parent.GetChild(rwindex - 13).GetChild(0).CompareTag("Player1")) ||
                (rwindex > 13 && transform.parent.GetChild(rwindex - 14).childCount > 0 && transform.parent.GetChild(rwindex - 14).GetChild(0).CompareTag("Player1")) ||
                (rwindex > 14 && transform.parent.GetChild(rwindex - 15).childCount > 0 && transform.parent.GetChild(rwindex - 15).GetChild(0).CompareTag("Player1")) ||
                (rwindex < transform.parent.childCount - 1 && transform.parent.GetChild(rwindex + 1).childCount > 0 && transform.parent.GetChild(rwindex + 1).GetChild(0).CompareTag("Player1")) ||
                (rwindex < transform.parent.childCount - 13 && transform.parent.GetChild(rwindex + 13).childCount > 0 && transform.parent.GetChild(rwindex + 13).GetChild(0).CompareTag("Player1")) ||
                (rwindex < transform.parent.childCount - 14 && transform.parent.GetChild(rwindex + 14).childCount > 0 && transform.parent.GetChild(rwindex + 14).GetChild(0).CompareTag("Player1")) ||
                (rwindex < transform.parent.childCount - 15 && transform.parent.GetChild(rwindex + 15).childCount > 0 && transform.parent.GetChild(rwindex + 15).GetChild(0).CompareTag("Player1")))
            {
                adjacentBSlotsPlayer1.Add(slot);
            }
        }
    }

    public List<Transform> AdjacentP1Available()
    {
        return adjacentBSlotsPlayer1;
    }
    #endregion Ai Logics

    public void UpdatePreviousCardsList() 
    {
        cardsPlacedInPreviousTurn.Clear();

        foreach (Transform slot in transform.parent)
        {
            int rwindex = slot.GetSiblingIndex();

            // Check if any adjacent slot has a card tagged as "Player1"
            if ((rwindex > 0 && transform.parent.GetChild(rwindex - 1).childCount > 0 && transform.parent.GetChild(rwindex - 1).GetChild(0).CompareTag("Player1")) ||
                (rwindex > 12 && transform.parent.GetChild(rwindex - 13).childCount > 0 && transform.parent.GetChild(rwindex - 13).GetChild(0).CompareTag("Player1")) ||
                (rwindex > 13 && transform.parent.GetChild(rwindex - 14).childCount > 0 && transform.parent.GetChild(rwindex - 14).GetChild(0).CompareTag("Player1")) ||
                (rwindex > 14 && transform.parent.GetChild(rwindex - 15).childCount > 0 && transform.parent.GetChild(rwindex - 15).GetChild(0).CompareTag("Player1")) ||
                (rwindex < transform.parent.childCount - 1 && transform.parent.GetChild(rwindex + 1).childCount > 0 && transform.parent.GetChild(rwindex + 1).GetChild(0).CompareTag("Player1")) ||
                (rwindex < transform.parent.childCount - 13 && transform.parent.GetChild(rwindex + 13).childCount > 0 && transform.parent.GetChild(rwindex + 13).GetChild(0).CompareTag("Player1")) ||
                (rwindex < transform.parent.childCount - 14 && transform.parent.GetChild(rwindex + 14).childCount > 0 && transform.parent.GetChild(rwindex + 14).GetChild(0).CompareTag("Player1")) ||
                (rwindex < transform.parent.childCount - 15 && transform.parent.GetChild(rwindex + 15).childCount > 0 && transform.parent.GetChild(rwindex + 15).GetChild(0).CompareTag("Player1")))
            {
                cardsPlacedInPreviousTurn.Add(slot);
            }
        }
    }

    public List<Transform> PreviouslyPlacedAvailable() 
    {
        return cardsPlacedInPreviousTurn ;
    }

    public void UpdatePreviousCardsListP2()
    {
        cardsPlacedInPreviousTurnP2.Clear();

        foreach (Transform slot in transform.parent)
        {
            int rwindex = slot.GetSiblingIndex();

            // Check if any adjacent slot has a card tagged as "Player2"
            if ((rwindex > 0 && transform.parent.GetChild(rwindex - 1).childCount > 0 && transform.parent.GetChild(rwindex - 1).GetChild(0).CompareTag("Player2")) ||
                (rwindex > 12 && transform.parent.GetChild(rwindex - 13).childCount > 0 && transform.parent.GetChild(rwindex - 13).GetChild(0).CompareTag("Player2")) ||
                (rwindex > 13 && transform.parent.GetChild(rwindex - 14).childCount > 0 && transform.parent.GetChild(rwindex - 14).GetChild(0).CompareTag("Player2")) ||
                (rwindex > 14 && transform.parent.GetChild(rwindex - 15).childCount > 0 && transform.parent.GetChild(rwindex - 15).GetChild(0).CompareTag("Player2")) ||
                (rwindex < transform.parent.childCount - 1 && transform.parent.GetChild(rwindex + 1).childCount > 0 && transform.parent.GetChild(rwindex + 1).GetChild(0).CompareTag("Player2")) ||
                (rwindex < transform.parent.childCount - 13 && transform.parent.GetChild(rwindex + 13).childCount > 0 && transform.parent.GetChild(rwindex + 13).GetChild(0).CompareTag("Player2")) ||
                (rwindex < transform.parent.childCount - 14 && transform.parent.GetChild(rwindex + 14).childCount > 0 && transform.parent.GetChild(rwindex + 14).GetChild(0).CompareTag("Player2")) ||
                (rwindex < transform.parent.childCount - 15 && transform.parent.GetChild(rwindex + 15).childCount > 0 && transform.parent.GetChild(rwindex + 15).GetChild(0).CompareTag("Player2")))
            {
                cardsPlacedInPreviousTurnP2.Add(slot);
            }
        }
    }

    public List<Transform> PreviouslyPlacedAvailableP2()
    {
        return cardsPlacedInPreviousTurnP2;
    }

    public void OnDrop(PointerEventData eventData)
    {
        DisplayCard card = eventData.pointerDrag.GetComponent<DisplayCard>();
        DisplayCard2 cardd = eventData.pointerDrag.GetComponent<DisplayCard2>();
        bool isP1Turn = ButtonTurn.GetPlayerTurn();

        if (card != null && transform.childCount == 0)
        {
            HandleCardDrop(card, isP1Turn);
        }
        else if (cardd != null && (cardd.gameObject.tag == "Player2" || !PhotonNetwork.IsMasterClient) && transform.childCount == 0)
        {
            HandleCardDropP2(cardd, isP1Turn);
        }
        else
        {
            Debug.Log("Card:"+card.gameObject.name);
            Debug.Log("Bslot ChildCount:"+transform.childCount);
            Debug.LogError("Invalid card detected.");
        }
    }

    private void HandleCardDrop(DisplayCard card, bool isP1Turn)
    {
        int cardEnergy = card.GetCardEnergy();
        if (int.TryParse(energyText.text, out currentEnergy) && currentEnergy >= cardEnergy && isP1Turn)
        {
            if (card.transform.parent.name == "Hand" && gmmm.currentPhase == GamePhase.Play && PreviouslyPlacedAvailable().Contains(transform))
            {
                UpdateEnergy(ref currentEnergy, cardEnergy, true);
                PlaceCard(card);
            }
            else if (card.transform.parent.tag == "BSlot" && gmmm.currentPhase == GamePhase.Move && card.canMove && card.AdjacentBSlotsAvailable().Contains(transform))
            {
                card.canMove = false;
                StartCoroutine(card.CanMoveNow(20f));
                UpdateEnergy(ref currentEnergy, 1, true);
                PlaceCard(card);
            }
            else
            {
                gmmm.ErrorSound();
            }
        }
        else
        {
            gmmm.ErrorSound();
        }
    }

    private void HandleCardDropP2(DisplayCard2 cardd, bool isP1Turn)
    {
        int carddEnergy = cardd.GetCardEnergy();
        if (int.TryParse(energyTextP2.text, out currentEnergyP2) && currentEnergyP2 >= carddEnergy && !isP1Turn)
        {
            if (cardd.transform.parent.name == "Hand2" && gmmm.currentPhase == GamePhase.Play && PreviouslyPlacedAvailableP2().Contains(transform))
            {
                UpdateEnergy(ref currentEnergyP2, carddEnergy, false);
                PlaceCardP2(cardd);
            }
            else if (cardd.transform.parent.tag == "BSlot" && gmmm.currentPhase == GamePhase.Move && cardd.canMove && cardd.AdjacentBSlotsAvailable().Contains(transform))
            {
                cardd.canMove = false;
                StartCoroutine(cardd.CanMoveNow(20f));
                UpdateEnergy(ref currentEnergyP2, 1, false);
                PlaceCardP2(cardd);
            }
            else
            {
                gmmm.ErrorSound();
            }
        }
        else
        {
            gmmm.ErrorSound();
        }
    }

    private void UpdateEnergy(ref int currentEnergy, int energyCost, bool isP1)
    {
        currentEnergy -= energyCost;
        if (isP1)
        {
            energyText.text = currentEnergy.ToString();
            healthBar.SetHealth(currentEnergy);
        }
        else
        {
            energyTextP2.text = currentEnergy.ToString();
            healthBar.SetHealth2(currentEnergy);
        }
        UpdateCoinImages(currentEnergy, isP1);
    }

    private void UpdateCoinImages(int currentEnergy, bool isP1)
    {
        GameObject[] coinImages = isP1 ? new GameObject[] { coinP1img, coinP1img2, coinP1img3, coinP1img4, coinP1img5, coinP1img6, coinP1img7, coinP1img8 } :
                                         new GameObject[] { coinP2img, coinP2img2, coinP2img3, coinP2img4, coinP2img5, coinP2img6, coinP2img7, coinP2img8 };

        for (int i = 0; i < coinImages.Length; i++)
        {
            coinImages[i].SetActive(currentEnergy > i);
        }
    }

    private void PlaceCard(DisplayCard card)
    {
        card.transform.SetParent(transform);
        card.transform.localPosition = Vector3.zero;
        card.GetComponent<CanvasGroup>().blocksRaycasts = true;
        card.photonView.RPC("DisableCardBack2RPC", RpcTarget.All);
        GetPlacementSound();

        // Sync the card placement with other clients
        int boardSlotViewID = photonView.ViewID;
        card.photonView.RPC("SyncCardPlacement", RpcTarget.Others, boardSlotViewID);
    }

    private void PlaceCardP2(DisplayCard2 cardd)
    {
        cardd.transform.SetParent(transform);
        cardd.transform.localPosition = Vector3.zero;
        cardd.GetComponent<CanvasGroup>().blocksRaycasts = true;
        Scene cs = SceneManager.GetActiveScene();
        if (cs.name == "AI")
        {
            Image carddBackImage = cardd.transform.Find("Back").GetComponent<Image>();
            carddBackImage.enabled = false;
        }
        cardd.photonView.RPC("DisableCardBackRPC", RpcTarget.All);
        GetPlacementSound();

        // Sync the card placement with other clients
        int boardSlotViewID = photonView.ViewID;
        cardd.photonView.RPC("SyncCardPlacement2", RpcTarget.Others, boardSlotViewID);
    }

    public void SpendOnAttack() 
    {
        int val = currentEnergy;
   
        if (val == 7) 
        {
            val -= 2;
            energyText.text = val.ToString();
            coinP1img8.SetActive(false);
            coinP1img7.SetActive(false);
            coinP1img6.SetActive(false);
        }
        if (val == 6)
        {
            val -= 2;
            energyText.text = val.ToString();
            coinP1img8.SetActive(false);
            coinP1img7.SetActive(false);
            coinP1img6.SetActive(false);
            coinP1img5.SetActive(false);
        }
        if (val == 5)
        {
            val -= 2;
            energyText.text = val.ToString();
            coinP1img8.SetActive(false);
            coinP1img7.SetActive(false);
            coinP1img6.SetActive(false);
            coinP1img5.SetActive(false);
            coinP1img4.SetActive(false);
        }
        if (val == 4)
        {
            val -= 2;
            energyText.text = val.ToString();
            coinP1img8.SetActive(false);
            coinP1img7.SetActive(false);
            coinP1img6.SetActive(false);
            coinP1img5.SetActive(false);
            coinP1img4.SetActive(false);
            coinP1img3.SetActive(false);
        }
        if (val == 3)
        {
            val -= 2;
            energyText.text = val.ToString();
            coinP1img8.SetActive(false);
            coinP1img7.SetActive(false);
            coinP1img6.SetActive(false);
            coinP1img5.SetActive(false);
            coinP1img4.SetActive(false);
            coinP1img3.SetActive(false);
            coinP1img2.SetActive(false);
        }
        if (val == 2)
        {
            val -= 2;
            energyText.text = val.ToString();
            coinP1img8.SetActive(false);
            coinP1img7.SetActive(false);
            coinP1img6.SetActive(false);
            coinP1img5.SetActive(false);
            coinP1img4.SetActive(false);
            coinP1img3.SetActive(false);
            coinP1img2.SetActive(false);
            coinP1img.SetActive(false);
        }

    }

    public void SpendOnAttack2()
    {
        int val = currentEnergyP2;
        if (val >= 2)
        {
            if (val ==8) 
            {
                val -= 2;
                energyTextP2.text = val.ToString();
                coinP2img8.SetActive(false);
                coinP2img7.SetActive(false);
            }

            if (val == 7)
            {
                val -= 2;
                energyTextP2.text = val.ToString();
                coinP2img8.SetActive(false);
                coinP2img7.SetActive(false);
                coinP2img6.SetActive(false);
            }
            if (val == 6)
            {
                val -= 2;
                energyTextP2.text = val.ToString();
                coinP2img8.SetActive(false);
                coinP2img7.SetActive(false);
                coinP2img6.SetActive(false);
                coinP2img5.SetActive(false);
            }
            if (val == 5)
            {
                val -= 2;
                energyTextP2.text = val.ToString();
                coinP2img8.SetActive(false);
                coinP2img7.SetActive(false);
                coinP2img6.SetActive(false);
                coinP2img5.SetActive(false);
                coinP2img4.SetActive(false);
            }
            if (val == 4)
            {
                val -= 2;
                energyTextP2.text = val.ToString();
                coinP2img8.SetActive(false);
                coinP2img7.SetActive(false);
                coinP2img6.SetActive(false);
                coinP2img5.SetActive(false);
                coinP2img4.SetActive(false);
                coinP2img3.SetActive(false);
            }
            if (val == 3)
            {
                val -= 2;
                energyTextP2.text = val.ToString();
                coinP2img8.SetActive(false);
                coinP2img7.SetActive(false);
                coinP2img6.SetActive(false);
                coinP2img5.SetActive(false);
                coinP2img4.SetActive(false);
                coinP2img3.SetActive(false);
                coinP2img2.SetActive(false);
            }
            if (val == 2)
            {
                val -= 2;
                energyTextP2.text = val.ToString();
                coinP2img8.SetActive(false);
                coinP2img7.SetActive(false);
                coinP2img6.SetActive(false);
                coinP2img5.SetActive(false);
                coinP2img4.SetActive(false);
                coinP2img3.SetActive(false);
                coinP2img2.SetActive(false);
                coinP2img.SetActive(false);
            }
        }

    }


    public void AnotherMethod()  // (1):CARD DRAW PHASE
    {
      //  UpdateMoveListP2();

        UpdatePreviousCardsList();
        Debug.Log("Previously Placed Cards P1:" + PreviouslyPlacedAvailable().Count);

       // UpdateMovementPhaseList();

        int value = currentEnergy;
        // Debug.Log("CE: " + value);
        if (value >= 0)
        {
            // value += 8;
            value = 8;
            energyText.text = value.ToString();
            healthBar.SetHealth(value);

            coinP1img.SetActive(true);
            coinP1img2.SetActive(true);
            coinP1img3.SetActive(true);
            coinP1img4.SetActive(true);
            coinP1img5.SetActive(true);
            coinP1img6.SetActive(true);
            coinP1img7.SetActive(true);
            coinP1img8.SetActive(true);

        }
        if (value == -1)
        {
            value = 9;
            energyText.text = value.ToString();
            healthBar.SetHealth(value);

            coinP1img.SetActive(true);
            coinP1img2.SetActive(true);
            coinP1img3.SetActive(true);
            coinP1img4.SetActive(true);
            coinP1img5.SetActive(true);
            coinP1img6.SetActive(true);
            coinP1img7.SetActive(true);
            coinP1img8.SetActive(true);
        }

    }

    public void AnotherMethod2()  // (1):CARD DRAW PHASE
    {
        AdjacentBslotListP1();

        UpdatePreviousCardsListP2();
        AiPlay();
       // UpdateMoveListP2();
        int value = currentEnergyP2;
        // Debug.Log("CE: " + value);
        if (value >= 0)
        {
            // value += 8;
            value = 8;
            energyTextP2.text = value.ToString();
            healthBar.SetHealth2(value);

            coinP2img.SetActive(true);
            coinP2img2.SetActive(true);
            coinP2img3.SetActive(true);
            coinP2img4.SetActive(true);
            coinP2img5.SetActive(true);
            coinP2img6.SetActive(true);
            coinP2img7.SetActive(true);
            coinP2img8.SetActive(true);
        }
        if (value == -1)
        {
            value = 9;
            energyTextP2.text = value.ToString();
            healthBar.SetHealth2(value);

            coinP2img.SetActive(true);
            coinP2img2.SetActive(true);
            coinP2img3.SetActive(true);
            coinP2img4.SetActive(true);
            coinP2img5.SetActive(true);
            coinP2img6.SetActive(true);
            coinP2img7.SetActive(true);
            coinP2img8.SetActive(true);
        }
        SetCurrentEnergyP2(value);
    }

    public static int GetCurrentEnergy()
    {
        return currentEnergy;
    }

    public static void SetCurrentEnergy(int energy) 
    {
        currentEnergy = energy;
    }

    public static void SetCurrentEnergyP2(int energy)
    {
        currentEnergyP2 = energy;
    }

    public static int GetCurrentEnergyP2()
    {
        return currentEnergyP2;
    }

    public void GetPlacementSound()
    {
        // src.clip = cardPlacementClip;
        // src.Play();
        AudioManager.instance.PlaySFX("Place");
    }

   

}






















