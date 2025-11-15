using System;

public class Variable
{
    public string ID = Guid.NewGuid().ToString();
    public string Name;
    public VariableType Type;
    public string Value;
    public bool Assigned;
    public bool Exposed = false;
    
    public Variable() { }
    
    public Variable(Variable v)
    {
        ID = v.ID;
        Name = v.Name;
        Type = v.Type;
        Value = v.Value;
        Assigned = v.Assigned;
        Exposed = v.Exposed;
    }

    /// <summary>
    /// Checks if Value can be correctly parsed based on its Type.
    /// Returns true if valid, false otherwise.
    /// </summary>
    public bool IsValid() =>
        Type switch
        {
            VariableType.String => true,
            VariableType.Bool => bool.TryParse(Value, out _),
            VariableType.Number => float.TryParse(Value, out _),
            _ => false
        };

    public object GetValue() =>
        Type switch
        {
            VariableType.String => Value,
            VariableType.Bool => bool.Parse(Value),
            VariableType.Number => float.Parse(Value),
            _ => throw new Exception("Invalid variable type")
        };

    public static VariableType DetectType(string value)
    {
        if (bool.TryParse(value, out _)) return VariableType.Bool;
        if (float.TryParse(value, out _)) return VariableType.Number;
        return VariableType.String;
    }
}