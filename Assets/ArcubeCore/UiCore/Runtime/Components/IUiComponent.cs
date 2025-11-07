using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IUiComponent
{
    Color Color { get; set; }
}

public interface ITextComponent : IUiComponent
{
    TMP_Text Text { get; set; }
}

public interface IImageComponent : IUiComponent
{
    Image Image { get; set; }
}