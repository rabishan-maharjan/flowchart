using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Arcube;
using Arcube.UiManagement;
using TMPro;
using UnityEngine;

public class MenuPanel : MonoBehaviour
{
    private FlowChartManager _flowChartManager;
    private const string Root = "Projects";
    private void Start()
    {
        _flowChartManager = AppManager.GetManager<FlowChartManager>();

        var drExecutionType = gameObject.FindObject<TMP_Dropdown>("dr_execution_type");
        drExecutionType.options = Enum.GetNames(typeof(ExecutionType)).Select(x => new TMP_Dropdown.OptionData(x)).ToList();
        drExecutionType.onValueChanged.AddListener((value) =>
        {
            _flowChartManager.ExecutionType = (ExecutionType)value;
        });
        
        gameObject.FindObject<ButtonImage>("b_run").OnClick.AddListener(Compile);
        
        gameObject.FindObject<ButtonImage>("b_new").OnClick.AddListener(_flowChartManager.New);
        
        gameObject.FindObject<ButtonImage>("b_open").OnClick.AddListener(Load);
        
        gameObject.FindObject<ButtonImage>("b_save").OnClick.AddListener(Save);
        
        gameObject.FindObject<ButtonImage>("b_save_as").OnClick.AddListener(SaveAs);
    }

    private async void Compile()
    {
        try
        {
            await Task.Yield();
            _flowChartManager.Compile();
            await Task.Yield();
            _ = UiManager.GetUi<ConsoleUi>().OpenAsync();
            await Task.Yield();
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
            var fileName = _flowChartManager.CurrentFile;
            if (string.IsNullOrEmpty(_flowChartManager.CurrentFile))
            {
                SaveAs();
                return;
            }
            
            _flowChartManager.Compile();
            _flowChartManager.Save(fileName);
            _flowChartManager.Work();
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
            var fileName = await FileBrowser.Instance.OpenSave(path, FlowChartManager.Ext);
            if(string.IsNullOrEmpty(fileName)) return;
            
            _flowChartManager.Compile();
            _flowChartManager.Save(fileName);
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
            var fileName = await FileBrowser.Instance.OpenLoad(path, FlowChartManager.Ext);
            if(string.IsNullOrEmpty(fileName)) return;
            
            _flowChartManager.Load(fileName);
        }
        catch(Exception e)
        {
            Log.AddException(e);
        }
    }
}