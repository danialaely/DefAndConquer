using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System;
using System.Reflection;

public class CardShuffler : MonoBehaviourPunCallbacks
{
    public List<DisplayCard> displayCards; // List of all display cards
    public Button shuffleButton; // Reference to the shuffle button
    public Animator cardAnimator; // Reference to the Animator component on the object you want to animate

    public GameObject deck;
    public GameObject hand;

    public BoardSlot boardSlot;
    public DisplayCard dc;

    private Dictionary<int, int> usedDetailsCount = new Dictionary<int, int>(); // Track used detail counts
    public GameManager gm;

    public AudioSource src;
    public AudioClip swordClip;

    public Zoom zm;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var card in displayCards)
            {
                onStartShuffle(card);
            }
        }

        if (SceneManager.GetActiveScene().name == "AI")
        {
            ShuffleCards();
        }
    }

    private void onStartShuffle(DisplayCard c)
    {
        int randomDetail;
        int detailCount;

        // Generate a random detail until it doesn't exceed the limit of 2
        do
        {
            randomDetail = UnityEngine.Random.Range(0, 40);
            usedDetailsCount.TryGetValue(randomDetail, out detailCount);
        } while (detailCount >= 2);

        usedDetailsCount[randomDetail] = detailCount + 1;

        if (c.transform.parent.name == "Deck" || c.transform.parent.name == "Deck2")
        {
            c.displayId = randomDetail;
            c.UpdateCardInformation();
        }
    }

    public void ShuffleCards()
    {
        if (gm.currentPhase == GamePhase.Draw)
        {
            if (shuffleButton.gameObject.name == "Shuffle")
            {
                boardSlot.AnotherMethod();
            }
            else if (shuffleButton.gameObject.name == "Shuffle2")
            {
                boardSlot.AnotherMethod2();
            }

            StartCoroutine(CardsDelay(2.1f));
            TriggerShuffleAnimation(hand.transform.childCount);

            usedDetailsCount.Clear();
            foreach (var card in displayCards)
            {
                ShuffleCard(card);
            }
        }
    }

    private void ShuffleCard(DisplayCard card)
    {
        int randomDetail;
        int detailCount;

        // Generate a random detail until it doesn't exceed the limit of 2
        do
        {
            randomDetail = UnityEngine.Random.Range(0, 40);
            usedDetailsCount.TryGetValue(randomDetail, out detailCount);
        } while (detailCount >= 2);

        usedDetailsCount[randomDetail] = detailCount + 1;

        if (card.transform.parent.name == "Deck" || card.transform.parent.name == "Deck2")
        {
            card.displayId = randomDetail;
            card.UpdateCardInformation();

            if (PhotonNetwork.IsMasterClient)
            {
                object[] content = new object[] { card.displayId, card.GetComponent<PhotonView>().ViewID };
                RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(4, content, options, SendOptions.SendReliable);

                card.UpdateClientInfo(card.displayId);
            }
        }
    }

    private void TriggerShuffleAnimation(int handchildCount)
    {
        if (gm.currentPhase == GamePhase.Draw)
        {
            string triggerName = "ShuffleTrigger" + (1+(8 - handchildCount)).ToString();
            if (triggerName != "ShuffleTrigger1") 
            {
                cardAnimator.SetTrigger(triggerName);
                Debug.Log(triggerName);
                zm.DeckSound();
                StartCoroutine(BackToDefault(2.0f + (handchildCount == 0 ? 1.0f : 0f)));
            }
        }
    }

    private IEnumerator BackToDefault(float delay)
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

