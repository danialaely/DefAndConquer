using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuToggleManager : MonoBehaviour
{
    public GameObject ToggleGroup;
    public Toggle EasyToggle;
    public Toggle MediumToggle;
    public Toggle HardToggle;

    public TMP_InputField userNameText;
    public static string playerName;

    public GameObject PlayerNamePanel;
    public GameObject WelcomePanel;

    public GameObject EasyBlurr;
    public GameObject MediumBlurr;
    public GameObject HardBlurr;

    void Start()
    {
        EasyToggle.onValueChanged.AddListener(delegate { ToggleValueChanged(); });
        MediumToggle.onValueChanged.AddListener(delegate { ToggleValueChanged(); });
        HardToggle.onValueChanged.AddListener(delegate { ToggleValueChanged(); });

        if (AudioManager.instance.continuedFromGame)
        {
            Time.timeScale = 1.0f;
            ActivateMyPanel(WelcomePanel.name);
            AudioManager.instance.PlayMusic("MenuAudio");
            AudioManager.instance.PlayMenuVideo();
        }
        else 
        {
            ActivateMyPanel(PlayerNamePanel.name);
            AudioManager.instance.PlayMusic("SignUpAudio");
            AudioManager.instance.PlaySignUpVideo();
        }
    }

    public void ActiveToggle()
    {
        if (EasyToggle.isOn)
        {
            Debug.Log("Easy");
            ToggleStateManager.EasyToggleOn = true;
            ToggleStateManager.MediumToggleOn = false;
            ToggleStateManager.HardToggleOn = false;

            EasyBlurr.SetActive(false);
            MediumBlurr.SetActive(true);
            HardBlurr.SetActive(true);
        }
        else if (MediumToggle.isOn)
        {
            Debug.Log("Medium");
            ToggleStateManager.EasyToggleOn = false;
            ToggleStateManager.MediumToggleOn = true;
            ToggleStateManager.HardToggleOn = false;

            EasyBlurr.SetActive(true);
            MediumBlurr.SetActive(false);
            HardBlurr.SetActive(true);
        }
        else if (HardToggle.isOn)
        {
            Debug.Log("Hard");
            ToggleStateManager.EasyToggleOn = false;
            ToggleStateManager.MediumToggleOn = false;
            ToggleStateManager.HardToggleOn = true;

            EasyBlurr.SetActive(true);
            MediumBlurr.SetActive(true);
            HardBlurr.SetActive(false);
        }
    }

    private void ToggleValueChanged()
    {
        ActiveToggle();
    }

    public void OnLoginClick()
    {
        string name = userNameText.text;
        if (!string.IsNullOrEmpty(name))
        {
            //Debug.Log(name);
            playerName = name; // Store it in static variable
            ActivateMyPanel(WelcomePanel.name);
            AudioManager.instance.PlayMusic("MenuAudio");
            AudioManager.instance.PlayMenuVideo();
        }
        else 
        {
            Debug.Log("Name field is empty");
        }
    }

    public void ActivateMyPanel(string panelName) 
    {
        PlayerNamePanel.SetActive(panelName.Equals(PlayerNamePanel.name));
        WelcomePanel.SetActive(panelName.Equals(WelcomePanel.name));
    }

    public string GetUserName() 
    {
        return playerName; // Use the static variable to persist the name
    }

}
