using UnityEngine;

public class DamagePlayerOnTriggerEnter : MonoBehaviour
{
    public GameObject Player;
   
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            Player.GetComponent<HealthManager>().UpdateHealth(1);
        }
    }
}
