using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    public Tile tilePrefab;
    public Sprite[] possibleSprites;

    private static Types.TileType previousType;
    private static bool newColor;

    private Tile spawnedTile;

    public static Dictionary< Types.TileType, Sprite > tileTypesDictionary;

    private void Awake()
    {
        tileTypesDictionary = new Dictionary< Types.TileType, Sprite >
        {
            { Types.TileType.BLUE,              possibleSprites[ (int)Types.TileType.BLUE ]},
            { Types.TileType.GREEN,             possibleSprites[ (int)Types.TileType.GREEN]},
            { Types.TileType.RED,               possibleSprites[ (int)Types.TileType.RED]},
            { Types.TileType.PINK,              possibleSprites[ (int)Types.TileType.PINK]},
            { Types.TileType.ORANGE,            possibleSprites[ (int)Types.TileType.ORANGE]},
            { Types.TileType.PURPLE,            possibleSprites[ (int)Types.TileType.PURPLE]},
            { Types.TileType.YELLOW,            possibleSprites[ (int)Types.TileType.YELLOW]},
            { Types.TileType.DARKGREEN,         possibleSprites[ (int)Types.TileType.DARKGREEN]},
            { Types.TileType.BROWN,             possibleSprites[ (int)Types.TileType.BROWN]},
        };

        previousType = Types.TileType.BLUE;
    }

    public static Types.TileType RandomizeTileType()
    {
        if (!newColor)
        {
            float randomizedType = Random.Range(0, GameManager.ColorsCount );

            while ((Types.TileType)randomizedType == previousType)
            {
                randomizedType = Random.Range(0, GameManager.ColorsCount );
            }

            previousType = (Types.TileType)randomizedType;
            return (Types.TileType)randomizedType; ;
        }
        else
        {
            newColor = false;
            return (Types.TileType)GameManager.ColorsCount - 1;
        }
    }

    public static void AddNewColor()
    {
        newColor = true;
    }

    public static Sprite AddTypeSprite( Types.TileType tileType )
    {
        Sprite newSprite;
        tileTypesDictionary.TryGetValue(tileType, out newSprite);
        return newSprite;
    }

    public void Subscribe(DragNavigation drag)
    {
        drag.InformTilesManager += new DragNavigation.TilesManagerEventHandler(CreateNewTile);
    }

    public void StartTileGeneration()
    {
        CreateNewTile();
    }

    public void CreateNewTile()
    {
        Vector2 newPos = new Vector3( 0.5f, -4.2f, 0 );
        spawnedTile = Instantiate( tilePrefab, newPos, Quaternion.identity);
        spawnedTile.Initialize();
        Subscribe( spawnedTile.GetComponent<DragNavigation>() );
    }

    public Tile CopyTile( Tile tileToCopy )
    {
        Tile newTile = Instantiate(tilePrefab, tileToCopy.transform.position, Quaternion.identity);
        newTile.tileType = tileToCopy.tileType;
        newTile.tileSprite = AddTypeSprite(newTile.tileType);
        newTile.GetComponent<SpriteRenderer>().sprite = newTile.tileSprite;
        Subscribe(newTile.GetComponent<DragNavigation>());
        FindObjectOfType<GameBoard>().SubscribeOnTileMovementEnded(newTile);
        return newTile;
    }
}

