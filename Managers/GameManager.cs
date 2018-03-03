using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static float offsetValue = 0.65f;

    public static LaneMovementOrder CreateLaneMovementOrder()
    {
        int movementType;
        int rowToMove;
        int columnToMove;

        movementType = Random.Range(0, System.Enum.GetValues(typeof(Types.LaneMovementType)).Length);

        switch ((Types.LaneMovementType)movementType)
        {
            case Types.LaneMovementType.RIGHT:
            case Types.LaneMovementType.LEFT:
                // Getting random ROW
                rowToMove = Random.Range(0, GameConsts.GAME_BOARD_ROWS);
                columnToMove = GameConsts.GAME_LOGIC_INVALID;
                break;
            case Types.LaneMovementType.UP:
            case Types.LaneMovementType.DOWN:
                // Getting random COLUMN
                columnToMove = Random.Range(0, GameConsts.GAME_BOARD_COLUMNS);
                rowToMove = GameConsts.GAME_LOGIC_INVALID;
                break;
            default:
                Debug.Log("ERROR: Wrong random in BumpMovesCounter");
                rowToMove = GameConsts.GAME_LOGIC_INVALID;
                columnToMove = GameConsts.GAME_LOGIC_INVALID;
                movementType = GameConsts.GAME_LOGIC_INVALID;
                break;
        }


        Vector3 arrowPos = GetArrowPosition(columnToMove, rowToMove, (Types.LaneMovementType)movementType);

        GameObject newArrow = ArrowManager.SpawnArrow(arrowPos, GetArrowRotation((Types.LaneMovementType)movementType));

        LaneMovementOrder newOrder = new LaneMovementOrder( columnToMove, rowToMove, (Types.LaneMovementType)movementType, newArrow );
        return newOrder;
    }

    public static Vector3 GetArrowPosition( int col, int row, Types.LaneMovementType dir )
    {
        GameBoard board = GameObject.FindGameObjectWithTag(GameConsts.GAME_BOARD_TAG).GetComponent<GameBoard>();
        GameBoardSlot slot;
        Vector3 pos, offset;

        switch ( dir )
        {
            case Types.LaneMovementType.RIGHT:
                slot = board.GetSlot(GameConsts.GAME_BOARD_COLUMNS - 1, row);
                offset = new Vector3(offsetValue, 0, 0);
                break;
            case Types.LaneMovementType.LEFT:
                slot = board.GetSlot(0, row);
                offset = new Vector3( -offsetValue, 0, 0);
                break;
            case Types.LaneMovementType.UP:
                slot = board.GetSlot(col, GameConsts.GAME_BOARD_ROWS - 1);
                offset = new Vector3( 0, offsetValue, 0 );
                break;
            case Types.LaneMovementType.DOWN:
                slot = board.GetSlot(col, 0 );
                offset = new Vector3(0, -offsetValue, 0);                
                break;
            default:
                pos = new Vector3(0, 0, 0);
                slot = null;
                offset = new Vector3(0, 0, 0);
                Debug.Log("GetPositionForArrow: ERROR! No slot selected, returning 0,0,0 position for arrow!");
                break;
        }

        pos = slot.transform.position + offset;

        return pos;
    }

    public static Vector3 GetArrowRotation( Types.LaneMovementType dir )
    {
        Vector3 newRot;
        float rotVal;

        switch (dir)
        {
            case Types.LaneMovementType.RIGHT:
                rotVal = 0;
                break;
            case Types.LaneMovementType.LEFT:
                rotVal = 180;
                break;
            case Types.LaneMovementType.UP:
                rotVal = 90;
                break;
            case Types.LaneMovementType.DOWN:
                rotVal = 270;
                break;
            default:
                rotVal = 0;
                break;
        }

        newRot = new Vector3(0, 0, rotVal);

        return newRot;
    }
}
