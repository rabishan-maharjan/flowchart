using System;
using System.IO;
using System.Threading.Tasks;
using Arcube;
using Arcube.UiManagement;
using UnityEngine;

public class MenuPanel : MonoBehaviour
{
    private FlowChartManager _flowChartManager;
    private IOManager _ioManager;
    private const string Root = "Projects";
    private const string Ext = "flw";
    private AppUi _appUi;
    private void Start()
    {
        _appUi = UiManager.GetUi<AppUi>();
        _flowChartManager = AppManager.GetManager<FlowChartManager>();
        _ioManager = AppManager.GetManager<IOManager>();
        
        gameObject.FindObject<ButtonImage>("b_run").OnClick.AddListener(Compile);
        
        gameObject.FindObject<ButtonImage>("b_new").OnClick.AddListener(() =>
        {
            _flowChartManager.New();
            _ioManager.New();
            _appUi.SetTitle("New Project");
            _appUi.variableListPanel.Clear();
        });
        
        gameObject.FindObject<ButtonImage>("b_open").OnClick.AddListener(Load);
        
        gameObject.FindObject<ButtonImage>("b_save").OnClick.AddListener(Save);
        
        gameObject.FindObject<ButtonImage>("b_save_as").OnClick.AddListener(SaveAs);
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
    
    
    private void Save()
    {
        try
        {
            var fileName = _ioManager.CurrentFile;
            if (string.IsNullOrEmpty(_ioManager.CurrentFile))
            {
                SaveAs();
                return;
            }
            
            UiManager.GetUi<GraphPanelUi>().GenerateCode();
            _appUi.SetTitle(Path.GetFileNameWithoutExtension(fileName));
            _ioManager.Save(fileName, _flowChartManager.Functions);
        }
        catch (Exception e)
        {
            Log.AddException(e);
        }
    }
    
    private async void SaveAs()
    {
        try
        {
            var path = Path.Combine(Application.persistentDataPath, Root);
            var fileName = await FileBrowser.Instance.OpenSave(path, Ext);
            if(string.IsNullOrEmpty(fileName)) return;
            
            UiManager.GetUi<GraphPanelUi>().GenerateCode();
            _appUi.SetTitle(Path.GetFileNameWithoutExtension(fileName));
            _ioManager.Save($"{fileName}.{Ext}", _flowChartManager.Functions);
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
            var path = Path.Combine(Application.persistentDataPath, Root);
            var fileName = await FileBrowser.Instance.OpenLoad(path, Ext);
            if(string.IsNullOrEmpty(fileName)) return;
            
            _appUi.SetTitle(Path.GetFileNameWithoutExtension(fileName));
            UiManager.GetUi<AppUi>().variableListPanel.Clear();
            var code = _flowChartManager.Functions = _ioManager.Load($"{fileName}.{Ext}");
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