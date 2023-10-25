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
        if (customNetworkManager.isNetworkActive)
        {
            if (isServer)
            {
                if (steamLobby != null)
                {
                    steamLobby.LeaveLobby(); // Call the LeaveLobby method from the SteamLobby script.
                }
                customNetworkManager.StopHost(); // Stop the host, which includes stopping the server.
            }
            else if (isClient)
            {
                if (steamLobby != null)
                {
                    steamLobby.LeaveLobby(); // Call the LeaveLobby method from the SteamLobby script.
                }
                customNetworkManager.StopClient();
                
            }
        }
    }
}
