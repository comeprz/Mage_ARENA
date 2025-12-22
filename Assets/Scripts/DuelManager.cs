using UnityEngine;

public class DuelManager : MonoBehaviour
{
    public static DuelManager I;

    [Header("References")]
    public Transform playerATarget; // transform du marker torse A
    public Transform playerBTarget; // transform du marker torse B
    public Transform firePage;      // transform du marker page Feu
    public CooldownUI fireCooldownUI;

    [Header("Fireball (reused instance)")]
    public FireballProjectile fireballInstance;   // drag & drop l'objet FireballReady
    public Transform fireballHome;                 // point “repos” (ex: un empty sur la page)


    [Header("Gameplay")]
    public int playerAHP = 100;
    public int playerBHP = 100;
    public float fireballSpeed = 2.0f;
    public int fireballDamage = 15;

    [Header("State")]
    public bool fireArmed = false;
    public bool playerAIsCaster = true; // pour MVP: toggle qui lance

    [Header("Cooldown")]
    public float fireCooldown = 5f;
    bool _fireOnCooldown = false;

    [Header("UI")]
    public HealthBarUI playerAHealthUI;
    public HealthBarUI playerBHealthUI;

    int _playerAMaxHP = 100;
    int _playerBMaxHP = 100;

    void Awake()
    {
        I = this;
        _playerAMaxHP = playerAHP;
        _playerBMaxHP = playerBHP;

        playerAHealthUI?.SetInstant(1f);
        playerBHealthUI?.SetInstant(1f);
    }

    public void ArmFire(bool armed)
    {
        fireArmed = armed;

        // Si on perd la page, on cache la boule "ready"
        if (!fireArmed && fireballInstance != null && !fireballInstance.IsInFlight)
            fireballInstance.gameObject.SetActive(false);

        // Si on retrouve la page, on la réaffiche seulement si pas en cooldown
        if (fireArmed && fireballInstance != null && !fireballInstance.IsInFlight)
        {
            if (!_fireOnCooldown)
            {
                fireballInstance.transform.SetParent(fireballHome, false);
                fireballInstance.transform.localPosition = Vector3.zero;
                fireballInstance.transform.localRotation = Quaternion.identity;
                fireballInstance.gameObject.SetActive(true);
            }
        }
    }

    public void CastFire()
    {
        if (!fireArmed) return;
        if (_fireOnCooldown) { Debug.Log("Fire on cooldown"); return; }

        if (fireballInstance == null || fireballHome == null) return;

        Transform target = playerAIsCaster ? playerBTarget : playerATarget;
        if (target == null) return;

        // Si déjà en vol, on ignore
        if (fireballInstance.IsInFlight) return;

        // Cache la boule "ready" en la détachant + en la laissant partir
        _fireOnCooldown = true;

        fireballInstance.transform.position = fireballHome.position;
        fireballInstance.transform.rotation = Quaternion.LookRotation((target.position - fireballHome.position).normalized);
        fireballInstance.transform.SetParent(null, true);
        fireballInstance.gameObject.SetActive(true);

        fireballInstance.Launch(
            target,
            fireballSpeed,
            fireballDamage,
            playerAIsCaster,
            onArrive: () =>
            {
                // À l'impact : on cache la boule, puis on la remet après cooldown
                fireballInstance.gameObject.SetActive(false);

                if (fireCooldownUI != null)
                    fireCooldownUI.StartCooldown(fireCooldown);

                Invoke(nameof(ResetFireAfterCooldown), fireCooldown);
            }
        );
    }

    void ResetFireAfterCooldown()
    {
        _fireOnCooldown = false;

        // Si la page n'est plus trackée, on garde la boule cachée
        if (!fireArmed) return;

        // Replacer la boule au "home" (sur la page)
        fireballInstance.transform.SetParent(fireballHome, false);
        fireballInstance.transform.localPosition = Vector3.zero;
        fireballInstance.transform.localRotation = Quaternion.identity;
        fireballInstance.gameObject.SetActive(true);
    }

    public void ApplyDamageTo(bool damagePlayerA, int dmg)
    {
        if (damagePlayerA) playerAHP -= dmg;
        else playerBHP -= dmg;

        playerAHP = Mathf.Max(0, playerAHP);
        playerBHP = Mathf.Max(0, playerBHP);

        Debug.Log($"HP A={playerAHP} | HP B={playerBHP}");

        // TODO: feedback visuel/sonore ici (flash réticule, son impact)
        if (playerAHP == 0 || playerBHP == 0)
            Debug.Log("Game Over");

        float a01 = (float)playerAHP / _playerAMaxHP;
        float b01 = (float)playerBHP / _playerBMaxHP;

        playerAHealthUI?.SetTarget(a01);
        playerBHealthUI?.SetTarget(b01);
    }
}
