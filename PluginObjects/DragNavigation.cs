using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragNavigation : MonoBehaviour
{
    private GameObject potentialSlot;
    private Vector2 startPos;

    private bool dragging;

    public delegate void TilesManagerEventHandler();
    public event TilesManagerEventHandler InformTilesManager;

    public delegate void GameBoardEventHandler();
    public event GameBoardEventHandler InformGameBoard;

    private Vector2 distance;
    private float posX;
    private float posY;

    private void Update()
    {
        RaycastHit hit = new RaycastHit();
        Ray ray = new Ray(transform.position, new Vector3(0, 0, 5f));
        Debug.DrawRay(ray.origin, ray.direction);
        if (dragging)
        {
            if (Physics.SphereCast(ray, 0.2f, out hit))
            {
                if (hit.collider.gameObject && hit.collider.CompareTag("slot"))
                {
                    Debug.Log("CAUGHT!!");
                    potentialSlot = hit.collider.gameObject;
                }
                else
                {
                    Debug.Log("NOTHING!");
                }
            }
        }
    }

    public void OnInformTilesManager()
    {
        InformTilesManager();
    }

    public void OnInformGameBoard()
    {
        Debug.Log("GameBoard Informed");
        InformGameBoard();
    }

    // Check if the picked tile is the same as the one in the current slot
    private bool IsTileIn(GameObject slot, GameObject tile)
    {
        return slot.GetComponent<GameBoardSlot>().objInSlot == tile.GetComponent<Tile>().gameObject;
    }

    private void OnMouseDown()
    {
        startPos = transform.position;
        distance = Camera.main.WorldToScreenPoint(transform.position);
        posX = Input.mousePosition.x - distance.x;
        posY = Input.mousePosition.y - distance.y;
        dragging = true;

        // Nulling current slot
        if (potentialSlot != null && IsTileIn(potentialSlot, gameObject))
        {
            potentialSlot.GetComponent<GameBoardSlot>().HandleSnap(false, gameObject);
        }

        GameObject board = GameObject.FindGameObjectWithTag(GameConsts.GAME_BOARD_TAG);
        if (board)
        {
            board.GetComponent<GameBoard>().SetDraggedTile(this);
        }
    }

    private void OnMouseDrag()
    {
        Vector2 mousePosition = new Vector2(Input.mousePosition.x - posX, Input.mousePosition.y - posY);
        Vector2 newPos = Camera.main.ScreenToWorldPoint(mousePosition);

        transform.position = newPos;
    }

    private void OnMouseUp()
    {
        dragging = false;

        if (potentialSlot != null && !potentialSlot.GetComponent<GameBoardSlot>().isOccupied)
        {
            // Setting up the position of the dragged object to the slot position
            transform.position = potentialSlot.transform.position;

            // Informing slot about object it has to snap
            potentialSlot.GetComponent<GameBoardSlot>().HandleSnap(true, gameObject);
            GetComponent<BoxCollider2D>().enabled = false;

            OnInformTilesManager();
            OnInformGameBoard();
        }
        else
        {
            transform.position = startPos;
        }
    }
}
