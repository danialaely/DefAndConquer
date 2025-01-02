using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public MSound[] musicsounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    public VideoPlayer videoPlayer; // Reference to the VideoPlayer component
    public VideoClip signUpVideo;   // Video clip for sign-up
    public VideoClip menuVideo;     // Video clip for menu
    public VideoClip settingsVideo; // Video clip for settings

    public bool continuedFromGame;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            continuedFromGame = false;
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //PlayMusic("Background");
    }

    public void PlayMusic(String name) 
    {
        MSound s = Array.Find(musicsounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound Not Found");
        }
        else 
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void StopMusic() 
    {
        if (musicSource != null) 
        {
            musicSource.Stop();
        }
    }

    public void PlaySFX(string name) 
    {
        MSound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sfx Not Found");
        }
        else
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }

    public void ToggleMusic() 
    {
        if (musicSource != null) 
        {
            musicSource.mute = !musicSource.mute;
        }
    }

    public void MusicVolume(float volume) 
    {
        musicSource.volume = volume;
    }
    public float GetMusicVolume()
    {
        return musicSource.volume; // Return the current volume of the AudioSource
    }

    public void SFXVolume(float volume) 
    {
        sfxSource.volume = volume;
    }
    public float GetSFXVolume()
    {
        return sfxSource.volume; // Return the current volume of the AudioSource
    }


    public void PlaySignUpVideo()
    {
        PlayVideo(signUpVideo);
    }

    public void PlayMenuVideo()
    {
        PlayVideo(menuVideo);
    }

    public void PlaySettingsVideo()
    {
        PlayVideo(settingsVideo);
    }

    // General method to play any video
    private void PlayVideo(VideoClip clip)
    {
        videoPlayer.clip = clip;  // Assign the video clip
        videoPlayer.Play();       // Play the video
    }

    public void StopVideo()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Stop();   // Stop the video
        }
    }

}
