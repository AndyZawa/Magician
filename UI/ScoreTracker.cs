using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScoreTracker : MonoBehaviour
{
    public static Text displayText;

    private void Start()
    {
        displayText = GetComponent<Text>();
    }

    public static void UpdateScoreTracker()
    {
        displayText.text = GameManager.Score.ToString();
    }
}
