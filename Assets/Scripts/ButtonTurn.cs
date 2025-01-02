using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Diagnostics.Tracing;

public class ButtonTurn : MonoBehaviourPunCallbacks, IOnEventCallback
{

    private static bool isPlayer1Turn = true;
    public TMP_Text turnText;
    private Coroutine turnCoroutine;

    public HealthBar turnBar;
    private int turnCount;
    private int turnCount2;
    // private Coroutine healthtimerCoroutine;

    public Camera mainCamera; // Reference to your main camera

    private float originalOrthographicSize;
    Vector3 originalCamPos;

    public Button deckP1;
    public Button deckP2;

    public GameManager gmm;
    public BoardSlot boardSlot;

    public GridLayoutGroup handGrid;
    GameObject selectedCard;

    List<Transform> availableSlots;
    Transform randomSlot;

    public GridLayoutGroup boardGrid;
    GameObject selectedCARD;

    List<Transform> availableSlotsMove;
    //  Transform randomSlotMove;

    public List<GameObject> CardPlacedToBoard = new List<GameObject>();

    public CCardShuffler CShufflerP2;
    public CardShuffler CShufflerP1;

    List<Transform> adjacentSlotsP1;
    Transform randomAdjSlotP1;

    public List<GameObject> AiAttackCards = new List<GameObject>();
    GameObject randomAiAtcCard;

    GameObject defenseCard;
    public Dice dice;

    public List<DisplayCard> allDisplayCards; // Reference to all DisplayCard instances
    public List<DisplayCard2> allDisplayCardsP2; // Reference to all DisplayCard instances
                                                 //  Scene currentScene;

    public GameToggleManager gameToggleManager;

    public string Movtag = "Player2";
    GameObject[] player2;

    public List<GameObject> adjacentp2List;
    Transform[] BSlots;

    public List<Transform> adjacentBSlots;
    Transform BoardSlt;

    // public GameObject phaseButton;
    public GameObject turnButton; //Line 827   if (PhotonNetwork.isMasterClient) --> turnButton.enabled = false;
    public GameObject phaseButton;

    private const byte TurnChangeEventCode = 1;
    public const byte ResetTimerEventCode = 2;
    private const byte ReadyPanelDeactivateEventCode = 6; // Use a unique event code

    public GameObject ReadyPanel;

    
    public Image ArenaBackground;
   
    public Sprite easyArenaImg;
    public Sprite mediumArenaImg;
    public Sprite hardArenaImg;

    public bool secondAttackCheck;

    IEnumerator changebgImag(float del) 
    {
        yield return new WaitForSeconds(del);
        Scene cs = SceneManager.GetActiveScene();

        if (cs.name == "AI")
        {
            if (gameToggleManager.HardToggle.isOn)
            {
                Debug.Log("Set Hard Image");
                ArenaBackground.sprite = hardArenaImg;
            }

            else if (gameToggleManager.EasyToggle.isOn)
            {
                Debug.Log("Set Easy Image");
                ArenaBackground.sprite = easyArenaImg;
            }

            else if (gameToggleManager.MediumToggle.isOn)
            {
                Debug.Log("Set Medium Image");
                ArenaBackground.sprite = mediumArenaImg;
            }

        }
    }

    private void Start()
    {
        boardSlot = FindAnyObjectByType<BoardSlot>();
        availableSlots = boardSlot.Available();
        //availableSlotsMove = boardSlot.MoveAvailable();
        adjacentSlotsP1 = boardSlot.AdjacentP1Available();
        boardSlot.AnotherMethod();
        allDisplayCards = new List<DisplayCard>(FindObjectsOfType<DisplayCard>());
        allDisplayCardsP2 = new List<DisplayCard2>(FindObjectsOfType<DisplayCard2>());

        Scene cs = SceneManager.GetActiveScene();
        StartCoroutine(changebgImag(0.01f));
        
        turnCoroutine = StartCoroutine(ChangeTurn(60.0f));
        
        TurnStarter(!isPlayer1Turn);
        secondAttackCheck = true;
        // currentScene = SceneManager.GetActiveScene();
        player2 = GameObject.FindGameObjectsWithTag(Movtag);
        GameObject[] gameObjectsWithTag = GameObject.FindGameObjectsWithTag("BSlot");
        BSlots = new Transform[gameObjectsWithTag.Length];
        for (int i = 0; i < gameObjectsWithTag.Length; i++)
        {
            BSlots[i] = gameObjectsWithTag[i].transform;
        }
        //BSlots =;

        if (mainCamera == null)
        {
            // If the mainCamera reference is not set, try to find the main camera in the scene
            mainCamera = Camera.main;
        }

        // Store the original orthographic size for resetting
        if (mainCamera != null)
        {
            originalOrthographicSize = mainCamera.orthographicSize;
            originalCamPos = mainCamera.transform.position;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            //  OnTurnButtonClick();
            ReadyPanel.SetActive(true);
            StartCoroutine(StartingtheGame(5.0f));
        }
        else if (!PhotonNetwork.IsMasterClient && cs.name == "SampleScene") 
        {
            ReadyPanel.transform.rotation = Quaternion.Euler(0,0,180);
        }

    }

