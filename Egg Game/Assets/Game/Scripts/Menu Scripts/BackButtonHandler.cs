using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class BackButtonHandler : NetworkBehaviour
{
    [SerializeField] private GameObject mapCanvas;
    [SerializeField] private Button backButton;
    private void Start()
    {
        mapCanvas = GameObject.Find("Map Canvas");
    }

    public void OnClickBackButton()
    {
        Debug.Log("Local player clicked Back Button 2.");

        // Find the CharacterSelection script associated with the local player
        CharacterSelection[] characterSelections = FindObjectsOfType<CharacterSelection>();
        foreach (CharacterSelection characterSelection in characterSelections)
        {
            if (characterSelection.isLocalPlayer)
            {
                Debug.Log("Found local player's CharacterSelection script.");

                // Deactivate the map canvas
                if (mapCanvas != null)
                {
                    mapCanvas.SetActive(false);
                    Debug.Log("Map canvas deactivated.");
                }

                // Undo the player's readiness
                characterSelection.CmdSetReadyState(false);
                Debug.Log("Player readiness undone.");
                break; // Stop searching after finding the correct instance
            }
        }
    }
}
