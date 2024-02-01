using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set;}

    public event EventHandler OnSelectedUnitChange;
    public event EventHandler OnSelectedActionChange;
    public event EventHandler OnActionStarted;
    public event EventHandler<bool> OnBusyChanged;


    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayermask;
    private bool isBusy;

    private BaseAction selectedAction;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        SetSelectedUnit(selectedUnit);
    }

    private void Update()
    {
        if (isBusy)
        {
            return;
        }

        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }


        if (TryHandleUnitSelection())
            return;
        HandleSelectedAction();
    }
    private bool TryHandleUnitSelection()
    {
        if (InputManager.Instance.LeftClick())
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(InputManager.Instance.GetMousePosition());
            RaycastHit2D targetRay = Physics2D.Raycast(new Vector2(worldPosition.x, worldPosition.y), Vector2.zero, 0, unitLayermask);
            if (targetRay)
            {
                if (targetRay.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    if(unit == selectedUnit)
                    {
                        return false;
                    }
                    if(unit.IsEnemy())
                    {
                        return false;   
                    }
                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }
        return false;
    }

    private void HandleSelectedAction()
    {
        if (InputManager.Instance.Rightclick())
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseChecker.GetPosition());
            if (!selectedAction.isValidPosition(mouseGridPosition))
            {
                return;
            }
            if (!selectedUnit.TrySpendAPtoTakeAction(selectedAction))
            {
                return;
            }
            SetBusy();
            selectedAction.TakeAction(mouseGridPosition, ClearBusy);
            OnActionStarted?.Invoke(this, EventArgs.Empty);
        }
    }

    private void SetBusy()
    {
        isBusy = true;
        OnBusyChanged?.Invoke(this, isBusy);
    }
    private void ClearBusy()
    {
        isBusy = false;
        OnBusyChanged?.Invoke(this, isBusy);
        OnSelectedActionChange?.Invoke(this, EventArgs.Empty);
    }
    public void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        selectedAction = unit.GetBaseActions()[0];
        OnSelectedUnitChange?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;
        OnSelectedActionChange?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit() => selectedUnit;

    public BaseAction GetSelectedAction() => selectedAction;

    public bool IsBusy() => isBusy;

    
}
