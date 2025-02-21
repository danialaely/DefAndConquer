using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Google;
using System.Threading.Tasks;

public class GoogleAuthentication : MonoBehaviour
{
    private string webClientId = "819320267393-bpi5d2kr9vf8epjrt8qg5fjhkdf9t7mj.apps.googleusercontent.com";  // Get from Google Cloud Console

    private GoogleSignInConfiguration configuration;

    private void Start()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true
        };

        SignInWithGoogle();
    }

    public void SignInWithGoogle()
    {
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleSignIn);
    }

    private void OnGoogleSignIn(System.Threading.Tasks.Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted || task.IsCanceled)
        {
            Debug.LogError("Google Sign-In Failed: " + task.Exception?.Message);
            return;
        }

        string idToken = task.Result.IdToken;
        Debug.Log("Google Sign-In Success! Token: " + idToken);

        // Send the token to PlayFab
       // LoginToPlayFab(idToken);
    }

  /*  private void LoginToPlayFab(string idToken)
    {
        var request = new LoginWithGoogleAccountRequest
        {
            TitleId = PlayFabSettings.TitleId,
            ServerAuthCode = idToken,
            CreateAccount = true // Creates a new PlayFab account if it doesn't exist
        };

        PlayFabClientAPI.LoginWithGoogleAccount(request, OnPlayFabLoginSuccess, OnPlayFabLoginError);
    }

    private void OnPlayFabLoginSuccess(LoginResult result)
    {
        Debug.Log("PlayFab Login Successful! PlayFab ID: " + result.PlayFabId);
    }

    private void OnPlayFabLoginError(PlayFabError error)
    {
        Debug.LogError("PlayFab Login Failed: " + error.GenerateErrorReport());
    }*/
}
