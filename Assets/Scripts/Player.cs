using System;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public float jumpForce = 8f;
    public LayerMask groundLayer;
    public float rayLength = 1.4f;
    public TextMeshProUGUI scoreText;

    public AudioClip jumpClip;
    public AudioClip landClip;
    public AudioClip dieClip;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator animator;
    private int score;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // ground check
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, rayLength, groundLayer);


        // debug
        Debug.DrawRay(transform.position, Vector3.down * rayLength, isGrounded ? Color.green : Color.red, 0f);

        // jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) 
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            
            AudioManager.instance.PlaySFX(jumpClip);
        }

        // if (isGrounded) 
        // {
        //     Animator.Play("Player_Run");
        // }
        // else {
        //     {
        //         Animator.Play("Player_Jump");
        //     }
        // }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Damage") 
        {
            AudioManager.instance.PlaySFX(dieClip);
            Invoke("WaitForSceneLoad", dieClip.length);
        }
        // else 
        // {
        //     AudioManager.instance.PlaySFX(landClip);
        // }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Damage") 
        {
            score++;
            scoreText.text = score.ToString();
        }
    }

    void WaitForSceneLoad()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}
