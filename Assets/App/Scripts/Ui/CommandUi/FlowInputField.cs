using System;
using Arcube;
using Arcube.UiManagement;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class FlowInputField : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    private void Reset()
    {
        inputField = GetComponent<TMP_InputField>();
    }

    public string Text
    {
        get => inputField.text;
        set => inputField.text = value;
    }
    
    public event Action OnDelete;
    private void Start()
    {
        gameObject.FindObject<ButtonImage>("b_clear").OnClick.AddListener(() =>
        {
            Text = "";
        });
        
        gameObject.FindObject<ButtonImage>("b_delete").OnClick.AddListener(() =>
        {
            OnDelete?.Invoke();
            inputField.SetTextWithoutNotify("");
            gameObject.SetActive(false);
        });
    }
}