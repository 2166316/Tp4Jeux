using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinController : MonoBehaviour
{
    public Button mainMenuButton;
    public GameObject baseCanvas;
    public GameObject winCanvas;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuButton.onClick.AddListener(Disconnect);
    }

    public void Disconnect()
    {
        if (NetworkManager.Singleton != null)
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.Shutdown();
            }
        }

        // Load the main menu scene
        SceneManager.LoadScene("GameScene");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered the trigger is the player
        if (other.CompareTag("Player"))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            baseCanvas.SetActive(false);
            winCanvas.SetActive(true);
        }
    }
}
