using Unity.VisualScripting;
using UnityEngine;


public class PlayAnimation : MonoBehaviour
{
    private bool IsRunning = false;
    public Animator anim;
    public void PlayAnimationFunction(string Animation)
    {
        if (Animation == "TakeDamage")
        {
            anim.SetTrigger("Damage");
        }
        else if(Animation == "Death")
        {
            anim.SetTrigger("Death");
        }
        else if(Animation == "Attack")
        {
            anim.SetTrigger("Attack");
        }
        else if (Animation == "Run")
        {
            IsRunning = !IsRunning;
            anim.SetBool("IsRunning", IsRunning);
        }
    }
}
