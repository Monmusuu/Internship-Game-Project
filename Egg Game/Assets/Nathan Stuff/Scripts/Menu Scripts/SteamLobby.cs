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
    public Toggle friendsOnlyToggle;

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
        background.SetActive(false);
        serverNameInputField.SetActive(false);

        Debug.Log("Public Lobby");
        ELobbyType lobbyType = ELobbyType.k_ELobbyTypePublic;

        string passwordName = serverPasswordInputField.GetComponent<TMP_InputField>().text;

        if (passwordProtectedToggle.isOn)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(0);
            string password = passwordName; // Replace with your desired password.
            SteamMatchmaking.SetLobbyData(lobbyID, "Password", password);

            Debug.Log("Password " + passwordName);
        }
        else if (friendsOnlyToggle.isOn)
        {
            Debug.Log("Friend Lobby");
            // If friends-only toggle is on, set the lobby type to friends-only.
            lobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;
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
    }

    public void JoinSelectedLobby()
    {
        background.SetActive(false);
        serverNameInputField.SetActive(false);
        
        // Get the currently selected lobby from the dropdown
        int selectedIndex = lobbyDropdown.value;
        
        // Check if a valid lobby index is selected
        if (selectedIndex >= 0 && selectedIndex < lobbyDropdown.options.Count)
        {
            // Get the lobby name from the selected option
            string selectedLobbyName = lobbyDropdown.options[selectedIndex].text;

            Debug.Log("Joining lobby: " + selectedLobbyName);
            background.SetActive(false);
            
            // Iterate through the lobbies to find the one with the matching name
            for (int i = 0; i < lobbyDropdown.options.Count; i++)
            {
                CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
                string lobbyName = SteamMatchmaking.GetLobbyData(lobbyID, "LobbyName");

                if (lobbyName == selectedLobbyName)
                {
                    // Join the selected lobby
                    SteamMatchmaking.JoinLobby(lobbyID);
                    break;
                }
            }
        }
    }

    private void OnLobbyMatchList(LobbyMatchList_t callback)
    {
        int numLobbies = (int)callback.m_nLobbiesMatching;

        // Iterate through the lobbies and add their names to the dropdown
        List<string> lobbyNames = new List<string>();

        for (int i = 0; i < numLobbies; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            string lobbyName = SteamMatchmaking.GetLobbyData(lobbyID, "LobbyName");

            if (!string.IsNullOrEmpty(lobbyName))
            {
                lobbyNames.Add(lobbyName);
            }
        }

        // Add the lobby names to the dropdown
        lobbyDropdown.AddOptions(lobbyNames);
    }
}
