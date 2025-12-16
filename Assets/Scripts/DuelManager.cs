using UnityEngine;

public class DuelManager : MonoBehaviour
{
    public static DuelManager I;

    [Header("References")]
    public Transform playerATarget; // transform du marker torse A
    public Transform playerBTarget; // transform du marker torse B
    public Transform firePage;      // transform du marker page Feu

    [Header("Prefabs")]
    public GameObject fireballPrefab;

    [Header("Gameplay")]
    public int playerAHP = 100;
    public int playerBHP = 100;
    public float fireballSpeed = 2.0f;
    public int fireballDamage = 15;

    [Header("State")]
    public bool fireArmed = false;
    public bool playerAIsCaster = true; // pour MVP: toggle qui lance

    void Awake()
    {
        I = this;
    }

    public void ArmFire(bool armed)
    {
        fireArmed = armed;
        Debug.Log($"Fire armed = {fireArmed}");
    }

    // Bouton (fake voix) -> lance
    public void CastFire()
    {
        if (!fireArmed)
        {
            Debug.Log("Fire not armed (page not detected).");
            return;
        }

        Transform caster = playerAIsCaster ? playerATarget : playerBTarget;
        Transform target = playerAIsCaster ? playerBTarget : playerATarget;

        if (caster == null || target == null)
        {
            Debug.LogWarning("Caster/Target missing (targets not detected or not assigned).");
            return;
        }

        // Spawn depuis la page Feu si tu veux (stylé), ou depuis le caster.
        Vector3 spawnPos = firePage != null ? firePage.position : caster.position;
        Quaternion spawnRot = Quaternion.LookRotation((target.position - spawnPos).normalized);

        GameObject go = Instantiate(fireballPrefab, spawnPos, spawnRot);
        var proj = go.AddComponent<FireballProjectile>();
        proj.Init(target, fireballSpeed, fireballDamage, playerAIsCaster);
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
    }
}
