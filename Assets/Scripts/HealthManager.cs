using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HealthManager : MonoBehaviour
{
    public float _MaxHealth;
    public UnityEvent OnDeath;
    public float _CurrentHealth;
    public DamageFlash _DamageFlash;
    private void Start()
    {
        _CurrentHealth = _MaxHealth;
    }
    void OnEnable()
    {
        _CurrentHealth = _MaxHealth;
    }
    public void UpdateHealth(int damage)
    {
        _CurrentHealth -= damage;
        if(_DamageFlash != null)
        {
            _DamageFlash.Flash();
        }
    }
    void Update()
    {
        if(_CurrentHealth <= 0)
        {
            OnDeath.Invoke();
        }
    }
}
