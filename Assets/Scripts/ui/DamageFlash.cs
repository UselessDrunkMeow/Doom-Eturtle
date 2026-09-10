using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageFlash : MonoBehaviour
{
    public float _Speed;
    public float baseAlpha;
    public bool doFlash;
    Image flash;
    void Start()
    {
        flash = GetComponent<Image>();             
    }
    private void Update()
    {    
        if(doFlash == true)
        {
            Color c = flash.color;
            c.a -= _Speed * Time.deltaTime;

            if (c.a <= 0)
            {
                doFlash = false;
            }
            flash.color = c;
        }
        
    }
    public void Flash()
    {
        Color c = flash.color;
        c.a = baseAlpha;
        flash.color = c;
        doFlash = true;
    }
}
