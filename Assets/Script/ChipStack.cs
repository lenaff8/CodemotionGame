using System;
using UnityEngine;
using System.Collections.Generic;

public class ChipStack : MonoBehaviour
{
    [Header("Energy Chips")]
    [SerializeField] private List<GameObject> energyChips = new List<GameObject>();
    [Header("People Chips")]
    [SerializeField] private List<GameObject> peopleChips = new List<GameObject>();
    [Header("Reputation Chips")]
    [SerializeField] private List<GameObject> reputationChips = new List<GameObject>();
    [Header("Money Chips")]
    [SerializeField] private List<GameObject> moneyChips = new List<GameObject>();



    public void SetScores(int energy, int people, int reputation, int money)
    {
        SetChipList(energyChips, energy);
        SetChipList(peopleChips, people);
        SetChipList(reputationChips, reputation);
        SetChipList(moneyChips, money);
    }

    private void SetChipList(List<GameObject> chips, int     score)
    {
        score = Mathf.Clamp(score, 0, chips.Count);
        for (int i = 0; i < chips.Count; i++)
        {
            chips[i].SetActive(i < score);
        }
    }

    private void Awake()
    {
        SetScores(5, 5, 5, 5);
    }
}