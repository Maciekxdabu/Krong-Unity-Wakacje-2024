using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    private Resolution[] _resolutions;
    [SerializeField] private TMP_Dropdown _resolutionDropdown;

    private void Start()
    {
        _resolutions = Screen.resolutions;

        FillResolutionDropdown(_resolutionDropdown, _resolutions);
    }

    public void SetFullscreenMode(int modeIndex)
    {
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

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
    }

    private void FillResolutionDropdown(TMP_Dropdown resolutionDropdown, Resolution[] resolutions)
    {
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        Array.Reverse(resolutions);

        foreach (Resolution resolution in resolutions)
        {
            string option = resolution.width + " x " + resolution.height;
            options.Add(option);

            if (resolution.width == Screen.currentResolution.width && resolution.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = options.Count - 1;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
}
