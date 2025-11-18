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
    
    public void SetTitle(string title)
    {
        t_title.text = title;
    }
}