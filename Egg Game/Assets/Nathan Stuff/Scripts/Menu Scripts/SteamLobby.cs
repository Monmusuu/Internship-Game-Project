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
    public Button refreshButton;
    public TMP_Dropdown lobbyDropdown;

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
        hostButton.SetActive(false);
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, customNetworkManager.maxConnections);
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

        // Get the host's Steam name
        string hostSteamName = SteamFriends.GetPersonaName();

        // Set lobby data with the host's Steam name
        SteamMatchmaking.SetLobbyData(lobbyID, HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(lobbyID, "LobbyName", hostSteamName);
        Debug.Log("Lobby created successfully!");
        Debug.Log("Lobby ID: " + lobbyID);
        Debug.Log("Host Steam Name: " + hostSteamName);
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
