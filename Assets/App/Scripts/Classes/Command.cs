using System.Threading;
using System.Threading.Tasks;
using Arcube;
using UnityEngine;

public abstract class Command : Node
{
    public bool Completed { get; set; } = false;
    public abstract Task Execute(CancellationTokenSource cts);

    public abstract string GetDescription();
    
    public abstract string GetValue();

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
                while (!Input.anyKeyDown)
                {
                    await Task.Yield();
                    cts.Token.ThrowIfCancellationRequested();
                }
                break;
        }
    }
}