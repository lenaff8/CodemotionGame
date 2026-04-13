using UnityEngine;
using TMPro;

public class LanguageButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;

    public void Toggle()
    {
        bool isEnglish = !LoginManager.IsEnglish;
        LoginManager.Instance.OnLanguageChanged(isEnglish);
        label.text = isEnglish ? "EN" : "ES";
    }
}
