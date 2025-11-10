using System;
using System.Globalization;
using System.Linq;

public static class OperatorHandler
{
    private static readonly string[] ArithmeticOperators =
    {
        "+", "-", "*", "/", "%"
    };

    private static readonly string[] LogicOperators =
    {
        "&&", "||", "==", "!=", "<", "<=", ">", ">="
    };

    public static bool Verify(string operatorName)
        => LogicOperators.Contains(operatorName) || ArithmeticOperators.Contains(operatorName);

    public static Variable OperateArithmetic(Variable v1, Variable v2, string operatorName)
    {
        float val1 = Convert.ToSingle(v1.GetValue());
        float val2 = Convert.ToSingle(v2.GetValue());
        float result = 0;

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

        bool bothInt = v1.Type == VariableType.Int && v2.Type == VariableType.Int;
        return new Variable
        {
            Type = bothInt ? VariableType.Int : VariableType.Float,
            Value = bothInt ? ((int)result).ToString() : result.ToString(CultureInfo.InvariantCulture),
            Assigned = true
        };
    }

    public static bool OperateLogic(Variable v1, Variable v2, string operatorName)
    {
        // Convert both to float if numeric
        bool v1IsNumber = v1.Type == VariableType.Int || v1.Type == VariableType.Float;
        bool v2IsNumber = v2.Type == VariableType.Int || v2.Type == VariableType.Float;

        if (v1IsNumber && v2IsNumber)
        {
            float val1 = Convert.ToSingle(v1.GetValue());
            float val2 = Convert.ToSingle(v2.GetValue());
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
        if (v1.Type == VariableType.Boolean && v2.Type == VariableType.Boolean)
        {
            bool b1 = (bool)v1.GetValue();
            bool b2 = (bool)v2.GetValue();
            return operatorName switch
            {
                "&&" => b1 && b2,
                "||" => b1 || b2,
                "==" => b1 == b2,
                "!=" => b1 != b2,
                _ => throw new Exception($"Invalid boolean operator {operatorName}")
            };
        }

        // Handle strings
        if (v1.Type == VariableType.String && v2.Type == VariableType.String)
        {
            string s1 = (string)v1.GetValue();
            string s2 = (string)v2.GetValue();
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