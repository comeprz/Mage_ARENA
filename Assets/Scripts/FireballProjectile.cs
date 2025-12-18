using System;
using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    Transform _target;
    float _speed;
    int _damage;
    bool _casterIsA;
    Action _onArrive;

    public bool IsInFlight { get; private set; } = false;

    public float hitDistance = 0.25f;

    public void Launch(Transform target, float speed, int damage, bool casterIsA, Action onArrive)
    {
        _target = target;
        _speed = speed;
        _damage = damage;
        _casterIsA = casterIsA;
        _onArrive = onArrive;

        IsInFlight = true;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (!IsInFlight) return;
        if (_target == null)
        {
            IsInFlight = false;
            _onArrive?.Invoke();
            return;
        }

        Vector3 to = _target.position - transform.position;
        float step = _speed * Time.deltaTime;

        if (to.magnitude <= hitDistance)
        {
            bool damagePlayerA = !_casterIsA;
            DuelManager.I.ApplyDamageTo(damagePlayerA, _damage);

            IsInFlight = false;

            // Option : petit “disparition” visuelle
            // gameObject.SetActive(false);

            _onArrive?.Invoke();
            return;
        }

        transform.position += to.normalized * step;
        transform.rotation = Quaternion.LookRotation(to.normalized);
    }
}
