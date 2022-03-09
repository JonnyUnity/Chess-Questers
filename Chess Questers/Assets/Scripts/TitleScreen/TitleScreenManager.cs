using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private GameObject _abandonButton;

    void Start()
    {
        // Check if a current run exists.
        // If so, change text on "Start Game" button and show "abandon run" button
        UpdateMenuButtons();

    }


    public void StartGame()
    {
        Debug.Log("Start Game!");
        SceneManager.LoadScene(1);

    }

    public void ContinueRun()
    {
        Debug.Log("Continue Run!");
        GameManager.Instance.ContinueQuest();
    }


    public void AbandonRun()
    {
        Debug.Log("Abandon run!");

        // delete run data
        SaveDataManager.Delete();
        UpdateMenuButtons();

    }


    public void Settings()
    {
        Debug.Log("Load Settings!");
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }


    private bool IsRunInProgress()
    {
        // the real guts of this method will check for a game save...
        return SaveDataManager.QuestDataExists();
    }

    private void UpdateMenuButtons()
    {
        bool runInProgress = IsRunInProgress();

        _startButton.SetActive(!runInProgress);
        _continueButton.SetActive(runInProgress);
        _abandonButton.SetActive(runInProgress);

    }


}
