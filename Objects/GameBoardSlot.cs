using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardSlot : MonoBehaviour
{
    public Sprite slotSprite;

    //[HideInInspector]
    public bool isOccupied;
    //[HideInInspector]
    public GameObject objInSlot;

    [HideInInspector]
    public int slotRowPos;
    [HideInInspector]
    public int slotColPos;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        GetComponent<SpriteRenderer>().sprite = slotSprite;
        ResetSlot();
    }

    public void SetBoardPosition(int rowPos, int colPos)
    {
        slotRowPos = rowPos;
        slotColPos = colPos;
    }

    public void HandleSnap( bool snap, GameObject obj )
    {
        // Handle snapping of the objects
        if( snap )
        {
            objInSlot = obj;
        }
        else
        {
            objInSlot = null;
        }

        isOccupied = snap;
    }

    public void ResetSlot()
    {
        objInSlot = null;
        isOccupied = false;
    }

    public void ClearSlot()
    {
        if (objInSlot != null)
        {
            Destroy(objInSlot.gameObject);
        }

        ResetSlot();
    }
}
