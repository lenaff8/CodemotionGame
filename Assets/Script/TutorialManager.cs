using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    private const string TUTORIAL_SHOWN_KEY = "TutorialShown";

    [Header("Panels")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject tutorialButton;
    [SerializeField] private GameObject gesturePanel;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        tutorialPanel.SetActive(false);
        tutorialButton.SetActive(false);
        gesturePanel.SetActive(false);
        if (PlayerPrefs.GetInt(TUTORIAL_SHOWN_KEY, 0) != 0)
        {
            tutorialButton.SetActive(true);
        }   
    }

    public void TryShowTutorial()
    {
        if (PlayerPrefs.GetInt(TUTORIAL_SHOWN_KEY, 0) == 0)
        {
            ShowTutorial();
        }
        else
        {
            tutorialButton.SetActive(true);
            GameManager.IsPlaying = true;
        }
    }
    
    public void ShowTutorial()
    {
        
        Debug.Log("Show Tutorial");
        tutorialPanel.SetActive(true);
        gesturePanel.SetActive(false);
        tutorialButton.SetActive(false);
    }


    // Called by the OK button on tutorialPanel
    public void OnOkPressed()
    {
        tutorialPanel.SetActive(false);
        tutorialButton.SetActive(true);
        if (PlayerPrefs.GetInt(TUTORIAL_SHOWN_KEY, 0) == 0)
        {
            gesturePanel.SetActive(true);
        }

        PlayerPrefs.SetInt(TUTORIAL_SHOWN_KEY, 1);
        PlayerPrefs.Save();

        GameManager.IsPlaying = true;
    }

    // Called by DraggableSprite when the user starts moving the card
    public void HideGesturePanel()
    {
        if (gesturePanel != null && gesturePanel.activeSelf)
            gesturePanel.SetActive(false);
    }
}
