using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Respawn : MonoBehaviour
{
    [SerializeField] Vector3 currentRespawnPoint;

    private void Start() 
    {
        currentRespawnPoint = transform.position;
    }
    
    public void ProcessDeath()
    {
        SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
    }

    public void RespawnPointRespawn()
    {
        transform.position = currentRespawnPoint;
    }

    public void SetRespawnPoint(Vector3 newRespawnPoint)
    {
        currentRespawnPoint = newRespawnPoint;
    }
}
