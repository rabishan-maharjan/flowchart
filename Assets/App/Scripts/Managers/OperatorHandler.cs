using System;
using System.Globalization;
using System.Linq;

public static class OperatorHandler
{
    public static readonly string[] ArithmeticOperators =
    {
        "+", "-", "*", "/", "%"
    };
    
    public static readonly string[] StringOperators =
    {
        "+"
    };

    public static readonly string[] LogicOperators =
    {
        "==", "!=", "<", "<=", ">", ">="
    };
    
    public static readonly string[] ConjunctionOperators =
    {
        "or", "and"
    };

    public static bool Verify(string operatorName)
        => LogicOperators.Contains(operatorName) || ArithmeticOperators.Contains(operatorName);

    public static Variable OperateArithmetic(Variable v1, Variable v2, string operatorName)
    {
        if (v1.Type != v2.Type)
        {
            throw new Exception($"Variables must be of the same type to perform arithmetic operation");
        }
        
        // ✅ String concatenation case
        if (v1.Type == VariableType.String || v2.Type == VariableType.String)
        {
            if (operatorName == "+")
            {
                return new Variable
                {
                    Type = VariableType.String,
                    Value = v1.Value + v2.Value,
                    Assigned = true
                };
            }

            throw new Exception($"Operator '{operatorName}' is not valid for strings.");
        }

        // ✅ Numeric arithmetic case
        var val1 = Convert.ToSingle(v1.GetValue());
        var val2 = Convert.ToSingle(v2.GetValue());
        float result;
        switch (operatorName)
        {
            case "+": result = val1 + val2; break;
            case "-": result = val1 - val2; break;
            case "*": result = val1 * val2; break;
            case "/":
                if (val2 == 0) throw new DivideByZeroException();
                result = val1 / val2;
                break;
            case "%":
                if (val2 == 0) throw new DivideByZeroException();
                result = val1 % val2;
                break;
            default:
                throw new Exception($"Unknown arithmetic operator: {operatorName}");
        }

        return new Variable
        {
            Type = VariableType.Number,
            Value = result.ToString(CultureInfo.InvariantCulture),
            Assigned = true
        };
    }


    public static bool OperateLogic(Variable v1, Variable v2, string operatorName)
    {
        // Convert both to float if numeric
        var v1IsNumber = v1.Type == VariableType.Number;
        var v2IsNumber = v2.Type == VariableType.Number;

        if (v1IsNumber && v2IsNumber)
        {
            var val1 = Convert.ToSingle(v1.GetValue());
            var val2 = Convert.ToSingle(v2.GetValue());
            return operatorName switch
            {
                ">" => val1 > val2,
                "<" => val1 < val2,
                ">=" => val1 >= val2,
                "<=" => val1 <= val2,
                "==" => val1 == val2,
                "!=" => val1 != val2,
                _ => throw new Exception($"Unknown logic operator: {operatorName}")
            };
        }

        // Handle booleans
        if (v1.Type == VariableType.Bool && v2.Type == VariableType.Bool)
        {
            var b1 = (bool)v1.GetValue();
            var b2 = (bool)v2.GetValue();
            return operatorName switch
            {
                "==" => b1 == b2,
                "!=" => b1 != b2,
                _ => throw new Exception($"Invalid boolean operator {operatorName}")
            };
        }

        // Handle strings
        if (v1.Type == VariableType.String && v2.Type == VariableType.String)
        {
            var s1 = (string)v1.GetValue();
            var s2 = (string)v2.GetValue();
            return operatorName switch
            {
                "==" => s1 == s2,
                "!=" => s1 != s2,
                _ => throw new Exception($"Invalid string operator {operatorName}")
            };
        }

        throw new Exception($"Unsupported comparison between {v1.Type} and {v2.Type}");
    }
}