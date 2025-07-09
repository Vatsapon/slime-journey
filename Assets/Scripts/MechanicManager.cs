using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class MechanicManager : MonoBehaviour {

    [Header("For Main Menu")]
    public TextMeshProUGUI versionText;
    public GameObject backgroundMusicObject;

    [Header("For Tutorial")]
    public Button nextButton;
    public Button previousButton;
    public GameObject windowListObject;

    private int currentWindow;
    [HideInInspector]
    public List<GameObject> windowList = new List<GameObject>();

    [Header("For Cheat Code")]
    public Button nextButtonC;
    public Button previousButtonC;
    public GameObject phaseWindowListObject;

    private int currentWindowC;
    [HideInInspector]
    public List<GameObject> phaseWindowList = new List<GameObject>();

    private void Start() {

        if (versionText != null) {
            versionText.text = versionText.text.Replace("<version>", Application.version);
        }

        if (backgroundMusicObject != null && GameObject.FindGameObjectWithTag("Music") == null) {
            GameObject backgroundMusic = Instantiate(backgroundMusicObject, transform.position, transform.rotation);
            DontDestroyOnLoad(backgroundMusic);
        }

        if (windowListObject != null) {

            for (int i = 0; i < windowListObject.transform.childCount; i++) {
                windowList.Add(windowListObject.transform.GetChild(i).gameObject);
            }

            currentWindow = 0;

            if (windowList.Count != 0) {
                windowList[0].SetActive(true);

                if (windowList.Count == 1) {
                    nextButton.interactable = false;
                }
            }
        }
        
        if (phaseWindowListObject != null) {

            for (int i = 0; i < phaseWindowListObject.transform.childCount; i++) {
                phaseWindowList.Add(phaseWindowListObject.transform.GetChild(i).gameObject);
            }

            currentWindowC = 0;

            if (phaseWindowList.Count != 0) {
                phaseWindowList[0].SetActive(true);

                if (phaseWindowList.Count == 1) {
                    nextButtonC.interactable = false;
                }
            }
        }
    }

    public void NextWindow() {

        if (currentWindow == windowList.Count - 1) {
            return;
        }

        windowList[currentWindow].SetActive(false);
        currentWindow++;
        windowList[currentWindow].SetActive(true);

        previousButton.interactable = true;

        if (currentWindow == windowList.Count - 1) {
            nextButton.interactable = false;
        } else {
            nextButton.interactable = true;
        }
    }

    public void PreviousWindow() {

        if (currentWindow == 0) {
            return;
        }

        windowList[currentWindow].SetActive(false);
        currentWindow--;
        windowList[currentWindow].SetActive(true);

        nextButton.interactable = true;

        if (currentWindow == 0) {
            previousButton.interactable = false;
        } else {
            previousButton.interactable = true;
        }
    }

    public void NextWindowC() {
        if (currentWindowC == phaseWindowList.Count - 1) {
            return;
        }

        phaseWindowList[currentWindowC].SetActive(false);
        currentWindowC++;
        phaseWindowList[currentWindowC].SetActive(true);

        previousButtonC.interactable = true;

        if (currentWindowC == phaseWindowList.Count - 1) {
            nextButtonC.interactable = false;
        } else {
            nextButtonC.interactable = true;
        }
    }

    public void PreviousWindowC() {
        if (currentWindowC == 0) {
            return;
        }

        phaseWindowList[currentWindowC].SetActive(false);
        currentWindowC--;
        phaseWindowList[currentWindowC].SetActive(true);

        nextButtonC.interactable = true;

        if (currentWindowC == 0) {
            previousButtonC.interactable = false;
        } else {
            previousButtonC.interactable = true;
        }
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadScene(string name) {
        Time.timeScale = 1;
        SceneManager.LoadScene(name);
    }

    public void LoadNextScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadPreviousScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}