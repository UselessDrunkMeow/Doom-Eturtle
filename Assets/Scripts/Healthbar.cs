using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public RectTransform HealthBar;
    public float _WidthPercentage;
    public HealthManager healthManager;
    void Start()
    {
        //healthManager = FindAnyObjectByType<BossScript>().gameObject.GetComponent<HealthManager>();
    }

    // Update is called once per frame
    void Update()
    {
        float healthPercentage = ((float)healthManager._CurrentHealth / healthManager._MaxHealth) * 100f;
        _WidthPercentage = healthPercentage;
        HealthBar.sizeDelta = new Vector2(_WidthPercentage, 100);
    }
}
