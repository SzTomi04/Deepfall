using UnityEngine;

public class AutoWalker : MonoBehaviour
{
    [Header("Beállítások")]
    public float moveSpeed = 5f;      // Milyen gyorsan menjen
    public bool isWalking = true;    // Le lehessen állítani
    public float turnInterval = 2f;  // Hány másodpercenként forduljon meg

    private Animator anim;
    private float timer;
    private int direction = 1;       // 1 = jobbra, -1 = balra

    void Start()
    {
        anim = GetComponent<Animator>();
        timer = turnInterval; // Beállítjuk az időzítőt
    }

    void Update()
    {
        if (isWalking)
        {
            // Időzítő csökkentése
            timer -= Time.deltaTime;

            // Ha lejárt a 2 másodperc
            if (timer <= 0)
            {
                TurnAround();
                timer = turnInterval; // Időzítő újraindítása
            }

            // Mozgatás az aktuális irányba (direction-nel szorozva)
            transform.Translate(Vector2.right * moveSpeed * direction * Time.deltaTime);

            if (anim != null)
            {
                anim.SetBool("run", true);
            }
        }
        else
        {
            if (anim != null)
            {
                anim.SetBool("run", false);
            }
        }
    }

    // Megfordulás kezelése
    void TurnAround()
    {
        direction *= -1; // Irány megfordítása (-1-ből 1 lesz, 1-ből -1)

        // A karakter kinézetének megfordítása (Sprite tükrözés)
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }

    public void StopWalking()
    {
        isWalking = false;
    }
}