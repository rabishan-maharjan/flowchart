using UnityEngine;
using Random = UnityEngine.Random;

public class RandomImageSpawner : MonoBehaviour
{
    public GameObject[] imagePrefabs;
    public int imageDensity = 5;
    public int referenceHeight = 50;
    public Vector2 minSize = new Vector2(50, 50);
    public Vector2 maxSize = new Vector2(150, 150);
    private void Start() => SpawnImagesEvenlyY();

    private void SpawnImagesEvenlyY()
    {
        var parentRect = (RectTransform)transform;
        var height = parentRect.rect.height;
        var width = parentRect.rect.width;

        var numberOfImages = Mathf.RoundToInt(imageDensity * (height / referenceHeight));
        var sliceHeight = height / numberOfImages;
        for (var i = 0; i < numberOfImages; i++)
        {
            var imagePrefab = imagePrefabs[Random.Range(0, imagePrefabs.Length)];
            var newImage = Instantiate(imagePrefab, parentRect);
            var imgRect = newImage.GetComponent<RectTransform>();
            imgRect.anchorMin = new Vector2(0.5f, 0.5f);
            imgRect.anchorMax = new Vector2(0.5f, 0.5f);
            imgRect.pivot = new Vector2(0.5f, 0.5f);
            imgRect.localScale = Vector3.one;

            // Random X in full width
            var x = Random.Range(-width / 2f, width / 2f);

            // Even Y in slice center
            var y = (-height / 2f) + (sliceHeight * i) + (sliceHeight / 2f);
            
            // Random size within min and max range
            var randomWidth = Random.Range(minSize.x, maxSize.x);
            var randomHeight = Random.Range(minSize.y, maxSize.y);
            imgRect.sizeDelta = new Vector2(randomWidth, randomHeight);


            imgRect.anchoredPosition = new Vector2(x, y);
        }
    }
}