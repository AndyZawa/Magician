using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowIndicator : MonoBehaviour
{
    public void Initialize( Sprite newSpr )
    {
        GetComponent<SpriteRenderer>().sprite = newSpr;
    }
}
