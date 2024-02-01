using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    private List<Unit> unitsHere;
    private GridSystem<GridObject> gridSystem;
    private GridPosition gridPosition;

    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        unitsHere = new List<Unit>();
    }

    public override string ToString()
    {
        string unitString = "";
        foreach (Unit unit in unitsHere)
        {
            unitString += unit + "\n";
        }
        return gridPosition.ToString() + "\n" + unitString;
    }
    public void AddUnitHere(Unit unit)
    {
        unitsHere.Add(unit);
    }
    public List<Unit> GetUnitsHere()
    {
        return unitsHere;
    }
    public void RemoveUnitHere(Unit unit)
    {
        unitsHere.Remove(unit);
    }

    public bool HasAnyUnit()
    {
        return unitsHere.Count > 0;
    }
    public Unit GetUnitHere()
    {
        if(HasAnyUnit())
        {
            return unitsHere[0];
        }
        
        return null;
    }
}
