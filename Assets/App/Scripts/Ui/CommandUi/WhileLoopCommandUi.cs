using System;
using System.Collections.Generic;
using System.Linq;
using Arcube;
using Arcube.UiManagement;
using UnityEngine;
using UnityEngine.UI;

public class WhileLoopCommandUi : CommandUi
{
    [SerializeField] private LogicExpressionPanel logicExpressionPrefab;
    [SerializeField] private Transform list;

    private FlowChartManager _flowChartManager;
    private List<Variable> _allVariables;
    private List<Variable> _exposedVariables;
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
            var loopCommand = (WhileLoopCommand)Command;
            foreach (var expression in loopCommand.Expressions)
            {
                AddField(expression);
            }

            if (loopCommand.Expressions.Count == 0) AddField(new LogicExpression());

            base.SetUi();
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
        
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)list.transform);
    }

    protected override void Apply()
    {
        if (_allVariables.Count == 0)
        {
            Debug.Log("Not enough variables");
            return;
        }

        var loopCommand = (WhileLoopCommand)Command;
        loopCommand.Expressions.Clear();

        for (var i = 0; i < list.childCount; i++)
        {
            var logicExpressionPanel = list.GetChild(i).GetComponent<LogicExpressionPanel>();
            if (!logicExpressionPanel) continue;
            
            var v1 = Variable.TryGetVariable(_exposedVariables[logicExpressionPanel.dr_variable_1.value].ID);
            var v2 = logicExpressionPanel.dr_variable_2.Value;

            if (v1 == null || v2 == null)
            {
                MessageUi.Show("Invalid variable");
                return;
            }

            var type1 = Variable.DetectType(v1.Value);
            var type2 = Variable.DetectType(v2.Value);
            
            v1.Type = type1;
            v2.Type = type2;
            
            if (v1.Type != v2.Type)
            {
                MessageUi.Show("Variable type mismatch");
                return;
            }
            
            loopCommand.Expressions.Add(new LogicExpression()
            {
                Variable1 = v1.ID,
                Variable2 = v2.ID,
                Operator = OperatorHandler.LogicOperators[logicExpressionPanel.dr_operator.value],
                ConjunctionOperator = logicExpressionPanel.dr_next_logic_operator.value > 0 ? OperatorHandler.ConjunctionOperators[logicExpressionPanel.dr_next_logic_operator.value - 1] : "",
            });
        }
        
        base.Apply();
    }
}