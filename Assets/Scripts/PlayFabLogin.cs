using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;
//using UnityEngine.UIElements;

public class PlayFabLogin : MonoBehaviour
{

    public GameObject UsernamePanel;
    public GameObject WelcomePanel;
    public GameObject LoadingPanel;
    public TMP_InputField usernameInputField; // Input field for player nickname
    private string defaultUsername = "Playing first time";
    public SceneM sm;
    public TMP_Text UsernameTxt;
    public MenuToggleManager menuToggleManager;

    private int myXP = 0;
    private string myRank = "Bronze";
    [SerializeField] private TMP_Text rankText;  // Drag your UI text here in Inspector
    [SerializeField] private TMP_Text XPText;

    [Header("XP Slider UI")]
    [SerializeField] private Slider xpSlider;

    [SerializeField] private GameObject playerStatePrefab; // Prefab for PlayerStateObject
    [SerializeField] private Transform contentParent;      // Parent object (Content in the ScrollView)

    public void Start()
    {
        //Note: Setting title Id here can be skipped if you have set the value in Editor Extensions already.
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "D8446"; // Please change this value to your own titleId from PlayFab Game Manager
        }
        
        LoadingPanel.SetActive(true);
#if UNITY_ANDROID
        var requestAndroid = new LoginWithAndroidDeviceIDRequest { AndroidDeviceId = ReturnMobileID(), CreateAccount = true };
        PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid, OnLoginAndroidSuccess, OnLoginAndroidFailure);

#elif UNITY_WEBGL
        // Retrieve High Score
         highscore = PlayerPrefs.GetInt("HighScore", 0);
        Debug.Log("High Score: " + highscore);

        OnLoginWebGLSuccess(highscore);
