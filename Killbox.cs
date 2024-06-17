using UnityEngine;
using UnityEngine.SceneManagement;

public class Killbox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if player has entered the killbox
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("SceneMain");
        }
    }
}