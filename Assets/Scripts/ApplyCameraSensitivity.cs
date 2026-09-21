// ApplyCameraSensitivity.cs — add to the Player prefab
using UnityEngine;
using StarterAssets;

[RequireComponent(typeof(FirstPersonController))]
public class ApplyCameraSensitivity : MonoBehaviour
{
    void Start()
    {
        GetComponent<FirstPersonController>().RotationSpeed =
            PlayerPrefs.GetFloat(CameraSensitivity.PrefKey, CameraSensitivity.DefaultValue);
    }
}