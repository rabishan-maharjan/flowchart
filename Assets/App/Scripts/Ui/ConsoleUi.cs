using Arcube;
using Arcube.UiManagement;
using TMPro;
using UnityEngine;

public class ConsoleUi : Ui
{
    [SerializeField] private ChatItemPanel compilerItem;
    [SerializeField] private ChatItemPanel userItem;
    [SerializeField] private TMP_InputField ip_input;
    [SerializeField] private ListContainer listContainer;
    protected override void Reset()
    {
        closable = true;
        deactivateOnClose = true;
        
        transform.TryFindObject(nameof(ip_input), out ip_input);
        listContainer = GetComponentInChildren<ListContainer>();
        
        base.Reset();
    }

    private Variable _activeVariable;
    private void Start()
    {
        Function.OnOutput += value =>
        {
            AddText(value, false);
        };
        
        Function.OnInput += value =>
        {
            _activeVariable = AppManager.GetManager<FlowChartManager>().VariableMap[value];
            ip_input.interactable = true;
        };

        Function.OnError += AddError;
        
        gameObject.FindObject<ButtonImage>("b_close").OnClick.AddListener(Close);
        
        ip_input.onSubmit.AddListener((value) =>
        {
            AddText(value, true);
            _activeVariable.Value = value;
            ((Command)Function.ActiveNode).Completed = true;
            ip_input.interactable = false;
        });
    }

    protected override void SetUi()
    {
        foreach (var panel in GetComponentsInChildren<ChatItemPanel>())
        {
            Destroy(panel.gameObject);
        }
        
        base.SetUi();
    }

    private void AddText(string text, bool isUser)
    {
        var selected = isUser ? userItem : compilerItem;
        var item =  Instantiate(selected, listContainer.transform);
        item.SetText(text, Color.black);
    }
    
    private void AddError(string text)
    {
        var item =  Instantiate(compilerItem, listContainer.transform);
        item.SetText(text, Color.red);
    }
}