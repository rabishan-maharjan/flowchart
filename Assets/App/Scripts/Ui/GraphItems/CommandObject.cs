using System.Collections;
using System.Threading.Tasks;
using Arcube;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class CommandObject : NodeObject
{
    [SerializeField] private TMP_Text t_work;
    [FormerlySerializedAs("t_command")] [SerializeField] private TMP_Text t_compile;
    protected override void Reset()
    {
        transform.TryFindObject(nameof(t_work), out t_work);
        transform.TryFindObject(nameof(t_compile), out t_compile);
        
        base.Reset();
    }

    private Command Command => (Command)Node;
    
    private void OnEnable() => FlowChartManager.OnCompileStateChanged += HandleCompileStateChanged;
    private void OnDisable() => FlowChartManager.OnCompileStateChanged -= HandleCompileStateChanged;

    private void HandleCompileStateChanged(CompileState state)
    {
        Editable = state == CompileState.Work;
        
        if(state == CompileState.Run) t_compile.text = Command.GetDescription();
        
        t_work.gameObject.SetActive(state != CompileState.Run);
        t_compile.gameObject.SetActive(state == CompileState.Run);
    }

    protected override IEnumerator Start()
    {
        yield return base.Start();
        
        Command.OnExecuteStart += HandleExecutionStart;
        Command.OnExecuteEnd += HandleExecutionEnd;
    }

    private void HandleExecutionEnd()
    {
        t_compile.text = Command.GetValueDescription();
        t_compile.gameObject.SetActive(true);
        Highlight(false);
    }

    private void HandleExecutionStart()
    {
        Highlight(true);
    }

    private float _lastClickTime;
    private const float DoubleClickTime = 0.3f;
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        
        if (Time.time - _lastClickTime < DoubleClickTime)
        {
            GraphPanelUi.Selected = null;
            _ = OpenCommandUi();
            _lastClickTime = 0;
        }
        else
        {
            _lastClickTime = Time.time;
        }
    }

    protected virtual async Task OpenCommandUi()
    {
        if(!Editable) return;
        
        await Task.Yield();
        Text = Command.GetDescription();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }

    private Tween _tween;
    private void Highlight(bool highlight)
    {
        if(highlight) _tween = Image.DOColor(GraphSettings.Instance.highlightColor, 0.5f).From(GraphSettings.Instance.colors[Node.Name]).SetLoops(-1, LoopType.Yoyo);
        else
        {
            _tween?.Kill();
            Image.color = GraphSettings.Instance.colors[Node.Name];
        }
    }
}