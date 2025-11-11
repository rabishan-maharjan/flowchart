using System;
using System.Collections.Generic;
using Arcube;
using UnityEngine;

public class Variable
{
    public string ID = Guid.NewGuid().ToString();
    public string Name;
    public VariableType Type;
    public string Value;
    public bool Assigned;
    public bool Exposed = false;

    /// <summary>
    /// Checks if Value can be correctly parsed based on its Type.
    /// Returns true if valid, false otherwise.
    /// </summary>
    public bool VerifyType()
    {
        try
        {
            switch (Type)
            {
                case VariableType.String:
                    return true;
                case VariableType.Boolean:
                    return bool.TryParse(Value, out _);
                case VariableType.Int:
                    return int.TryParse(Value, out _);
                case VariableType.Float:
                    return float.TryParse(Value, out _);
                default:
                    return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Variable '{Name}' has invalid value '{Value}' for type {Type}: {e.Message}");
            return false;
        }
    }
    
    public object GetValue() =>
        Type switch
        {
            VariableType.String => Value,
            VariableType.Boolean => bool.Parse(Value),
            VariableType.Int => int.Parse(Value),
            VariableType.Float => float.Parse(Value),
            _ => throw new Exception("Invalid variable type")
        };
}

public class Function
{
    public List<Variable> Variables { get; set; } = new(); 
    public List<Node> Nodes { get; set; } = new();

    public static event Action<string> OnInput;
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