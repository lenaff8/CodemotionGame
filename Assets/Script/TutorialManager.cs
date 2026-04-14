using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    private const string TUTORIAL_SHOWN_KEY = "TutorialShown";

    [Header("Panels")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject gesturePanel;

    [Header("Chip Legend Sprites (Energy, People, Reputation, Money)")]
    [SerializeField] private Image chipIconEnergy;
    [SerializeField] private Image chipIconPeople;
    [SerializeField] private Image chipIconReputation;
    [SerializeField] private Image chipIconMoney;

    [SerializeField] private Sprite spriteEnergy;
    [SerializeField] private Sprite spritePeople;
    [SerializeField] private Sprite spriteReputation;
    [SerializeField] private Sprite spriteMoney;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        tutorialPanel.SetActive(false);
        gesturePanel.SetActive(false);
    }

    public void TryShowTutorial()
    {
        if (PlayerPrefs.GetInt(TUTORIAL_SHOWN_KEY, 0) == 0)
        {
            PopulateChipLegend();
            tutorialPanel.SetActive(true);
        }
        else
        {
            GameManager.IsPlaying = true;
        }
    }

    private void PopulateChipLegend()
    {
        SetIcon(chipIconEnergy,     spriteEnergy);
        SetIcon(chipIconPeople,     spritePeople);
        SetIcon(chipIconReputation, spriteReputation);
        SetIcon(chipIconMoney,      spriteMoney);
    }

    private void SetIcon(Image img, Sprite spr)
    {
        if (img != null && spr != null)
            img.sprite = spr;
    }

    // Called by the OK button on tutorialPanel
    public void OnOkPressed()
    {
        tutorialPanel.SetActive(false);
        gesturePanel.SetActive(true);

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
