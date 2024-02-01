using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBuildingSystem : MonoBehaviour
{
    public static TowerBuildingSystem Instance { get; private set; }

    private int currentgold;
    public event EventHandler OnGoldChange;

    private void Awake()
    {
        Instance = this;
    }

    public void AddGold(int gold)
    {
        currentgold += gold;
        OnGoldChange?.Invoke(this, EventArgs.Empty);
    }
    public void SpendGold(int gold)
    {
        currentgold -= gold;
        OnGoldChange?.Invoke(this, EventArgs.Empty);
    }

    public bool TrySpendGoldToBuildTower(Unit unit, GridPosition gridPosition)
    {
        if(currentgold >= unit.GetCost())
        {
            SpendGold(unit.GetCost());
            BuildTowerAtPosition(unit, gridPosition);
            return true;
        }
        Debug.Log("Not Enough Gold");
            return false;
    }
    public bool TestSpendGoldToBuildTower(Unit unit)
    {
        if (currentgold >= unit.GetCost())
        {
            return true;
        }
        Debug.Log("Not Enough Gold");
        return false;
    }

    private void BuildTowerAtPosition(Unit unit, GridPosition gridPosition)
    {
        Unit priorUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        GameObject builtObject = Instantiate(unit.transform, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity).gameObject;
        Unit builtUnit = builtObject.GetComponent<Unit>();
        if (priorUnit != null) 
        {
            int priorAP = priorUnit.GetCurrentAP();
            int priorHP = priorUnit.GetNumericalHP();
            builtUnit.SetUnitHP(priorHP);
            builtUnit.SetAP(priorAP);
            priorUnit.DestroyUnit();
        }
        else
        {
            builtUnit.SetAP(builtUnit.GetAPMax()/2);
        }
        if (builtUnit.IsEnemy() == false) 
            UnitActionSystem.Instance.SetSelectedUnit(builtUnit);
    }

    public int GetGold() => currentgold;
}
