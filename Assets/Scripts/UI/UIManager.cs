﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

/// <summary>
/// A class which manages pages of UI elements
/// and the game's UI
/// </summary>
public class UIManager : MonoBehaviour
{

    [Header("Page Management")]
    [Tooltip("The pages (Panels) managed by the UI Manager")]
    public List<UIPage> pages;
    [Tooltip("The index of the active page in the UI")]
    public int currentPage = 0;
    [Tooltip("The page (by index) switched to when the UI Manager starts up")]
    public int defaultPage = 0;

    [Header("Pause Settings")]
    [Tooltip("The index of the pause page in the pages list")]
    public int pausePageIndex = 1;

    public int continuePageIndex = 1;
    [Tooltip("Whether or not to allow pausing")]
    public bool allowPause = true;

    // Whether or not the application is paused
    private bool isPaused = false;

    // A list of all UI element classes
    private List<UIelement> UIelements;

    // The event system handling UI navigation
    [HideInInspector]
    public EventSystem eventSystem;
    // The Input Manager to listen for pausing
    [SerializeField]
    private InputManager inputManager;

    [SerializeField] private GameObject pauseEffect;
    [SerializeField] private GameObject unpauseEffect;

    [SerializeField] private BossHealthBar healthBar;
    [SerializeField] private Text timeBonus;
    [SerializeField] private float timeBonusFadeDelay = 2;
    [SerializeField] private float timeBonusFadeTime = 1;
    [SerializeField] private Text winScreenScoreText;

    /// <summary>
    /// Description:
    /// Standard Unity function called whenever the attached game object is enabled
    /// 
    /// When this component wakes up (including switching scenes) it sets itself as the GameManager's UI manager
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    private void OnEnable()
    {
        SetupGameManagerUIManager();
    }

    /// <summary>
    /// Description:
    /// Sets this component as the UI manager for the GameManager
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    private void SetupGameManagerUIManager()
    {
        if (GameManager.Instance != null && GameManager.Instance.uiManager == null)
        {
            GameManager.Instance.uiManager = this;
        }     
    }

    /// <summary>
    /// Description:
    /// Finds and stores all UIElements in the UIElements list
    /// Input:
    /// None
    /// Return:
    /// void (no return)
    /// </summary>
    private void SetUpUIElements()
    {
        UIelements = FindObjectsOfType<UIelement>().ToList();
    }

    public void SetWinScreenScore(int score) {
        winScreenScoreText.text = "Score: " + score;
    }

