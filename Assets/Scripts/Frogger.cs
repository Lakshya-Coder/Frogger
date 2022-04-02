using System;
using System.Collections;
using UnityEngine;

public class Frogger : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite idleSprite;
    public Sprite leapSprite;
    public Sprite deadSprite;
    private Vector3 spawnPosition;
    private float farthestRow;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnPosition = transform.position;
    }

    private void Update()
    {
        if (isUp()) {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            Move(Vector3.up);
        }
        else if (isDown()) {
            transform.rotation = Quaternion.Euler(0, 0, 180);
            Move(Vector3.down);
        }
        else if (isLeft()) {
            transform.rotation = Quaternion.Euler(0, 0, 90);
            Move(Vector3.left);
        }
        else if (isRight()) {
            transform.rotation = Quaternion.Euler(0, 0, -90);
            Move(Vector3.right);
        }
    }

    private void Move(Vector3 direction)
    {
        Vector3 destination = transform.position + direction;

        Collider2D barrier = Physics2D.OverlapBox(
            destination, Vector2.zero, 0f, LayerMask.GetMask("Barrier")
        );
        Collider2D platform = Physics2D.OverlapBox(
            destination, Vector2.zero, 0f, LayerMask.GetMask("Platform")
        );
        Collider2D obstacle = Physics2D.OverlapBox(
            destination, Vector2.zero, 0f, LayerMask.GetMask("Obstacle")
        );

        if (barrier != null)
        {
            return;
        }
        
        // if (platform != null)
        // {
        //     transform.SetParent(platform.transform);
        // }
        // else
        // {
        //     transform.SetParent(null);
        // }
        transform.SetParent(platform != null ? platform.transform : null);

        if (obstacle != null && platform == null)
        {
            transform.position = destination;
            Death();
        }
        else
        {
            FindObjectOfType<GameManager>().PlayMovementSound();
            if (destination.y > farthestRow)
            {
                farthestRow = destination.y;
                FindObjectOfType<GameManager>().AdvancedRow();
            }
            
            StartCoroutine(Leap(destination));
        }
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (enabled && col.gameObject.layer == LayerMask.NameToLayer("Obstacle") && transform.parent == null)
        {
            Death();
        }
    }

    public void Death()
    {
        StopAllCoroutines();
        
        transform.rotation = Quaternion.identity;
        spriteRenderer.sprite = deadSprite;
        enabled = false;
        
        FindObjectOfType<GameManager>().Died();
    }

    public void Respawn()
    {
        StopAllCoroutines();
        
        transform.rotation = Quaternion.identity;
        transform.position = spawnPosition;
        farthestRow = spawnPosition.y;
        spriteRenderer.sprite = idleSprite;
        gameObject.SetActive(true);
        enabled = true;
    }

    private IEnumerator Leap(Vector3 destination)
    {
        spriteRenderer.sprite = leapSprite;
        
        Vector3 startPosition = transform.position;
        float elapsed = 0f;
        float duration = 0.125f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPosition, destination, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = destination;
        
        spriteRenderer.sprite = idleSprite;
    }

    private bool isRight()
    {
        return Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
    }

    private bool isLeft()
    {
        return Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
    }

    private bool isDown()
    {
        return Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
    }

    private bool isUp()
    {
        return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
    }
}