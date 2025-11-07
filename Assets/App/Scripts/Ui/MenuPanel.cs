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
        
        gameObject.FindObject<ButtonImage>("b_new").OnClick.AddListener(() =>
        {
            _flowChartManager.New();
        });
        
        gameObject.FindObject<ButtonImage>("b_save").OnClick.AddListener(() =>
        {
            //show filename save ui
            var fileName = "test.flw";
            UiManager.GetUi<GraphPanelUi>().GenerateCode();
            _ioManager.Save(fileName, _flowChartManager.Functions);
        });
        
        gameObject.FindObject<ButtonImage>("b_open").OnClick.AddListener(Load);
        
        gameObject.FindObject<ButtonImage>("b_run").OnClick.AddListener(async () =>
        {
            _ = UiManager.GetUi<ConsoleUi>().OpenAsync();
            await Task.Yield();
            UiManager.GetUi<GraphPanelUi>().GenerateCode();
            _flowChartManager.Compile();
        });
    }

    private async void Load()
    {
        try
        {
            //show filename load ui
            var fileName = "test.flw";
            var code = _flowChartManager.Functions = _ioManager.Load(fileName);
            await UiManager.GetUi<GraphPanelUi>().Decompile(code);
        }
        catch(Exception e)
        {
            Log.AddException(e);
        }
    }
}