    private void Update()
    {
        if (selectedCard != null)
            if (selectedCard.transform.parent.name == "DiscardPile")
            {
                CardPlacedToBoard.Remove(selectedCard);
            }
        //   Debug.Log("Adjacent cards P2:"+boardSlot.MoveAvailable().Count);
        Debug.Log("Adjacent cards P1:" + boardSlot.AdjacentP1Available().Count);
        Debug.Log("Player1Turn from getplayerturn:"+GetPlayerTurn());
        Debug.Log("Simple is player1Turn:"+isPlayer1Turn);
    }

    #region Ai 
    public IEnumerator PlaceToBoard(float delayed)
    {
        yield return new WaitForSeconds(delayed);

        // Select a random slot and card
        SelectedRandomSetupSlot();
        SelectRandomCard();

        // Place the card on the board
        selectedCard.transform.SetParent(randomSlot);
        selectedCard.transform.localPosition = Vector3.zero;

        // Disable the card back image
        selectedCard.transform.Find("Back").GetComponent<Image>().enabled = false;

        // Play placement sound
        boardSlot.GetPlacementSound();

        // Add card to the list of placed cards
        CardPlacedToBoard.Add(selectedCard);
        Debug.Log($"Card in new list: {CardPlacedToBoard.Count}");

        // Update player's energy
        int cardEnergy = selectedCard.GetComponent<DisplayCard2>().GetCardEnergy();
        int currentEnergy = BoardSlot.GetCurrentEnergyP2();
        int newEnergy = currentEnergy - cardEnergy;
        BoardSlot.SetCurrentEnergyP2(newEnergy);

        Debug.Log($"Card Energy: {cardEnergy}");
        Debug.Log($"Old CEnergy: {currentEnergy}");
        Debug.Log($"New CEnergy: {newEnergy}");

        // Update the energy display
        UpdateEnergyDisplay(newEnergy);
    }

    private void UpdateEnergyDisplay(int newEnergy)
    {
        // Access the coin images directly and disable them based on the new energy level
        GameObject[] coinImages = {
        boardSlot.coinP2img, boardSlot.coinP2img2, boardSlot.coinP2img3,
        boardSlot.coinP2img4, boardSlot.coinP2img5, boardSlot.coinP2img6,
        boardSlot.coinP2img7, boardSlot.coinP2img8
        };

        for (int i = 7; i >= newEnergy; i--)
        {
            coinImages[i].SetActive(false);
        }

        // If energy is 0 or negative, update the energy text
        if (newEnergy <= 0)
        {
            boardSlot.energyTextP2.text = "0";
        }
    }

    public IEnumerator MoveInBoard(float del)
    {
        yield return new WaitForSeconds(del);

        // Retrieve adjacent cards and a random move slot
        GetAdjacentBslotCards();
        GetRandomMoveBSlot();

        // Check if the selected card can move
        DisplayCard2 dpcrd2 = selectedCARD.GetComponent<DisplayCard2>();
        if (dpcrd2.canMove)
        {
            Debug.Log($"This is the card been selected to Move: {selectedCARD.name}");
            Debug.Log($"This is the Board Slot the card will move to: {BoardSlt.name}");

            // Move the card to the selected slot
            selectedCARD.transform.SetParent(BoardSlt);
            selectedCARD.transform.localPosition = Vector3.zero;
            dpcrd2.canMove = false;

            // Update player's energy
            int newEnergy = UpdateEnergy(BoardSlot.GetCurrentEnergyP2(), 1);

            // Update the energy display
            UpdateEnergyDisplay(newEnergy);
        }
    }

    private int UpdateEnergy(int currentEnergy, int cost)
    {
        int newEnergy = currentEnergy - cost;
        BoardSlot.SetCurrentEnergyP2(newEnergy);
        return newEnergy;
    }

    public void SelectedRandomSetupSlot() //PICKING SETUP SLOT
    {
        // Check if any slot has no child
        bool hasEmptySlot = false;
        foreach (var slot in availableSlots)
        {
            if (slot.childCount == 0)
            {
                hasEmptySlot = true;
                break;
            }
        }

        // If there are no slots without children, exit the function
        if (!hasEmptySlot)
        {
            Debug.Log("All slots have children. Exiting function.");
            return;
        }

        // List to store indices of available slots without children
        List<int> emptyIndices = new List<int>();    //CONTINUE NEXT WORK FROM HERE
        for (int i = 0; i < availableSlots.Count; i++)
        {
            if (availableSlots[i].childCount == 0)
            {
                emptyIndices.Add(i);
            }
        }

        // Select a random empty slot
        int randomIndex = emptyIndices[Random.Range(0, emptyIndices.Count)];
        randomSlot = availableSlots[randomIndex];

        Debug.Log("Random Slot from BSlot:" + randomSlot);
    }

