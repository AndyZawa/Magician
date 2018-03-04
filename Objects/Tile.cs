using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Types.TileType tileType    { get; set; }
    public Sprite tileSprite;
    public bool moving;
    public bool afterTransfer;

    public delegate void TileEventHandler();
    public event TileEventHandler TileMovementEnded;

    private float speed;
    private Vector3 moveTo;

    private void Awake()
    {
        tileType = TilesManager.RandomizeTileType();
        tileSprite = TilesManager.AddTypeSprite(tileType);
        GetComponent<SpriteRenderer>().sprite = tileSprite;
        speed = GameConsts.TILE_MOVEMENT_SPEED;
    }

    private void LateUpdate()
    {
        if( moving )
        {
            if( transform.position == moveTo )
            {
                OnTileMovementEnded();             
            }

            UpdateTilePosition(transform.position, moveTo);
        }        
    }

    public void UpdateTilePosition( Vector3 from, Vector3 to )
    {
        transform.position = Vector3.MoveTowards( from, to, speed );
    }

    public void SetDestination( Vector3 dest )
    {
        moveTo = dest;
    }

    public void SetTileInMotion( bool setInMotion )
    {
        moving = setInMotion;
    }

    // EVENTS
    public void OnTileMovementEnded()
    {
        SetTileInMotion(false);
        TileMovementEnded();
    }
}
