using System;
using System.Collections.Generic;
using System.Linq;
using Arcube;
using Arcube.UiManagement;
using UnityEngine;

public class LogicCommandUi : CommandUi
{
    [SerializeField] private LogicExpressionPanel logicExpressionPrefab;

    private List<Variable> _allVariables;
    private List<Variable> _exposedVariables;
    [SerializeField] private Transform list;
    private FlowChartManager _flowChartManager;
    protected override void SetUi()
    {
        try
        {
            foreach (var e in GetComponentsInChildren<LogicExpressionPanel>())
            {
                Destroy(e.gameObject);
            }
            
            _flowChartManager = AppManager.GetManager<FlowChartManager>();
            _allVariables = _flowChartManager.ActiveVariables;
            _exposedVariables = _allVariables.Where(v => v.Exposed).ToList();
            
            //load old variables
            var logicCommand = (LogicCommand)Command;
            foreach (var expression in logicCommand.Expressions)
            {
                AddField(expression);
            }
            
            if(list.childCount == 1) AddField(new LogicExpression());
        }
        catch(Exception e)
        {
            Log.AddException(e);
        }
    }
    
    private void AddField(LogicExpression expression)
    {
        var logicExpressionPanel = Instantiate(logicExpressionPrefab, list);
        logicExpressionPanel.Set(_exposedVariables, expression);
        
        logicExpressionPanel.onOperatorSelected.AddListener(value =>
        {
            if (value == 0)
            {
                var expressions = list.GetComponentsInChildren<LogicExpressionPanel>();
                var index = Array.IndexOf(expressions, logicExpressionPanel);
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
                var expressions = list.GetComponentsInChildren<LogicExpressionPanel>();
                var index = Array.IndexOf(expressions, logicExpressionPanel);
                for (var i = index + 1; i < expressions.Length; i++)
                {
                    expressions[i].SetActive(true);
                }
                
                if(list.childCount == logicExpressionPanel.transform.GetSiblingIndex() + 1)
                {
                    AddField(new LogicExpression());
                }
            }
        });
    }

    protected override void Apply()
    {
        if (_allVariables.Count == 0)
        {
            Debug.Log("Not enough variables");
            return;
        }

        var logicCommand = (LogicCommand)Command;
        logicCommand.Expressions.Clear();

        for (var i = 0; i < list.childCount; i++)
        {
            var logicExpressionPanel = list.GetChild(i).GetComponent<LogicExpressionPanel>();
            if (!logicExpressionPanel) continue;
            
            logicCommand.Expressions.Add(new LogicExpression()
            {
                Variable1 = _allVariables[logicExpressionPanel.dr_variable_1.value].ID,
                Variable2 = logicExpressionPanel.dr_variable_2.Value.ID,
                Operator = logicExpressionPanel.dr_operator.value > 0 ? OperatorHandler.LogicOperators[logicExpressionPanel.dr_operator.value] : "",
                ConjunctionOperator = logicExpressionPanel.dr_next_logic_operator.value > 0 ? OperatorHandler.ConjunctionOperators[logicExpressionPanel.dr_next_logic_operator.value - 1] : "",
            });
        }
        
        base.Apply();
    }
}