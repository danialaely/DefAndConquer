
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace
using Unity.VisualScripting;

public class UIController : MonoBehaviour
{
    public Slider musicSlider;        // Reference to your Slider
    public TMP_Text musicText;        // Reference to your TMP_Text to display percentage

    public Slider sfxSlider;
    public TMP_Text sfxText;

    public static UIController instance;
    void OnEnable()
    {
        if (AudioManager.instance == null)
        {
            Debug.LogError("AudioManager instance is missing.");
        }
        else
        {
            musicSlider.value = AudioManager.instance.GetMusicVolume();
            UpdateMusicText(musicSlider.value);

            sfxSlider.value = AudioManager.instance.GetSFXVolume();
            UpdateSfxText(sfxSlider.value);
        }
    }

    void Start()
    {
        //musicSlider = GameObject.FindGameObjectWithTag("MSlider").GetComponent<Slider>();

        // Initialize the slider and text

        musicSlider.minValue = 0;
        musicSlider.maxValue = 1;
        musicSlider.value = AudioManager.instance.GetMusicVolume(); // Assuming you have a method to get the current volume
        UpdateMusicText(musicSlider.value);

        // Add listener for when the slider value changes
        musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);

        sfxSlider.minValue = 0;
        sfxSlider.maxValue = 1;
        sfxSlider.value = AudioManager.instance.GetSFXVolume();
        UpdateSfxText(sfxSlider.value);

        sfxSlider.onValueChanged.AddListener(OnSfxSliderValueChanged);
    }

    public void ToggleMusic()
    {
        AudioManager.instance.ToggleMusic();
    }

    public void MusicVolume()
    {
        // Adjust the volume in the AudioManager based on the slider value
        AudioManager.instance.MusicVolume(musicSlider.value);

        // Update the text to show the percentage
        UpdateMusicText(musicSlider.value);
    }

    public void SFXVolume()
    {
        AudioManager.instance.SFXVolume(sfxSlider.value);

        UpdateSfxText(sfxSlider.value);
    }

    // Listener for slider value changes
    private void OnMusicSliderValueChanged(float value)
    {
        MusicVolume(); // Call your method to update the AudioManager and the text
    }

    private void OnSfxSliderValueChanged(float val)
    {
        SFXVolume();
    }

    // Updates the music volume text to display the percentage
    private void UpdateMusicText(float value)
    {
        int percentage = Mathf.RoundToInt(value * 100); // Convert to a percentage
        musicText.text = percentage.ToString() + "%";   // Update the TMP_Text component
    }

    private void UpdateSfxText(float val)
    {
        int percent = Mathf.RoundToInt(val * 100);
        sfxText.text = percent.ToString() + "%";
    }
}