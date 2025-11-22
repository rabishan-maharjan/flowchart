using System.Collections.Generic;
using System.Linq;
using Arcube;
using UnityEngine;
using UnityEngine.UI;

public class OutputCommandUi : CommandUi
{
    [SerializeField] private DropDownWithInputField dr_prefab;
    protected override void Reset()
    {
        transform.TryFindObject(nameof(dr_prefab), out dr_prefab);

        base.Reset();
    }

    private List<Variable> _allVariables;
    private List<Variable> _exposedVariables;
    [SerializeField] private Transform list;
    protected override void SetUi()
    {
        foreach (var variableSelector in GetComponentsInChildren<DropDownWithInputField>())
        {
            Destroy(variableSelector.gameObject);
        }

        var flowChartManager = AppManager.GetManager<FlowChartManager>(); 
        _allVariables = flowChartManager.ActiveVariables;
        _exposedVariables = _allVariables.Where(v => v.Exposed || v.BranchID == Command.BranchID).ToList();
        
        //load old variables
        var outputCommand = (OutputCommand)Command;
        foreach (var variable in outputCommand.Variables)
        {
            var v = flowChartManager.VariableMap[variable];
            AddNewField(v);
        }
        
        AddNewField(new Variable());
    }
    
    private void AddNewField(Variable variable)
    {
        var variableSelector = Instantiate(dr_prefab, list);
        variableSelector.Set(_exposedVariables, variable);

        void HandleValueSelected(string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            var index = variableSelector.transform.GetSiblingIndex();
            if (index == variableSelector.transform.parent.childCount - 1)
            {
                AddNewField(new Variable());
            }
        }

        variableSelector.onValueChanged.AddListener(HandleValueSelected);
            
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)list.transform);
    }

    protected override void Apply()
    {
        //update graph
        var outputCommand = (OutputCommand)Command;
        outputCommand.Variables.Clear();
        foreach (var variableSelector in GetComponentsInChildren<DropDownWithInputField>())
        {
            var variable = variableSelector.Value;
            if (variable == null) continue;
            var id = variableSelector.Value.ID;
            outputCommand.Variables.Add(id);
        }

        base.Apply();
    }
}