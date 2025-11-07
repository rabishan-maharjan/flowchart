using System.Threading.Tasks;
using Arcube;
using Arcube.UiManagement;
using TMPro;
using UnityEngine;

public class VariableItemPanel : PanelItem
{
    [SerializeField] private TMP_Text t_type;
    [SerializeField] private TMP_Text t_name;
    [SerializeField] private TMP_Text t_value;
    protected override void Reset()
    {
        transform.TryFindObject(nameof(t_type), out t_type);
        transform.TryFindObject(nameof(t_name), out t_name);
        transform.TryFindObject(nameof(t_value), out t_value);
        
        base.Reset();
    }

    private Variable _variable;
    private void Start()
    {
        gameObject.FindObject<ButtonImage>("b_edit").OnClick.AddListener(() =>
        {
            _ = EditVariable();
        });
        
        gameObject.FindObject<ButtonImage>("b_remove").OnClick.AddListener(() =>
        {
            AppManager.GetManager<FlowChartManager>().RemoveVariable(_variable);
            Destroy(gameObject);
        });
    }

    private async Task EditVariable()
    {
        await UiManager.GetUi<VariableUi>().Open(_variable);
        SetUi();
    }

    public override void Set<T>(T item)
    {
        _variable = item as Variable;
        if(_variable == null) return;

        SetUi();

        base.Set(item);
    }

    private void SetUi()
    {
        t_type.text = _variable.Type;
        t_name.text = _variable.Name;
        t_value.text = _variable.Value;
    }
}