// 11/16/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;
using TMPro;

public class DynamicInputFieldWidth : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField; // Reference to the TMP_InputField component
    [SerializeField] private int rightOffset = 20; // Offset to prevent text overlapping with the dropdown arrow
    [SerializeField] private int minWidth = 100; // Minimum width of the input field
    [SerializeField] private int maxWidth = 500; // Maximum width of the input field
    
    [SerializeField] private RectTransform[] relatives; // Array of RectTransforms to adjust their width
    private RectTransform inputFieldRect; // Reference to the RectTransform of the input field
    
    private void Start()
    {
        if (inputField == null)
        {
            inputField = GetComponent<TMP_InputField>();
        }

        inputFieldRect = inputField.GetComponent<RectTransform>();

        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(AdjustInputFieldWidth);
            AdjustInputFieldWidth(inputField.text); // Initial adjustment
        }
        else
        {
            Debug.LogWarning("InputField reference is missing.");
        }
    }

    private void AdjustInputFieldWidth(string text)
    {
        if (inputFieldRect == null)
        {
            Debug.LogWarning("RectTransform reference is missing.");
            return;
        }

        // Create a temporary TextMeshProUGUI to calculate the preferred width of the text
        var tempText = new GameObject("TempText", typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>();
        tempText.font = inputField.textComponent.font;
        tempText.fontSize = inputField.textComponent.fontSize;
        tempText.text = text;

        // Calculate the preferred width of the text
        var preferredWidth = tempText.preferredWidth;

        // Ensure the width is within the minimum and maximum bounds and add the right offset
        var newWidth = Mathf.Clamp(preferredWidth + rightOffset, minWidth, maxWidth);

        // Adjust the width of the input field
        inputFieldRect.sizeDelta = new Vector2(newWidth, inputFieldRect.sizeDelta.y);

        // Adjust the width of the relatives
        foreach (var relative in relatives)
        {
            if (relative != null)
            {
                relative.sizeDelta = new Vector2(newWidth, relative.sizeDelta.y);
            }
        }

        // Destroy the temporary text object
        Destroy(tempText.gameObject);
    }
}