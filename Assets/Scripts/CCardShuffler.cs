using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Pun.Demo.PunBasics;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class CCardShuffler : MonoBehaviour
{
    public List<DisplayCard2> displayCards; // List of all display cards
    public Button shuffleButton; // Reference to the shuffle button
    public Animator cardAnimator; // Reference to the Animator component on the object you want to animate


    public GameObject deck;
    public GameObject hand;

    public BoardSlot boardSlot;
    public DisplayCard2 dc;

    private Dictionary<int, int> usedDetailsCount = new Dictionary<int, int>(); // Track used detail counts

    public GameManager gm;

    public AudioSource src;
    public AudioClip swordClip;

    public Zoom zm;

    public GridLayoutGroup handGrid;
    GameObject selectedCard;

    List<Transform> availableSlots;
    Transform randomSlot;

    private void Start()
    {
         boardSlot = FindAnyObjectByType<BoardSlot>();
       // availableSlots = boardSlot.Available();
      //  Debug.Log("Available SLots:"+availableSlots.Count);
       // MoveSelectedCardToRandomSlot();

        //   Debug.Log("Available Slots:" + boardSlot.Available().Count);


        // Add a click listener to the shuffle button
            foreach (var card in displayCards)
            {
                onStartShuffle(card);
            }
        if (PhotonNetwork.IsMasterClient) 
        {
        }

        Scene currentScene = SceneManager.GetActiveScene();
        bool isP1Turn = ButtonTurn.GetPlayerTurn();
        if (currentScene.name == "AI" && !isP1Turn)
        {
            // Call the method if the current scene is "Ai"
            ShuffleCards();
        }
        else if(currentScene.name == "SampleScene" && !isP1Turn)
        {
          //  ShuffleCards();    YE WALA COMMENT KIYA HAI ABHI
            //shuffleButton.onClick.AddListener(ShuffleCards);
        }

    }

    /*  private void Update()
      {
          Scene currentScene = SceneManager.GetActiveScene();
          bool isP1Turn = ButtonTurn.GetPlayerTurn();
          if (currentScene.name == "AI" && !isP1Turn && gm.currentPhase == GamePhase.Draw)
          {
              // Call the method if the current scene is "Ai"
              ShuffleCards();
              //gm.ChangePhase(GamePhase.Setup);
              StartCoroutine(ChangingAIPhase(3.0f));
              StopCoroutine(ChangingAIPhase(3.001f));

              //StartCoroutine(MoveSelectedCardToRandomSlot(3.2f));
              //StopCoroutine(MoveSelectedCardToRandomSlot(3.201f));
          }
      }*/

    private void onStartShuffle(DisplayCard2 c)
    {
        int randomDetail;
        int detailCount;

        // Generate a random detail until it doesn't exceed the limit of 2
        do
        {
            randomDetail = Random.Range(0, 40);
            usedDetailsCount.TryGetValue(randomDetail, out detailCount);
        } while (detailCount >= 2);

        // Update the used detail count
        usedDetailsCount[randomDetail] = detailCount + 1;

        // Set the random detail ID and update the card information
        if (c.transform.parent.name == "Deck" || c.transform.parent.name == "Deck2")
        {
            c.displayId = randomDetail;
            c.UpdateCardInformation();
        }


    }

    public void ShuffleCards()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (gm.currentPhase == GamePhase.Draw) 
        {
        if (shuffleButton.gameObject.name == "Shuffle")
        {
            boardSlot.AnotherMethod();
        }

        if (shuffleButton.gameObject.name == "Shuffle2" && currentScene.name == "SampleScene")
        {
            boardSlot.AnotherMethod2();
        }
        }


        StartCoroutine(CardsDelay(2.1f));
        TriggerShuffleAnimation(hand.transform.childCount);

        // Reset the used detail count dictionary
        usedDetailsCount.Clear();

        foreach (var card in displayCards)
        {
            ShuffleCard(card);
        }
    }


    private void ShuffleCard(DisplayCard2 card)
    {
        // Initialize variables to track the random detail and its count
        int randomDetail;
        int detailCount;

        // Generate a random detail until it doesn't exceed the limit of 2
        do
        {
            randomDetail = Random.Range(0, 40);
            usedDetailsCount.TryGetValue(randomDetail, out detailCount);
        } while (detailCount >= 2);

        // Update the used detail count
        usedDetailsCount[randomDetail] = detailCount + 1;

        // Set the random detail ID and update the card information
        if (card.transform.parent.name == "Deck" || card.transform.parent.name == "Deck2")
        {
            card.displayId = randomDetail;
            card.UpdateCardInformation();

            // If the current client is the Master, send the displayId to the Client
            if (PhotonNetwork.IsMasterClient)
            {
                object[] content = new object[] { card.displayId, card.GetComponent<PhotonView>().ViewID };
                RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(5, content, options, SendOptions.SendReliable);

                card.UpdateClientInfo(card.displayId);
            }
           // gm.ChangePhase(GamePhase.Play);
        }


    }

    private void TriggerShuffleAnimation(int handchildCount)
    {
        if (gm.currentPhase == GamePhase.Draw)
        {
            string triggerName = "ShuffleTrigger" + (1 + (8 - handchildCount)).ToString();
            cardAnimator.SetTrigger(triggerName);
            Debug.Log(triggerName);
            zm.DeckSound();
            StartCoroutine(BackToDefault(2.0f + (handchildCount == 0 ? 1.0f : 0f)));
        }
    }

    IEnumerator BackToDefault(float delay)
    {
        yield return new WaitForSeconds(delay);
        cardAnimator.SetTrigger("BackTrigger");
    }

    public void AttackSound()
    {
        //src.clip = swordClip;
        //src.Play();
        AudioManager.instance.PlaySFX("Sword");
    }

    private IEnumerator CardsDelay(float delay)
    {
        //int childCount = deck.transform.childCount;
        //int handchildCount = hand.transform.childCount;
        yield return new WaitForSeconds(delay);

        if (deck.transform.childCount > 0 && gm.currentPhase == GamePhase.Draw)
        {
            int cardsToMove = 8 - hand.transform.childCount;
            for (int i = 0; i < cardsToMove; i++)
            {
                Transform cardToMove = deck.transform.GetChild(deck.transform.childCount - 1);
                cardToMove.SetParent(hand.transform);
            }
            gm.ChangePhase(GamePhase.Play);
        }
    }
}
