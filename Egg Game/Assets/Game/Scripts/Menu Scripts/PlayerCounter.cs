using UnityEngine;
using Mirror;

public class PlayerCounter : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnPlayerCountChanged))]
    public int playerCount = 0;

    // Callback method for the hook in the SyncVar
    void OnPlayerCountChanged(int oldValue, int newValue)
    {
        playerCount = newValue;

        // Additional logic to handle player count changes on clients if needed
        Debug.Log("Player count changed to: " + playerCount);
    }
}
