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

        // Irányba fordulás
        if (horizontalInput > 0.01f)
        {
            transform.localScale = new Vector3(1, 1, 1); 
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1); 
        }

        bool grounded = isGrounded();
        bool touchingWall = onWall();

        // Animációk frissítése
        anim.SetBool("run", horizontalInput != 0 && grounded); 
        anim.SetBool("grounded", grounded);
        
        // Csak akkor játsszuk le a fal animációt, ha rátapadt a falra
        bool isGrabbingWall = touchingWall && !grounded && horizontalInput != 0;
        anim.SetBool("onwall", isGrabbingWall);

        if (wallJumpCooldown < 0)
        {
            // FALRA TAPADÁS LOGIKA
            if (isGrabbingWall) 
            {
                // Ha a levegőben van, érinti a falat, és nyomja a gombot a fal felé: rátapad!
                body.linearVelocity = Vector2.zero; // Teljesen megállítjuk
                body.gravityScale = 0; // Kikapcsoljuk a gravitációt
            }
            else
            {
                // NORMÁL MOZGÁS (Ha nem lóg a falon)
                if (movementEnabled)
                {
                    body.linearVelocity = new Vector2(moveInput.x * speed, body.linearVelocity.y);
                }
                else
                {
                    body.linearVelocity = new Vector2(0f, body.linearVelocity.y);
                }
                body.gravityScale = 3; // Alap gravitáció visszaállítása
            }
            
            // UGRÁS
            if (playerInput.actions["Jump"].WasPressedThisFrame())
            {
                Jump(grounded, touchingWall);
            }
        }
        else
        {
            // Wall jump cooldown alatt nem engedjük az alap mozgást, hogy el tudjon rugaszkodni
            wallJumpCooldown -= Time.deltaTime;
        }
    }

    private void Jump(bool grounded, bool touchingWall)
    {
        Debug.Log("Jump triggered");
        if (grounded)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
            anim.SetTrigger("jump");
        }
        else if (touchingWall && !grounded)
        {
            wallJumpCooldown = 0.2f; 
            // Visszapattanás a falról
            body.linearVelocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, jumpForce * 0.8f);
            anim.SetTrigger("jump");
            
            // Azonnal megfordul, ha elrugaszkodott
            transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), 1, 1);
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
        if (body.linearVelocity.y > 0.2f) 
            return false;

        // ITT VOLT A HIBA: Még keskenyebbre vettük a dobozt (-0.2f), 
        // hogy a falon lógva a karakter sarka véletlenül se érezze a falat padlónak!
        Vector2 boxSize = new Vector2(boxCollider.bounds.size.x - 0.2f, boxCollider.bounds.size.y);

        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxSize,
            0f,
            Vector2.down,
            0.1f, 
            groundLayer); 

        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        Vector2 boxSize = new Vector2(boxCollider.bounds.size.x, boxCollider.bounds.size.y - 0.2f); 

        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center, 
            boxSize, 
            0f, 
            new Vector2(transform.localScale.x, 0), 
            0.1f, 
            groundLayer); 
            
        return raycastHit.collider != null;
    }
}