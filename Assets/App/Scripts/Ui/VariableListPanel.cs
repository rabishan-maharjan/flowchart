using System;
using Arcube;
using Arcube.UiManagement;
using UnityEngine;

public class VariableListPanel : MonoBehaviour
{
    private ListContainer _listContainer;
    private void Start()
    {
        _listContainer = GetComponentInChildren<ListContainer>();
        gameObject.FindObject<ButtonImage>("b_add_variable").OnClick.AddListener(AddVariable);
    }

    private async void AddVariable()
    {
        try
        {
            var variable = await UiManager.GetUi<VariableUi>().Open(new Variable());
            if (!variable.Assigned)
            {
                Debug.Log("Assign value");
                return;
            }

            var panel = _listContainer.CreateItem<VariableItemPanel>();
            panel.Set(variable);
            var siblingIndex = panel.transform.GetSiblingIndex();
            panel.transform.SetSiblingIndex(siblingIndex - 1);
            
            AppManager.GetManager<FlowChartManager>().AddVariable(variable);
        }
        catch (Exception e)
        {
            Log.AddException(e);
        }
    }
}