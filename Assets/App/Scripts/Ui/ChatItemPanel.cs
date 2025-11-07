using Arcube;
using Arcube.UiManagement;
using TMPro;
using UnityEngine;

public class ChatItemPanel : Panel
{
    [SerializeField] private TMP_Text t_text;
    private void Reset()
    {
        transform.TryFindObject(nameof(t_text), out t_text);
    }

    public void SetText(string text, Color color)
    {
        t_text.text = text;
        t_text.color = color;
    }
}