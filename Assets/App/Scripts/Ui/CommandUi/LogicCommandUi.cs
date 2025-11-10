using System.Collections.Generic;
using System.Linq;
using Arcube;
using TMPro;
using UnityEngine;

public class LogicCommandUi : CommandUi
{
    [SerializeField] private TMP_Dropdown dr_expression_1;
    [SerializeField] private TMP_Dropdown dr_expression_2;
    [SerializeField] private FlowInputField ip_logic;
    [SerializeField] private FlowInputField ip_expression_2;

    protected override void Reset()
    {
        base.Reset();
        
        transform.TryFindObject(nameof(dr_expression_1), out dr_expression_1);
        transform.TryFindObject(nameof(dr_expression_2), out dr_expression_2);
        transform.TryFindObject(nameof(ip_logic), out ip_logic);
        transform.TryFindObject(nameof(ip_expression_2), out ip_expression_2);
    }

    private List<Variable> _allVariables;
    protected override void Start()
    {
        base.Start();
        
        dr_expression_2.onValueChanged.AddListener((value) =>
        {
            ip_expression_2.gameObject.SetActive(value == 0);
        });
        
        ip_expression_2.OnDelete += () =>
        {
            dr_expression_2.gameObject.SetActive(true);
        };
    }

    protected override void SetUi()
    {
        _allVariables = AppManager.GetManager<FlowChartManager>().ActiveVariables;
        
        var variablesNames = _allVariables.Select(v => v.Name).ToList();
        dr_expression_1.options = variablesNames.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
        
        variablesNames.Insert(0, "Select");
        variablesNames.Insert(1, "New");
        dr_expression_2.options = variablesNames.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
    }

    protected override void Apply()
    {
        if (_allVariables.Count == 0)
        {
            Debug.Log("Not enough variables");
            return;
        }

        var logicCommand = (LogicCommand)Command;
        logicCommand.Expression1 = _allVariables[dr_expression_1.value];

        if (dr_expression_2.gameObject.activeSelf)
        {
            logicCommand.Expression2 = _allVariables[dr_expression_2.value];
        }
        else
        {
            //validate
            logicCommand.Expression2 = new Variable
            {
                Name = ip_expression_2.Text,
                Assigned = true,
                Type = logicCommand.Expression1.Type,
                Value = ip_expression_2.Text
            };
        }

        logicCommand.Operator = ip_logic.Text;
        
        base.Apply();
    }
}
