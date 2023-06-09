using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class VotingSystem : MonoBehaviour
{
    public int numMaps = 5; // Number of available maps
    public TextMeshProUGUI[] voteCountTexts; // Array to store the TextMeshProUGUI components for displaying vote counts

    private Dictionary<int, bool> hasVoted; // Dictionary to track whether each player has voted
    private Dictionary<int, int> selectedMapIndex; // Dictionary to store the selected map index for each player
    private int[] voteCounts; // Array to store the vote counts for each map

    private void Start()
    {
        InitializeVotingSystem();
    }

    public void Vote(int playerID, int mapIndex)
    {
        if (hasVoted.ContainsKey(playerID) && !hasVoted[playerID])
        {
            // Increment the vote count for the selected map
            voteCounts[mapIndex]++;
            Debug.Log("Player " + playerID + " voted for map " + mapIndex);

            // Set the selected map index for the player and mark them as voted
            selectedMapIndex[playerID] = mapIndex;
            hasVoted[playerID] = true;

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
    }

    public void UndoVote(int playerID)
    {
        if (hasVoted.ContainsKey(playerID) && hasVoted[playerID])
        {
            int selectedMap = selectedMapIndex[playerID];

            // Decrement the vote count for the selected map
            voteCounts[selectedMap]--;
            Debug.Log("Player " + playerID + " vote for map " + selectedMap + " undone.");

            // Reset the selected map index and mark the player as not voted
            selectedMapIndex[playerID] = -1;
            hasVoted[playerID] = false;

            // Update the vote count display
            UpdateVoteCountDisplay();
        }
    }

    public bool HasVoted(int playerID)
    {
        if (hasVoted.ContainsKey(playerID))
        {
            return hasVoted[playerID];
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
        for (int i = 0; i < PlayerSaveData.playerNumber; i++)
        {
            hasVoted.Add(i, false);
            selectedMapIndex.Add(i, -1);
        }

        // Update the vote count display
        UpdateVoteCountDisplay();
    }

    private void UpdateVoteCountDisplay()
    {
        for (int i = 0; i < numMaps; i++)
        {
            voteCountTexts[i].text = voteCounts[i].ToString();
        }
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
        string[] sceneNames = { "Nathan", "MapScene2", "MapScene3", "MapScene4", "MapScene5" };

        // Check if the map index is within the valid range
        if (mapIndex >= 0 && mapIndex < sceneNames.Length)
        {
            string sceneName = sceneNames[mapIndex];
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Invalid map index: " + mapIndex);
        }
    }
}