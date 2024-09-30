using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Tooltip("Canvas Group which is enabled first and added to the stack")]
    [SerializeField] private CanvasGroup startingGroup;

    [Tooltip("Only used for setting up the groups on Start")]
    [SerializeField] private CanvasGroup[] registeredGroups;

    [Tooltip("Deactivates objects on platforms like Web where exit doesn't make sense")]
    [SerializeField] private GameObject[] hideIfCantExit;

    private List<CanvasGroup> canvasGroups = new List<CanvasGroup>();

    // ---------- Unity messages

    private void Start()
    {
        processHideIfCantExit();
        //prepare the CanvasGroup's
        foreach (CanvasGroup group in registeredGroups)
        {
            DisableCanvasGroup(group);
            group.ignoreParentGroups = true;
        }

        //enable the first CanvasGroup
        canvasGroups.Clear();
        if (startingGroup != null)
        {
            canvasGroups.Add(startingGroup);
            EnableCanvasGroup(canvasGroups[0]);
        }
        Debug.Assert(canvasGroups.Count > 0, "No starting Canvas group has been set");
    }

    private void processHideIfCantExit()
    {
        if (Application.platform != RuntimePlatform.WebGLPlayer) {
            return;
        }

        foreach (var go in hideIfCantExit) {
            go.SetActive(false);
        }
    }

    // ---------- public messages (called from Events in Inspector)

    /// <summary>
    /// Switches focus over to the next CavasGroup <br/>
    /// (disables the last Canvas)
    /// </summary>
    public void OnNextCanvasGroupBtn(CanvasGroup nextCanvasGroup)
    {
        DisableCanvasGroup(canvasGroups[canvasGroups.Count - 1]);
        canvasGroups.Add(nextCanvasGroup);
        EnableCanvasGroup(nextCanvasGroup);
    }

    /// <summary>
    /// Goes back to the last CanvasGroup (if possible) <br/>
    /// (will forget the current CanvasGroup)
    /// </summary>
    public void OnBackBtn()
    {
        if (canvasGroups.Count <= 1)
        {
            Debug.LogWarning("WAR: There are noe menu's to go back to");
            return;
        }

        DisableCanvasGroup(canvasGroups[canvasGroups.Count-1]);
        canvasGroups.RemoveAt(canvasGroups.Count-1);
        EnableCanvasGroup(canvasGroups[canvasGroups.Count-1]);
    }

    public void OnExitGameBtn()
    {
        Application.Quit();
    }

    public void OnLevelClickedBtn(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    // ---------- private methods

    /// <summary>
    /// Enables visibility and interactability for the given CanvasGroup
    /// </summary>
    private void EnableCanvasGroup(CanvasGroup group)
    {
        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    /// <summary>
    /// Disables visibility and interactability for the given CanvasGroup
    /// </summary>
    private void DisableCanvasGroup(CanvasGroup group)
    {
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }
}
