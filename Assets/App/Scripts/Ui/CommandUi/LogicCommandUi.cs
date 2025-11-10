using System.Collections.Generic;
using System.Linq;
using Arcube;
using TMPro;
using UnityEngine;

public class LogicCommandUi : CommandUi
{
    [SerializeField] private TMP_Dropdown dr_expression_1;
    [SerializeField] private TMP_Dropdown dr_expression_2;
    [SerializeField] private TMP_Dropdown dr_operator;
    [SerializeField] private FlowInputField ip_expression_1;
    [SerializeField] private FlowInputField ip_expression_2;

    protected override void Reset()
    {
        base.Reset();
        
        transform.TryFindObject(nameof(dr_expression_1), out dr_expression_1);
        transform.TryFindObject(nameof(dr_expression_2), out dr_expression_2);
        transform.TryFindObject(nameof(dr_operator), out dr_operator);
        transform.TryFindObject(nameof(ip_expression_1), out ip_expression_1);
        transform.TryFindObject(nameof(ip_expression_2), out ip_expression_2);
    }

    private List<Variable> _allVariables;

    protected override void Start()
    {
        base.Start();
        
        _allVariables = AppManager.GetManager<FlowChartManager>().ActiveVariables;
        
        
        var variablesNames = _allVariables.Select(v => v.Name).ToList();
        variablesNames.Insert(0, "Select");
        variablesNames.Insert(1, "New");
        dr_expression_1.options = variablesNames.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
        dr_expression_2.options = dr_expression_2.options;
        
        dr_expression_1.onValueChanged.AddListener((value) =>
        {
            ip_expression_1.gameObject.SetActive(value == 0);
        });
        
        dr_expression_2.onValueChanged.AddListener((value) =>
        {
            ip_expression_2.gameObject.SetActive(value == 0);
        });
        
        ip_expression_1.OnDelete += () =>
        {
            dr_expression_1.gameObject.SetActive(true);
        };
        
        ip_expression_2.OnDelete += () =>
        {
            dr_expression_2.gameObject.SetActive(true);
        };
    }

    protected override void Apply()
    {
        var logicCommand = (LogicCommand)Command;

        logicCommand.Expression1 = new Variable()
        {
            Name = ip_expression_1.Text,
            Assigned = true,
            Type = VariableType.String,
            Value = ip_expression_1.Text
        };
        
        logicCommand.Expression2 = new Variable()
        {
            Name = ip_expression_2.Text,
            Assigned = true,
            Type = VariableType.String,
            Value = ip_expression_2.Text
        };
        
        logicCommand.Operator = (Operator)dr_operator.value;
        
        base.Apply();
    }
}
