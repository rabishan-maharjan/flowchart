using System;
using System.Threading.Tasks;
using Arcube;
using Arcube.UiManagement;
using UnityEngine;

public class MenuPanel : MonoBehaviour
{
    private FlowChartManager _flowChartManager;
    private IOManager _ioManager;
    private void Start()
    {
        _flowChartManager = AppManager.GetManager<FlowChartManager>();
        _ioManager = AppManager.GetManager<IOManager>();
        
        gameObject.FindObject<ButtonImage>("b_run").OnClick.AddListener(Compile);
        
        gameObject.FindObject<ButtonImage>("b_new").OnClick.AddListener(() =>
        {
            _flowChartManager.New();
            UiManager.GetUi<AppUi>().variableListPanel.Clear();
        });
        
        gameObject.FindObject<ButtonImage>("b_open").OnClick.AddListener(Load);
        
        gameObject.FindObject<ButtonImage>("b_save").OnClick.AddListener(() =>
        {
            //show filename save ui
            var fileName = "test.flw";
            UiManager.GetUi<GraphPanelUi>().GenerateCode();
            _ioManager.Save(fileName, _flowChartManager.Functions);
        });
        
        gameObject.FindObject<ButtonImage>("b_save_as").OnClick.AddListener(() =>
        {
            //show filename save ui
            var fileName = "test.flw";
            UiManager.GetUi<GraphPanelUi>().GenerateCode();
            _ioManager.Save(fileName, _flowChartManager.Functions);
        });
    }

    private async void Compile()
    {
        try
        {
            _ = UiManager.GetUi<ConsoleUi>().OpenAsync();
            await Task.Yield();
            UiManager.GetUi<GraphPanelUi>().GenerateCode();
            _flowChartManager.Run();
        }
        catch (Exception e)
        {
            Log.AddException(e);
        }
    }

    private async void Load()
    {
        try
        {
            //show filename load ui
            var fileName = "test.flw";
            var code = _flowChartManager.Functions = _ioManager.Load(fileName);
            foreach (var function in code)
            {
                foreach (var variable in function.Value.Variables)
                {
                    _flowChartManager.AddVariable(variable);
                    UiManager.GetUi<AppUi>().variableListPanel.CreateVariable(variable);
                }
            }
            
            await UiManager.GetUi<GraphPanelUi>().GenerateFlowChart(code);
        }
        catch(Exception e)
        {
            Log.AddException(e);
        }
    }
}