using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arcube;
using Arcube.UiManagement;
using TMPro;
using UnityEngine;

public class LoopCommandUi : CommandUi
{
    [SerializeField] private TMP_Dropdown dr_variable;
    [SerializeField] private InputFieldSimple ip_count;
    [SerializeField] private ToggleButton tb_reverse;
    [SerializeField] private InputFieldSimple ip_step;
    protected override void Reset()
    {
        transform.TryFindObject(nameof(dr_variable), out dr_variable);
        transform.TryFindObject(nameof(ip_count), out ip_count);
        transform.TryFindObject(nameof(tb_reverse), out tb_reverse);
        transform.TryFindObject(nameof(ip_step), out ip_step);
        
        base.Reset();
    }

    private List<Variable> _exposedVariables;
    public override Task Open(Command command)
    {
        var loopCommand = (LoopCommand)command;
        ip_count.Text = loopCommand.Count.ToString();
        tb_reverse.IsOn = loopCommand.Reverse;
        ip_step.Text = loopCommand.Steps.ToString();

        var variables = AppManager.GetManager<FlowChartManager>().ActiveVariables;
        _exposedVariables = variables.Where(v => v.Exposed && v.Type == VariableType.Number).ToList();
        var names = _exposedVariables.Select(v => v.Name).ToList();
        names.Insert(0, "Auto");
        dr_variable.options = names.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
        var selected = Variable.TryGetVariable(loopCommand.Variable);
        dr_variable.value = selected != null ? _exposedVariables.IndexOf(selected) + 1 : 0;
        
        return base.Open(command);
    }

    protected override void Apply()
    {
        var loopCommand = (LoopCommand)Command;
        
        int.TryParse(ip_count.Text, out loopCommand.Count);
        int.TryParse(ip_step.Text, out loopCommand.Steps);
        loopCommand.Reverse = tb_reverse.IsOn;
        if (dr_variable.value > 0)
        {
            var v = _exposedVariables[dr_variable.value - 1];
            loopCommand.Variable = v.ID;
        }
        else  loopCommand.Variable = "";
        
        
        base.Apply();
    }
}