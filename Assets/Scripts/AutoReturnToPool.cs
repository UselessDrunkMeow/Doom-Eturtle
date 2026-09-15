using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AutoReturnToPool : MonoBehaviour
{
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        // When particle finishes playing, disable itself so ObjectPool can re-use it
        if (ps != null && !ps.IsAlive(true))
        {
            gameObject.SetActive(false);
        }
    }
}