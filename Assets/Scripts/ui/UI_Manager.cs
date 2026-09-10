using UnityEngine;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    public TextMeshPro _HPText;
    public HealthManager _PlayerHealth;
   
    void Update()
    {
        _HPText.text = _PlayerHealth.ToString();
    }
}
