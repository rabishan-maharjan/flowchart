using System;
using System.Linq;
using Arcube;
using TMPro;
using UnityEngine;

public class OperationCommandUi : CommandUi
{
    [SerializeField] private TMP_Dropdown dr_add_expression;
    
    [SerializeField] private TMP_InputField ip_text;
    [SerializeField] private TMP_InputField ip_number;
    [SerializeField] private TMP_Dropdown dr_add_variable;
    [SerializeField] private TMP_Dropdown dr_add_function;
    [SerializeField] private TMP_Dropdown dr_add_operator;
    protected override void Reset()
    {
        transform.TryFindObject(nameof(dr_add_expression), out dr_add_expression);
        transform.TryFindObject(nameof(ip_text), out ip_text);
        transform.TryFindObject(nameof(ip_number), out ip_number);
        transform.TryFindObject(nameof(dr_add_variable), out dr_add_variable);
        transform.TryFindObject(nameof(dr_add_function), out dr_add_function);
        transform.TryFindObject(nameof(dr_add_operator), out dr_add_operator);
        
        base.Reset();
    }
    
    protected override void Start()
    {
        base.Start();
        
        dr_add_expression.options = Enum.GetNames(typeof(ExpressionType)).Select(n => new TMP_Dropdown.OptionData(n)).ToList();
        dr_add_expression.onValueChanged.AddListener((value) =>
        {
            var obj = CreateExpressionField((ExpressionType)value);
            if(!obj) return;
            obj.gameObject.SetActive(true);
            dr_add_expression.transform.SetAsLastSibling();
            dr_add_expression.SetValueWithoutNotify(0);
        });
        dr_add_expression.value = -1;
    }
    
    private Component CreateExpressionField(ExpressionType expressionType) =>
        expressionType switch
        {
            ExpressionType.Text => Instantiate(ip_text, ip_text.transform.parent),
            ExpressionType.Number => Instantiate(ip_number, ip_number.transform.parent),
            ExpressionType.Variable => Instantiate(dr_add_variable, dr_add_variable.transform.parent),
            ExpressionType.Function => Instantiate(dr_add_function, dr_add_function.transform.parent),
            ExpressionType.Operator => Instantiate(dr_add_operator, dr_add_operator.transform.parent),
            _ => null
        };
    
    protected override void Apply()
    {
        //update graph
    }

    private object GetValue(ExpressionType type) =>
        type switch
        {
            ExpressionType.Text => ip_text.text,
            ExpressionType.Number => int.Parse(ip_number.text),
            ExpressionType.Variable => dr_add_variable.value,
            ExpressionType.Function => dr_add_function.value,
            ExpressionType.Operator => dr_add_operator.value,
            _ => null
        };
}