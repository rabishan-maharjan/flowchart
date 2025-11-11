using System.Collections.Generic;
using Arcube;
using Arcube.UiManagement;
using UnityEngine;

public class OutputCommandUi : CommandUi
{
    [SerializeField] private DropDownWithInputField dr_prefab;
    [SerializeField] private ButtonImage b_add;
    protected override void Reset()
    {
        transform.TryFindObject(nameof(b_add), out b_add);
        transform.TryFindObject(nameof(dr_prefab), out dr_prefab);

        base.Reset();
    }

    private List<Variable> _allVariables;
    protected override void Start()
    {
        b_add.OnClick.AddListener(() =>
        {
            var variableSelector = Instantiate(dr_prefab, dr_prefab.transform.parent);
            variableSelector.Set(_allVariables);
            
            b_add.transform.SetAsLastSibling();
        });
        
        base.Start();
    }

    protected override void SetUi()
    {
        _allVariables = AppManager.GetManager<FlowChartManager>().ActiveVariables;
        if (_allVariables.Count == 0)
        {
            //in future just show text field
        }
        
        foreach (var variableSelector in GetComponentsInChildren<DropDownWithInputField>())
        {
            Destroy(variableSelector.gameObject);
        }
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

        Close();
    }
}