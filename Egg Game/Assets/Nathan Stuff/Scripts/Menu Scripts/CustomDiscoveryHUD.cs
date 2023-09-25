using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using Mirror.Discovery;

public class CustomDiscoveryHUD : MonoBehaviour
{
    public NetworkDiscovery networkDiscovery;
    public Button refreshButton;
    public Button hostButton;
    public TMP_Dropdown serverDropdown;
    public GameObject dropdownItemPrefab;
    public Button joinButton; // Reference to the join button

    private List<ServerResponse> discoveredServers = new List<ServerResponse>();

    // Start is called before the first frame update
    void Start()
    {
        networkDiscovery.OnServerFound.AddListener(OnDiscoveredServer);
        refreshButton.onClick.AddListener(RefreshServerList);
        hostButton.onClick.AddListener(StartServerAndAdvertise);
        joinButton.onClick.AddListener(JoinSelectedServer); // Add a click event for joining

        // Initially, disable the join button until a server is selected
        joinButton.interactable = false;
    }

    void Connect(ServerResponse info)
    {
        // Implement your logic to connect to the selected game here
        Debug.Log("Connecting to game at " + info.EndPoint.Address);

        networkDiscovery.StopDiscovery();
        NetworkManager.singleton.StartClient(info.uri);

    }

    public void RefreshServerList()
    {
        // Clear the list of discovered servers
        discoveredServers.Clear();

        // Clear the TMP Dropdown options
        serverDropdown.ClearOptions();

        // Restart the discovery process
        networkDiscovery.StartDiscovery();

        Debug.Log("Finding Servers");

        // Disable the join button until a server is selected
        joinButton.interactable = false;
    }

    public void OnDiscoveredServer(ServerResponse info)
    {
        // You can check the versioning or other criteria here
        discoveredServers.Add(info);

        // Create a new dropdown item for this server
        CreateDropdownItem(info);
    }

    void CreateDropdownItem(ServerResponse serverInfo)
    {
        // Create a new option data for the TMP Dropdown
        TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(serverInfo.EndPoint.Address.ToString());

        // Add the option data to the TMP Dropdown's options list
        serverDropdown.options.Add(optionData);

        // Refresh the TMP Dropdown to update the visible options
        serverDropdown.RefreshShownValue();

        // Enable the join button now that a server is available
        joinButton.interactable = true;
    }

    void JoinSelectedServer()
    {
        int selectedIndex = serverDropdown.value;
        if (selectedIndex >= 0 && selectedIndex < discoveredServers.Count)
        {
            ServerResponse selectedServer = discoveredServers[selectedIndex];
            Connect(selectedServer);
        }
    }

    void StartServerAndAdvertise()
    {
        // Start your Mirror game server logic here (e.g., NetworkServer.Listen(...))

        // Advertise the server to the network
        networkDiscovery.AdvertiseServer();

        Debug.Log("Advertise Servers");
        // You can optionally perform additional setup and logic here
    }
}
