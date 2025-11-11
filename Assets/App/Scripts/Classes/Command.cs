using System.Threading.Tasks;

public abstract class Command : Node
{
    public bool Completed { get; set; } = false;
    public virtual async Task Execute()
    {
        while (!Completed) await Task.Yield();
    }

    public abstract string GetDescription();
}