    /// <summary>
    /// Description:
    /// Gets the event system from the scene if one exists
    /// If one does not exist a warning will be displayed
    /// Input:
    /// None
    /// Return:
    /// void (no return)
    /// </summary>
    private void SetUpEventSystem()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogWarning("There is no event system in the scene but you are trying to use the UIManager. /n" +
                "All UI in Unity requires an Event System to run. /n" + 
                "You can add one by right clicking in hierarchy then selecting UI->EventSystem.");
        }
    }

    /// <summary>
    /// Description:
    /// Attempts to set up an input manager with this UI manager so it can get pause input
    /// Input:
    /// None
    /// Returns:
    /// void (no return)
    /// </summary>
    private void SetUpInputManager()
    {
        if (inputManager == null)
        {
            inputManager = InputManager.instance;
        }
        if (inputManager == null)
        {
            Debug.LogWarning("The UIManager can not find an Input Manager in the scene, without an Input Manager the UI can not pause");
        }
    }

    /// <summary>
    /// Description:
    /// If the game is paused, unpauses the game.
    /// If the game is not paused, pauses the game.
    /// Inputs:
    /// None
    /// Retuns:
    /// void (no return)
    /// </summary>
    public void TogglePause()
    {
        if (allowPause)
        {
            if (isPaused)
            {
                SetActiveAllPages(false);
                GameManager.Instance.Paused = false;
                isPaused = !isPaused;
                if (unpauseEffect != null)
                    Instantiate(unpauseEffect, transform.position, transform.rotation, null);
            }
            else
            {
                GoToPage(pausePageIndex);
                GameManager.Instance.Paused = true;
                isPaused = !isPaused;
                if (pauseEffect != null)
                    Instantiate(pauseEffect, transform.position, transform.rotation, null);
            }
        }      
    }

    public void ShowContinuePage(bool show) {
        if (show) {
            GoToPage(continuePageIndex);
            allowPause = false;
            GameManager.Instance.Paused = true;
        } else {
            SetActiveAllPages(false);
            allowPause = true;
            GameManager.Instance.Paused = false;
        }
    }



    /// <summary>
    /// Description:
    /// Goes through all UI elements and calls their UpdateUI function
    /// Input:
    /// None
    /// Return:
    /// void (no return)
    /// </summary>
    public void UpdateUI()
    {
        SetUpUIElements();
        foreach (UIelement uiElement in UIelements)
        {
            uiElement.UpdateUI();
        }
    }

    /// <summary>
    /// Description:
    /// Default Unity function that runs once when the script is first started and before Update
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    private void Start()
    {
        SetUpInputManager();
        SetUpEventSystem();
        SetUpUIElements();
        UpdateUI();
    }

    /// <summary>
    /// Description:
    /// Default function from Unity that runs every frame
    /// Inputs: 
    /// none
    /// Returns: 
    /// void (no return)
    /// </summary>
    private void Update()
    {
        CheckPauseInput();
    }

    /// <summary>
    /// Description:
    /// If the input manager is set up, reads the pause input
    /// Inputs:
    /// none
    /// Returns:
    /// void (no return)
    /// </summary>
    private void CheckPauseInput()
    {
        if (inputManager != null)
        {
            if (inputManager.pausePressed)
            {
                TogglePause();
            }
        }
    }
    /// <summary>
    /// Description:
    /// Goes to a page by that page's index
    /// Inputs: 
    /// int page
    /// Returns: 
    /// void (no return)
    /// </summary>
    /// <param name="pageIndex">The index in the page list to go to</param>
    public void GoToPage(int pageIndex)
    {
        if (pageIndex < pages.Count && pages[pageIndex] != null)
        {
            SetActiveAllPages(false);
            pages[pageIndex].gameObject.SetActive(true);
            pages[pageIndex].SetSelectedUIToDefault();
        }
    }

    /// <summary>
    /// Description:
    /// Goes to a page by that page's name
    /// Inputs: 
    /// string pageName
    /// Returns: 
    /// void (no return)
    /// </summary>
    /// <param name="pageName">The name of the page in the game you want to go to, if their are duplicates this picks the first found</param>
    public void GoToPageByName(string pageName)
    {
        UIPage page = pages.Find(item => item.name == pageName);
        int pageIndex = pages.IndexOf(page);
        GoToPage(pageIndex);
    }

    /// <summary>
    /// Description:
    /// Turns all stored pages on or off depending on parameters
    /// Input: 
    /// bool enable
    /// Returns: 
    /// void (no return)
    /// </summary>
    /// <param name="activated">The true or false value to set all page game object's activeness to</param>
    public void SetActiveAllPages(bool activated)
    {
        if (pages != null)
        {
            foreach (UIPage page in pages)
            {
                if (page != null)
                    page.gameObject.SetActive(activated);
            }
        }
    }

    public void ShowHealthBar() {
        if (healthBar != null && healthBar.gameObject.activeSelf == false)
            healthBar.gameObject.SetActive(true);
    }

    public void HideHealthBar() {
        if (healthBar != null && healthBar.gameObject.activeSelf == true)
            healthBar.Hide();
    }

    public void ShowTimeBonus(int score) {
        timeBonus.text = "Time Bonus: " + score;
        timeBonus.gameObject.SetActive(true);
        timeBonus.color = new Color(timeBonus.color.r, timeBonus.color.g, timeBonus.color.b, 1);
        LeanTween.alphaText(timeBonus.GetComponent<RectTransform>(), 0, timeBonusFadeTime).setDelay(timeBonusFadeDelay).setOnComplete(DisableTimeBonusText);
    }

    public void DisableTimeBonusText() {
        timeBonus.gameObject.SetActive(false);
    }
}
