using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public int rowSize;
    public int columnSize;
    public int movesCounter;
    public int movesThreshold;

    public int tileEventsRequired;
    public int tileEventsCatched;

    public GameBoardSlot slotPrefab;

    public Queue<LaneMovementOrder> movOrders;

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

        movOrders = new Queue<LaneMovementOrder>();
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

    public void SubscribeOnInformGameBoard(DragNavigation drag)
    {
        drag.InformGameBoard += new DragNavigation.GameBoardEventHandler(CheckBoard);
        drag.InformGameBoard += new DragNavigation.GameBoardEventHandler(BumpMovesCounter);
    }

    public void SubscribeOnTileMovementEnded(Tile tile)
    {
        tile.TileMovementEnded += new Tile.TileEventHandler(IncreaseTileEventCounter);
        tile.TileMovementEnded += new Tile.TileEventHandler(CheckOrderExecution);
    }

    public void IncreaseTileEventCounter()
    {
        tileEventsCatched++;
    }

    public void CheckOrderExecution()
    {
        if (tileEventsCatched >= tileEventsRequired)
        {
            DestroyCurrentOrder();

            ResetEventCounter();

            if (movOrders.Count != 0)
            {
                Execute();
            }
        }
    }

    private void DestroyCurrentOrder()
    {
        LaneMovementOrder order = movOrders.Dequeue();
        order.DestroyArrow();
        Destroy(order);
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
                                if (boardSize[x, rowIndex].isOccupied)
                                {
                                    if (x == columnSize - 1)
                                    {
                                        SubscribeOnTileMovementEnded(boardSize[x, rowIndex].objInSlot.GetComponent<Tile>());
                                        boardSize[x, rowIndex].ProcessEventAndClearSlot();
                                    }
                                    else if ( x + 1 < columnSize )
                                    {
                                        MoveTile(boardSize[x, rowIndex].objInSlot, boardSize[x, rowIndex], boardSize[x + 1, rowIndex]);
                                    }
                                }
                            }
                            #endregion
                            break;
                        case Types.LaneMovementType.LEFT:
                            #region MOVING LANE LEFT
                            for (int x = 0; x < columnSize; x++)
                            {
                                if (boardSize[x, rowIndex].isOccupied)
                                {
                                    if (x == 0)
                                    {
                                        SubscribeOnTileMovementEnded(boardSize[x, rowIndex].objInSlot.GetComponent<Tile>());
                                        boardSize[x, rowIndex].ProcessEventAndClearSlot();
                                    }
                                    else if (x < columnSize )
                                    {
                                        MoveTile(boardSize[x, rowIndex].objInSlot, boardSize[x, rowIndex], boardSize[x - 1, rowIndex]);
                                    }
                                }
                            }
                            #endregion
                            break;
                        case Types.LaneMovementType.UP:
                            #region MOVING LANE UP
                            for (int x = rowSize - 1; x >= 0; x--)
                            {
                                if (boardSize[columnIndex, x].isOccupied)
                                {
                                    if (x == rowSize - 1)
                                    {
                                        SubscribeOnTileMovementEnded(boardSize[columnIndex, x].objInSlot.GetComponent<Tile>());
                                        boardSize[columnIndex, x].ProcessEventAndClearSlot();
                                    }
                                    else if (x + 1 < rowSize )
                                    {
                                        MoveTile(boardSize[columnIndex, x].objInSlot, boardSize[columnIndex, x], boardSize[columnIndex, x + 1]);    
                                    }
                                }
                            }
                            #endregion
                            break;
                        case Types.LaneMovementType.DOWN:
                            #region MOVING LANE DOWN
                            for (int x = 0; x < rowSize; x++)
                            {
                                if (boardSize[columnIndex, x].isOccupied)
                                {
                                    if (x == 0 )
                                    {
                                        SubscribeOnTileMovementEnded(boardSize[columnIndex, x].objInSlot.GetComponent<Tile>());
                                        boardSize[columnIndex, x].ProcessEventAndClearSlot();
                                    }
                                    else if (x + 1 < rowSize)
                                    {
                                        MoveTile(boardSize[columnIndex, x].objInSlot, boardSize[columnIndex, x], boardSize[columnIndex, x - 1]);    
                                    }
                                }
                            }
                            #endregion
                            break;
                    }

                    //CheckBoard();
                }

                public void MoveTile( GameObject tile, GameBoardSlot from, GameBoardSlot to)
                {
                    Tile newTile = tile.GetComponent<Tile>();
                    SubscribeOnTileMovementEnded( newTile );
                    newTile.SetDestination(to.transform.position);
                    newTile.SetTileInMotion(true);

                    to.objInSlot = from.objInSlot;
                    to.isOccupied = true;

                    from.ResetSlot();
                }
        #endregion

    #endregion

   
    public int CountTilesInLane( int colIndex, int rowIndex, Types.LaneMovementType movType )
    {
        int counter = 0;
        bool isRow;

        switch( movType )
        {
            case Types.LaneMovementType.RIGHT:
            case Types.LaneMovementType.LEFT:
                isRow = true;
                break;
            case Types.LaneMovementType.UP:
            case Types.LaneMovementType.DOWN:
                isRow = false;
                break;
            default:
                Debug.Log("Error! Wrong movement type!");
                return -1;
        }

        if( isRow )
        {
            for( int x = 0; x < columnSize; x++ )
            {
                if( boardSize[ x, rowIndex ].isOccupied )
                {
                    counter++;
                }
            }
        }
        else
        {
            for( int x = 0; x < rowSize; x++ )
            {
                if( boardSize[ colIndex, x ].isOccupied )
                {
                    counter++;
                }
            }
        }
        

        return counter;
    }

    public void Execute()
    {
        ExecuteLaneMovementOrder( movOrders.Peek() );
    }

    public void ResetEventCounter()
    {
        tileEventsRequired = 0;
        tileEventsCatched = 0;
    }

    public void ExecuteLaneMovementOrder( LaneMovementOrder order)
    {
        // MOVING LANE
        tileEventsRequired = CountTilesInLane(order.columnToMove, order.rowToMove, order.movementType);

        // Proceed moving tiles only if any been found
        if (tileEventsRequired != 0)
        {
            MoveLane(order.columnToMove, order.rowToMove, order.movementType);
        }
        else
        {
            CheckOrderExecution();
        }
    }

    public void BumpMovesCounter()
    { 
        //movesCounter++;
        if( movesCounter >= movesThreshold )
        {
            Execute();
            ResetMovesCounter();
        }        
    }

    private void ResetMovesCounter() 
    {
        movesCounter = 0;
    }

    public void TEMP_TEST_A()
    {
        RequestLaneOrder();
    }

    public void TEMP_TEST_B()
    {
        if( movOrders.Count != 0 )
        {
            Execute();
        }
    }
        
    public void RequestLaneOrder()
    {
        movOrders.Enqueue( GameManager.CreateLaneMovementOrder() );
    }  
}
