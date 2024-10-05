using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    private Resolution[] _resolutions;
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private TMP_Dropdown _fullscreenModeDropdown;
    [SerializeField] private TMP_Dropdown _antialiasingDropdown;

    private void Start()
    {
        _resolutions = Screen.resolutions;
        Array.Reverse(_resolutions);

        FillResolutionDropdown(_resolutionDropdown, _resolutions);
    }

    public void SetFullscreenMode()
    {
        int modeIndex = _fullscreenModeDropdown.value;

        switch (modeIndex)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }
    }

    public void SetResolution()
    {
        int resolutionIndex = _resolutionDropdown.value;
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
    }

    // public void SetAntialiasingMode()
    // {
    //     int modeIndex = _antialiasingDropdown.value;

    //     switch (modeIndex)
    //     {
    //         case 0:
    //             Rendering.Postprocessing;
    //             break;
    //         case 1:
    //             QualitySettings.antiAliasing = 2;
    //             break;
    //         case 2:
    //             QualitySettings.antiAliasing = 4;
    //             break;
    //     }
    // }

    private void FillResolutionDropdown(TMP_Dropdown resolutionDropdown, Resolution[] resolutions)
    {
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        foreach (Resolution resolution in resolutions)
        {
            double refreshRate = Math.Round(resolution.refreshRateRatio.value * 100) / 100;
            string option = resolution.width + " x " + resolution.height + " (" + refreshRate + " Hz)";
            options.Add(option);

            if (resolution.width == Screen.currentResolution.width && resolution.height == Screen.currentResolution.height && resolution.refreshRateRatio.value == Screen.currentResolution.refreshRateRatio.value)
            {
                currentResolutionIndex = options.Count - 1;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
}
