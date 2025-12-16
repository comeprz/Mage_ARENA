using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    Transform _target;
    float _speed;
    int _damage;
    bool _casterIsA;

    public float hitDistance = 0.12f; // à ajuster selon la taille du marker
    public float lifeTime = 5f;

    public void Init(Transform target, float speed, int damage, bool casterIsA)
    {
        _target = target;
        _speed = speed;
        _damage = damage;
        _casterIsA = casterIsA;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (_target == null) { Destroy(gameObject); return; }

        Vector3 to = _target.position - transform.position;
        float step = _speed * Time.deltaTime;

        if (to.magnitude <= hitDistance)
        {
            // Si caster est A, on damage B, sinon on damage A
            bool damagePlayerA = !_casterIsA;
            DuelManager.I.ApplyDamageTo(damagePlayerA, _damage);
            Destroy(gameObject);
            return;
        }

        transform.position += to.normalized * step;
        transform.rotation = Quaternion.LookRotation(to.normalized);
    }
}
