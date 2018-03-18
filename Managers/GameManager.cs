using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // VARIABLES
    public static int MovesCounter {get; set;}
    public static int Score { get; set; }

    public static int difficultyBumpThreshold;

    private static int colorsCount;
    private static int ordersCount;

    //PROPERTIES
    public static int ColorsCount
    {
        get { return colorsCount; }
        set
        {
            if ((colorsCount + 1) > TilesManager.tileTypesDictionary.Count )
            {
                Debug.Log("All colors in play! ");
                return;
            }
            else
            {
                colorsCount = value;
            }
        }
    }

    public static int OrdersCount
    {
        get { return ordersCount;}
        set
        {
            if( (ordersCount + 1) > GameConsts.GAME_BOARD_MAX_REQUESTS_AT_ONCE )
            {
                Debug.Log("Maximum order amount!");
                return;
            }
            else
            {
                ordersCount = value;
            }
        }
    }


    // METHODS
    private void Start()
    {
        StartGame();

        FindObjectOfType<TilesManager>().StartTileGeneration();
    }

    public static LaneMovementOrder CreateLaneMovementOrder( int colToMove, int rowToMove, Types.LaneMovementType dirToMove, int orderNumber )
    {
        GameObject newArrow = ArrowManager.SpawnArrowForLane(colToMove, rowToMove, dirToMove, orderNumber);

        LaneMovementOrder newOrder = new LaneMovementOrder(colToMove, rowToMove, dirToMove, newArrow );
        return newOrder;
    }

    public static void StartGame()
    {
        Score = 0;
        ColorsCount = GameConsts.GAME_LOGIC_START_COLORS_COUNT;
        OrdersCount = GameConsts.GAME_LOGIC_START_ORDER_AMOUNT;      

        difficultyBumpThreshold = GameConsts.THRESHOLD_FOR_DIFFICULTY_BUMP;

        MovesCounter = GameConsts.GAME_LOGIC_MOVES_THRESHOLD;
        MovesTracker.UpdateMovesTracker();
        ScoreTracker.UpdateScoreTracker();
    }

    public static void AddScore( int score )
    {
        Score += score;
        ScoreTracker.UpdateScoreTracker();
    }

    public static void IncreaseDifficulty()
    {
        difficultyBumpThreshold += GameConsts.THRESHOLD_FOR_DIFFICULTY_BUMP;

        //  FIRST COLOR DIFFICULTY BUMP - ADDING NEW COLORS
        if ( ColorsCount < GameConsts.FIRST_COLOR_DIFFICULTY_CHANGE )
        {
            ColorsCount++;
            TilesManager.AddNewColor();
            return;
        }

        // FIRST ORDER DIFFICULTY BUMP - ADDING ADDITIONAL ORDER TO PROCEED
        if (OrdersCount < GameConsts.FIRST_ORDER_DIFFICULTY_CHANGE)
        {
            OrdersCount++;
            return;
        }

        // SECOND COLOR DIFFICULTY BUMP - ADDING NEW COLORS
        if (ColorsCount < GameConsts.SECOND_COLOR_DIFFICULTY_CHANGE)
        {
            ColorsCount++;
            TilesManager.AddNewColor();
            return;
        }

        // SECOND ORDER DIFFICULTY BUMP - ADDING ADDITIONAL ORDER TO PROCEED
        if (OrdersCount < GameConsts.SECOND_ORDER_DIFFICULTY_CHANGE)
        {
            OrdersCount++;
            return;
        }
    }
}
