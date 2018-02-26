using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Types.TileType tileType    { get; set; }
    public Sprite tileSprite;
    public bool beenChecked;

    private void Awake()
    {
        tileType = TilesManager.RandomizeTileType();
        tileSprite = TilesManager.AddTypeSprite(tileType);
        GetComponent<SpriteRenderer>().sprite = tileSprite;
    }
}
