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

    public List<LaneMovementOrder> movementOrders;

    public DragNavigation draggedTile;

    private GameBoardSlot[,] boardSize;

    private void Update()
    {
        Debug.Log("!");
    }

    public void Awake()
    {
        rowSize = GameConsts.GAME_BOARD_ROWS;
        columnSize = GameConsts.GAME_BOARD_COLUMNS;
        transform.tag = GameConsts.GAME_BOARD_TAG;
        boardSize = new GameBoardSlot[columnSize, rowSize];
        InitializeBoard();

        movementOrders = new List<LaneMovementOrder>();
        movementOrders.Add( GameManager.CreateLaneMovementOrder() );
    }

    private void InitializeBoard()
    {
        for (int x = 0; x < columnSize; x++)
        {
            for (int y = 0; y < rowSize; y++)
            {
                GameBoardSlot temp;
                temp = SpawnBoardSlot(x, y);
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



    #region Core Mechanics
        #region Finding neighbours
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

                private bool AddFoundSlot(GameBoardSlot foundSlot, ref List<GameBoardSlot> connectedSlots)
                {
                    if (!connectedSlots.Contains(foundSlot))
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

                public GameBoardSlot GetSlot(int colIndex, int rowIndex)
                    {
                        if (rowIndex >= rowSize || colIndex >= columnSize || rowIndex < 0 || colIndex < 0)
                        {
                            return null;
                        }
                        else
                        {
                            return boardSize[colIndex, rowIndex];
                        }
                    }

        #endregion

        #region Clearing slots
                private bool AnySlotsCleared(List<GameBoardSlot> foundSlots)
                {
                    if (foundSlots.Count >= 3)
                    {
                        ClearFoundSlots(foundSlots);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                private void ClearFoundSlots(List<GameBoardSlot> foundSlots)
                {
                    foreach (GameBoardSlot slot in foundSlots)
                    {
                        slot.ClearSlot();
                    }
                }
        #endregion

        #region Checking for clearing
                public void CheckBoard()
                {
                    List<GameBoardSlot> connectedSlots = new List<GameBoardSlot>();

                    for (int x = 0; x < rowSize; x++)
                    {
                        for (int y = 0; y < columnSize; y++)
                        {
                            GameBoardSlot tempSlot = GetSlot(x, y);
                            if (tempSlot && tempSlot.isOccupied)
                            {
                                CheckValidNeighbours(tempSlot, ref connectedSlots);
                                if (AnySlotsCleared(connectedSlots))
                                {
                                    Debug.Log("TO DO: Score Tracking!!");
                                }

                                connectedSlots.Clear();
                            }
                        }
                    }
                }

        #endregion

        #region Moving Columns and Rows
                public void MoveLane( int columnIndex, int rowIndex, Types.LaneMovementType movType)
                {
                    switch (movType)
                    {
                        case Types.LaneMovementType.RIGHT:
                            #region MOVING LANE RIGHT
                            for (int x = columnSize - 1; x >= 0; x--)
                            {
                                if (x == columnSize - 1)
                                {
                                    boardSize[x, rowIndex].ClearSlot();
                                }
                                else if (x + 1 < columnSize && boardSize[x, rowIndex].isOccupied)
                                {
                                    MoveTile(boardSize[x, rowIndex], boardSize[x + 1, rowIndex]);
                                }
                            }
                            #endregion
                            break;
                        case Types.LaneMovementType.LEFT:
                            #region MOVING LANE LEFT
                            for (int x = 0; x < columnSize; x++)
                            {
                                if ( x == 0 )
                                {
                                    boardSize[x, rowIndex].ClearSlot();
                                }
                                else if (x < columnSize && boardSize[x, rowIndex].isOccupied)
                                {
                                    MoveTile(boardSize[x, rowIndex], boardSize[x - 1, rowIndex]);
                                }
                            }
                            #endregion
                            break;
                        case Types.LaneMovementType.UP:
                            #region MOVING LANE UP
                            for (int x = rowSize - 1; x >= 0; x--)
                            {
                                if (x == rowSize - 1)
                                {
                                    boardSize[columnIndex, x].ClearSlot();
                                }
                                else if (x + 1 < columnSize && boardSize[columnIndex, x].isOccupied)
                                {
                                    MoveTile(boardSize[columnIndex, x], boardSize[columnIndex, x + 1]);
                                }
                            }
                            #endregion
                            break;
                        case Types.LaneMovementType.DOWN:
                            #region MOVING LANE DOWN
                            for (int x = 0; x < rowSize; x++)
                            {
                                if (x == 0)
                                {
                                    boardSize[columnIndex, x].ClearSlot();
                                }
                                else if (x < columnSize && boardSize[columnIndex, x].isOccupied)
                                {
                                    MoveTile(boardSize[columnIndex, x], boardSize[columnIndex, x - 1]);
                                }
                            }
                            #endregion
                            break;
                    }

                    //CheckBoard();
                }

                public void MoveTile(GameBoardSlot from, GameBoardSlot to)
                {
                    to.objInSlot = from.objInSlot;
                    to.objInSlot.gameObject.transform.position = to.transform.position;
                    to.isOccupied = true;

                    from.ResetSlot();
                }
        #endregion

    #endregion

    public void ExecuteLaneMovementOrders( List<LaneMovementOrder> orders )
    {
        foreach (LaneMovementOrder order in orders)
        {
            MoveLane(order.columnToMove, order.rowToMove, order.movementType);     
        }

        for (int i = movementOrders.Count - 1; i >= 0; i--)
        {
            movementOrders[i].DestroyArrow();
            Destroy( movementOrders[i] );
        }

        movementOrders.Clear();
    }

    public void BumpMovesCounter()
    { 
        movesCounter++;
        if( movesCounter >= movesThreshold )
        {
            ExecuteLaneMovementOrders(movementOrders);
            ResetCounter();
        }        
    }

    private void ResetCounter()
    {
        movesCounter = 0;
    }

    public void TEMP_TEST()
    {
        movementOrders.Add( GameManager.CreateLaneMovementOrder() );        
    }
       
}
