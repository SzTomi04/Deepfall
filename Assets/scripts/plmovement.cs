using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerMovement : MonoBehaviour
{
    [Header("Beállítások")]
    public float speed = 8f;
    public float jumpForce = 12f;
    
    // EGYETLEN LAYER a falaknak és a padlónak
    [SerializeField] private LayerMask groundLayer; 
    
    private Animator anim;
    private Rigidbody2D body;
    private PlayerInput playerInput;
    private float horizontalInput;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown; 
    private bool movementEnabled = true;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        horizontalInput = playerInput.actions["Move"].ReadValue<Vector2>().x;
        Vector2 moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

        if (horizontalInput > 0.01f)
        {
            transform.localScale = new Vector3(1, 1, 1); 
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1); 
        }

        anim.SetBool("run", horizontalInput != 0);  
        anim.SetBool("grounded", isGrounded());

        if (wallJumpCooldown < 0)
        {
            if (movementEnabled)
            {
                body.linearVelocity = new Vector2(moveInput.x * speed, body.linearVelocity.y);
            }
            else
            {
                body.linearVelocity = new Vector2(0f, body.linearVelocity.y);
            }

            if(onWall() && !isGrounded())
            {
                anim.SetBool("onwall", onWall());
                body.gravityScale = 0;
                body.linearVelocity = Vector2.zero;
            }
            else
            {
                body.gravityScale = 3;
                anim.SetBool("onwall", onWall());
            }
            
            if (playerInput.actions["Jump"].WasPressedThisFrame())
            {
                Jump();
            }
        }
        else
        {
            wallJumpCooldown -= Time.deltaTime;
        }
    }

    private void Jump()
    {
        Debug.Log("Jump triggered");
        if (isGrounded())
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
            anim.SetTrigger("jump");
        }
        else if (onWall() && !isGrounded())
        {
            wallJumpCooldown = 0.2f; 
            body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
            anim.SetTrigger("jump");
        }
    }
    
    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded();
    }

    public void DisableMovementForSeconds(float seconds)
    {
        if (movementEnabled)
            StartCoroutine(DisableMovementRoutine(seconds));
    }

    private System.Collections.IEnumerator DisableMovementRoutine(float seconds)
    {
        movementEnabled = false;
        yield return new WaitForSeconds(seconds);
        movementEnabled = true;
    }

    private bool isGrounded()
    {
        if (body.linearVelocity.y > 0.1f)
            return false;

        Vector2 boxSize = new Vector2(boxCollider.bounds.size.x - 0.1f, boxCollider.bounds.size.y);

        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxSize,
            0f,
            Vector2.down,
            0.05f,
            groundLayer); 

        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        Vector2 boxSize = new Vector2(boxCollider.bounds.size.x, boxCollider.bounds.size.y - 0.1f);

        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center, 
            boxSize, 
            0f, 
            new Vector2(transform.localScale.x, 0), 
            0.05f, 
            groundLayer); 
            
        return raycastHit.collider != null;
    }
}