using UnityEngine;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    public TextMeshProUGUI _HPText;
    public HealthManager _PlayerHealth;
   
    void Update()
    {
        _HPText.text = _PlayerHealth._CurrentHealth.ToString();
    }
}
