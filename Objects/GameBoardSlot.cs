using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardSlot : MonoBehaviour
{
    public Sprite slotSprite;

    //[HideInInspector]
    public bool isOccupied;

    public bool beenChecked;

    //[HideInInspector]
    public GameObject objInSlot;


    //[HideInInspector]
    public int slotRowPos;
    //[HideInInspector]
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

    public void SetBoardPosition(int colPos, int rowPos )
    {
        slotColPos = colPos;
        slotRowPos = rowPos;        
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
        beenChecked = false;
    }

    public void ProcessEventAndClearSlot()
    {
        objInSlot.GetComponent<Tile>().OnTileMovementEnded();
        ClearSlot();
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
