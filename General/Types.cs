using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Types : MonoBehaviour
{
    public enum TileType
    {
        BLUE,
        GREEN,
        RED,     
        YELLOW,
        PURPLE,
        ORANGE,
        PINK,
        DARKGREEN,
        BROWN
    }

    public enum LaneMovementType
    {
        RIGHT,
        LEFT,
        UP,
        DOWN,
    }

    public enum GameBoardSlotBorder
    {
        RIGHT,
        LEFT,
        UP,
        DOWN
    }
}