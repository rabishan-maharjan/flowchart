using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RandomSprite : MonoBehaviour
{
    [SerializeField] private Image[] images;
    private void Reset()
    {
        images = GetComponentsInChildren<Image>();
    }

    [SerializeField] private Sprite[] sprites;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        images ??= GetComponentsInChildren<Image>();
        foreach (var image in images)
        {
            image.sprite = sprites[Random.Range(0, sprites.Length)];
        }
    }
}