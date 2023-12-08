using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using TMPro;

public class LobbyData : MonoBehaviour
{
    public CSteamID lobbyID;
    public string lobbyName;
    public TMP_Text lobbyNameText;
    public TMP_Text playerNumberText;
    public Image lockedImage;
    public GameObject serverPasswordJoinInputField; // Field for players to input the join password
    public GameObject joinButton;

    // Call this method to set the lobby data for this item
    public void SetLobbyData()
    {
        // Assuming lobbyNameText is the TMP_Text component where you want to display the lobby name
        if (lobbyNameText != null)
        {
            lobbyNameText.text = lobbyName;
        }

        // Set up the join button's onClick listener
        if (joinButton != null)
        {
            Button button = joinButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners(); // Clear existing listeners
                button.onClick.AddListener(() => JoinSelectedLobby());
            }
        }

        string password = SteamMatchmaking.GetLobbyData(lobbyID, "Password");
        lockedImage.gameObject.SetActive(!string.IsNullOrEmpty(password));
        serverPasswordJoinInputField.SetActive(!string.IsNullOrEmpty(password));

        // Update the player count
        UpdatePlayerCount();
    }

    public void JoinSelectedLobby()
    {
        // Get the lobby data from the selected lobby item
        CSteamID lobbyID = this.lobbyID;  // Assuming you have a reference to the lobby ID in the LobbyData script
        string selectedLobbyName = this.lobbyName;

        // Print lobby data for debugging
        Debug.Log("Lobby Data for " + selectedLobbyName + ": " + SteamMatchmaking.GetLobbyData(lobbyID, "Password"));

        // Check if this lobby is password-protected
        bool isPasswordProtected = !string.IsNullOrEmpty(SteamMatchmaking.GetLobbyData(lobbyID, "Password"));
        Debug.Log("Is Password Protected: " + isPasswordProtected);

        if (isPasswordProtected)
        {
            Debug.Log("Selected lobby name: " + selectedLobbyName);

            if (serverPasswordJoinInputField != null)
            {
                // Prompt the player for the password
                string enteredPassword = serverPasswordJoinInputField.GetComponent<TMP_InputField>().text;
                Debug.Log("Entered password: " + enteredPassword);

                if (enteredPassword == SteamMatchmaking.GetLobbyData(lobbyID, "Password"))
                {
                    // Correct password entered, join the lobby
                    SteamMatchmaking.JoinLobby(lobbyID);

                    // Disable or hide the UI elements for a clean interface
                    gameObject.SetActive(false);
                }
                else
                {
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
            // Lobby is not password-protected, join directly
            SteamMatchmaking.JoinLobby(lobbyID);

            // Disable or hide the UI elements for a clean interface
            gameObject.SetActive(false);
        }
    }

    private void UpdatePlayerCount()
    {
        if (playerNumberText != null)
        {
            // Get the current player count in the lobby
            int playerCount = SteamMatchmaking.GetNumLobbyMembers(lobbyID);
            playerNumberText.text = playerCount + "/6";
        }
    }
}
