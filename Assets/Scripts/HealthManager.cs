using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HealthManager : MonoBehaviour
{
    public float _MaxHealth;
    public UnityEvent OnDeath;
    public float _CurrentHealth;
    public DamageFlash _DamageFlash;
    public float IFrames;
    public bool IsPlayer;
    bool isDead;
    float time;


    private void Start()
    {
        _CurrentHealth = _MaxHealth;
        isDead = false;
    }
    void OnEnable()
    {
        isDead = false;
        _CurrentHealth = _MaxHealth;
    }
    public void UpdateHealth(int damage)
    {
        if (time > IFrames)
        {
            time = 0;
            _CurrentHealth -= damage;
            if (_DamageFlash != null)
            {
                _DamageFlash.Flash();
            }
        }
    }
    void Update()
    {
        time = time + Time.deltaTime;
        if (_CurrentHealth <= 0 && !isDead)
        {
            isDead = true;
            OnDeath.Invoke();
        }
    }
}
