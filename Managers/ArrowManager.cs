using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    public static GameObject arrow;
    public static Sprite firstArrow;
    public static Sprite secondArrow;
    public static Sprite thirdArrow;
    public static Vector3 offset;

    public static float offsetValue = GameConsts.ARROW_SPAWN_OFFSET_VALUE;

    private void Awake()
    {
        arrow = Resources.Load<GameObject>(GameConsts.ARROW_PREFAB_PATH);

        firstArrow = Resources.Load<Sprite>( GameConsts.FIRST_ARROW_INDICATOR_PREFAB_PATH );
        secondArrow = Resources.Load<Sprite>(GameConsts.SECOND_ARROW_INDICATOR_PREFAB_PATH);
        thirdArrow = Resources.Load<Sprite>(GameConsts.THIRD_ARROW_INDICATOR_PREFAB_PATH) as Sprite;
    }

    public static GameObject SpawnArrowForLane( int col, int row, Types.LaneMovementType dir, int orderNumber)
    {
        Sprite newSpr;

        switch (orderNumber)
        {
            case 0:
                newSpr = firstArrow;
                break;
            case 1:
                newSpr = secondArrow;
                break;
            case 2:
                newSpr = thirdArrow;
                break;
            default:
                Debug.Log("SpawnArrow(): Error! Wrong order number!!");
                newSpr = null;
                break;
        }

        Vector3 slotPosition, rotation;
        slotPosition = GetArrowPosition(col, row, dir);
        rotation = GetArrowRotation(dir);

        GameObject newArrow = Instantiate(arrow, slotPosition + offset, Quaternion.Euler(rotation));

        newArrow.GetComponent<ArrowIndicator>().Initialize(newSpr);
        return newArrow;
    }

    public static Vector3 GetArrowPosition(int col, int row, Types.LaneMovementType dir)
    {
        GameBoard board = GameObject.FindGameObjectWithTag(GameConsts.GAME_BOARD_TAG).GetComponent<GameBoard>();
        GameBoardSlot slot;
        Vector3 pos, offset;

        switch (dir)
        {
            case Types.LaneMovementType.RIGHT:
                slot = board.GetSlot(GameConsts.GAME_BOARD_COLUMNS - 1, row);
                offset = new Vector3(offsetValue, 0, 0);
                break;
            case Types.LaneMovementType.LEFT:
                slot = board.GetSlot(0, row);
                offset = new Vector3(-offsetValue, 0, 0);
                break;
            case Types.LaneMovementType.UP:
                slot = board.GetSlot(col, GameConsts.GAME_BOARD_ROWS - 1);
                offset = new Vector3(0, offsetValue, 0);
                break;
            case Types.LaneMovementType.DOWN:
                slot = board.GetSlot(col, 0);
                offset = new Vector3(0, -offsetValue, 0);
                break;
            default:
                pos = new Vector3(0, 0, 0);
                slot = null;
                offset = new Vector3(0, 0, 0);
                Debug.Log("GetPositionForArrow: ERROR! No slot selected, returning 0,0,0 position for arrow!");
                break;
        }

        if (!slot)
        {
            Debug.Log("FUCKUP!");
        }
        pos = slot.transform.position + offset;

        return pos;
    }

    public static Vector3 GetArrowRotation(Types.LaneMovementType dir)
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
