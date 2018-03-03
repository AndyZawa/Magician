using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneMovementOrder : Object
{
    public int rowToMove;
    public int columnToMove;
    public Types.LaneMovementType movementType;
    public GameObject orderArrow;

    public LaneMovementOrder( int col, int row, Types.LaneMovementType movType, GameObject arrow )
    {
        rowToMove = row;
        columnToMove = col;
        movementType = movType;
        orderArrow = arrow;
    }

    public void DestroyArrow()
    {
        Destroy(orderArrow.gameObject);
    }
}
