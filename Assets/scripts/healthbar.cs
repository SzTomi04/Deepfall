using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    private Animator anim;
    private bool isinanim = false;
    [SerializeField] private GameObject healthBarUI;

    void Start()
    {
        currentHealth = maxHealth;
        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = currentHealth;
        }
    }

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (slider != null)
        {
            slider.value = currentHealth;
        }

        // if the damaged object has a PlayerMovement, briefly disable movement
        PlayerMovement pm = GetComponent<PlayerMovement>();
        if (pm == null)
        {
            pm = GetComponentInParent<PlayerMovement>();
        }
        if (pm != null)
        {
            pm.DisableMovementForSeconds(1f); // 1 second stun
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("A karakter meghalt!");

        AutoWalker walker = GetComponent<AutoWalker>();
        if (walker != null)
        {
            walker.StopWalking();
        }

        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        if (anim != null && !isinanim)
        {
            anim.SetTrigger("die");
            isinanim = true;

            yield return new WaitForSeconds(5f);
            gameObject.SetActive(false);
        }

        if (healthBarUI != null)
        {
            healthBarUI.SetActive(false);
        }
    }
}