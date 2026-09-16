using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CameraSensitivity : MonoBehaviour
{
    public FirstPersonController Player;
    public Slider Slider;
    void Update()
    {
        Player.RotationSpeed = Slider.value;
    }
}
