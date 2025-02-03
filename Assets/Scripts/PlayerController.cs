using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public VideoTrigger video;
    private Vector2 move;
    private bool isKnockedBack = false;
    private float knockbackDuration = 0.1f;
    private float knockbackTime = 0f;
    private Vector3 knockbackDirection;

    private bool canMove = true;
    private float disableMoveTime = 0.2f;
    private float disableMoveTimer = 0f;

    private PlayerInput playerInput;
    public CanvasScript canvas;

    public void onMove(InputAction.CallbackContext context)
    {
        if (canMove)
        {
            move = context.ReadValue<Vector2>();
        }
    }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        video = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<VideoTrigger>();
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<CanvasScript>();
    }


    void Update()
    {
        if (isKnockedBack)
        {
            HandleKnockback();
        }
        else
        {
            movePlayer();
        }

        if (!canMove)
        {
            disableMoveTimer -= Time.deltaTime;
            if (disableMoveTimer <= 0)
            {
                canMove = true;
            }
        }

        // Jika tombol Space ditekan, semua wall menyala merah
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HighlightAllEnemies();
        }

        
    }

    public void movePlayer()
    {
        Vector3 movement = new Vector3(move.x, 0f, move.y);

        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);
            transform.Translate(movement * speed * Time.deltaTime, Space.World);
        }
    }

    void HandleKnockback()
    {
        transform.Translate(knockbackDirection * speed * Time.deltaTime, Space.World);
        knockbackTime -= Time.deltaTime;

        if (knockbackTime <= 0)
        {
            isKnockedBack = false;
            playerInput.enabled = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isKnockedBack && (move.x != 0 || move.y != 0))
        {
            Vector3 direction = new Vector3(move.x, 0f, move.y).normalized;
            if (direction != Vector3.zero)
            {
                knockbackDirection = -direction;
                isKnockedBack = true;
                knockbackTime = knockbackDuration;
                playerInput.enabled = false;
            }

            canMove = false;
            disableMoveTimer = disableMoveTime;

            StartCoroutine(ChangeEnemyColor(collision.gameObject));
            canvas.minOneHealth();
        }


        if (collision.gameObject.CompareTag("Finish"))
        {
            canvas.Finish();
        }


    }


    private IEnumerator ChangeEnemyColor(GameObject enemy) // damage the heart here
    {
        Renderer enemyRenderer = enemy.GetComponent<Renderer>();
        Color originalColor = enemyRenderer.material.color;
        enemyRenderer.material.color = Color.red;

        yield return new WaitForSeconds(0.2f);

        enemyRenderer.material.color = originalColor;

        video.jumpScare();
    }

    // Method untuk menyalakan semua musuh ketika Space ditekan
    private void HighlightAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // Cari semua musuh
        foreach (GameObject enemy in enemies)
        {
            StartCoroutine(RedAllEnemies(enemy)); // Ubah warna sementara
        }
    }

    private IEnumerator RedAllEnemies(GameObject enemy)
    {
        Renderer enemyRenderer = enemy.GetComponent<Renderer>();
        Color originalColor = enemyRenderer.material.color;
        enemyRenderer.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        enemyRenderer.material.color = originalColor;
    }
}
