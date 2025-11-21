using System;
using System.Collections.Generic;
using System.Linq;
using Arcube;
using Arcube.UiManagement;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class OperatorExpressionPanel : MonoBehaviour
{
    [SerializeField] public DropDownWithInputField dr_variable;
    [SerializeField] public TMP_Dropdown dr_operator;
    private void Reset()
    {
        transform.TryFindObject(nameof(dr_variable), out dr_variable);
        transform.TryFindObject(nameof(dr_operator), out dr_operator);
    }

    public UnityEvent<int> onOperatorSelected;
    private void Start()
    {
        dr_operator.onValueChanged.AddListener(value =>
        {
            onOperatorSelected.Invoke(value);
        });
        
        gameObject.FindObject<ButtonImage>("b_delete").OnClick.AddListener(() =>
        {
            if (transform.parent.childCount == 1)
            {
                MessageUi.Show("At least one variable is required");
            }
            else
            {
                Destroy(gameObject);
            }
        });
    }

    public void Set(List<Variable> allVariables, Variable selected, string operatorName)
    {
        dr_variable.Set(allVariables, selected);
        var operators = new List<string>(GetOperators(selected.Type));
        operators.Insert(0, "None");
        dr_operator.options = operators.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
        if(!string.IsNullOrEmpty(operatorName)) dr_operator.SetValueWithoutNotify( operators.IndexOf(operatorName) + 1);
    }
    
    private string[] GetOperators(VariableType type) =>
        type switch
        {
            VariableType.Number => OperatorHandler.ArithmeticOperators,
            VariableType.String => OperatorHandler.StringOperators,
            VariableType.Bool => OperatorHandler.BooleanOperators,
            _ => throw new Exception($"Unsupported variable type: {type}")
        };

    public void SetActive(bool value)
    {
        dr_operator.interactable = value;
        dr_variable.SetActive(value);
    }
}