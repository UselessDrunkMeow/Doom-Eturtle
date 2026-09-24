// CameraSensitivity.cs  — on the menu slider
using UnityEngine;
using UnityEngine.UI;

public class CameraSensitivity : MonoBehaviour
{
    public const string PrefKey = "CameraSensitivity";
    public const float DefaultValue = 1f;

    public Slider Slider;

    void Start()
    {
        Slider.SetValueWithoutNotify(PlayerPrefs.GetFloat(PrefKey, DefaultValue));
        Slider.onValueChanged.AddListener(OnChanged);
    }

    void OnDestroy()
    {
        if (Slider != null) Slider.onValueChanged.RemoveListener(OnChanged);
    }

    void OnChanged(float value)
    {
        PlayerPrefs.SetFloat(PrefKey, value);
        PlayerPrefs.Save();

        // apply immediately if a player is present (e.g. pause menu)
        var player = Object.FindAnyObjectByType<StarterAssets.FirstPersonController>();
        if (player != null) player.RotationSpeed = value;
    }
}