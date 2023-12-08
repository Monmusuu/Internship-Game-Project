using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class SteamLobby : MonoBehaviour
{
    public GameObject hostButton = null;
    public GameObject refreshButton;
    public GameObject serverNameInputField;
    public GameObject serverPasswordInputField;
    public GameObject background;
    public Toggle passwordProtectedToggle;
    public Toggle friendsOnlyToggle;
    public Toggle privateOnlyToggle;
    public TMP_InputField lobbyNameSearchInputField;
    public GameObject lobbiesMenu;
    public GameObject lobbiesDataItemPrefab;
    public GameObject lobbiesListContent;
    public List<GameObject> lisOfLobbies = new List<GameObject>();

    private CustomNetworkManager customNetworkManager;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    private Callback<LobbyMatchList_t> lobbyMatchList; // Callback for lobby list

    public CSteamID createdLobbyID;

    private const string HostAddressKey = "HostAddress";

    // Modify the Start method to include the new callback
    private void Start()
    {
        customNetworkManager = GetComponent<CustomNetworkManager>();

        if (!SteamManager.Initialized)
        {
            Debug.LogError("Steam is not initialized.");
            return;
        }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        lobbyMatchList = Callback<LobbyMatchList_t>.Create(OnLobbyMatchListReceived); // Add this line
        // Request the lobby list
        SteamMatchmaking.RequestLobbyList();
    }

    public void HostLobby()
    {

        ELobbyType lobbyType;

        if (passwordProtectedToggle.isOn)
        {
            // Check if the password is empty or contains only spaces
            string password = serverPasswordInputField.GetComponent<TMP_InputField>().text;
            if (string.IsNullOrWhiteSpace(password))
            {
                // Display an error message or handle it in your game's UI
                Debug.LogError("Password cannot be empty or contain only spaces.");
                return; // Exit the method without creating the lobby
            }

            // Set it to the appropriate lobby type for password protection.
            lobbyType = ELobbyType.k_ELobbyTypePublic; // Or another appropriate type.
        }
        else if (friendsOnlyToggle.isOn)
        {
            Debug.Log("Friend Lobby");
            lobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;
        }
        else if (privateOnlyToggle.isOn)
        {
            Debug.Log("Private Lobby");
            lobbyType = ELobbyType.k_ELobbyTypePrivate;
        }
        else
        {
            Debug.Log("Public Lobby");
            lobbyType = ELobbyType.k_ELobbyTypePublic;
        }

        //background.SetActive(false);
        //lobbyDropdown.SetActive(false);

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

        // Disable the lobby menu and background when the host enters their own game
        lobbiesMenu.SetActive(false);
        background.SetActive(false);

        customNetworkManager.StartHost();

        // Get the lobby ID
        createdLobbyID = new CSteamID(callback.m_ulSteamIDLobby);

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
            SteamMatchmaking.SetLobbyData(createdLobbyID, "Password", password);

            Debug.Log("Password: " + password);
        }

        // Set lobby data with the host's Steam name
        SteamMatchmaking.SetLobbyData(createdLobbyID, HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(createdLobbyID, "LobbyName", serverName );
        Debug.Log("Lobby created successfully!");
        Debug.Log("Lobby ID: " + createdLobbyID);
        Debug.Log("Host Steam Name: " + serverName);

        // Additional debug log
        Debug.Log("Lobby item instantiated for host: " + serverNameInputField.GetComponent<TMP_InputField>().text);
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (Mirror.NetworkServer.active)
        {
            Debug.Log("Already In Server");
            return;
        }
        else
        {
            Debug.Log("Client Joining");

            // Disable the lobby menu and background when entering the game
            lobbiesMenu.SetActive(false);
            background.SetActive(false);

            string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
            customNetworkManager.networkAddress = hostAddress;
            // Get the lobby ID
            createdLobbyID = new CSteamID(callback.m_ulSteamIDLobby);
            customNetworkManager.StartClient();

            Debug.Log("Entered lobby with host address: " + hostAddress);
        }
    }

    public void RefreshLobbyList(){
        SteamMatchmaking.RequestLobbyList();
    }

    public void RequestLeaveLobby()
    {
        if (SteamManager.Initialized)
        {
            // Ensure all players leave the lobby
            SteamMatchmaking.LeaveLobby(createdLobbyID);
        }
    }


    private void OnDestroy()
    {
        // Unsubscribe from Steamworks callbacks in OnDestroy
        if (lobbyCreated != null)
        {
            lobbyCreated.Dispose();
            lobbyCreated = null;
        }

        if (gameLobbyJoinRequested != null)
        {
            gameLobbyJoinRequested.Dispose();
            gameLobbyJoinRequested = null;
        }

        if (lobbyEntered != null)
        {
            lobbyEntered.Dispose();
            lobbyEntered = null;
        }
    }

    public void DestroyLobbies(){
        foreach(GameObject lobbyItem in lisOfLobbies){
            Destroy(lobbyItem);
        }
        lisOfLobbies.Clear();
    }

    private void OnLobbyMatchListReceived(LobbyMatchList_t callback)
    {
        // Extract the lobby IDs from the callback
        List<CSteamID> lobbyIDs = new List<CSteamID>();
        for (int i = 0; i < callback.m_nLobbiesMatching; i++)
        {
            lobbyIDs.Add(SteamMatchmaking.GetLobbyByIndex(i));
        }

        // Now, you can use the lobby IDs to display the list in your UI
        DisplayLobbies(lobbyIDs);
    }



    public void DisplayLobbies(List<CSteamID> lobbyIDs)
    {
        // Clear existing lobby items
        DestroyLobbies();

        foreach (CSteamID lobbyID in lobbyIDs)
        {
            GameObject createdItem = Instantiate(lobbiesDataItemPrefab);
            createdItem.GetComponent<LobbyData>().lobbyID = lobbyID;
            createdItem.GetComponent<LobbyData>().lobbyName = SteamMatchmaking.GetLobbyData(lobbyID, "LobbyName");
            createdItem.GetComponent<LobbyData>().SetLobbyData();

            createdItem.transform.SetParent(lobbiesListContent.transform);
            createdItem.transform.localScale = Vector3.one;

            lisOfLobbies.Add(createdItem);
        }
    }
}