#endif
    }

    private string GetRankFromXP(int xp)
    {
        if (xp <= 100) return "Bronze";
        if (xp <= 200) return "Silver";
        if (xp <= 300) return "Gold";
        if (xp <= 400) return "Platinum";
        if (xp <= 500) return "Diamond";
        if (xp <= 600) return "Master";
        return "Grandmaster";
    }

    private void OnLoginAndroidSuccess(LoginResult result)
    {
      //  FetchHighScoreFromPlayFab();
        Debug.Log("Successfully Loggedin with Android ID");
        Debug.Log("✅ Logged into PlayFab. PlayFabId: " + result.PlayFabId);

        // 👇 Keep nickname as user input (e.g., "David", "Mark")
        // PhotonNetwork.NickName = playerName;  // (already done elsewhere)

        // 👇 Store PlayFabId in Photon Custom Properties
        ExitGames.Client.Photon.Hashtable playerData = new ExitGames.Client.Photon.Hashtable();
        playerData["PlayFabId"] = result.PlayFabId;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerData);

        Debug.Log("PlayFabId stored in Photon custom properties.");
        LoadingPanel.SetActive(false);
        FetchXPAndRankFromPlayFab();
        GetPuzzleLeaderboard();

        // Now safe to load profile picture
        FindObjectOfType<ProfilePictureManager>()?.LoadProfilePictureFromPlayFab();
        //  GetPlayerDisplayName();
        //  GetLeaderboard();
        //LoadingPanel.SetActive(false);
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest(), profileResult =>
        {
            string currentUsername = profileResult.PlayerProfile?.DisplayName;

            if (string.IsNullOrEmpty(currentUsername) || currentUsername == defaultUsername)
            {
                // No username set -> Show UsernamePanel
                //UsernamePanel.SetActive(true);
                ActivateMyPanel(UsernamePanel.name);
            }
            else
            {
                // Username exists -> Hide UsernamePanel
                //UsernamePanel.SetActive(false);
                ActivateMyPanel(WelcomePanel.name);
                PlayerPrefs.SetString("Username", currentUsername);
                UpdateUsernameUI(currentUsername);
                menuToggleManager.SetUserName(currentUsername);

                // ✅ Add this line:
                PhotonNetwork.NickName = currentUsername;
                Debug.Log("Photon NickName synced to: " + PhotonNetwork.NickName);
            }
        }, error => Debug.LogError(error.GenerateErrorReport()));

        
    }

    private void OnLoginAndroidFailure(PlayFabError error)
    {
        ActivateMyPanel(WelcomePanel.name);
        Debug.LogError(error.GenerateErrorReport());
    }

    public static string ReturnMobileID()
    {
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        return deviceID;
    }

    private void FetchXPAndRankFromPlayFab()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), res =>
        {
            if (res.Data != null && res.Data.ContainsKey("xp"))
                int.TryParse(res.Data["xp"].Value, out myXP);
            else
                myXP = 0;

            myRank = GetRankFromXP(myXP);

            // ✔ Store for Photon to use later
            PlayerPrefs.SetInt("PlayerXP", myXP);
            PlayerPrefs.SetString("PlayerRank", myRank);
            
            UpdateUI();
        },
        err =>
        {
            myXP = 0;
            myRank = "Bronze";
            PlayerPrefs.SetInt("PlayerXP", 0);
            PlayerPrefs.SetString("PlayerRank", "Bronze");
            
            UpdateUI();
        });
    }

    public void UpdateUI() 
    {
        // ✔ Update UI
        rankText.text = myRank;
        // Show XP progress out of 100
        int xpProgress = myXP % 100;
        XPText.text = $"XP: {xpProgress} / 100";

        xpSlider.value = xpProgress / 100f;
       // xpPercentText.text = $"{xpProgress}%";
    }

    public void SetUsername()
    {
        string newUsername = usernameInputField.text;
        if (!string.IsNullOrEmpty(newUsername))
        {
            var request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = newUsername
            };

            PlayFabClientAPI.UpdateUserTitleDisplayName(request, result =>
            {
                Debug.Log("Username set to: " + result.DisplayName);
                PlayerPrefs.SetString("Username", result.DisplayName);
                //UsernamePanel.SetActive(false);
                ActivateMyPanel(WelcomePanel.name);
            }, error => Debug.LogError(error.GenerateErrorReport()));
        }
    }

    public void ActivateMyPanel(string panelName)
    {
        UsernamePanel.SetActive(panelName.Equals(UsernamePanel.name));
        WelcomePanel.SetActive(panelName.Equals(WelcomePanel.name));
    }

    private void UpdateUsernameUI(string username)
    {
        if (UsernameTxt != null)
        {
            UsernameTxt.text = username;
        }
        Debug.Log("Username updated in UI: " + username);
    }

    public void GetPuzzleLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "XP", // <- Your stat
            StartPosition = 0,
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(request,
            result =>
            {
                Debug.Log("Puzzle leaderboard fetched.");
                var reversedList = result.Leaderboard.OrderBy(entry => entry.StatValue).ToList();
                DisplayLeaderboard(result.Leaderboard, contentParent);  // ← global
            },
            error =>
            {
                Debug.LogError("Failed to fetch Puzzle leaderboard: " + error.GenerateErrorReport());
            });
    }

    private void DisplayLeaderboard(List<PlayerLeaderboardEntry> leaderboard, Transform contentParent4)
    {
        Debug.Log("Rank\tName\t\tScore");

        // Clear old entries
        foreach (Transform child in contentParent4)
        {
            Destroy(child.gameObject);
        }

        int pos = 0;

        // Display new entries
        foreach (var entry in leaderboard)
        {
            pos++;
            GameObject playerRow = Instantiate(playerStatePrefab, contentParent4);

           // Image im = 
            TMP_Text playerNameText = playerRow.transform.GetChild(1).GetComponent<TMP_Text>();
            TMP_Text snoText = playerRow.transform.GetChild(2).GetComponent<TMP_Text>();
            TMP_Text scoreText = playerRow.transform.GetChild(3).GetComponent<TMP_Text>();

            //snoText.text = (entry.Position + 1).ToString();
            snoText.text = pos.ToString();
            playerNameText.text = string.IsNullOrEmpty(entry.DisplayName) ? "Anonymous" : entry.DisplayName;
            scoreText.text = entry.StatValue.ToString();

            Debug.Log($"{entry.Position + 1}\t{playerNameText.text}\t{entry.StatValue}");
        }
    }
}