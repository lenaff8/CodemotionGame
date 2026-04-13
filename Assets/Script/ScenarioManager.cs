using System;
using UnityEngine;
using System.Collections.Generic;

public class ScenarioManager : MonoBehaviour
{
    public static ScenarioManager Instance;
    public DraggableSprite draggableSprite;
    public Action onScenarioGenerated;

    private Queue<AIScenario> generatedScenarios = new Queue<AIScenario>();
    public AIScenario CurrentScenario => generatedScenarios.Peek();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        AIScenarioGenerator.Instance.onAIScenarioGenerated += OnAIScenarioGenerated;
        AIScenarioGenerator.Instance.onAIGenerationError += OnAIGenerationError;

        AIScenarioGenerator.Instance.GenerateScenario(4);
    }

    private void OnAIScenarioGenerated(AIScenario aiScenario)
    {
        generatedScenarios.Enqueue(aiScenario);
        onScenarioGenerated?.Invoke();
    }

    private void OnAIGenerationError(List<string> errors)
    {
        Debug.LogError("Error generando escenario de IA");
        foreach (var error in errors)
        {
            Debug.LogError("- " + error);
        }
    }

    public AIScenario GetNextScenario()
    {
        AIScenarioGenerator.Instance.GenerateScenario();
        return generatedScenarios.Dequeue();
    }

    public void ResetGame()
    {
        generatedScenarios.Clear();
        draggableSprite.ResetForNewGame();
        AIScenarioGenerator.Instance.GenerateScenario(4);
    }
}