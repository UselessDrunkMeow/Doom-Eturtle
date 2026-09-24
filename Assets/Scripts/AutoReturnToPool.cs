using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AutoReturnToPool : MonoBehaviour
{
    private ParticleSystem ps;
    [SerializeField] private AudioSource audio;
    [SerializeField] private AudioClip explosion;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        
    }
    void OnEnable()
    {
        if(audio == null)
        {
            Debug.Log("no audio");
        }
        else
        {
            audio.clip = explosion;
            audio.Play();
        }
        
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