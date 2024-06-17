using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    static public bool goalMet = false;

    public TextMeshProUGUI winText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            goalMet = true;
            Time.timeScale = 0;
            winText.gameObject.SetActive(true);
        }
    }

    void Start()
    {
        winText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (goalMet && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            goalMet = false;
            Time.timeScale = 1;
            SceneManager.LoadScene("SceneMain");
        }
    }
}