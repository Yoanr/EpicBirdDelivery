using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Vector2 initPosition;

    public string horizontal;
    public string vertical;

    public GameObject Letter;

    public float speedMax; // 10
    public float speed;
    public float speedBase;

    public int staminaMax; // 100
    public int stamina;

    public int staminaGain; // 10
    public int staminaPerte; // 5

    private Rigidbody2D player;       //Store a reference to the Rigidbody2D component required to use 2D Physics.
    private bool hasLetter;
    private Vector2 position;
    private float timer;

    private BoxCollider2D collider;
    private SpriteRenderer renderer;

    // Use this for initialization
    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        renderer = GetComponent<SpriteRenderer>();

        player = GetComponent<Rigidbody2D>();
        Initialize();
    }

    public void Initialize()
    {
        timer = Time.time;

        this.stamina = staminaMax;
        this.speed = speedMax;
        renderer.color = Color.white;
        collider.enabled = true;

        transform.position = initPosition;
        Letter.transform.position = transform.position;

        //Get and store a reference to the Rigidbody2D component so that we can access it.
        if (horizontal == "Horizontal_player1")
        {
            hasLetter = true;
        }
        else
        {
            hasLetter = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            SoundManager.PlayBruitage("aouch");
            stamina = stamina - 20;
            if (hasLetter)
            {
                GameManager.hp = GameManager.hp - 1;
            }
            if (collider.enabled) StartCoroutine(NoColliderCoroutine());
        }
        else if (collision.gameObject.layer == 10)
        {
            GameManager.hp = Mathf.Min(3, GameManager.hp + 1);
            collision.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameManager.hasLost)
        {
            player.velocity = new Vector2(0, 0);
            return;
        }

        if (Time.time - timer >= 1.5f)
        {
            UpdateStamina(); // Decrease or increase the tiredness
            UpdateSpeed();
            timer = Time.time;
        }

        UpdateLetter();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            hasLetter = !hasLetter;
        }

        UpdateVelocity();
    }

    private void UpdateStamina()
    {
        if (hasLetter)
        {
            stamina = Mathf.Max(0, stamina - staminaPerte);
        }
        else
        {
            stamina = Mathf.Min(100, stamina + staminaGain);
        }
    }

    private void UpdateSpeed()
    {
        if (hasLetter)
        {
            speed = Mathf.Max(speedBase, speedMax * ((float)stamina / staminaMax));
        }
        else
        {
            speed = speedMax;
        }
    }

    private void UpdateVelocity()
    {
        float moveHorizontal = Input.GetAxis(horizontal);
        //Store the current vertical input in the float moveVertical.
        float moveVertical = Input.GetAxis(vertical);

        //Use the two store floats to create a new Vector2 variable movement.
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        if (horizontal == "Horizontal_player1")
        {
            if (transform.position.x > 0 && movement.x > -0.5 || transform.position.x < -8.5 && movement.x < 0)
                movement.x = 0;
            if (transform.position.y > 4 && movement.y > 0 || transform.position.y < -4 && movement.y < 0)
                movement.y = 0;
        }
        else
        {
            if (transform.position.x > 8.5 && movement.x > 0 || transform.position.x < 0.5 && movement.x < 0)
                movement.x = 0;
            if (transform.position.y > 4 && movement.y > 0 || transform.position.y < -4 && movement.y < 0)
                movement.y = 0;
        }

        //Call the AddForce function of our Rigidbody2D rb2d supplying movement multiplied by speed to move our player.
        player.velocity = movement * speed;
    }

    private void UpdateLetter()
    {
        Letter.SetActive(hasLetter);

        Letter.transform.position = player.transform.position;
    }

    private IEnumerator NoColliderCoroutine()
    {
        collider.enabled = false;
        for (int i = 0; i < 6; i++)
        {
            renderer.color = i % 2 == 0 ? Color.red : Color.white;
            yield return new WaitForSeconds(0.3f);
        }
        collider.enabled = true;
    }
}