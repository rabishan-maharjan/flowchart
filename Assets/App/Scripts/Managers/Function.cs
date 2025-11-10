using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arcube;
using UnityEngine;

public class Variable
{
    public string Name;
    public VariableType Type;
    public string Value;
    public bool Assigned;
}

public class Function
{
    public List<Variable> Variables { get; set; } = new(); 
    public List<Node> Nodes { get; set; } = new();

    public static event Action<Variable> OnInput;
    public static Action<string> OnOutput;
    public static event Action<string> OnError;
    public static Node ActiveNode { get; private set; }
    public async void Execute()
    {
        try
        {
            var nodeId = Nodes.Find(x => x is StartNode).NextNode;
            if(string.IsNullOrEmpty(nodeId)) return;
            var node = Nodes.Find(x => x.ID == nodeId);
            while(node is not EndNode)
            {
                Debug.Log($"Executing {node.Name}");
                
                var command = (Command)node;
                if (command is InputCommand inputCommand) {OnInput?.Invoke(inputCommand.Variable);}
                await command.Execute();
                nodeId = command.NextNode;
                if(string.IsNullOrEmpty(nodeId)) break; 
                node = Nodes.Find(x => x.ID == nodeId);
            }
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.Message);
            Log.AddException(e);
        }
    }
}