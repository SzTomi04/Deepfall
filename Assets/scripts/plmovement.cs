using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem; // Ezt hagyd itt, kell az eléréshez

public class PlayerMovement : MonoBehaviour
{
    [Header("Beállítások")]
    public float speed = 8f;
    public float jumpForce = 12f;
    [SerializeField] private LayerMask groundLayer; // Ez a LayerMask, amit az Inspectorban állítasz be a talajhoz
    [SerializeField] private LayerMask wallLayer; // Ez a LayerMask, amit az Inspectorban állítasz be a falhoz
    private Animator anim;
    private Rigidbody2D body;
    private PlayerInput playerInput;
    private float horizontalInput;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown; // Ez a változó fogja kezelni a falugrás utáni rövid időt, amikor nem lehet újra ugran
    private bool movementEnabled = true;
    
     


    private void Awake()
    {
        // Megkeressük a komponenseket
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
            transform.localScale = new Vector3(1, 1, 1); // Jobbra néz
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Balra néz
        }



        // Animációk kezelése
        anim.SetBool("run", horizontalInput != 0);  
        anim.SetBool("grounded", isGrounded());
        


        //wall jump cooldown kezelése
        if (wallJumpCooldown < 0)
        {
            if (movementEnabled)
            {
                body.linearVelocity = new Vector2(moveInput.x * speed, body.linearVelocity.y);
            }
            else
            {
                // keep vertical velocity but stop horizontal movement
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

     //JUMP
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
            wallJumpCooldown = 0.2f; // Beállítjuk a falugrás utáni rövid időt
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

    RaycastHit2D raycastHit = Physics2D.BoxCast(
        boxCollider.bounds.center,
        boxCollider.bounds.size,
        0f,
        Vector2.down,
        0.05f,
        groundLayer | wallLayer); // Itt hozzáadtam a wallLayer-t is, hogy a falon állást is talajnak vegye);

    return raycastHit.collider != null;
}

    private bool onWall()
    {
        // Alapértelmezetten nem vagyunk falon
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, new Vector2(transform.localScale.x, 0), 0.05f, wallLayer);
        return raycastHit.collider != null;
    }
    


}