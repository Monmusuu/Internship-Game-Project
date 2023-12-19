using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;       
using UnityEngine.Audio;   
using TMPro;

public class SettingMenu : MonoBehaviour
{
    private Resolution[] m_resolutions;
    public TMP_Dropdown m_resolutionsDropdown;
    public Toggle m_fullscreenToggle;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    const string Mixer_MUSIC = "MusicVolume";
    const string Mixer_SFX = "SFXVolume";
    const string Mixer_Master = "MasterVolume";

    [SerializeField] private AudioMixer mixer;

    public Button masterButton;
    public Button sfxButton;
    public Button musicButton;

    public Sprite nonMuteBasicAudio;
    public Sprite muteBasicAudio;
    public Sprite nonMuteUniqueAudio;
    public Sprite muteUniqueAudio;

    private void Awake() {
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }
    void Start(){
        
        ConfigureResolutions();
        SetFullscreen(Screen.fullScreen);

        if(PlayerPrefs.HasKey("Resolution")){
            SetResolution(PlayerPrefs.GetInt("Resolution"));
        }
        if(PlayerPrefs.HasKey("Fullscreem")){
            if(PlayerPrefs.GetInt("Fullscreem") == 1){
                SetFullscreen(true);
            }else {
                SetFullscreen(false);
             }
        }

            // Retrieve and set saved audio volumes
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            float masterVolume = PlayerPrefs.GetFloat("MasterVolume");
            masterSlider.value = masterVolume;
            SetMasterVolume(masterVolume);
        }

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume");
            musicSlider.value = musicVolume;
            SetMusicVolume(musicVolume);
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
            sfxSlider.value = sfxVolume;
            SetSFXVolume(sfxVolume);
        }
    }
    
    void ConfigureResolutions(){
        
        m_resolutions = Screen.resolutions;
        m_resolutionsDropdown.ClearOptions();
        List<string> resolutionOptions = new List<string>();
        int currentResolutionIndex = 0;
       
        for(int i = 0; i < m_resolutions.Length; i++){
            string option = m_resolutions[i].width + " x " + m_resolutions[i].height;
            resolutionOptions.Add(option);
           
            if(m_resolutions[i].width == Screen.currentResolution.width &&
               m_resolutions[i].height == Screen.currentResolution.height){
                currentResolutionIndex = i;
            }
        }
        m_resolutionsDropdown.AddOptions(resolutionOptions);
        m_resolutionsDropdown.value = currentResolutionIndex;
        m_resolutionsDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex){
        Resolution resolution = m_resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("Resolution", resolutionIndex);
    }

    public void SetFullscreen(bool isFullscreen){
        Screen.fullScreen = isFullscreen;
        if(isFullscreen){
            PlayerPrefs.SetInt("Fullscreen", 1);
        }else {
            PlayerPrefs.SetInt("Fullscreen",0);
        }

    }

    public void SetMasterVolume(float value){
        if (value <= 0.0001f)
        {
            masterButton.image.sprite = muteBasicAudio; // Change the image to the mute sprite
        }
        else
        {
            masterButton.image.sprite = nonMuteBasicAudio; // Change the image to the unmute sprite
        }
        mixer.SetFloat(Mixer_Master, Mathf.Log10(value)*20);
        PlayerPrefs.SetFloat("MasterVolume", value); // Save the master volume to PlayerPrefs
    }

    public void SetMusicVolume(float value){
        if (value <= 0.0001f)
        {
            musicButton.image.sprite = muteUniqueAudio; // Change the image to the mute sprite
        }
        else
        {
            musicButton.image.sprite = nonMuteUniqueAudio; // Change the image to the unmute sprite
        }
        mixer.SetFloat(Mixer_MUSIC, Mathf.Log10(value)*20);
        PlayerPrefs.SetFloat("MusicVolume", value); // Save the music volume to PlayerPrefs
    }

    public void SetSFXVolume(float value){
        if (value <= 0.0001f)
        {
            sfxButton.image.sprite = muteBasicAudio; // Change the image to the mute sprite
        }
        else
        {
            sfxButton.image.sprite = nonMuteBasicAudio; // Change the image to the unmute sprite
        }
        mixer.SetFloat(Mixer_SFX, Mathf.Log10(value)*20);
        PlayerPrefs.SetFloat("SFXVolume", value); // Save the SFX volume to PlayerPrefs
    }

    public void ToggleMuteMaster()
    {
        if (masterSlider.value > 0.0001f)
        {
            // Store the current master volume
            PlayerPrefs.SetFloat("PreviousMasterVolume", masterSlider.value);
            // Mute the master volume
            masterSlider.value = 0.0001f;
            masterButton.image.sprite = muteBasicAudio;
        }
        else
        {
            // Restore the previous master volume
            float previousVolume = PlayerPrefs.GetFloat("PreviousMasterVolume", 1.0f); // Default to 1.0 if not found
            masterSlider.value = previousVolume;
            masterButton.image.sprite = nonMuteBasicAudio;
        }
        SetMasterVolume(masterSlider.value); // Update the Audio Mixer settings
    }

    public void ToggleMuteMusic()
    {
        if (musicSlider.value > 0.0001f)
        {
            // Store the current music volume
            PlayerPrefs.SetFloat("PreviousMusicVolume", musicSlider.value);
            // Mute the music volume
            musicSlider.value = 0.0001f;
            musicButton.image.sprite = muteUniqueAudio;
        }
        else
        {
            // Restore the previous music volume
            float previousVolume = PlayerPrefs.GetFloat("PreviousMusicVolume", 1.0f); // Default to 1.0 if not found
            musicSlider.value = previousVolume;
            musicButton.image.sprite = nonMuteUniqueAudio;
        }
        SetMusicVolume(musicSlider.value); // Update the Audio Mixer settings
    }

    public void ToggleMuteSFX()
    {
        if (sfxSlider.value > 0.0001f)
        {
            // Store the current SFX volume
            PlayerPrefs.SetFloat("PreviousSFXVolume", sfxSlider.value);
            // Mute the SFX volume
            sfxSlider.value = 0.0001f;
            sfxButton.image.sprite = muteBasicAudio;
        }
        else
        {
            // Restore the previous SFX volume
            float previousVolume = PlayerPrefs.GetFloat("PreviousSFXVolume", 1.0f); // Default to 1.0 if not found
            sfxSlider.value = previousVolume;
            sfxButton.image.sprite = nonMuteBasicAudio;
        }
        SetSFXVolume(sfxSlider.value); // Update the Audio Mixer settings
    }
}
