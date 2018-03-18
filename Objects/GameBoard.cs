using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public int rowSize;
    public int columnSize;

    public int tileEventsRequired;
    public int tileEventsCatched;

    private int difficultyBumpCounter;

    public GameBoardSlot slotPrefab;

    public Queue<LaneMovementOrder> movOrders;

    public DragNavigation draggedTile;

    private GameBoardSlot[,] boardSize;

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
        if (tileEventsCatched >= tileEventsRequired && movOrders.Count != 0 )
        {
            DestroyCurrentOrder();

            if (movOrders.Count != 0)
            {
                StartCoroutine( Execute( GameConsts.GAME_BOARD_EXECUTE_DELAY ) );
            }
            else
            {
                CheckBoard();
                StartCoroutine( RequestLaneOrderMultiple( GameConsts.GAME_BOARD_REQUEST_DELAY) );
            }
        }
    }

    private void DestroyCurrentOrder()
    {
        if( movOrders.Count == 0 )
        {
            Debug.Log("Trying to destroy an order when none exist!!");
        }
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

    private bool IsBoardEmpty()
    {
        for (int x = 0; x < rowSize; x++)
        {
            for (int y = 0; y < columnSize; y++)
            {
                if( boardSize[x,y].isOccupied )
                {
                    return false;
                }
            }
        }

        return true;
    }

    private bool CanBumpDifficulty()
    {
        return (difficultyBumpCounter >= GameManager.difficultyBumpThreshold) ? true : false;
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
        if (foundSlots.Count >= GameConsts.GAME_BOARD_TILES_NUMBER_TO_MATCH )
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
        int score = 0;
        int mult = 0;

        difficultyBumpCounter++;
        if (CanBumpDifficulty())
        {
            difficultyBumpCounter = 0;
            GameManager.IncreaseDifficulty();
        }

        foreach (GameBoardSlot slot in foundSlots)
        {
            mult = slot.GetTile().scoreMult;
            score += GameConsts.TILE_SCORE_VALUE;
            slot.ClearSlot();
        }

        score = (mult != 0) ? score *= mult : score;

        GameManager.AddScore(score);
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

        if( IsBoardBlocked() )
        {
            ResetBoard();
            movOrders.Clear();
            GameManager.StartGame();
        }
    }

    private bool IsBoardBlocked()
    {
        for (int x = 0; x < rowSize; x++)
        {
            for (int y = 0; y < columnSize; y++)
            {
                if (!boardSize[x, y].isOccupied)
                {
                    return false;
                }
            }
        }

        return true;
    }
    #endregion

        #region Moving Columns and Rows
        public void MoveLane( int columnIndex, int rowIndex, Types.LaneMovementType movType)
        {
            Tile tempTile = null;
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
                                tempTile = FindObjectOfType<TilesManager>().CopyTile(boardSize[x, rowIndex].GetTile());
                                boardSize[x, rowIndex].CollapseTile(movType);
                            }
                            else if ( x + 1 < columnSize )
                            {
                                MoveTile(boardSize[x, rowIndex].objInSlot, boardSize[x, rowIndex], boardSize[x + 1, rowIndex]);
                            }
                        }

                        if (x == 0)
                        {
                            if (tempTile != null)
                            {
                                boardSize[x, rowIndex].UncollapseTile(movType, boardSize[x, rowIndex], tempTile);
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
                                tempTile = FindObjectOfType<TilesManager>().CopyTile(boardSize[x, rowIndex].GetTile());
                                boardSize[x, rowIndex].CollapseTile(movType);
                            }
                            else if (x < columnSize )
                            {
                                MoveTile(boardSize[x, rowIndex].objInSlot, boardSize[x, rowIndex], boardSize[x - 1, rowIndex]);
                            }
                        }

                        if (x == columnSize - 1)
                        {
                            if (tempTile != null)
                            {
                                boardSize[x, rowIndex].UncollapseTile(movType, boardSize[x, rowIndex], tempTile);
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
                                tempTile = FindObjectOfType<TilesManager>().CopyTile(boardSize[columnIndex, x].GetTile());
                                boardSize[columnIndex, x].CollapseTile(movType);
                            }
                            else if (x + 1 < rowSize )
                            {
                                MoveTile(boardSize[columnIndex, x].objInSlot, boardSize[columnIndex, x], boardSize[columnIndex, x + 1]);    
                            }
                        }

                        if (x == 0)
                        {
                            if (tempTile != null)
                            {
                                boardSize[columnIndex, x].UncollapseTile(movType, boardSize[columnIndex, x], tempTile);
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
                            // COLLAPSING TILE
                            if (x == 0 )
                            {
                                SubscribeOnTileMovementEnded(boardSize[columnIndex, x].objInSlot.GetComponent<Tile>());
                                tempTile = FindObjectOfType<TilesManager>().CopyTile(boardSize[columnIndex, x].GetTile());
                                boardSize[columnIndex, x].CollapseTile(movType);
                            }
                            // MOVING TILES
                            else if (x < rowSize )
                            {
                                MoveTile(boardSize[columnIndex, x].objInSlot, boardSize[columnIndex, x], boardSize[columnIndex, x - 1]);    
                            }
                        }

                        // UNCOLLAPSING TILE
                        if (x == rowSize - 1)
                        {
                            if (tempTile != null)
                            {
                                boardSize[columnIndex, x].UncollapseTile(movType, boardSize[columnIndex, x], tempTile);
                            }
                        }
                }
                    #endregion
                    break;
            }
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

    IEnumerator Execute( float time )
    {
        yield return new WaitForSecondsRealtime( time );

        ResetEventCounters();
        ExecuteLaneMovementOrder( movOrders.Peek() );
    }

    public void ResetEventCounters()
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
        // Bump moves counter
        GameManager.MovesCounter--;
        
        // If counter value equals or is higher than the threhsold, we trigger executing lines
        if( GameManager.MovesCounter <= 0 )
        {
            if( movOrders.Count != 0 )
            {
                StartCoroutine( Execute(GameConsts.GAME_BOARD_EXECUTE_DELAY ) );
            }
            else
            {
                RequestLaneOrder( 0 );
            }
            

            // Resetting moves counter
            GameManager.MovesCounter = GameConsts.GAME_LOGIC_MOVES_THRESHOLD;
        }        
    }
    
    IEnumerator RequestLaneOrderMultiple( float time )
    {
        yield return new WaitForSecondsRealtime( time );

        //for (int x = 0; x < Random.Range(1, GameConsts.GAME_BOARD_MAX_REQUESTS_AT_ONCE); x++)
        //{
        //    RequestLaneOrder(x);
        //}

        for( int x = 0; x < GameManager.OrdersCount; x ++ )
        {
            RequestLaneOrder(x);
        }
    }    

    public void RequestLaneOrder( int orderNumber)
    {
        int col, row;
        int x = 0;
        Types.LaneMovementType dir;

        if( IsBoardEmpty() )
        {
            RandomizeLaneForOrder( out col, out row, out dir);
            while ( !IsOrderValid(col, row) && x < 10 )
            {
                RandomizeLaneForOrder(out col, out row, out dir);
                x++;
            }

            if( x >= 10 )
            {
                col = row = GameConsts.GAME_LOGIC_INVALID;
                dir = Types.LaneMovementType.DOWN;
            }
        }
        else
        {
            FindLaneForOrder(out col, out row, out dir);
        }

        if( col >= 0 || row >= 0 )
        {
            movOrders.Enqueue(GameManager.CreateLaneMovementOrder(col, row, dir, orderNumber));
        }
    }  

    private bool IsOrderValid( int col, int row )
    {
        if (movOrders.Count != 0)
        {
            foreach (LaneMovementOrder order in movOrders)
            {
                if (order.rowToMove < 0 && order.columnToMove == row)
                {
                    return false;
                }
                else if (order.columnToMove < 0 && order.rowToMove == row)
                {
                    return false;
                }
            }

            return true;
        }
        else
        {
            return true;
        }
    }

    private void GetValidLanes( ref List<int> columns, ref List<int> rows, ref List<Types.LaneMovementType> potentialDirections )
    {
        columns.Clear();
        rows.Clear();
        potentialDirections.Clear();

        // Looking for columns
        if (FindColumns(ref columns))
        {
            potentialDirections.Add(Types.LaneMovementType.UP);
            potentialDirections.Add(Types.LaneMovementType.DOWN);
        }
        // Looking for rows
        if (FindRows(ref rows))
        {
            potentialDirections.Add(Types.LaneMovementType.LEFT);
            potentialDirections.Add(Types.LaneMovementType.RIGHT);
        }
    }

    private void FindLaneForOrder( out int col, out int row, out Types.LaneMovementType dir )
    {
        List<Types.LaneMovementType> potentialDirections = new List<Types.LaneMovementType>();

        List<int> columns = new List<int>();
        List<int> rows = new List<int>();

        GetValidLanes(ref columns, ref rows, ref potentialDirections);

        int rand;

        if (potentialDirections.Count != 0)
        {
            dir = potentialDirections[Random.Range(0, potentialDirections.Count)];

            switch (dir)
            {
                case Types.LaneMovementType.DOWN:
                case Types.LaneMovementType.UP:
                    rand = Random.Range(0, columns.Count);
                    col = columns[rand];
                    row = GameConsts.GAME_LOGIC_INVALID;
                    break;
                case Types.LaneMovementType.LEFT:
                case Types.LaneMovementType.RIGHT:
                    rand = Random.Range(0, rows.Count);
                    row = rows[rand];
                    col = GameConsts.GAME_LOGIC_INVALID;
                    break;
                default:
                    col = row = GameConsts.GAME_LOGIC_INVALID;
                    break;
            }
        }
        else
        {
            col = row = GameConsts.GAME_LOGIC_INVALID;
            //INVALID
            dir = Types.LaneMovementType.DOWN;
        }
    }

    private void RandomizeLaneForOrder(out int col, out int row, out Types.LaneMovementType dir )
    {
        dir = (Types.LaneMovementType)Random.Range(0, System.Enum.GetValues(typeof(Types.LaneMovementType)).Length);

        switch (dir)
        {
            case Types.LaneMovementType.DOWN:
            case Types.LaneMovementType.UP:
                col = Random.Range(0, columnSize);
                row = GameConsts.GAME_LOGIC_INVALID;
                break;
            case Types.LaneMovementType.LEFT:
            case Types.LaneMovementType.RIGHT:
                row = Random.Range(0, rowSize);
                col = GameConsts.GAME_LOGIC_INVALID;
                break;
            default:
                col = row = GameConsts.GAME_LOGIC_INVALID;
                break;
        }
    }

    #region Finding columns & rows. Checking if they're not already ordered.
    private bool FindColumns(ref List<int> columns)
    {
        for( int x = 0; x < rowSize; x ++ )
        {
            if (!IsColumnOrdered(x))
            {
                for (int y = 0; y < columnSize; y++)
                {
                    GameBoardSlot temp = GetSlot(x, y);
                    if (temp.isOccupied )
                    {
                        columns.Add(x);
                        break;
                    }
                }
            }
        }

        return columns.Count != 0;
    }

    private bool FindRows( ref List<int> rows )
    {
        for (int y = 0; y < columnSize; y++)
        {
            if (!IsRowOrdered(y))
            {
                for (int x = 0; x < rowSize; x++)
                {
                    GameBoardSlot temp = GetSlot(x, y);
                    if (temp.isOccupied )
                    {
                        rows.Add(y);
                        break;
                    }
                }
            }
        }

        return rows.Count != 0;
    }

    private bool IsRowOrdered( int row )
    {
        foreach( LaneMovementOrder order in movOrders )
        {
            if( order.rowToMove == row )
            {
                return true;
            }
        }

        return false;
    }

    private bool IsColumnOrdered( int col )
    {
        foreach( LaneMovementOrder order in movOrders )
        {
            if( order.columnToMove == col )
            {
                return true;
            }
        }

        return false;
    }
    #endregion


    // DEBUG DEBUG DEBUG DEBUG DEBUG
    public void TEMP_TEST_A()
    {
        RequestLaneOrder( 0 );
    }

    public void TEMP_TEST_B()
    {
        if (movOrders.Count != 0)
        {
            StartCoroutine(Execute(0));
        }
    }
}

