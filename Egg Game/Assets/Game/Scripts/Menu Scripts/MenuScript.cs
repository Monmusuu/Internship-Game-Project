using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Mirror;

public class MenuScript : MonoBehaviour
{
    // Reference to the PauseScreen game object
    public GameObject m_menuScreen;

    public GameObject m_hostMenuScreen;
    // Reference to the SettingsScreen game object
    public GameObject m_settingsScreen;

    public GameObject m_controlsScreen;

    public GameObject LobbyHostObject;
    public GameObject LobbyListObject;

    // Variables
    [HideInInspector]
    public bool m_menuScreenIsActive = true;

    [HideInInspector]
    public bool m_hostMenuScreenIsActive = false;

    [HideInInspector]
    public bool m_settingsScreenIsActive = false;

    [HideInInspector]
    public bool m_controlsScreenIsActive = false;
    public bool isPause = false;

    private void Start() {
        if(SceneManager.GetActiveScene().name == "CharacterSelection"){
            m_menuScreen.SetActive(false);
        }
        UnityEngine.Cursor.visible = true;
    }

    private void Update() {
        // Check if the current scene is not the "Menu" scene
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name == "CharacterSelection")
        {
            isPause = !isPause;
            if (isPause)
            {
                MenuON();
            }
            else
            {
                MenuOFF();
            }
        }else if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != "CharacterSelection" && SceneManager.GetActiveScene().name != "Menu"){
            if (NetworkServer.active){
                isPause = !isPause;
                if(isPause){
                    MenuONHost();
                }else{
                    MenuOFFHost();
                }

            }else{
                isPause = !isPause;
                if (isPause)
                {
                    MenuON();
                }
                else
                {
                    MenuOFF();
                }
            }
        }
    }

    public void MenuON(){
        m_menuScreen.SetActive(true);
        m_settingsScreen.SetActive(false);
        m_controlsScreen.SetActive(false);
    }

    public void MenuOFF(){
        m_menuScreen.SetActive(false);
        m_settingsScreen.SetActive(false);
        m_controlsScreen.SetActive(false);
    }

    public void MenuONHost(){
        m_hostMenuScreen.SetActive(true);
        m_settingsScreen.SetActive(false);
        m_controlsScreen.SetActive(false);
    }

    public void MenuOFFHost(){
        m_hostMenuScreen.SetActive(false);
        m_settingsScreen.SetActive(false);
        m_controlsScreen.SetActive(false);
    }

    public void SwitchToHostMenuScreen() {
        m_settingsScreen.SetActive(false);
        m_settingsScreenIsActive = false;
        m_controlsScreen.SetActive(false);
        m_controlsScreenIsActive = false;
        m_hostMenuScreen.SetActive(true);
        m_hostMenuScreenIsActive = true;
    }

    public void SwitchToSettingsScreen() {
        m_menuScreen.SetActive(false);
        m_menuScreenIsActive = false;
        m_hostMenuScreen.SetActive(false);
        m_hostMenuScreenIsActive = false;
        m_controlsScreen.SetActive(false);
        m_controlsScreenIsActive = false;
        m_settingsScreen.SetActive(true);
        m_settingsScreenIsActive = true;
    }
    public void SwitchToMenuScreen() {
        m_settingsScreen.SetActive(false);
        m_settingsScreenIsActive = false;
        m_controlsScreen.SetActive(false);
        m_controlsScreenIsActive = false;
        m_menuScreen.SetActive(true);
        m_menuScreenIsActive = true;
    }


    public void SwitchToControls(){
        m_menuScreen.SetActive(false);
        m_menuScreenIsActive = false;
        m_hostMenuScreen.SetActive(false);
        m_hostMenuScreenIsActive = false;
        m_settingsScreen.SetActive(false);
        m_settingsScreenIsActive = false;
        m_controlsScreen.SetActive(true);
        m_controlsScreenIsActive = true;
    }

    public void SwitchToLobby(){
        LobbyHostObject.SetActive(true);
        LobbyListObject.SetActive(true);
    }

    public void SwitchToMenu(){
        LobbyHostObject.SetActive(false);
        LobbyListObject.SetActive(false);
    }

}
