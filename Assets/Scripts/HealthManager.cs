using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HealthManager : MonoBehaviour
{
    public float _MaxHealth;
    public UnityEvent OnDeath;
    float currentHealth;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth <= 0)
        {
            OnDeath.Invoke();
        }
    }
}
