using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreRate : MonoBehaviour
{
    [SerializeField] private TMP_Text t_score;
    [SerializeField] private List<Image> stars;
    private void Reset()
    {
        t_score = GetComponentInChildren<TMP_Text>();
        for(var i = 0; i < transform.childCount; i++)
        {
            stars.Add(transform.GetChild(0).GetComponent<Image>());
        }
    }

    public void SetRating(float score)
    {
        t_score.text = $"{(int)(score * 100)}";
        var scorePerStar = 1f / stars.Count; // Divide the total score range evenly among the stars.
        for (var i = 0; i < stars.Count; i++)
        {
            // Calculate the portion of the score that applies to this star.
            var startRange = i * scorePerStar;
            var endRange = startRange + scorePerStar;

            if (score >= endRange)
            {
                // If the score is above the current star's range, fill it completely.
                stars[i].fillAmount = 1f;
            }
            else if (score > startRange)
            {
                // If the score is within the current star's range, calculate the fill amount.
                stars[i].fillAmount = (score - startRange) / scorePerStar;
            }
            else
            {
                // If the score is below the current star's range, leave it empty.
                stars[i].fillAmount = 0f;
            }
        }   
    }
}