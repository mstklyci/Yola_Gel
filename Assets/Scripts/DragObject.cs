using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DragObject : MonoBehaviour
{
    private bool moving;
    public bool gameStarted;
    private float startPosX;
    private float startPosY;
    private Vector3 startPosition;

    private Transform panelTransform;
    private GameObject lastDropZone;

    private Sprite initialSprite;
    private SpriteRenderer spriteRenderer;

    TrafficSign trafficSign;
    [SerializeField] private int boardNumber;

    private void Awake()
    {
        startPosition = transform.localPosition;
        panelTransform = transform.parent;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            initialSprite = spriteRenderer.sprite;
        }
        gameStarted = false;
    }

    private void OnMouseDown()
    {
        if (Time.timeScale == 0) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        startPosX = mousePos.x - this.transform.position.x;
        startPosY = mousePos.y - this.transform.position.y;
        moving = true;

        GetComponent<Collider2D>().enabled = false;

        if (lastDropZone != null)
        {
            ToggleDropZoneVisuals(lastDropZone, true);
            lastDropZone = null;
        }
    }

    private void OnMouseUp()
    {
        if (Time.timeScale == 0) return;

        moving = false;

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("DropZone") && gameStarted == false)
            {
                if (Mathf.Abs(transform.position.x - hit.transform.position.x) <= 2f &&
                    Mathf.Abs(transform.position.y - hit.transform.position.y) <= 2f)
                {
                    transform.DOMove(hit.transform.position, 0.3f).SetEase(Ease.InOutCubic);

                    ToggleDropZoneVisuals(hit.collider.gameObject, false);
                    lastDropZone = hit.collider.gameObject;
                    trafficSign = hit.collider.GetComponent<TrafficSign>();
                    trafficSign.SelectSign(boardNumber);

                    UpdateSignDirection(trafficSign.rotation);
                }
                else
                {
                    ResetToPanelPosition();
                }
            }
            else
            {
                ResetToPanelPosition();
            }
        }
        else
        {
            ResetToPanelPosition();
        }

        GetComponent<Collider2D>().enabled = true;
    }

    private void Update()
    {
        if (Time.timeScale == 0 || !moving) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY, transform.position.z);
    }

    private void ResetToPanelPosition()
    {
        transform.DOMove(panelTransform.TransformPoint(startPosition), 0.3f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            if (spriteRenderer != null && initialSprite != null)
            {
                spriteRenderer.sprite = initialSprite;
            }
        });

        if (lastDropZone != null)
        {
            ToggleDropZoneVisuals(lastDropZone, true);
            lastDropZone = null;
        }
    }

    public void UpdateStartPosition()
    {
        startPosition = transform.localPosition;
    }

    private void ToggleDropZoneVisuals(GameObject dropZone, bool isVisible)
    {
        SpriteRenderer dropZoneRenderer = dropZone.GetComponent<SpriteRenderer>();
        if (dropZoneRenderer != null)
        {
            dropZoneRenderer.enabled = isVisible;
        }

        Transform arrow = dropZone.transform.Find("Arrow");
        if (arrow != null)
        {
            arrow.gameObject.SetActive(isVisible);
        }
    }

    private void UpdateSignDirection(int rotation)
    {
        gameObject.transform.rotation = Quaternion.Euler(0, 0, rotation);
    }
}