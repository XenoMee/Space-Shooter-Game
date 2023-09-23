using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadPrefs : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private bool _canUse = false;
    [SerializeField] private Settings _settingsController;

    [Header("Volume Settings")]
    [SerializeField] private Text _volumeTextValue = null;
    [SerializeField] private Slider _volumeSlider = null;

    [Header("Brightness Settings")]
    [SerializeField] private Slider _brightnessSlider = null;
    [SerializeField] private Text _brightnessTextValue = null;

    [Header("Quality Level Settings")]
    [SerializeField] private TMP_Dropdown _qualityDropdown;

    [Header("Fullscreen Settings")]
    [SerializeField] private Toggle _fullScreenToggle;

    [Header("Sensitivity Settings")]
    [SerializeField] private Text _controllerSensTextValue = null;
    [SerializeField] private Slider _controllerSensSlider = null;

    [Header("Invert Y Settings")]
    [SerializeField] private Toggle _invertYToggle = null;
    
    private void Awake()
    {
        if(_canUse)
        {
            if (PlayerPrefs.HasKey("masterVolume")){
                float localVolume = PlayerPrefs.GetFloat("masterVolume");

                _volumeTextValue.text = localVolume.ToString("0.0");
                _volumeSlider.value = localVolume;
                AudioListener.volume = localVolume;
            }
            else
            {
                _settingsController.ResetButton("Audio");
            }

            if (PlayerPrefs.HasKey("masterQuality"))
            {
                int localQuality = PlayerPrefs.GetInt("masterQuality");
                _qualityDropdown.value = localQuality;
                QualitySettings.SetQualityLevel(localQuality);
            }

            if (PlayerPrefs.HasKey("masterFullscreen"))
            {
                int localFullscreen = PlayerPrefs.GetInt("masterFullscreen");

                if(localFullscreen == 1)
                {
                    Screen.fullScreen = true;
                    _fullScreenToggle.isOn = true;
                }
                else
                {
                    Screen.fullScreen = false;
                    _fullScreenToggle.isOn = false;
                }
            }

            if (PlayerPrefs.HasKey("masterBrightness"))
            {
                float localBrightness = PlayerPrefs.GetFloat("masterBrightness");

                _brightnessTextValue.text = localBrightness.ToString("0.0");
                _brightnessSlider.value = localBrightness;
            }

            if (PlayerPrefs.HasKey("masterSen"))
            {
                float localSensitivity = PlayerPrefs.GetFloat("masterSen");

                _controllerSensTextValue.text = localSensitivity.ToString("0.0");
                _controllerSensSlider.value = localSensitivity;
                _settingsController.mainControllerSens = Mathf.RoundToInt(localSensitivity);
            }

            if (PlayerPrefs.HasKey("masterInvertY"))
            {
                if(PlayerPrefs.GetInt("masterInvertY") == 1)
                {
                    _invertYToggle.isOn = true;
                }
                else
                {
                    _invertYToggle.isOn = false;
                }
            }
        }
    }

}
