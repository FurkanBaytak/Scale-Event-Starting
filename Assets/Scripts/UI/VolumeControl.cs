using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class VolumeControl : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider vfxVolumeSlider;

    public TMP_Text masterVolumeText;
    public TMP_Text musicVolumeText;
    public TMP_Text vfxVolumeText;

    void Start()
    {
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        vfxVolumeSlider.onValueChanged.AddListener(SetVFXVolume);

        masterVolumeSlider.value = PlayerPrefs.GetFloat("Master", 0.75f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("Music", 0.75f);
        vfxVolumeSlider.value = PlayerPrefs.GetFloat("SFX", 0.75f);

        UpdateVolumeText(masterVolumeSlider.value, masterVolumeText);
        UpdateVolumeText(musicVolumeSlider.value, musicVolumeText);
        UpdateVolumeText(vfxVolumeSlider.value, vfxVolumeText);
    }

    public void SetMasterVolume(float volume)
    {
        if (volume == 0)
        {
            audioMixer.SetFloat("Master", -80f);
        }
        else
        {
            audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat("Master", volume);
        UpdateVolumeText(volume, masterVolumeText);
    }

    public void SetMusicVolume(float volume)
    {
        if (volume == 0)
        {
            audioMixer.SetFloat("Music", -80f);
        }
        else
        {
            audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat("Music", volume);
        UpdateVolumeText(volume, musicVolumeText);
    }

    public void SetVFXVolume(float volume)
    {
        if (volume == 0)
        {
            audioMixer.SetFloat("SFX", -80f);
        }
        else
        {
            audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        }
        PlayerPrefs.SetFloat("SFX", volume);
        UpdateVolumeText(volume, vfxVolumeText);
    }

    private void UpdateVolumeText(float volume, TMP_Text volumeText)
    {
        int volumePercentage = Mathf.RoundToInt(volume * 100);
        volumeText.text = volumePercentage.ToString();
    }
}
