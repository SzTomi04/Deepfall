using UnityEngine;
using UnityEngine.InputSystem; // Kell az új rendszerhez!

public class plattack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] shots;
    
    private PlayerMovement playerMovement; // Kijavítottam a gépelési hibát is (Movament -> Movement)
    private Animator anim;
    private PlayerInput playerInput; // Kell az új input lekéréséhez
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update() // Update-be kell tenni, nem Awake-be!
    {
        // ÚJ INPUT SYSTEM LEKÉRDEZÉS:
        if (playerInput.actions["Attack"].WasPressedThisFrame() && cooldownTimer > attackCooldown && playerMovement.canAttack())
        {
            Attack();
        }
        
        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        
        anim.SetTrigger("attack");
        cooldownTimer = 0;
        
        // Lövedék pozicionálása és indítása
        shots[FindShotIndex()].transform.position = firePoint.position;
        shots[FindShotIndex()].GetComponent<shot>().SetDirection(Mathf.Sign(transform.localScale.x));
    }
    private int FindShotIndex()
    {
        for (int i = 0; i < shots.Length; i++)
        {
            if (!shots[i].activeInHierarchy)
                return i;
        }
        return 0; // Ha minden lövedék aktív, visszatérünk az elsővel (vagy kezelheted másképp)
    }

}