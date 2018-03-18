using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Types.TileType tileType { get; set; }
    public Sprite tileSprite;
    public bool moving;
    public bool destroying;


    public bool collapsing;
    public bool uncollapsing;
    public int collapseMultiplier;

    public int scoreMult = 0;

    public delegate void TileEventHandler();
    public event TileEventHandler TileMovementEnded;

    private float speed;
    private Vector3 moveTo;

    private bool xAxis;
    private Vector3 transferEndPos;

    private void Awake()
    {
        speed = GameConsts.TILE_MOVEMENT_SPEED;
    }

    public void Initialize()
    {
        tileType = TilesManager.RandomizeTileType();
        tileSprite = TilesManager.AddTypeSprite(tileType);
        GetComponent<SpriteRenderer>().sprite = tileSprite;        
    }

    private void FixedUpdate()
    {
        // MOVING TILESS
        if (moving)
        {
            UpdateTilePosition(transform.position, moveTo, speed);

            if (transform.position == moveTo)
            {
                moving = false;
                OnTileMovementEnded();
            }            
        }

        // TRANSFERING TILE TO NEXT SLOT
        if(collapsing)
        {
            UpdateTilePosition(transform.position, transferEndPos, speed);
            UpdateTileScale();

            if (transform.position == transferEndPos)
            {
                Destroy(gameObject);
                collapsing = false;
            }
        }

        if (uncollapsing)
        {
            UpdateTilePosition(transform.position, transferEndPos, speed);
            UpdateTileScale();

            if (transform.position == transferEndPos)
            {
                OnTileMovementEnded();
                transform.localScale = new Vector3(1f, 1f, 1f);
                uncollapsing = false;
            }
        }
    }

    public void UpdateTilePosition(Vector3 from, Vector3 to, float speed )
    {
        transform.position = Vector3.MoveTowards(from, to, speed);
    }

    public void SetDestination(Vector3 dest)
    {
        moveTo = dest;
    }

    public void SetTileInMotion(bool setInMotion)
    {
        moving = setInMotion;
    }

    public void SetTileDestroy(float delay)
    {
        StartCoroutine(TileDestroy(delay));
    }

    public void IncreaseScoreMultiplier()
    {
        scoreMult++;
    }

    IEnumerator TileDestroy(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        for (float f = 1f; f >= 0; f -= GameConsts.TILE_DESTROYING_SPEED)
        {
            transform.localScale = new Vector3(f, f, f);
            yield return null;
        }

        Destroy(gameObject);
    }

    // EVENTS
    public void OnTileMovementEnded()
    {
        IncreaseScoreMultiplier();
        TileMovementEnded();
        TileMovementEnded = null;
    }

    public void ProceedCollapse(Types.LaneMovementType dir, Vector3 endPos )
    {
        SetAxis(dir);

        collapseMultiplier = -1;
        transferEndPos = endPos;
        collapsing = true;
    }

    private void SetAxis( Types.LaneMovementType dir )
    {
        switch (dir)
        {
            case Types.LaneMovementType.LEFT:
            case Types.LaneMovementType.RIGHT:
                xAxis = true;
                break;
            case Types.LaneMovementType.UP:
            case Types.LaneMovementType.DOWN:
                xAxis = false;
                break;
        }
    }

    public void ProceedUncollapse( Types.LaneMovementType dir, Vector3 endPos )
    {
        SetAxis(dir);

        collapseMultiplier = 1;
        transferEndPos = endPos;
        uncollapsing = true;
    }


    public void SetUncollapsedTile( Vector3 startPos, Types.LaneMovementType dir )
    {
        Vector3 scale;

        switch( dir )
        {
            case Types.LaneMovementType.LEFT:
            case Types.LaneMovementType.RIGHT:
                scale = new Vector3(0f, transform.localScale.y, transform.localScale.z);
                break;
            case Types.LaneMovementType.UP:
            case Types.LaneMovementType.DOWN:
                scale = new Vector3(transform.localScale.x, 0f, transform.localScale.z);
                break;
            default:
                scale = Vector3.zero;
                Debug.Log("ERROR!! Scale for uncollapsed tile is wrong!");
                break;
        }

        transform.position = startPos;
        transform.localScale = scale;
    }

    public void UpdateTileScale()
    {
        if (xAxis)
        {
            transform.localScale += new Vector3(GameConsts.TILE_SCALE_RATIO * collapseMultiplier, 0, 0);
        }
        else
        {
             transform.localScale += new Vector3(0, GameConsts.TILE_SCALE_RATIO * collapseMultiplier, 0);
        }
    }
}
