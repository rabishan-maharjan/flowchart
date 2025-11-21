using System;
using System.Collections.Generic;
using System.Linq;
using Arcube;
using Arcube.UiManagement;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LogicExpressionPanel : Panel
{
    [SerializeField] public TMP_Dropdown dr_variable_1;
    [SerializeField] public TMP_Dropdown dr_operator;
    [SerializeField] public DropDownWithInputField dr_variable_2;
    [SerializeField] public TMP_Dropdown dr_next_logic_operator;
    [SerializeField] private ButtonImage b_delete;
    private void Reset()
    {
        transform.TryFindObject(nameof(dr_variable_1), out dr_variable_1);
        transform.TryFindObject(nameof(dr_operator), out dr_operator);
        transform.TryFindObject(nameof(dr_variable_2), out dr_variable_2);
        transform.TryFindObject(nameof(dr_next_logic_operator), out dr_next_logic_operator);
        transform.TryFindObject(nameof(b_delete), out b_delete);
    }

    public UnityEvent<int> onOperatorSelected;
    private void Start()
    {
        dr_next_logic_operator.onValueChanged.AddListener((value) =>
        {
            onOperatorSelected.Invoke(value);
        });
        
        b_delete.OnClick.AddListener(() =>
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
        
        dr_variable_1.onValueChanged.AddListener((value) =>
        {
            var v1 = Variable.TryGetVariable(dr_variable_1.options[dr_variable_1.value].text);
            if (v1 == null)
            {
                MessageUi.Show("Variable is not a valid logic expression");
                return;
            }
            
            var operators = new List<string>(GetOperators(v1.Type));
            dr_operator.options = operators.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
        });
    }

    public void Set(List<Variable> allVariables, LogicExpression expression)
    {
        if(allVariables.Count == 0)
        {
            MessageUi.Show("No variables available");
            return;
        }
        
        dr_variable_1.options = allVariables.Select(n => new TMP_Dropdown.OptionData(n.Name)).ToList();
        var v1 = Variable.TryGetVariable(expression.Variable1);
        
        dr_variable_1.SetValueWithoutNotify(allVariables.IndexOf(v1));
        
        var v2 = Variable.TryGetVariable(expression.Variable2);
        dr_variable_2.Set(allVariables, v2);

        var v = v1 ?? allVariables[0];
        var operators = new List<string>(GetOperators(v.Type));
        dr_operator.options = operators.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
        if(!string.IsNullOrEmpty(expression.Operator)) dr_operator.SetValueWithoutNotify( operators.IndexOf(expression.Operator));
        
        operators = new List<string>(OperatorHandler.BooleanOperators);
        operators.Insert(0, "None");
        dr_next_logic_operator.options = operators.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
        if(!string.IsNullOrEmpty(expression.ConjunctionOperator)) dr_next_logic_operator.SetValueWithoutNotify( Array.IndexOf(OperatorHandler.BooleanOperators, expression.ConjunctionOperator));
    }
    
    private string[] GetOperators(VariableType type)
    {
        return type switch
        {
            VariableType.Number => OperatorHandler.LogicOperators,
            VariableType.String => OperatorHandler.StringLogicOperators,
            VariableType.Bool => OperatorHandler.StringLogicOperators,
            _ => throw new Exception($"Unsupported variable type: {type}")
        };
    }
    
    public void SetActive(bool value)
    {
        dr_operator.interactable = value;
        dr_variable_1.interactable = value;
        dr_variable_2.SetActive(value);;
        dr_next_logic_operator.interactable = value;
    }
}