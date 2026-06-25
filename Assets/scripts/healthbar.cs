using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement; // EZT HOZZÁADTUK A PÁLYAVÁLTÁSHOZ!

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    
    [Header("Halál utáni pálya")]
    public string endSceneName = "end"; // Ide írd be Unity-ben az End Scene nevét!

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

        PlayerMovement pm = GetComponent<PlayerMovement>();
        if (pm == null)
        {
            pm = GetComponentInParent<PlayerMovement>();
        }
        if (pm != null)
        {
            pm.enabled = false; 
        }

        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        if (anim != null && !isinanim)
        {
            anim.SetTrigger("die");
            isinanim = true;

            // Megvárjuk a halál animációt
            yield return new WaitForSeconds(5f);
        }

        if (healthBarUI != null)
        {
            healthBarUI.SetActive(false);
        }

        // PÁLYAVÁLTÁS AZ END SCENE-RE
        if (!string.IsNullOrWhiteSpace(endSceneName))
        {
            SceneManager.LoadScene(endSceneName);
        }

        // Csak ezután kapcsoljuk ki a playert, különben leállna a Coroutine!
        gameObject.SetActive(false); 
    }
}