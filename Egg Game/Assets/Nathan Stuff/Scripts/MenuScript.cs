using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuScript : MonoBehaviour
{
    public GameObject menuFirstButton, settingsFirstButton, settingsClosedButton, controlsButton;
    
    // Reference to the PauseScreen game object
    public GameObject m_menuScreen;
    // Reference to the SettingsScreen game object
    public GameObject m_settingsScreen;

    public GameObject m_controlsScreen;

    // Variables
    [HideInInspector]
    public bool m_menuScreenIsActive = true;
    [HideInInspector]
    public bool m_settingsScreenIsActive = false;

    [HideInInspector]
    public bool m_controlsScreenIsActive = false;

    void Start(){
        m_menuScreen.SetActive(true);
        m_settingsScreen.SetActive(false);
        m_controlsScreen.SetActive(false);
    }

    public void SwitchToSettingsScreen() {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(settingsFirstButton);
        m_menuScreen.SetActive(false);
        m_menuScreenIsActive = false;
        m_controlsScreen.SetActive(false);
        m_controlsScreenIsActive = false;
        m_settingsScreen.SetActive(true);
        m_settingsScreenIsActive = true;
    }
    public void SwitchToMenuScreen() {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuFirstButton);
        m_settingsScreen.SetActive(false);
        m_settingsScreenIsActive = false;
        m_controlsScreen.SetActive(false);
        m_controlsScreenIsActive = false;
        m_menuScreen.SetActive(true);
        m_menuScreenIsActive = true;
    }

    public void SwitchToControls(){
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(controlsButton);
        m_menuScreen.SetActive(false);
        m_menuScreenIsActive = false;
        m_settingsScreen.SetActive(false);
        m_settingsScreenIsActive = false;
        m_controlsScreen.SetActive(true);
        m_controlsScreenIsActive = true;
    }
}
