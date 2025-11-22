using System;
using System.Collections.Generic;
using System.Linq;
using Arcube;
using Arcube.UiManagement;
using UnityEngine;

public class ForLoopCommandUi : CommandUi
{
    [SerializeField] private InputFieldSimple ip_name;
    [SerializeField] private DropDownWithInputField dr_ip_start;
    [SerializeField] private DropDownWithInputField dr_ip_end;
    [SerializeField] private DropDownWithInputField dr_ip_step;
    protected override void Reset()
    {
        transform.TryFindObject(nameof(ip_name), out ip_name);
        transform.TryFindObject(nameof(dr_ip_start), out dr_ip_start);
        transform.TryFindObject(nameof(dr_ip_end), out dr_ip_end);
        transform.TryFindObject(nameof(dr_ip_step), out dr_ip_step);
        
        base.Reset();
    }
    
    private List<Variable> _exposedVariables;
    protected override void SetUi()
    {
        try
        {
            var loopCommand = (ForLoopCommand)Command;
            var nameVar = Variable.TryGetVariable(loopCommand.Variable);
            ip_name.Text = nameVar != null ? nameVar.Name : "i";

            var variables = AppManager.GetManager<FlowChartManager>().GetExposedAndLocalVariables(Command.BranchID);
            _exposedVariables = variables.Where(v => v.Type == VariableType.Number).ToList();
            
            var v = Variable.TryGetVariable(loopCommand.Start) ?? new Variable();
            dr_ip_start.Set(_exposedVariables, v);
        
            v = Variable.TryGetVariable(loopCommand.End) ?? new Variable();
            dr_ip_end.Set(_exposedVariables, v);
        
            v = Variable.TryGetVariable(loopCommand.Steps) ?? new Variable {Value = "1"};
            dr_ip_step.Set(_exposedVariables, v);
        }
        catch (Exception e)
        {
            Log.AddException(e);
        }
    }

    protected override void Apply()
    {
        try
        {
            if (string.IsNullOrEmpty(ip_name.Text) || string.IsNullOrEmpty(dr_ip_start.Text) || string.IsNullOrEmpty(dr_ip_end.Text) || string.IsNullOrEmpty(dr_ip_step.Text))
            {
                MessageUi.Show("Incomplete parameters");
                return;
            }
            
            var loopCommand = (ForLoopCommand)Command;
            
            var variables = AppManager.GetManager<FlowChartManager>().ActiveVariables;
            if (string.IsNullOrEmpty(loopCommand.Variable))
            {
                var oldI = variables.FirstOrDefault(v => v.Name == ip_name.Text);
                if (oldI != null)
                {
                    MessageUi.Show($"Variable with name {oldI.Name} already exists");
                    return;
                }
                
                var i = new Variable
                {
                    Name = ip_name.Text,
                    BranchID = loopCommand.ID,
                    Value = "0",
                    Type = VariableType.Number,
                    Scope = VariableScope.Local
                };
                loopCommand.Variable = i.ID;
                loopCommand.AddLocalVariable(i);
            }
            else
            {
                var oldI = variables.FirstOrDefault(v => v.Name == ip_name.Text && v.BranchID != loopCommand.ID);
                if (oldI != null)
                {
                    MessageUi.Show($"Variable with name {oldI.Name} already exists");
                    return;
                }
                
                var v = Variable.TryGetVariable(loopCommand.Variable);
                v.Name = ip_name.Text;
            }

            var start = dr_ip_start.Value;
            start.Type = VariableType.Number;
            var end = dr_ip_end.Value;
            end.Type = VariableType.Number;
            var steps = dr_ip_step.Value;
            steps.Type = VariableType.Number;
            
            loopCommand.Start = start.ID; 
            loopCommand.End = end.ID;
            loopCommand.Steps = steps.ID;

            base.Apply();
        }
        catch (Exception e)
        {
            Log.AddException(e);
        }
    }
}