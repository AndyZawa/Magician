using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConsts
{
    // BOARD SETTINGS
    public const int GAME_BOARD_ROWS = 5;
    public const int GAME_BOARD_COLUMNS = 5;
    public const int GAME_BOARD_MAX_REQUESTS_AT_ONCE = 4;
    public const int GAME_BOARD_TILES_NUMBER_TO_MATCH = 4;
    public const float GAME_BOARD_EXECUTE_DELAY = 0.17f;
    public const float GAME_BOARD_CHECK_DELAY = 0.1f;
    public const float GAME_BOARD_REQUEST_DELAY = 0.2f;

    // GAME LOGIC SETTINGS
    public const int GAME_LOGIC_MOVES_THRESHOLD = 2;
    public const int GAME_LOGIC_INVALID = -1;
    public const int GAME_LOGIC_START_ORDER_AMOUNT = 1;
    public const int GAME_LOGIC_START_COLORS_COUNT = 3;


    // TILES SETTINGS
    public const float TILE_MOVEMENT_SPEED = 0.15f;
    public const float TILE_DESTROYING_SPEED = 0.30f;
    public const float TILE_DESTROYING_DELAY = 0.05f;
    public const float TILE_SCALE_RATIO = 0.3f;
    public const int TILE_SCORE_VALUE = 5;
    
    public const int TILE_DEFAULT_SCALE_VALUE = 1;

    // DIFFICULTY MANAGEMENT
    public const int FIRST_COLOR_DIFFICULTY_CHANGE = 4;
    public const int SECOND_COLOR_DIFFICULTY_CHANGE = 6;
    public const int FIRST_ORDER_DIFFICULTY_CHANGE = 2;
    public const int SECOND_ORDER_DIFFICULTY_CHANGE = 3;
    public const int THRESHOLD_FOR_DIFFICULTY_BUMP = 3;

    //ARROW MANAGER
    public const float ARROW_SPAWN_OFFSET_VALUE = 0.65f;

    // TAGS
    public const string GAME_BOARD_TAG = "GameBoard";

    // PATHS
    public const string ARROW_PREFAB_PATH = "ArrowPrefab";
    public const string FIRST_ARROW_INDICATOR_PREFAB_PATH = "FirstArrow";
    public const string SECOND_ARROW_INDICATOR_PREFAB_PATH = "SecondArrow";
    public const string THIRD_ARROW_INDICATOR_PREFAB_PATH = "ThirdArrow";
}