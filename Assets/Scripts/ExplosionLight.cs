using UnityEngine;

// Put this on a Point Light that is a child of an explosion prefab.
// Every time the explosion is enabled (also when it comes out of the object pool)
// the light flashes to Peak Intensity and fades out over Duration seconds.
[RequireComponent(typeof(Light))]
public class ExplosionLight : MonoBehaviour
{
    [Header("Flash")]
    [Tooltip("How bright the flash is at its peak. 0 = use the intensity set on the Light component.")]
    [SerializeField] private float peakIntensity = 0f;

    [Tooltip("How long the flash lasts in seconds.")]
    [SerializeField] private float duration = 0.3f;

    [Tooltip("Brightness over time (0 = start, 1 = end). Value 1 = full brightness.")]
    [SerializeField] private AnimationCurve fade = new AnimationCurve(
        new Keyframe(0f, 1f, 0f, -3f),
        new Keyframe(1f, 0f, 0f, 0f));

    [Header("Optional")]
    [Tooltip("Colour over time. Leave white to keep the Light's own colour.")]
    [SerializeField] private Gradient colorOverTime = new Gradient();

    [Tooltip("Also shrink the light's range as it fades.")]
    [SerializeField] private bool shrinkRange = false;

    private Light lightSource;
    private float baseIntensity;
    private float baseRange;
    private Color baseColor;
    private float timer;
    private bool playing;

    private void Awake()
    {
        lightSource = GetComponent<Light>();
        baseIntensity = peakIntensity > 0f ? peakIntensity : lightSource.intensity;
        baseRange = lightSource.range;
        baseColor = lightSource.color;
    }

    private void OnEnable()
    {
        Flash();
    }

    private void OnDisable()
    {
        playing = false;
        lightSource.enabled = false;
    }

    // Can also be called from other scripts to re-trigger the flash.
    public void Flash()
    {
        timer = 0f;
        playing = true;
        lightSource.enabled = true;
        Apply(0f);
    }

    private void Update()
    {
        if (!playing) return;

        timer += Time.deltaTime;
        float t = duration > 0f ? timer / duration : 1f;

        if (t >= 1f)
        {
            playing = false;
            lightSource.enabled = false;
            return;
        }

        Apply(t);
    }

    private void Apply(float t)
    {
        float k = Mathf.Max(0f, fade.Evaluate(t));
        lightSource.intensity = baseIntensity * k;
        lightSource.color = baseColor * colorOverTime.Evaluate(t);
        if (shrinkRange)
        {
            lightSource.range = Mathf.Max(0.01f, baseRange * k);
        }
    }
}
