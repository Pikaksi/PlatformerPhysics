using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPointTrigger : MonoBehaviour
{
    [SerializeField] float xSpawnOffset = 0f;
    [SerializeField] float ySpawnOffset = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Respawn>().SetRespawnPoint(transform.position + new Vector3(xSpawnOffset, ySpawnOffset, 0));
        }
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = new Color(1, 0, 0, 1f);
        Gizmos.DrawWireSphere(transform.position + new Vector3(xSpawnOffset, ySpawnOffset, 0), 0.5f);
    }
}
