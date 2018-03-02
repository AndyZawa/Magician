using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public int rowSize;
    public int columnSize;
    public int movesCounter;
    public int movesThreshold;
    public GameBoardSlot slotPrefab;

    public delegate void DragNavigationEventHandler();
    public event DragNavigationEventHandler GetDraggedTile;

    public DragNavigation draggedTile;

    //private BoardLane[] rows;
    //private BoardLane[] columns;

    private GameBoardSlot[,] boardSize;

    private void Update()
    {
        Debug.Log("!");
    }

    public void Awake()
    {
        boardSize = new GameBoardSlot[columnSize, rowSize];

        // Initializing rows and calling constructors
        //rows = new BoardLane[rowSize];
        //for( int x = 0; x < rowSize; x++ )
        //{
        //    rows[x] = new BoardLane(columnSize);
        //}

        //// Initializing columns and calling constructors
        //columns = new BoardLane[columnSize];
        //for( int y = 0; y < columnSize; y++ )
        //{
        //    columns[y] = new BoardLane(rowSize);
        //}

        // Board Init
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        for (int x = 0; x < columnSize; x++)
        {
            for (int y = 0; y < rowSize; y++)
            {
                GameBoardSlot temp;
                temp = SpawnBoardSlot(x, y);
                //rows[x].FillLanePosition( temp, y );
                //columns[y].FillLanePosition( temp, x );
            }
        }
    }

    private GameBoardSlot SpawnBoardSlot(int x, int y)
    {
        GameBoardSlot newSlot = Instantiate(slotPrefab, GetWorldPosition(x + transform.position.x, y + transform.position.y), Quaternion.identity);
        newSlot.transform.parent = transform;
        newSlot.SetBoardPosition(x, y);
        boardSize[x, y] = newSlot;
        return newSlot;
    }

    public Vector3 GetWorldPosition(float x, float y)
    {
        return new Vector3((x - columnSize / 2), (y - rowSize / 2) - 1, 1f);
    }

    public void SubscribeOnInformGameBoard( DragNavigation drag )
    {
        drag.InformGameBoard += new DragNavigation.GameBoardEventHandler(CheckBoard);
        drag.InformGameBoard += new DragNavigation.GameBoardEventHandler(BumpMovesCounter);
    }

    public void SetDraggedTile( DragNavigation drag )
    {
        draggedTile = drag;
        SubscribeOnInformGameBoard( draggedTile );
    }

    public void ResetBoard()
    {
        for (int x = 0; x < rowSize; x++)
        {
            for (int y = 0; y < columnSize; y++)
            {
                boardSize[x, y].ClearSlot();
            }
        }
    }

    public void CheckBoard()
    {
        List<GameBoardSlot> connectedSlots = new List<GameBoardSlot>();

        for (int x = 0; x < rowSize; x++)
        {
            for (int y = 0; y < columnSize; y++)
            {
                GameBoardSlot tempSlot = GetSlot(x, y);
                if ( tempSlot && tempSlot.isOccupied)
                {
                    CheckValidNeighbours(tempSlot, ref connectedSlots);
                    if( AnySlotsCleared( connectedSlots ) )
                    {
                        Debug.Log("TO DO: Score Tracking!!");
                    }

                    connectedSlots.Clear();
                }
            }
        }
    }

    #region Checking for Neighbours
    public void CheckValidNeighbours(GameBoardSlot slotToCheck, ref List<GameBoardSlot> connectedSlots)
    {
        GameBoardSlot tempSlot;

        // Left Neighbour
        tempSlot = GetSlot(slotToCheck.slotColPos - 1, slotToCheck.slotRowPos);
        if (tempSlot && tempSlot.isOccupied)
        {
            if (slotToCheck.objInSlot.GetComponent<Tile>().tileType == tempSlot.objInSlot.GetComponent<Tile>().tileType)
            {
                if ( AddFoundSlot(tempSlot, ref connectedSlots) )
                {
                    CheckValidNeighbours(tempSlot, ref connectedSlots);
                }
            }
        }

        // Right Neighbour
        tempSlot = GetSlot(slotToCheck.slotColPos + 1, slotToCheck.slotRowPos);
        if (tempSlot && tempSlot.isOccupied)
        {
            if (slotToCheck.objInSlot.GetComponent<Tile>().tileType == tempSlot.objInSlot.GetComponent<Tile>().tileType)
            {
                if (AddFoundSlot(tempSlot, ref connectedSlots))
                {
                    CheckValidNeighbours(tempSlot, ref connectedSlots);
                }
            }
        }

        // Upper Neighbour
        tempSlot = GetSlot(slotToCheck.slotColPos, slotToCheck.slotRowPos + 1);
        if (tempSlot && tempSlot.isOccupied)
        {
            if (slotToCheck.objInSlot.GetComponent<Tile>().tileType == tempSlot.objInSlot.GetComponent<Tile>().tileType)
            {
                if (AddFoundSlot(tempSlot, ref connectedSlots))
                {
                    CheckValidNeighbours(tempSlot, ref connectedSlots);
                }
            }
        }

        // Lower Neighbour
        tempSlot = GetSlot(slotToCheck.slotColPos, slotToCheck.slotRowPos - 1);
        if (tempSlot && tempSlot.isOccupied)
        {
            if (slotToCheck.objInSlot.GetComponent<Tile>().tileType == tempSlot.objInSlot.GetComponent<Tile>().tileType)
            {
                if (AddFoundSlot(tempSlot, ref connectedSlots))
                {
                    CheckValidNeighbours(tempSlot, ref connectedSlots);
                }
            }
        }

        AddFoundSlot(slotToCheck, ref connectedSlots);
    }
    #endregion

    private bool AddFoundSlot(GameBoardSlot foundSlot, ref List<GameBoardSlot> connectedSlots)
    {
        if( !connectedSlots.Contains(foundSlot) )
        {
            foundSlot.beenChecked = true;
            connectedSlots.Add(foundSlot);
            return true;
        }
        else
        {
            return false;
        }
    }

    public GameBoardSlot GetSlot( int colIndex, int rowIndex )
    {
        if( rowIndex >= rowSize || colIndex >= columnSize || rowIndex < 0 || colIndex < 0 )
        {
            return null;
        }
        else
        {
            return boardSize[colIndex, rowIndex];
        }
    }

    #region Clearing slots
    private bool AnySlotsCleared( List<GameBoardSlot> foundSlots )
    {
        if( foundSlots.Count >= 3 )
        {
            ClearFoundSlots(foundSlots);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ClearFoundSlots( List<GameBoardSlot> foundSlots )
    {
        foreach( GameBoardSlot slot in foundSlots )
        {
            slot.ClearSlot();
        }
    }
    #endregion

    public void BumpMovesCounter()
    {
        movesCounter++;
        if( movesCounter == movesThreshold )
        {
            int R = Random.Range(0, rowSize - 1);

            MoveRow(R, Types.LaneMovementType.RIGHT);
            movesCounter = 0;
        }
    }

    public void MoveRow( int rowIndex, Types.LaneMovementType movType )
    {
       for (int x = columnSize - 1; x >= 0; x--)
        {
            if( x == columnSize - 1 )
            {
                boardSize[x, rowIndex].ClearSlot();
            }
            else if( x + 1 < columnSize && boardSize[x, rowIndex].isOccupied )
            { 
                MoveTile( boardSize[x, rowIndex], boardSize[x + 1, rowIndex] );
            }
        }

        CheckBoard();
    }

    public void MoveTile( GameBoardSlot from, GameBoardSlot to )
    {
        to.objInSlot = from.objInSlot;
        to.objInSlot.gameObject.transform.position = to.transform.position;
        to.isOccupied = true;

        from.ResetSlot();
    }

    public void MoveColumn( int columnIndex )
    {
    }
}
