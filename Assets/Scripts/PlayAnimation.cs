using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
    public void PlayAnimationFunction(Animation anim)
    {
        anim = GetComponent<Animation>();
        foreach (AnimationState state in anim)
        {
            state.speed = 1F;
        }
    }
}
