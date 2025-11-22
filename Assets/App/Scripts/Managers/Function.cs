using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Arcube;
using UnityEngine;

public class Function
{
    public List<Variable> Variables { get; set; } = new(); 
    public List<Node> Nodes { get; set; } = new();

    public static Action<string> OnInput;
    public static Action<string> OnOutput;
    public static event Action<string> OnError;
    public static Node ActiveNode { get; set; }
    public async Task Execute(CancellationTokenSource cts)
    {
        var backup = Variables.ConvertAll(v => new Variable(v));
        try
        {
            var nodeId = Nodes.Find(x => x is StartNode).NextNode;
            if (string.IsNullOrEmpty(nodeId)) return;
            ActiveNode = Nodes.Find(x => x.ID == nodeId);
            while (ActiveNode is not EndNode)
            {
                //Debug.Log($"Executing {ActiveNode.Name}");

                var command = (Command)ActiveNode;
                await command.Execute(cts);
                await Task.Yield();
                nodeId = command.NextNode;
                if (string.IsNullOrEmpty(nodeId)) break;
                ActiveNode = Nodes.Find(x => x.ID == nodeId);
            }
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.Message);
            Log.AddException(e);
        }
        finally
        {
            foreach (var node in Nodes)
            {
                if (node is Command command) command.Completed = false;
            }
            
            for (var i = 0; i < Variables.Count; i++)
            {
                Variables[i].Value = backup[i].Value;
                Variables[i].Assigned = backup[i].Assigned;
            }
        }
    }
}