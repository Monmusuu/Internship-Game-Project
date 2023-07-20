using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror;

public class VotingSystem : NetworkBehaviour
{
    public int numMaps = 5; // Number of available maps
    [SerializeField] private TextMeshProUGUI[] voteCountTexts; // Array to store the TextMeshProUGUI components for displaying vote counts

    private Dictionary<int, bool> hasVoted; // Dictionary to track whether each player has voted
    private Dictionary<int, int> selectedMapIndex; // Dictionary to store the selected map index for each player
    private int[] voteCounts; // Array to store the vote counts for each map

    public static VotingSystem Instance { get; private set; }

    private void Start()
    {
        Instance = this;

        InitializeVotingSystem();
    }

    public void Vote(int connectionId, int mapIndex)
    {
        if (!hasVoted.ContainsKey(connectionId) || !hasVoted[connectionId])
        {
            // Increment the vote count for the selected map
            voteCounts[mapIndex]++;
            Debug.Log("Player with connection ID " + connectionId + " voted for map " + mapIndex);

            // Set the selected map index for the player and mark them as voted
            selectedMapIndex[connectionId] = mapIndex;

            if (!hasVoted.ContainsKey(connectionId))
            {
                hasVoted.Add(connectionId, true);
            }
            else
            {
                hasVoted[connectionId] = true;
            }

            // Check if all players have voted
            if (AllPlayersVoted())
            {
                // Determine the map with the most votes
                int winningMapIndex = GetWinningMapIndex();

                // Switch to the corresponding scene
                SwitchToMapScene(winningMapIndex);
            }

            // Update the vote count display
            UpdateVoteCountDisplay();
        }
        else
        {
            // Player already voted, so undo the vote
            UndoVote(connectionId);
        }
    }


    public void UndoVote(int connectionId)
    {
        if (hasVoted[connectionId])
        {
            int selectedMap = selectedMapIndex[connectionId];

            // Decrement the vote count for the selected map
            voteCounts[selectedMap]--;
            Debug.Log("Player with connection ID " + connectionId + " vote for map " + selectedMap + " undone.");

            // Reset the selected map index and mark the player as not voted
            selectedMapIndex[connectionId] = -1;
            hasVoted[connectionId] = false;

            // Update the vote count display
            UpdateVoteCountDisplay();
        }
    }

    public void ClearVote(int connectionId)
    {
        if (hasVoted[connectionId])
        {
            int selectedMap = selectedMapIndex[connectionId];

            // Decrement the vote count for the selected map
            voteCounts[selectedMap]--;

            // Reset the selected map index and mark the player as not voted
            selectedMapIndex[connectionId] = -1;
            hasVoted[connectionId] = false;

            // Update the vote count display
            UpdateVoteCountDisplay();
        }
    }

    public bool HasVoted(int connectionId)
    {
        if (hasVoted.ContainsKey(connectionId))
        {
            return hasVoted[connectionId];
        }
        return false;
    }

    private void InitializeVotingSystem()
    {
        // Initialize the dictionaries for each player
        hasVoted = new Dictionary<int, bool>();
        selectedMapIndex = new Dictionary<int, int>();

        // Initialize the vote counts array
        voteCounts = new int[numMaps];

        // Initialize each player's vote status and selected map index
        foreach (var connection in NetworkServer.connections)
        {
            int connectionId = connection.Value.connectionId;
            hasVoted.Add(connectionId, false);
            selectedMapIndex.Add(connectionId, -1);
        }

        // Update the vote count display
        UpdateVoteCountDisplay();
    }

    private void UpdateVoteCountDisplay()
    {
        for (int i = 0; i < numMaps; i++)
        {
            voteCountTexts[i].text = voteCounts[i].ToString();
            RpcUpdateVoteCountDisplay(i, voteCounts[i].ToString());
        }
    }

    [ClientRpc]
    private void RpcUpdateVoteCountDisplay(int mapIndex, string voteCount)
    {
        voteCountTexts[mapIndex].text = voteCount;
    }

    private bool AllPlayersVoted()
    {
        foreach (var player in hasVoted)
        {
            if (!player.Value)
            {
                return false;
            }
        }
        return true;
    }

    private int GetWinningMapIndex()
    {
        int maxVotes = 0;
        List<int> winningMaps = new List<int>();

        for (int i = 0; i < numMaps; i++)
        {
            if (voteCounts[i] > maxVotes)
            {
                maxVotes = voteCounts[i];
                winningMaps.Clear();
                winningMaps.Add(i);
            }
            else if (voteCounts[i] == maxVotes)
            {
                winningMaps.Add(i);
            }
        }

        if (winningMaps.Count == 1)
        {
            return winningMaps[0];
        }
        else
        {
            // Randomly choose one of the winning maps if there is a tie
            int randomIndex = Random.Range(0, winningMaps.Count);
            int winningMapIndex = winningMaps[randomIndex];

            // Check if the winning map was randomly selected
            bool isRandomSelection = winningMaps.Count > 1;

            // Debug the winning map information
            Debug.Log("Winning Map: " + winningMapIndex + " (Randomly Selected: " + isRandomSelection + ")");

            return winningMapIndex;
        }
    }

    private void SwitchToMapScene(int mapIndex)
    {
        // Replace "MapScene1", "MapScene2", etc. with the actual scene names for each map index
        string[] sceneNames = { "Nathan", "MapScene2", "MapScene3", "MapScene4", "MapScene5", "MapScene6" };

        // Check if the map index is within the valid range
        if (mapIndex >= 0 && mapIndex < sceneNames.Length)
        {
            string sceneName = sceneNames[mapIndex];
            Debug.Log("Switching to scene: " + sceneName);

            // Call the ServerChangeScene method on the NetworkManager to change the scene
            if (NetworkManager.singleton != null)
            {
                NetworkManager.singleton.ServerChangeScene(sceneName);
            }
            else
            {
                Debug.LogError("NetworkManager not found. Make sure you have a NetworkManager in your scene.");
            }
        }
        else
        {
            Debug.LogError("Invalid map index: " + mapIndex);
        }
    }
}

