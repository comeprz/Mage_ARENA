using UnityEngine;
using Vuforia;

public class ArmOnTarget : MonoBehaviour
{
    public bool armValue = true;
    ObserverBehaviour _obs;

    void Awake()
    {
        _obs = GetComponent<ObserverBehaviour>();
        _obs.OnTargetStatusChanged += OnStatus;
    }

    void OnDestroy()
    {
        if (_obs != null) _obs.OnTargetStatusChanged -= OnStatus;
    }

    void OnStatus(ObserverBehaviour behaviour, TargetStatus status)
    {
        bool tracked = status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED;
        DuelManager.I.ArmFire(tracked && armValue);
    }
}
