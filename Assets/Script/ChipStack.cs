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

    [Header("Vibration Settings")]
    [SerializeField] private float vibrationAmount = 0.015f;
    [SerializeField] private float vibrationSpeed = 0.05f;

    private List<GameObject> vibratingGroups = new List<GameObject>();
    private Dictionary<GameObject, Vector3> originalPositions = new Dictionary<GameObject, Vector3>();
    private float vibrationTimer = 0f;

    public void SetScores(int energy, int people, int reputation, int money)
    {
        SetChipList(energyChips, energy, GetEnergyTransform());
        SetChipList(peopleChips, people, GetPeopleTransform());
        SetChipList(reputationChips, reputation, GetReputationTransform());
        SetChipList(moneyChips, money, GetMoneyTransform());
    }

    private void SetChipList(List<GameObject> chips, int score, Transform chipGroupTransform)
    {
        score = Mathf.Clamp(score, 0, chips.Count);
        for (int i = 0; i < chips.Count; i++)
        {
            chips[i].SetActive(i < score);
        }

        if (chipGroupTransform == null)
            return;

        // Activar vibración si la puntuación es >= 8
        if (score >= 8)
        {
            AddGroupToVibration(chipGroupTransform.gameObject);
        }
        else
        {
            // Detener vibración si la puntuación baja de 8
            RemoveGroupFromVibration(chipGroupTransform.gameObject);
        }
    }

    private void AddGroupToVibration(GameObject target)
    {
        // Verificar si ya está en vibración
        if (vibratingGroups.Contains(target))
            return;

        // Guardar posición original si no la tenemos
        if (!originalPositions.ContainsKey(target))
        {
            originalPositions[target] = target.transform.localPosition;
        }

        vibratingGroups.Add(target);
    }

    private void RemoveGroupFromVibration(GameObject target)
    {
        if (vibratingGroups.Remove(target))
        {
            // Restaurar posición original
            if (originalPositions.ContainsKey(target))
            {
                target.transform.localPosition = originalPositions[target];
            }
        }
    }

    private void Update()
    {
        if (vibratingGroups.Count > 0)
        {
            vibrationTimer += Time.deltaTime;

            // Aplicar vibración a todos los grupos
            float vibrationOffset = Mathf.Sin(vibrationTimer / vibrationSpeed * Mathf.PI) * vibrationAmount;

            foreach (GameObject group in vibratingGroups)
            {
                if (group != null && originalPositions.ContainsKey(group))
                {
                    group.transform.localPosition = originalPositions[group] + Vector3.right * vibrationOffset;
                }
            }
        }
    }

    private Transform GetEnergyTransform()
    {
        return energyChips.Count > 0 ? energyChips[0].transform.parent : null;
    }

    private Transform GetPeopleTransform()
    {
        return peopleChips.Count > 0 ? peopleChips[0].transform.parent : null;
    }

    private Transform GetReputationTransform()
    {
        return reputationChips.Count > 0 ? reputationChips[0].transform.parent : null;
    }

    private Transform GetMoneyTransform()
    {
        return moneyChips.Count > 0 ? moneyChips[0].transform.parent : null;
    }

    private void Awake()
    {
        SetScores(5, 5, 5, 5);
    }
}