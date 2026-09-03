using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HealthManager : MonoBehaviour
{
    public float _MaxHealth;
    public UnityEvent OnDeath;
    public float _CurrentHealth;
    private void Start()
    {
        _CurrentHealth = _MaxHealth;
    }
    void Update()
    {
        if(_CurrentHealth <= 0)
        {
            OnDeath.Invoke();
        }
    }
}
