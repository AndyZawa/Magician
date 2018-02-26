using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public int rows;
    public int columns;
    public GameBoardSlot slotPrefab;

    private GameBoardSlot[,] boardSize;

    public void Awake()
    {
        boardSize = new GameBoardSlot[rows, columns];
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                SpawnBoardSlot(x, y);
            }
        }
    }

    private void SpawnBoardSlot(int x, int y)
    {
        GameBoardSlot newSlot = Instantiate(slotPrefab, GetWorldPosition(x + transform.position.x, y + transform.position.y), Quaternion.identity);
        newSlot.transform.parent = transform;
        newSlot.SetBoardPosition(x, y);
        boardSize[x, y] = newSlot;
    }

    public Vector3 GetWorldPosition(float x, float y)
    {
        return new Vector3((x - columns / 2), (y - rows / 2) - 1, 1f);
    }

    public void ResetBoard()
    {
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                boardSize[x, y].ClearSlot();
            }
        }
    }
}