    public void SelectRandomMoveSlot() //Movement of card on board MovePhase
    {
        /*   do 
           {
               randomSlotMove = availableSlotsMove[Random.Range(0, availableSlotsMove.Count)];    //APPLY SAME LOGIC FOR CARD SELECTION
           } while (randomSlotMove.childCount > 0);
           Debug.Log("Move Phase BoardSlot:"+randomSlotMove);
        */
    }



    public void SelectRandomCard()
    {
        int cardCount = handGrid.transform.childCount;
        bool attackingCard = false;
        //movableCards.OrderByDescending(card => card.GetComponent<DisplayCard2>().GetCardAttack() + GetComponent<DisplayCard2>().GetCardDefense()).First();

        if (cardCount > 0)
        {
            if (gameToggleManager.EasyToggle.isOn)
            {
                // Generate a random index within the range of cardCount
                int randomIndex = Random.Range(0, cardCount);  //hand.OrderByDescending(card => card.AttackPower + card.DefensePower).First();

                // Get the selected card object
                selectedCard = handGrid.transform.GetChild(randomIndex).gameObject;
                Debug.Log("Selected Card from Hand:" + selectedCard);
                // Now you have the selected card, you can perform actions on it
                // For example, you can disable the card, remove it from the hand, etc.
            }
            else if (gameToggleManager.MediumToggle.isOn)
            {
                Transform childWithMostEnergy = null;
                float maxEnergy = float.MinValue;

                foreach (Transform h in handGrid.transform)
                {
                    DisplayCard2 dp = h.GetComponent<DisplayCard2>();
                    if (dp != null)
                    {
                        if ((dp.GetCardAttack() + dp.GetCardHealth()) > maxEnergy)
                        {
                            maxEnergy = dp.GetCardAttack() + dp.GetCardHealth();
                            childWithMostEnergy = h;
                        }
                    }
                }
                if (childWithMostEnergy != null)
                {
                    selectedCard = childWithMostEnergy.gameObject;
                }
                // selectedCard = handGrid.transform.GetChild(0).gameObject;
            }
            else if (gameToggleManager.HardToggle.isOn)
            {
                Transform childWithMostAttacking = null;
                float maxAttack = float.MinValue;

                attackingCard = !attackingCard;
                if (attackingCard)
                {
                    foreach (Transform h in handGrid.transform)
                    {
                        DisplayCard2 dp = h.GetComponent<DisplayCard2>();
                        if (dp != null)
                        {
                            if (dp.GetCardAttack() > maxAttack)
                            {
                                maxAttack = dp.GetCardAttack();
                                childWithMostAttacking = h;
                            }
                        }
                    }
                    if (childWithMostAttacking != null)
                    {
                        selectedCard = childWithMostAttacking.gameObject;
                    }
                }
                else
                {
                    foreach (Transform h in handGrid.transform)
                    {
                        DisplayCard2 dp = h.GetComponent<DisplayCard2>();
                        if (dp != null)
                        {
                            if (dp.GetCardHealth() > maxAttack)
                            {
                                maxAttack = dp.GetCardHealth();
                                childWithMostAttacking = h;
                            }
                        }
                    }
                    if (childWithMostAttacking != null)
                    {
                        selectedCard = childWithMostAttacking.gameObject;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("No cards in the hand.");
        }
    }

    public void SelectCardToMove()  // SELECTED CARD(P2 CARD) FROM BOARD TO MOVE 
    {
        var movableCards = CardPlacedToBoard.Where(card => card.GetComponent<DisplayCard2>().canMove).ToList();
        //movableCards.OrderByDescending(card => card.GetComponent<DisplayCard2>().GetCardAttack() + GetComponent<DisplayCard2>().GetCardDefense()).First();
        if (movableCards.Count > 0)
        {
            selectedCARD = movableCards[Random.Range(0, movableCards.Count)]; //movableCards[0]
        }
        else
        {
            Debug.Log("CanMove is False");
        }
        /* do 
         {
             selectedCARD = CardPlacedToBoard[Random.Range(0, CardPlacedToBoard.Count)];   //WORKING HERE
         } while (selectedCARD.GetComponent<DisplayCard2>().canMove); */
    }

    public void GetAdjacentP2Cards()  //Adjacent cards of selected p2 Card. 
    {
        SelectCardToMove();
        adjacentp2List.Clear();

        foreach (GameObject p2 in player2)
        {
            float distance = Vector3.Distance(p2.transform.position, selectedCARD.transform.position);
            if (p2.gameObject.name == "SHCardP2")
            {
                if (distance < 280f && p2 != selectedCARD.gameObject)
                {
                    adjacentp2List.Add(p2);
                }
            }
            else
            {
                DisplayCard2 displayCard2 = p2.GetComponent<DisplayCard2>();
                if (distance < 280f && p2 != selectedCARD.gameObject && displayCard2.canMove)
                {
                    adjacentp2List.Add(p2);
                }
            }
        }
    }

    public void GetAdjacentBslotCards()
    {
        GetAdjacentP2Cards();
        adjacentBSlots.Clear();

        foreach (Transform bslot in BSlots)
        {
            foreach (GameObject p2 in adjacentp2List)
            {
                float dist = Vector3.Distance(p2.transform.position, bslot.transform.position);
                if (dist < 270f && bslot.childCount == 0)
                {
                    adjacentBSlots.Add(bslot);
                }
            }
        }
    }

    public void ResetCanMoveP2() //RESETTING CANMOVE TO TRUE
    {
        foreach (GameObject p2 in CardPlacedToBoard)
        {
            DisplayCard2 dpcard2 = p2.GetComponent<DisplayCard2>();
            if (dpcard2.canMove == false)
            {
                dpcard2.canMove = true;
            }
        }
    }

    public void GetRandomMoveBSlot()
    {
        //selectedCARD = CardPlacedToBoard[Random.Range(0, CardPlacedToBoard.Count)];
        adjacentBSlots.Sort((x, y) =>
        {
            int numX = ExtractNumber(x.name);
            int numY = ExtractNumber(y.name);
            return numX.CompareTo(numY);
        });

        if (gameToggleManager.EasyToggle.isOn || gameToggleManager.MediumToggle.isOn)
        {
            BoardSlt = adjacentBSlots[Random.Range(0, adjacentBSlots.Count)];
        }

        else if (gameToggleManager.HardToggle.isOn)
        {    //if selectedCARD.defense > selectedcardattack
            float cdefense = selectedCARD.GetComponent<DisplayCard2>().GetCardHealth();
            float cattack = selectedCARD.GetComponent<DisplayCard2>().GetCardAttack();
            Debug.Log("ATTACK:" + cattack + "\t" + "DEFENSE:" + cdefense);

            if (cattack > cdefense)
            {
                for (int i = adjacentBSlots.Count - 1; i >= 0; i--)
                {
                    if (adjacentBSlots[i].childCount == 0)
                    {
                        BoardSlt = adjacentBSlots[i];
                        break; // Exit the loop as soon as we find a slot without a child
                    }
                }
            }
            else if (cattack < cdefense)
            {
                for (int i = 0; i <= adjacentBSlots.Count; i++)
                {
                    if (adjacentBSlots[i].childCount == 0)
                    {
                        BoardSlt = adjacentBSlots[i];
                        break; // Exit the loop as soon as we find a slot without a child
                    }
                }
            }
            //else
        }

        /* foreach (Transform t in adjacentBSlots) 
         {
             Debug.Log("TILE NAME:"+t.name);
         }
         Debug.Log("TOTAL TILES:"+adjacentBSlots.Count); */
    }

    int ExtractNumber(string name)
    {
        // Use regular expression to extract the numeric part of the name
        Match match = Regex.Match(name, @"\d+");
        if (match.Success)
        {
            return int.Parse(match.Value);
        }
        return 0;
    }

    public void Attack()
    {
        //Same logic random slot, the adjacent would be "player2"
        //boardSlot.AdjacentP1Available().Clear();
        adjacentSlotsP1.Clear();
        AiAttackCards.Clear();
        adjacentSlotsP1.Clear();
        Debug.Log("Cleared Adj P1");
        boardSlot.AdjacentBslotListP1();
        boardSlot.UpdatePreviousCardsListP2();
        Debug.Log("Adjacent Slots P1:"+adjacentSlotsP1.Count);
        if (adjacentSlotsP1.Count > 0)
        {
            foreach (Transform slot in adjacentSlotsP1)
            {
                if (slot.childCount > 0)  // Added check for child count
                {
                    if (slot.GetChild(0).gameObject.CompareTag("Player2"))  // Changed to CompareTag for efficiency
                    {
                        // Make a new list and add them to that list   List name: AiAttackCards
                        AiAttackCards.Add(slot.GetChild(0).gameObject);
                        Debug.Log("Addedto AiAttackCards List");
                    }
                }
            }
        }
    }

    public void SelectAttackCard()
    {
        Attack();
        if (AiAttackCards.Count > 0)
        {
            GameObject cardWithMostAttacking = null;
            float maxEnergy = float.MinValue;

            if (gameToggleManager.EasyToggle.isOn)
            {
                do
                {
                    randomAiAtcCard = AiAttackCards[Random.Range(0, AiAttackCards.Count)];
                } while (randomAiAtcCard.name == "SHCardP2");
            }
            else if (gameToggleManager.HardToggle.isOn || gameToggleManager.MediumToggle.isOn)
            {
                foreach (GameObject atc in AiAttackCards)
                {
                    DisplayCard2 dp = atc.GetComponent<DisplayCard2>();
                    if (dp != null)
                    {
                        if (dp.GetCardAttack() > maxEnergy)
                        {
                            maxEnergy = dp.GetCardAttack();
                            cardWithMostAttacking = atc;
                        }
                    }
                }
                if (cardWithMostAttacking != null)
                {
                    randomAiAtcCard = cardWithMostAttacking.gameObject;
                }
            }

            Debug.Log("Picked card for attack:" + randomAiAtcCard);
            randomAiAtcCard.GetComponent<DisplayCard2>().OnPtClc();
            // Assuming adjCards is a list of adjacent cards of the player being attacked
            List<GameObject> adjCards = randomAiAtcCard.GetComponent<DisplayCard2>().adjCards;

            // Check if there are adjacent cards
            if (adjCards.Count > 0)
            {
                // Select a random adjacent card for defense
                if (gameToggleManager.EasyToggle.isOn)
                {
                    defenseCard = adjCards[Random.Range(0, adjCards.Count)];  //RATHER THAN PICKING RANDOM CARD FOR DEFENSE, CHOOSE WITH ONE LOWER DEFENSE
                }
                else if (gameToggleManager.HardToggle.isOn || gameToggleManager.MediumToggle.isOn)
                {
                    GameObject childWithLowestEnergy = null;
                    float minEnergy = float.MaxValue;

                    foreach (GameObject def in adjCards)
                    {
                        DisplayCard d = def.GetComponent<DisplayCard>();

                        if (d != null)
                        {
                            if (d.GetCardHealth() < minEnergy)
                            {
                                minEnergy = d.GetCardHealth();
                                childWithLowestEnergy = def;
                            }
                        }
                    }
                    if (childWithLowestEnergy != null)
                    {
                        defenseCard = childWithLowestEnergy.gameObject;
                    }
                }

                if (defenseCard.gameObject.name == "SHCardP1")
                {
                    defenseCard.GetComponent<ShP1Card>().OnPtcClick();
                }
                else 
                {
                    defenseCard.GetComponent<DisplayCard>().OnPtcClk();
                }
                StartCoroutine(RollingDice(2.0f));
                StartCoroutine(DeselectAtcCard(6.0f));
                if (gameToggleManager.EasyToggle.isOn || gameToggleManager.MediumToggle.isOn)
                {
                    StartCoroutine(TurnBtnActive(7.0f));
                    StartCoroutine(PhaseBtnActive(7.0f));
                }
                if (gameToggleManager.HardToggle.isOn) 
                {
                    secondAttackCheck = true;
                }
                
            }
            else
            {
                Debug.Log("No adjacent cards for defense.");
            }
            // randomAiAtcCard.GetComponent<DisplayCard2>().isSelected = true;
            //randomAiAtcCard.GetComponent<DisplayCard2>().outerBorder.color = Color.white;
        }
        else
        {
            Debug.Log("No Cards for Attack");
            StartCoroutine(TurnBtnActive(1.0f));
            StartCoroutine(PhaseBtnActive(1.0f));
            secondAttackCheck = false;
        }
    }

    public void SecondAttackCard()
    {
        Attack();
        if (AiAttackCards.Count > 0)
        {
            GameObject cardWithMostAttacking = null;
            float maxEnergy = float.MinValue;
            
            if (gameToggleManager.HardToggle.isOn)
            {
                foreach (GameObject atc in AiAttackCards)
                {
                    DisplayCard2 dp = atc.GetComponent<DisplayCard2>();
                    if (dp != null)
                    {
                        if (dp.GetCardAttack() > maxEnergy)
                        {
                            maxEnergy = dp.GetCardAttack();
                            cardWithMostAttacking = atc;
                        }
                    }
                }
                if (cardWithMostAttacking != null)
                {
                    randomAiAtcCard = cardWithMostAttacking.gameObject;
                }
            }

            Debug.Log("Picked card for attack:" + randomAiAtcCard);
            randomAiAtcCard.GetComponent<DisplayCard2>().OnPtClc();
            // Assuming adjCards is a list of adjacent cards of the player being attacked
            List<GameObject> adjCards = randomAiAtcCard.GetComponent<DisplayCard2>().adjCards;

            // Check if there are adjacent cards
            if (adjCards.Count > 0)
            {
                // Select a random adjacent card for defense
                if (gameToggleManager.EasyToggle.isOn)
                {
                    defenseCard = adjCards[Random.Range(0, adjCards.Count)];  //RATHER THAN PICKING RANDOM CARD FOR DEFENSE, CHOOSE WITH ONE LOWER DEFENSE
                }
                else if (gameToggleManager.HardToggle.isOn)
                {
                    GameObject childWithLowestEnergy = null;
                    float minEnergy = float.MaxValue;

                    foreach (GameObject def in adjCards)
                    {
                        DisplayCard d = def.GetComponent<DisplayCard>();

                        if (d != null)
                        {
                            if (d.GetCardHealth() < minEnergy)
                            {
                                minEnergy = d.GetCardHealth();
                                childWithLowestEnergy = def;
                            }
                        }
                    }
                    if (childWithLowestEnergy != null)
                    {
                        defenseCard = childWithLowestEnergy.gameObject;
                    }
                }

                if (defenseCard.gameObject.name == "SHCardP1")
                {
                    defenseCard.GetComponent<ShP1Card>().OnPtcClick();
                }
                else
                {
                    defenseCard.GetComponent<DisplayCard>().OnPtcClk();
                }
                StartCoroutine(RollingDice(2.0f));
                StartCoroutine(DeselectAtcCard(6.0f));
                StartCoroutine(TurnBtnActive(7.0f));
                StartCoroutine(PhaseBtnActive(7.0f));
                secondAttackCheck = true;
               
            }
            else
            {
                Debug.Log("No adjacent cards for defense.");
            }
            // randomAiAtcCard.GetComponent<DisplayCard2>().isSelected = true;
            //randomAiAtcCard.GetComponent<DisplayCard2>().outerBorder.color = Color.white;
        }
        else
        {
            Debug.Log("No Cards for Attack in 2nd Attack");
            StartCoroutine(TurnBtnActive(1.0f));
            StartCoroutine(PhaseBtnActive(1.0f));
            secondAttackCheck = true;
        }
    }

    IEnumerator RollingDice(float delay)
    {
        yield return new WaitForSeconds(delay);
        //dice.OnMouseDown();
        dice.AiRollDice();
    }

    IEnumerator DeselectAtcCard(float del)
    {
        yield return new WaitForSeconds(del);
        if (defenseCard.gameObject.name == "SHCardP1")
        {
            defenseCard.GetComponent<ShP1Card>().OnPtcClick();
        }
        else 
        {
            defenseCard.GetComponent<DisplayCard>().OnPtcClk();
        }
        randomAiAtcCard.GetComponent<DisplayCard2>().OnPtClc();
    }

    IEnumerator ChangeAIPhaseToMove(float delayed)
    {
        yield return new WaitForSeconds(delayed);
        gmm.ChangePhase(GamePhase.Move);
    }

    IEnumerator ChangeAIPhaseToAttack(float delayed)
    {
        yield return new WaitForSeconds(delayed);
        gmm.ChangePhase(GamePhase.Attack);
        //SelectAttackCard();
    }

    IEnumerator Attacking(float del)
    {
        yield return new WaitForSeconds(del);
        SelectAttackCard();
    }

    IEnumerator SecondAttacking(float delay) 
    {
        yield return new WaitForSeconds(delay);
        SecondAttackCard();
    }

    IEnumerator TurnBtnActive(float delay) 
    {
        yield return new WaitForSeconds(delay);
        turnButton.SetActive(true);
        StartCoroutine(ChangeTurnAfterAIAttack(1.0f));
    }

    IEnumerator ChangeTurnAfterAIAttack(float delay) 
    {
        yield return new WaitForSeconds(delay);
        OnTurnButtonClick();
    }

    IEnumerator PhaseBtnActive(float delay) 
    {
        yield return new WaitForSeconds(delay);
        phaseButton.SetActive(true);
    }

    IEnumerator CheckingConditionForSecondAttack(float del) 
    {
        yield return new WaitForSeconds(del);
        int val = BoardSlot.GetCurrentEnergyP2();

        if (secondAttackCheck == true && val > 2)
        {
            Debug.Log("Perform Second Attack.");
            StartCoroutine(SecondAttacking(3.0f));
        }
        else 
        {
            Debug.Log("Condition false hai bhai");
        }
    }

    IEnumerator ChangeAIPhaseToSetup(float delay)
    {
        yield return new WaitForSeconds(delay);
        gmm.ChangePhase(GamePhase.Play);
        //  SelectRandomCard();
        if (gameToggleManager.EasyToggle.isOn || gameToggleManager.MediumToggle.isOn)
        {
            StartCoroutine(PlaceToBoard(2.0f));
            StartCoroutine(ChangeAIPhaseToMove(3.0f));
            StartCoroutine(MoveInBoard(4.0f));
            //StartCoroutine(MoveInBoard(7.0f));
            StartCoroutine(ChangeAIPhaseToAttack(5.0f));
            StartCoroutine(Attacking(6.0f));
            //StartCoroutine(TurnBtnActive(11.0f));
        }
        else if (gameToggleManager.HardToggle.isOn)
        {
            StartCoroutine(PlaceToBoard(2.0f));
            StartCoroutine(PlaceToBoard(3.0f));
            StartCoroutine(ChangeAIPhaseToMove(4.0f));
            StartCoroutine(MoveInBoard(5.0f));
            //StartCoroutine(MoveInBoard(7.0f));
            StartCoroutine(ChangeAIPhaseToAttack(6.0f));
            StartCoroutine(Attacking(6.0f));
            StartCoroutine(CheckingConditionForSecondAttack(12.0f));
            
                    //StartCoroutine(TurnBtnActive(12.0f));
            //StartCoroutine(TurnBtnActive(21.0f));   ******************
            //StartCoroutine(PhaseBtnActive(21.0f));  ******************
          /*  int val = BoardSlot.GetCurrentEnergyP2();
            if (val > 2)
            {
                StartCoroutine(Attacking(15.0f));
            }
            else if (val < 2) 
            {
                if (gmm.currentPhase == GamePhase.Attack) 
                {
                    StartCoroutine(TurnBtnActive(12.0f));
                    StartCoroutine(PhaseBtnActive(12.0f));
                } 
            }*/
            //StartCoroutine(SelectAttackCard(7.0f));
        }
    }

    #endregion Ai

    IEnumerator StartingtheGame(float delay)
    {
        yield return new WaitForSeconds(delay);
        OnTurnButtonClick();
    }
    public void OnTurnButtonClick()
    {
        ResetTimer();
        Scene cs = SceneManager.GetActiveScene();
        if (cs.name == "SampleScene")
        {
            Debug.Log("Turn button clicked, raising event");
            ReadyPanel.SetActive(false);

            // Raise event to deactivate ReadyPanel on client
            RaiseEventOptions panelEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions panelSendOptions = new SendOptions { Reliability = true };

            // Raising the event with no content since we are only signaling the action
            PhotonNetwork.RaiseEvent(ReadyPanelDeactivateEventCode, null, panelEventOptions, panelSendOptions);

            // Toggle the turn
            isPlayer1Turn = !isPlayer1Turn;

            object[] content = new object[] { isPlayer1Turn }; // Array contains the data you want to send
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };

            PhotonNetwork.RaiseEvent(TurnChangeEventCode, content, raiseEventOptions, sendOptions);

            UpdateTurn(isPlayer1Turn);
        }
        if (cs.name == "AI")
        {
            if (isPlayer1Turn)
            {
                gmm.ChangePhase(GamePhase.Draw);
                turnText.text = "P2 Turn";  //Button 
                turnCount = 0;
                turnBar.SetTurnTime(turnCount);
                turnCount2 = 60;
                turnBar.SetTurnTime2(turnCount2);

                deckP1.enabled = false;
                deckP2.enabled = true;
                boardSlot.AnotherMethod2();

                Scene currentScene = SceneManager.GetActiveScene();
                if (gmm.currentPhase == GamePhase.Draw)
                {
                    CShufflerP2.ShuffleCards();
                    ResetCanMoveP2();
                    adjacentBSlots.Clear();
                    if (currentScene.name == "AI")
                    {
                        turnButton.SetActive(false);
                        phaseButton.SetActive(false);
                        StartCoroutine(ChangeAIPhaseToSetup(3.0f));
                    }
                }
                foreach (DisplayCard otherCard in allDisplayCards)
                {
                    if (otherCard.isSelected)
                    {
                        otherCard.OnPtcClk();
                    }
                }
            }
            else
            {
                gmm.ChangePhase(GamePhase.Draw);
                turnText.text = "P1 Turn";
                turnCount = 60;
                turnBar.SetTurnTime(turnCount);

                turnCount2 = 0;
                turnBar.SetTurnTime2(turnCount2);

                deckP1.enabled = true;
                deckP2.enabled = false;
                boardSlot.AnotherMethod();
                CShufflerP1.ShuffleCards();
                StopCoroutine(TurnBtnActive(0.0f));
                StopCoroutine(PhaseBtnActive(0.0f));
                Scene currentScene = SceneManager.GetActiveScene();

                if (currentScene.name == "SampleScene")
                {
                    foreach (DisplayCard2 otherCard in allDisplayCardsP2)
                    {
                        if (otherCard.isSelected)
                        {
                            otherCard.OnPtClc();
                        }
                    }
                }
            }
        isPlayer1Turn = !isPlayer1Turn;
        }

        // Toggle the turn
        Debug.Log("PLAYER 1 TURN:" + isPlayer1Turn);
    }

    private void UpdateTurn(bool newIsPlayer1Turn)
    {
        isPlayer1Turn = !isPlayer1Turn;
        isPlayer1Turn = newIsPlayer1Turn;

        if (isPlayer1Turn)
        {
            gmm.ChangePhase(GamePhase.Draw);
            turnText.text = "P1 Turn";
            turnCount = 60;
            turnBar.SetTurnTime(turnCount);

            turnCount2 = 0;
            turnBar.SetTurnTime2(turnCount2);

            deckP1.enabled = false;
            deckP2.enabled = true;

            CShufflerP1.ShuffleCards();
            if (PhotonNetwork.IsMasterClient)
            {
                turnButton.SetActive(true);
                phaseButton.SetActive(true);
            }
            else
            {
                turnButton.SetActive(false);
                phaseButton.SetActive(false);
            }

        }
        else
        {
            gmm.ChangePhase(GamePhase.Draw);
            turnText.text = "P2 Turn";
            turnCount2 = 60;
            turnBar.SetTurnTime2(turnCount2);

            turnCount = 0;
            turnBar.SetTurnTime(turnCount2);

            if (mainCamera != null)
            {
                mainCamera.orthographicSize = originalOrthographicSize;
                mainCamera.transform.position = originalCamPos;
            }

            deckP1.enabled = true;
            deckP2.enabled = false;
            CShufflerP2.ShuffleCards();

            if (PhotonNetwork.IsMasterClient)
            {
                turnButton.SetActive(false);
                phaseButton.SetActive(false);
            }
            else
            {
                turnButton.SetActive(true);
                phaseButton.SetActive(true);
            }
        }
    }

    public static bool GetPlayerTurn()
    {
       return isPlayer1Turn;
    }

    private IEnumerator ChangeTurn(float delay)
    {
        Scene currentS = SceneManager.GetActiveScene();
        while (true)
        {
            if (currentS.name == "AI")
            {
                if (isPlayer1Turn)
                {
                    gmm.ChangePhase(GamePhase.Draw);
                    turnText.text = "P1 Turn";
                    turnCount = 60;
                    turnBar.SetTurnTime(turnCount);

                    turnCount2 = 0;
                    turnBar.SetTurnTime2(turnCount2);

                    deckP1.enabled = true;
                    deckP2.enabled = false;
                    //  boardSlot.AnotherMethod();
                }
                else
                {
                    gmm.ChangePhase(GamePhase.Draw);
                    turnText.text = "P2 Turn";
                    turnCount2 = 60;
                    turnBar.SetTurnTime2(turnCount2);
                    // StartCoroutine(Turnbar2(1.0f));

                    turnCount = 0;
                    turnBar.SetTurnTime(turnCount2);

                    if (mainCamera != null)
                    {
                        mainCamera.orthographicSize = originalOrthographicSize;
                        mainCamera.transform.position = originalCamPos;
                    }

                    deckP1.enabled = false;
                    deckP2.enabled = true;
                    // boardSlot.AnotherMethod2();
                }
            }
            // Debug.Log("IS PLAYER TURN:"+isPlayer1Turn);

            yield return new WaitForSeconds(delay);
            // isPlayer1Turn = !isPlayer1Turn;
            OnTurnButtonClick();
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void ResetTimer()
    {
        if (turnCoroutine != null)
        {
            StopCoroutine(turnCoroutine);
        }

        // Send the reset timer event to the client
        object[] content = new object[] { 60.0f }; // Pass any additional data if necessary
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        PhotonNetwork.RaiseEvent(ResetTimerEventCode, content, raiseEventOptions, sendOptions);

        turnCoroutine = StartCoroutine(ChangeTurn(60.0f));
    }

    private void ResetClientTimer(float timerValue)
    {
        if (turnCoroutine != null)
        {
            StopCoroutine(turnCoroutine);
        }

        turnCoroutine = StartCoroutine(ChangeTurn(timerValue));
    }

    private IEnumerator Turnbar(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            if (turnCount >= 0)
            {
                turnCount--;
                // Debug.Log("TurnCount:" + turnCount);
                turnBar.SetTurnTime(turnCount);
            }
        }
    }

    private IEnumerator Turnbar2(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            if (turnCount2 >= 0)
            {
                turnCount2--;
                // Debug.Log("TurnCount2:" + turnCount2);

                turnBar.SetTurnTime2(turnCount2);
            }

        }
    }

    private void TurnStarter(bool isP1)
    {

        if (!isP1)
        {
            //   turnCount = 30;
            //  turnBar.SetTurnTime(turnCount);

            StartCoroutine(Turnbar(1.0f));
            StartCoroutine(Turnbar2(1.0f));
        }

    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == TurnChangeEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            bool newIsPlayer1Turn = (bool)data[0];

            // Update the local turn state
            UpdateTurn(newIsPlayer1Turn);
        }

        if (eventCode == ResetTimerEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            float newTimerValue = (float)data[0];

            // Reset the client's timer and restart the coroutine
            ResetClientTimer(newTimerValue);
        }

        if (eventCode == ReadyPanelDeactivateEventCode)
        {
            // Deactivate ReadyPanel on the client side
            ReadyPanel.SetActive(false);
        }
    }

}
