using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using TMPro;

public class SteamLobby : MonoBehaviour
{
    public GameObject hostButton = null;
    public GameObject refreshButton;
    public TMP_Dropdown lobbyDropdown;
    public GameObject serverNameInputField;
    public GameObject serverPasswordInputField;
    public GameObject background;
    public Toggle passwordProtectedToggle;
    public TMP_InputField serverPasswordJoinInputField;
    public Toggle friendsOnlyToggle;
    public Toggle privateOnlyToggle;

    private CustomNetworkManager customNetworkManager;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    private Callback<LobbyMatchList_t> lobbyMatchList; // Callback for lobby list

    private const string HostAddressKey = "HostAddress";

    private void Start()
    {
        customNetworkManager = GetComponent<CustomNetworkManager>();

        if (!SteamManager.Initialized)
        {
            return;
        }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        // Create the lobby match list callback
        lobbyMatchList = Callback<LobbyMatchList_t>.Create(OnLobbyMatchList);

        RefreshLobbies();
    }

    public void HostLobby()
    {
        //background.SetActive(false);
        ELobbyType lobbyType;

        if (passwordProtectedToggle.isOn)
        {
            Debug.Log("Password Public Lobby");
            // Set it to the appropriate lobby type for password protection.
            lobbyType = ELobbyType.k_ELobbyTypePublic; // Or another appropriate type.
        }
        else if (friendsOnlyToggle.isOn)
        {
            Debug.Log("Friend Lobby");
            lobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;
        }else if (privateOnlyToggle.isOn){
            Debug.Log("Private Lobby");
            lobbyType = ELobbyType.k_ELobbyTypePrivate;
        }
        else
        {
            Debug.Log("Public Lobby");
            lobbyType = ELobbyType.k_ELobbyTypePublic;
        }

        SteamMatchmaking.CreateLobby(lobbyType, customNetworkManager.maxConnections);
    }


    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            hostButton.SetActive(true);
            Debug.LogError("Failed to create lobby. Steam error: " + callback.m_eResult.ToString());
            return;
        }

        customNetworkManager.StartHost();

        // Get the lobby ID
        CSteamID lobbyID = new CSteamID(callback.m_ulSteamIDLobby);

        string serverName = serverNameInputField.GetComponent<TMP_InputField>().text;
        if (string.IsNullOrEmpty(serverName))
        {
            serverName = SteamFriends.GetPersonaName();
        }

        if (passwordProtectedToggle.isOn)
        {
            string passwordName = serverPasswordInputField.GetComponent<TMP_InputField>().text;
            // After creating the lobby, set the password data
            string password = passwordName; // Replace with your desired password.
            SteamMatchmaking.SetLobbyData(lobbyID, "Password", password);

            Debug.Log("Password: " + password);
        }

        // Set lobby data with the host's Steam name
        SteamMatchmaking.SetLobbyData(lobbyID, HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(lobbyID, "LobbyName", serverName );
        Debug.Log("Lobby created successfully!");
        Debug.Log("Lobby ID: " + lobbyID);
        Debug.Log("Host Steam Name: " + serverName);
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active)
        {
            return;
        }

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        customNetworkManager.networkAddress = hostAddress;
        customNetworkManager.StartClient();
        hostButton.SetActive(false);
    }

public void RefreshLobbies()
{
    Debug.Log("Looking for Lobbies");
    // Clear the existing dropdown options
    lobbyDropdown.ClearOptions();

    // Request the lobby list from Steam
    SteamMatchmaking.RequestLobbyList();

    // Iterate through the lobbies and add their names to the dropdown
    List<string> lobbyNames = new List<string>();

    // Add the lobby names to the dropdown
    lobbyDropdown.AddOptions(lobbyNames);

    Debug.Log("Added lobby names to the dropdown.");
}



public void JoinSelectedLobby()
{
    // Get the currently selected lobby from the dropdown
    int selectedIndex = lobbyDropdown.value;

    // Check if a valid lobby index is selected
    if (selectedIndex >= 0 && selectedIndex < lobbyDropdown.options.Count)
    {
        // Get the lobby name from the selected option
        string selectedLobbyName = lobbyDropdown.options[selectedIndex].text;
        Debug.Log("Selected lobby name: " + selectedLobbyName);

        // Get the CSteamID for the selected lobby
        CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(selectedIndex);
        Debug.Log("Selected lobby ID: " + lobbyID);

        // Print lobby data for debugging
        Debug.Log("Lobby Data for " + selectedLobbyName + ": " + SteamMatchmaking.GetLobbyData(lobbyID, "Password"));

        // Check if this lobby is password-protected
        bool isPasswordProtected = !string.IsNullOrEmpty(SteamMatchmaking.GetLobbyData(lobbyID, "Password"));
        Debug.Log("Is Password Protected: " + isPasswordProtected);

        if (isPasswordProtected)
        {
            Debug.Log("Selected index: " + selectedIndex);
            Debug.Log("Selected lobby name: " + selectedLobbyName);

            if (serverPasswordJoinInputField != null)
            {
                // Debug the selected item's name for verification
                Debug.Log("Selected lobby item name: " + selectedLobbyName);

                // Prompt the player for the password
                string enteredPassword = serverPasswordJoinInputField.text;
                Debug.Log("Entered password: " + enteredPassword);

                if (enteredPassword == SteamMatchmaking.GetLobbyData(lobbyID, "Password"))
                {
                    //background.SetActive(false);
                    serverNameInputField.SetActive(false);
                    Debug.Log("Joining lobby: " + selectedLobbyName);
                    // Correct password entered, join the lobby
                    SteamMatchmaking.JoinLobby(lobbyID);
                }
                else
                {
                    Debug.Log("Wrong Password");
                    // Incorrect password entered, handle accordingly (e.g., display an error message)
                    Debug.LogError("Incorrect password entered for the lobby.");
                }
            }
            else
            {
                Debug.LogWarning("Password input field not found for selected lobby.");
            }
            
        }
        else
        {
            //background.SetActive(false);
            Debug.Log("Joining lobby: " + selectedLobbyName);
            // Lobby is not password-protected, join directly
            SteamMatchmaking.JoinLobby(lobbyID);
        }
    }
}


    private void OnLobbyMatchList(LobbyMatchList_t callback)
    {
        int numLobbies = (int)callback.m_nLobbiesMatching;

        // Clear the existing dropdown options
        lobbyDropdown.ClearOptions();

        for (int i = 0; i < numLobbies; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            string lobbyName = SteamMatchmaking.GetLobbyData(lobbyID, "LobbyName");
            bool isPasswordProtected = !string.IsNullOrEmpty(SteamMatchmaking.GetLobbyData(lobbyID, "Password"));

            if (!string.IsNullOrEmpty(lobbyName))
            {
                // Add the lobby name to the dropdown
                lobbyDropdown.AddOptions(new List<string> { lobbyName });

            }
        }
    }
}