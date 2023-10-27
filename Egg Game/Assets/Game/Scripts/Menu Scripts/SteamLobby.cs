using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using TMPro;

public class SteamLobby : NetworkBehaviour
{
    public GameObject hostButton = null;
    public GameObject refreshButton;
    public GameObject lobbyDropdown;
    public GameObject serverNameInputField;
    public GameObject serverPasswordInputField;
    public GameObject background;
    public Toggle passwordProtectedToggle;
    public GameObject serverPasswordJoinInputField;
    public Toggle friendsOnlyToggle;
    public Toggle privateOnlyToggle;

    private CustomNetworkManager customNetworkManager;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;
    private Callback<LobbyMatchList_t> lobbyMatchList; // Callback for lobby list

    public CSteamID createdLobbyID;

    private const string HostAddressKey = "HostAddress";

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

        // Create the lobby match list callback
        lobbyMatchList = Callback<LobbyMatchList_t>.Create(OnLobbyMatchList);

        RefreshLobbies();
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

        background.SetActive(false);
        lobbyDropdown.SetActive(false);

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
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active)
        {
            Debug.Log("Already In Server");
            return;
        }else{
            Debug.Log("Client Joining");
    
            string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
            customNetworkManager.networkAddress = hostAddress;
            // Get the lobby ID
            createdLobbyID = new CSteamID(callback.m_ulSteamIDLobby);
            customNetworkManager.StartClient();
            hostButton.SetActive(false);

            Debug.Log("Entered lobby with host address: " + hostAddress);
        }


    }

public void RefreshLobbies()
{
    Debug.Log("Looking for Lobbies");
    // Clear the existing dropdown options
    lobbyDropdown.GetComponent<TMP_Dropdown>().ClearOptions();

    // Request the lobby list from Steam
    SteamMatchmaking.RequestLobbyList();

    // Iterate through the lobbies and add their names to the dropdown
    List<string> lobbyNames = new List<string>();

    // Add the lobby names to the dropdown
    lobbyDropdown.GetComponent<TMP_Dropdown>().AddOptions(lobbyNames);

    Debug.Log("Added lobby names to the dropdown.");
}

public void TogglePasswordInputField()
{
    // Get the currently selected lobby from the dropdown
    int selectedIndex = lobbyDropdown.GetComponent<TMP_Dropdown>().value;

    // Check if a valid lobby index is selected
    if (selectedIndex >= 0 && selectedIndex < lobbyDropdown.GetComponent<TMP_Dropdown>().options.Count)
    {
        // Get the lobby name from the selected option
        string selectedLobbyName = lobbyDropdown.GetComponent<TMP_Dropdown>().options[selectedIndex].text;

        // Get the CSteamID for the selected lobby
        CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(selectedIndex);

        // Check if this lobby is password-protected
        bool isPasswordProtected = !string.IsNullOrEmpty(SteamMatchmaking.GetLobbyData(lobbyID, "Password"));

        // Disable the password input field if the lobby is not password-protected
        if (serverPasswordJoinInputField != null)
        {
            serverPasswordJoinInputField.SetActive(isPasswordProtected);
        }
    }
}


public void JoinSelectedLobby()
{
    // Get the currently selected lobby from the dropdown
    int selectedIndex = lobbyDropdown.GetComponent<TMP_Dropdown>().value;

    // Check if a valid lobby index is selected
    if (selectedIndex >= 0 && selectedIndex < lobbyDropdown.GetComponent<TMP_Dropdown>().options.Count)
    {
        // Get the lobby name from the selected option
        string selectedLobbyName = lobbyDropdown.GetComponent<TMP_Dropdown>().options[selectedIndex].text;
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
                string enteredPassword = serverPasswordJoinInputField.GetComponent<TMP_InputField>().text;
                Debug.Log("Entered password: " + enteredPassword);

                if (enteredPassword == SteamMatchmaking.GetLobbyData(lobbyID, "Password"))
                {
                    //background.SetActive(false);
                    Debug.Log("Joining lobby: " + selectedLobbyName);
                    // Correct password entered, join the lobby
                    SteamMatchmaking.JoinLobby(lobbyID);

                    background.SetActive(false);
                    lobbyDropdown.SetActive(false);
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

            background.SetActive(false);
            lobbyDropdown.SetActive(false);
        }
    }

    // Call the function to toggle the password input field
    TogglePasswordInputField();
}


    private void OnLobbyMatchList(LobbyMatchList_t callback)
    {
        int numLobbies = (int)callback.m_nLobbiesMatching;

        // Clear the existing dropdown options
        TMP_Dropdown dropdown = lobbyDropdown.GetComponent<TMP_Dropdown>();
        dropdown.ClearOptions();

        for (int i = 0; i < numLobbies; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            string lobbyName = SteamMatchmaking.GetLobbyData(lobbyID, "LobbyName");

            // Check if the lobby is still active (for example, check if it has players)
            // You might need to implement a mechanism to check if the lobby is valid.

            if (!string.IsNullOrEmpty(lobbyName))
            {
                // Add the lobby name to the dropdown
                dropdown.AddOptions(new List<string> { lobbyName });
            }
            else
            {
                // Lobby is not valid, remove it from Steam's lobby list
                SteamMatchmaking.LeaveLobby(lobbyID);
            }
        }

        // Call the function to toggle the password input field
        TogglePasswordInputField();
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

        if (lobbyMatchList != null)
        {
            lobbyMatchList.Dispose();
            lobbyMatchList = null;
        }
    }
}