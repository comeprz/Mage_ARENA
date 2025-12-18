using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    [SerializeField] Image fillImage; // l'Image UI en mode Filled
    [SerializeField] GameObject root; // optionnel: pour cacher tout le ring

    float _duration;
    float _t;
    bool _running;

    void Awake()
    {
        if (root == null) root = gameObject;
        SetVisible(false);
    }

    public void StartCooldown(float duration)
    {
        _duration = Mathf.Max(0.01f, duration);
        _t = 0f;
        _running = true;
        SetVisible(true);
        SetFill(1f); // au départ: plein, puis se vide
    }

    void Update()
    {
        if (!_running) return;

        _t += Time.deltaTime;
        float p = Mathf.Clamp01(_t / _duration);

        // 1 -> 0 (se vide). Si tu préfères se remplir : SetFill(p)
        SetFill(1f - p);

        if (p >= 1f)
        {
            _running = false;
            SetVisible(false);
        }
    }

    void SetFill(float v)
    {
        if (fillImage != null) fillImage.fillAmount = v;
    }

    void SetVisible(bool v)
    {
        if (root != null) root.SetActive(v);
    }
}
