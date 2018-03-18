using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MovesTracker : MonoBehaviour
{
    private static Text displayText;

    private void Start()
    {
        displayText = GetComponent<Text>();
    }

    public static void UpdateMovesTracker()
    {
        displayText.text = GameManager.MovesCounter.ToString();
    }
}
