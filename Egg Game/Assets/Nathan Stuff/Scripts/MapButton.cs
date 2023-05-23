using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MapButton : MonoBehaviour
{
    public TMP_Text voteCountText; // Text component to display the vote count
    private HashSet<int> votedCursorIds = new HashSet<int>(); // Set to store the voted cursor IDs

    public int VoteCount => votedCursorIds.Count; // Get the current vote count

    public void ToggleVote(int cursorId)
    {
        if (votedCursorIds.Contains(cursorId))
        {
            votedCursorIds.Remove(cursorId); // Remove the cursor ID from the set of voted cursor IDs
        }
        else
        {
            votedCursorIds.Add(cursorId); // Add the cursor ID to the set of voted cursor IDs
        }

        UpdateVoteCountText(); // Update the vote count text
    }

    private void UpdateVoteCountText()
    {
        if (voteCountText != null)
        {
            voteCountText.text = VoteCount.ToString(); // Update the vote count text
        }
    }
}