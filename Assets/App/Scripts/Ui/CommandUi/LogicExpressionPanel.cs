using System;
using System.Collections.Generic;
using System.Linq;
using Arcube;
using Arcube.UiManagement;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class LogicExpressionPanel : Panel
{
    [SerializeField] public TMP_Dropdown dr_variable_1;
    [SerializeField] public TMP_Dropdown dr_operator;
    [SerializeField] public DropDownWithInputField dr_variable_2;
    [SerializeField] public TMP_Dropdown dr_next_logic_operator;
    private void Reset()
    {
        transform.TryFindObject(nameof(dr_variable_1), out dr_variable_1);
        transform.TryFindObject(nameof(dr_operator), out dr_operator);
        transform.TryFindObject(nameof(dr_variable_2), out dr_variable_2);
    }

    public UnityEvent<int> onOperatorSelected;
    private void Start()
    {
        dr_next_logic_operator.onValueChanged.AddListener((value) =>
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

    public void Set(List<Variable> allVariables, LogicExpression expression)
    {
        dr_variable_1.options = allVariables.Select(n => new TMP_Dropdown.OptionData(n.Name)).ToList();
        var v1 = Variable.TryGetVariable(expression.Variable1);
        if(v1 != null) dr_variable_1.SetValueWithoutNotify(allVariables.IndexOf(v1));
        
        var v2 = Variable.TryGetVariable(expression.Variable1);
        if(v2 != null) dr_variable_2.Set(allVariables, v2);
        
        var operators = new List<string>(OperatorHandler.LogicOperators);
        dr_operator.options = operators.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
        if(!string.IsNullOrEmpty(expression.Operator)) dr_operator.SetValueWithoutNotify( Array.IndexOf(OperatorHandler.LogicOperators, expression.Operator));
        
        operators = new List<string>(OperatorHandler.ConjunctionOperators);
        operators.Insert(0, "None");
        dr_next_logic_operator.options = operators.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
        if(!string.IsNullOrEmpty(expression.ConjunctionOperator)) dr_next_logic_operator.SetValueWithoutNotify( Array.IndexOf(OperatorHandler.ConjunctionOperators, expression.ConjunctionOperator));
    }
    
    public void SetActive(bool value)
    {
        dr_operator.interactable = value;
        dr_variable_1.interactable = value;
        dr_variable_2.SetActive(value);;
        dr_next_logic_operator.interactable = value;
    }
}