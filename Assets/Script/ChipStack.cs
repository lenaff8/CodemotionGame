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
    [SerializeField] private float vibrationAmount = 0.015f; // Amplitud de la vibración en píxeles
    [SerializeField] private float vibrationSpeed = 0.05f; // Velocidad de la vibración

    private Vector3 originalEnergyPos;
    private Vector3 originalPeoplePos;
    private Vector3 originalReputationPos;
    private Vector3 originalMoneyPos;

    private float vibrationTimer = 0f;
    private bool isVibrating = false;
    private GameObject currentVibrationTarget;

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

        // Activar vibración si la puntuación es exactamente 8
        if (score >= 8 && chipGroupTransform != null)
        {
            StartVibration(chipGroupTransform.gameObject);
        }
        else if (score < 8 && currentVibrationTarget == chipGroupTransform.gameObject)
        {
            // Detener vibración si la puntuación deja de ser 8
            StopVibration(chipGroupTransform);
        }
    }

    private void StartVibration(GameObject target)
    {
        if (currentVibrationTarget != target)
        {
            // Detener vibración anterior si existe
            if (currentVibrationTarget != null)
            {
                StopVibration(currentVibrationTarget.transform);
            }

            currentVibrationTarget = target;
            isVibrating = true;
            vibrationTimer = 0f;
        }
    }

    private void StopVibration(Transform target)
    {
        if (target != null)
        {
            target.localPosition = GetOriginalPosition(target.gameObject);
        }
        isVibrating = false;
        currentVibrationTarget = null;
    }

    private void Update()
    {
        if (isVibrating && currentVibrationTarget != null)
        {
            vibrationTimer += Time.deltaTime;

            // Aplicar vibración horizontal en loop infinito
            float vibrationOffset = Mathf.Sin(vibrationTimer / vibrationSpeed * Mathf.PI) * vibrationAmount;
            Vector3 originalPos = GetOriginalPosition(currentVibrationTarget);
            currentVibrationTarget.transform.localPosition = originalPos + Vector3.right * vibrationOffset;
        }
    }

    private Vector3 GetOriginalPosition(GameObject target)
    {
        if (GetEnergyTransform() != null && GetEnergyTransform().gameObject == target)
            return originalEnergyPos;
        if (GetPeopleTransform() != null && GetPeopleTransform().gameObject == target)
            return originalPeoplePos;
        if (GetReputationTransform() != null && GetReputationTransform().gameObject == target)
            return originalReputationPos;
        if (GetMoneyTransform() != null && GetMoneyTransform().gameObject == target)
            return originalMoneyPos;

        return target.transform.localPosition;
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
        // Guardar posiciones originales
        if (GetEnergyTransform() != null)
            originalEnergyPos = GetEnergyTransform().localPosition;
        if (GetPeopleTransform() != null)
            originalPeoplePos = GetPeopleTransform().localPosition;
        if (GetReputationTransform() != null)
            originalReputationPos = GetReputationTransform().localPosition;
        if (GetMoneyTransform() != null)
            originalMoneyPos = GetMoneyTransform().localPosition;

        SetScores(5, 5, 5, 5);
    }
}