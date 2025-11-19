using System.Threading.Tasks;
using Arcube;
using Arcube.UiManagement;
using TMPro;
using UnityEngine;

public class AppUi : Ui
{
    public VariableListPanel variableListPanel;
    [SerializeField] private TMP_Text t_title;
    protected override void Reset()
    {
        variableListPanel = GetComponentInChildren<VariableListPanel>();
        transform.TryFindObject(nameof(t_title), out t_title);
        
        base.Reset();
    }

    public override Task Register()
    {
        var flowChartManager =AppManager.GetManager<FlowChartManager>(); 
        flowChartManager.OnProjectStateChanged += (state, projectName) =>
        {
            switch (state)
            {
                case AppState.New:
                    variableListPanel.Clear();
                    SetTitle("New Project");
                    break;
                case AppState.Save:
                    SetTitle(projectName);
                    break;
                case AppState.Load:
                    variableListPanel.Clear();
                    foreach (var variable in flowChartManager.VariableMap)
                    {
                        variableListPanel.CreateVariable(variable.Value);
                    }
                    SetTitle(projectName);
                    break;
            }
        };
        
        return base.Register();
    }

    private void SetTitle(string title)
    {
        t_title.text = title;
    }
}