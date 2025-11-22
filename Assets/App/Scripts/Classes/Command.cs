using System;
using System.Threading;
using System.Threading.Tasks;
using Arcube;
using Newtonsoft.Json;
using UnityEngine;

public abstract class Command : Node
{
    [JsonIgnore] public Action OnExecuteStart;
    [JsonIgnore] public Action OnExecuteEnd;
    public bool Completed { get; set; } = false;
    public abstract Task<bool> Execute(CancellationTokenSource cts);

    [JsonIgnore] protected string Description;
    public abstract string GetDescription();
    
    public abstract string GetValueDescription();

    protected async Task Wait(CancellationTokenSource cts)
    {
        var executionType = AppManager.GetManager<FlowChartManager>().ExecutionType;
        switch (executionType)
        {
            case ExecutionType.Normal:
                return;
            case ExecutionType.TwoSeconds:
                await Task.Delay(2000, cts.Token);
                break;
            case ExecutionType.KeyPress:
                while (!Input.GetKeyDown(KeyCode.N))
                {
                    await Task.Yield();
                    cts.Token.ThrowIfCancellationRequested();
                }
                break;
        }
    }
}