using UnityEngine;
using System;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // true = jugando, false = login o game over
    public static bool IsPlaying = false;

    private int rounds = 1;
    public int Rounds => rounds;
    
    public enum StatType
    {
        Energy = 0,
        People = 1,
        Reputation = 2,
        Money = 3
    }

    public int[] stats = new int[4] { 5, 5, 5, 5 };

    public Action onGameOver;
    public Action<StatType, bool> onGameOverCard;
    [SerializeField] private ChipStack chipStack;
    [SerializeField] private TextMeshProUGUI scoreText;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        IsPlaying = false;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void ApplyScenarioEffects(bool chooseRight)
    {
        AIScenario scenario = ScenarioManager.Instance.CurrentScenario;
        CharacterEffect effect = chooseRight ? scenario.effects.right : scenario.effects.left;
        // Mapear efectos de IA a stats del juego
        stats[(int)StatType.Money] += effect.money;
        stats[(int)StatType.Reputation] += effect.reputation;
        stats[(int)StatType.People] += effect.people;
        stats[(int)StatType.Energy] += effect.energy;
        chipStack.SetScores(stats[(int)StatType.Energy], stats[(int)StatType.People], stats[(int)StatType.Reputation], stats[(int)StatType.Money]);
        CheckGameOver();
    }

    private void CompleteRound()
    {
        ++rounds;
        scoreText.text = $"{rounds} días al cargo";
    }

    private void CheckGameOver()
    {
        for (int i = 0; i < stats.Length; i++)
        {
            if (stats[i] <= 0 || stats[i] >= 10)
            {
                onGameOverCard?.Invoke((StatType)i, stats[i] >= 10);
                return;
            }
        }
        CompleteRound();
    }

    public void StartGameOverTimer(float delay)
    {
        IsPlaying = false;
        StartCoroutine(DelayedGameOver(delay));
    }

    private IEnumerator DelayedGameOver(float delay)
    {
        yield return new WaitForSeconds(delay);
        onGameOver?.Invoke();
    }

    public void ResetGame()
    {
        stats = new int[4] { 5, 5, 5, 5 };
        rounds = 1;
        scoreText.text = "1 día al cargo";
        chipStack.SetScores(5, 5, 5, 5);
        IsPlaying = true;
    }
}