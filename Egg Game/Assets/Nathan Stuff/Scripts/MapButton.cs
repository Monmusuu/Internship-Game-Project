using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MapButton : MonoBehaviour
{
    public TMP_Text voteCountText; // Reference to the text component displaying the vote count
    private int voteCount = 0; // Initial vote count

    private void Start()
    {
        UpdateVoteCountText();
    }

    public void CastVote()
    {
        voteCount++;
        UpdateVoteCountText();
    }

    private void UpdateVoteCountText()
    {
        voteCountText.text = voteCount.ToString();
    }
}
