using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

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

  
    private void OnLoginAndroidSuccess(LoginResult result)
    {
      //  FetchHighScoreFromPlayFab();
        Debug.Log("Successfully Loggedin with Android ID");
        LoadingPanel.SetActive(false);
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
}