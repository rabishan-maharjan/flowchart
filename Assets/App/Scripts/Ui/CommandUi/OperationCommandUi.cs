using System;
using System.Collections.Generic;
using System.Linq;
using Arcube;
using Arcube.UiManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IGraph
{
    void Restore();
    void Highlight();
    void Error();
}

public class OperationCommandUi : CommandUi
{
    [SerializeField] private TMP_Dropdown dr_variable;
    [SerializeField] private OperatorExpressionPanel operatorExpressionPanelPrefab;
    protected override void Reset()
    {
        transform.TryFindObject(nameof(dr_variable), out dr_variable);
        
        base.Reset();
    }

    [SerializeField] private Transform list;
    
    private List<Variable> _allVariables;
    private List<Variable> _exposedVariables;
    private FlowChartManager _flowChartManager;
    protected override void SetUi()
    {
        foreach (var e in GetComponentsInChildren<OperatorExpressionPanel>())
        {
            Destroy(e.gameObject);
        }
        
        _flowChartManager = AppManager.GetManager<FlowChartManager>();
        _allVariables = _flowChartManager.ActiveVariables;
        _exposedVariables = _allVariables.Where(v => v.Exposed).ToList();
        var variableNames = _exposedVariables.Select(variable => variable.Name).ToList();
        dr_variable.options = variableNames.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
        
        //load old variables
        var operationCommand = (OperationCommand)Command;
        foreach (var expression in operationCommand.Expressions)
        {
            AddField(expression);
        }
        
        if(operationCommand.Expressions.Count == 0) AddField(new Expression());
    }

    private void AddField(Expression expression)
    {
        var selected = _exposedVariables[dr_variable.value];
        var variables = _exposedVariables.Where(v => v.Type == selected.Type && v.Type != VariableType.Bool).ToList();
        var operatorExpression = Instantiate(operatorExpressionPanelPrefab, list);
        
        var v = new Variable();
        if (expression.Variable != null) _flowChartManager.VariableMap.TryGetValue(expression.Variable, out v);
        operatorExpression.Set(variables, v, expression.Operator);
        
        operatorExpression.onOperatorSelected.AddListener(value =>
        {
            if (value == 0)
            {
                //destroy next child
                var expressions = list.GetComponentsInChildren<OperatorExpressionPanel>();
                var index = Array.IndexOf(expressions, operatorExpression);
                if(index < expressions.Length - 1)
                {
                    for (var i = index + 1; i < expressions.Length; i++)
                    {
                        expressions[i].SetActive(false);
                    }

                    MessageUi.Show("Remaining expressions will be ignored");
                }
            }
            else
            {
                var expressions = list.GetComponentsInChildren<OperatorExpressionPanel>();
                var index = Array.IndexOf(expressions, operatorExpression);
                for (var i = index + 1; i < expressions.Length; i++)
                {
                    expressions[i].SetActive(true);
                }
                
                if(list.childCount == operatorExpression.transform.GetSiblingIndex() + 1)
                {
                    AddField(new Expression());
                }
            }
        });
        
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)list.transform);
    }

    protected override void Apply()
    {
        if (_allVariables.Count == 0)
        {
            Debug.Log("Not enough variables");
            return;
        }
        var operationCommand = (OperationCommand)Command;
        operationCommand.Expressions.Clear();
        
        var v = _exposedVariables[dr_variable.value];
        operationCommand.Variable = v.ID;
        for (var i = 0; i < list.childCount; i++)
        {
            var operatorExpression = list.GetChild(i).GetComponent<OperatorExpressionPanel>();
            if (!operatorExpression) continue;
            
            var ve = operatorExpression.dr_variable.Value;
            ve.Type = Variable.DetectType(ve.Value);
            if (ve.Type != v.Type)
            {
                MessageUi.Show("Variable type mismatch");
                return;
            }
            
            operationCommand.Expressions.Add(new Expression()
            {
                Variable = operatorExpression.dr_variable.Value.ID,
                Operator = operatorExpression.dr_operator.value > 0 ? OperatorHandler.ArithmeticOperators[operatorExpression.dr_operator.value - 1] : ""
            });
        }
        
        base.Apply();
    }
}