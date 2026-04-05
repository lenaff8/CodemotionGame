using UnityEngine;
using TMPro;

public class CardScenario : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI sentenceText;
    [SerializeField]
    private TextMeshProUGUI leftResponseText;
    [SerializeField]
    private TextMeshProUGUI rightResponseText;

    public void UpdateScenarioTexts()
    {
        var scenario = ScenarioManager.Instance.CurrentScenario;

        sentenceText.text = scenario.situation;
        leftResponseText.text = scenario.left_option;
        rightResponseText.text = scenario.right_option;
    }
    
    public void UpdateNextScenarioTexts()
    {
        ScenarioManager.Instance.GetNextScenario();
        UpdateScenarioTexts();
    }
}
