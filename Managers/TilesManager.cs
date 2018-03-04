using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    public Tile tilePrefab;
    public Sprite[] possibleSprites;

    private Tile spawnedTile;

    public static Dictionary< Types.TileType, Sprite > tileTypesDictionary;

    private void Awake()
    {
        tileTypesDictionary = new Dictionary< Types.TileType, Sprite >
        {
            { Types.TileType.AIR,         possibleSprites[ (int)Types.TileType.AIR ]  },
            { Types.TileType.WATER,       possibleSprites[ (int)Types.TileType.WATER] },
            { Types.TileType.FIRE,        possibleSprites[ (int)Types.TileType.FIRE]  },
            { Types.TileType.EARTH,       possibleSprites[ (int)Types.TileType.EARTH] },
        };
    }

    public static Types.TileType RandomizeTileType()
    {
        float randomizedType = Random.Range(0, System.Enum.GetValues(typeof(Types.TileType)).Length);
        return (Types.TileType)randomizedType; ;
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
        Subscribe( spawnedTile.GetComponent<DragNavigation>() );
    }
}

