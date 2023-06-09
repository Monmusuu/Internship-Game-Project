using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
     // public static instance of the GameManager, Initiailized to 'null' to ensure it's empty to start
    public static GameManager Instance = null;
    // public reference of the PauseMenu object to allow access to turn the gameobject on and off when the game is paused
    private GameObject m_pauseMenu;
    private bool m_isPaused;

    public bool GetIsPaused(){return m_isPaused;}
    public void SetIsPaused(bool value){m_isPaused = value;}

    void Awake() {
        // On Awake, create the instance of the GameManager that will be used for the duration of the program
        SetInstance();
    }

    void Start(){
        Time.timeScale = 1;
        m_pauseMenu = GameObject.Find("MenuHolder");
        m_pauseMenu.SetActive(false);
    }



    void SetInstance(){
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
        //DontDestroyOnLoad(gameObject);
    }

    // Function to pause the game and activate the PauseMenu object
    public void PauseGame() {
        // Check to make sure the bool is false, indicating that the game is NOT paused at the moment...
        if (!m_isPaused) {
            // Set the timescale of the game to 0. This will disable actions/movements based on Rigidbodies.
            Time.timeScale = 0;
            // Set the m_isPaused bool to true, indicating a paused gamestate.
            m_isPaused = true;
            // Turn on the PauseMenu gameobject
            m_pauseMenu.SetActive(true);

            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
    }

    // Function to unpause the game and deactivate the PauseMenu object
    public void UnpauseGame() {
        // Check to make sure the bool is true, indicating that the game IS paused at the moment...
        if(m_isPaused) {
            // Set the timescale of the game to 1.  This will enable actions/movements based on Rididbodies.
            Time.timeScale = 1;
            // Set the m_isPaused bool to false, indicating an unpaused gamestate
            m_isPaused = false;
            // Turn off the PauseMenu gameobject
            m_pauseMenu.SetActive(false);
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
