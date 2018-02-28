using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardLane
{
    public GameBoardSlot[] positions;

    public BoardLane( int size )
    {
        positions = new GameBoardSlot[size];
    }

    public void FillLanePosition( GameBoardSlot slotToPlace, int slotIndex )
    {
        positions[slotIndex] = slotToPlace;
    }

    public void AdjustPositions( Types.LaneMovementType movementType )
    {
        switch(movementType)
        {
            case Types.LaneMovementType.RIGHT:
            case Types.LaneMovementType.UP:
                break;
            case Types.LaneMovementType.LEFT:
            case Types.LaneMovementType.DOWN:
                break;
            default:
                break;
        }
    }

    public void MoveTile( GameBoardSlot from, GameBoardSlot to )
    {
        
    }
}
