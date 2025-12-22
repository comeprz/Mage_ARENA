using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] RectTransform fillRect; // HP_Fill RectTransform
    [SerializeField] Image fillImage;        // HP_Fill Image (pour la couleur)
    [SerializeField] float smooth = 10f;

    float _current01 = 1f;
    float _target01 = 1f;

    public void SetInstant(float normalized01)
    {
        _current01 = _target01 = Mathf.Clamp01(normalized01);
        Apply(_current01);
    }

    public void SetTarget(float normalized01)
    {
        _target01 = Mathf.Clamp01(normalized01);
    }

    void Update()
    {
        _current01 = Mathf.Lerp(_current01, _target01, Time.deltaTime * smooth);
        Apply(_current01);
    }

    void Apply(float v)
    {
        // Animation largeur (scale X)
        if (fillRect != null)
        {
            var s = fillRect.localScale;
            s.x = Mathf.Clamp01(v);
            fillRect.localScale = s;
        }

        // Couleur selon % HP (optionnel)
        if (fillImage != null)
        {
            if (v > 0.6f) fillImage.color = Color.green;
            else if (v > 0.3f) fillImage.color = Color.yellow;
            else fillImage.color = Color.red;
        }
    }
}
