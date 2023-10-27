using UnityEngine;
using Mirror;
using Steamworks;

public class DisconnectButton : NetworkBehaviour
{
    public CustomNetworkManager customNetworkManager; // Reference to your CustomNetworkManager.
    private SteamLobby steamLobby; // Reference to the SteamLobby script.

    private void Start()
    {
        customNetworkManager = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        steamLobby = GameObject.Find("NetworkManager").GetComponent<SteamLobby>();
    }

    public void Disconnect()
    {
        if (isServer)
        {
            steamLobby.RequestLeaveLobby();
            customNetworkManager.StopHost();
            Debug.Log("Server disconnecting.");
        }
        else
        {
            steamLobby.RequestLeaveLobby();
            customNetworkManager.StopClient();
            Debug.Log("Client disconnecting.");
        }
    }
}
