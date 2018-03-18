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

    public void CollapseTile( Types.LaneMovementType direction )
    {
        Vector3 collapseEndPoint = GetBoardSlotBorder(this, direction, true);
        objInSlot.GetComponent<Tile>().ProceedCollapse(direction, collapseEndPoint);
        ResetSlot();
    }

    public void UncollapseTile( Types.LaneMovementType direction, GameBoardSlot transferTo, Tile tile )
    {
        Vector3 uncollapseEndPoint = GetBoardSlotBorder(transferTo, direction, false);
        tile.SetUncollapsedTile(uncollapseEndPoint, direction);

        transferTo.objInSlot = tile.gameObject;
        transferTo.isOccupied = true;
        transferTo.objInSlot.GetComponent<Tile>().ProceedUncollapse(direction, transferTo.transform.position);
    }


    public Tile GetTile()
    {
        return objInSlot.GetComponent<Tile>();
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
            objInSlot.GetComponent<Tile>().SetTileDestroy( GameConsts.TILE_DESTROYING_DELAY );
        }

        ResetSlot();
    }

    private Vector3 GetBoardSlotBorder(GameBoardSlot slot, Types.LaneMovementType dir, bool collapse)
    {
        Vector3 startPos = slot.transform.position;
        int dirMult = (collapse) ? -1 : 1;

        switch (dir)
        {
            case Types.LaneMovementType.LEFT:
                startPos += new Vector3((slot.slotSprite.bounds.size.x / 2) * dirMult, 0, 0);
                break;
            case Types.LaneMovementType.RIGHT:
                startPos -= new Vector3((slot.slotSprite.bounds.size.x / 2) * dirMult, 0, 0);
                break;
            case Types.LaneMovementType.UP:
                startPos -= new Vector3(0, (slot.slotSprite.bounds.size.y / 2) * dirMult, 0);
                break;
            case Types.LaneMovementType.DOWN:
                startPos += new Vector3(0, (slot.slotSprite.bounds.size.y / 2) * dirMult, 0);
                break;
            default:
                startPos = Vector3.zero;
                break;
        }

        return startPos;
    }
}
