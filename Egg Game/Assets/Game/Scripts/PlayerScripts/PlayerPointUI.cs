using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;
using Mirror;

public class PlayerPointUI : NetworkBehaviour
{
    private const int MaxPlayers = 6;
    public TMP_Text PlayerName;
    public Image Avatar;
    public Image Point1;
    public Image Point2;
    public Image Point3;
    public Image Point4;
    public Image Point5;
    public Image Point6;
    public Image Point7;
    public Image Point8;
    public Image[] playerBodySprite;
    public GameObject playerObject;
    public Player player;
    private int playerConnectionId;
    public GameObject UIHolderObject;
    public bool playerFound = false;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure that the scale is set to (1, 1, 1)
        transform.localScale = Vector3.one;

        StartCoroutine(TryAssignPlayerTag());
    }

    // Update is called once per frame
    void Update()
    {
        // Assuming Player script has a 'currentScore' variable
        if (player != null)
        {
            // Assuming you want to correlate the score to points 1 to 6
            int currentScore = player.currentScore;

            // Set the color of the corresponding points based on the current score
            SetPointColor(Point1, 1, currentScore);
            SetPointColor(Point2, 2, currentScore);
            SetPointColor(Point3, 3, currentScore);
            SetPointColor(Point4, 4, currentScore);
            SetPointColor(Point5, 5, currentScore);
            SetPointColor(Point6, 6, currentScore);
        }

        if(playerObject == null && playerFound){
            Destroy(gameObject);
        }
        
    }

    IEnumerator TryAssignPlayerTag()
    {
        // Try to assign the player tag until the player object is found
        while (!playerFound)
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                string pointTag = "Point" + i;

                // Check if an object with this tag already exists
                GameObject existingPoints = GameObject.FindGameObjectWithTag(pointTag);

                if (existingPoints == null)
                {
                    // No existing player with this tag, it's available
                    gameObject.tag = pointTag;
                    Debug.Log("Assigned tag: " + pointTag);

                    // Find the player object with the same tag as this UI
                    playerObject = GameObject.FindGameObjectWithTag("Player" + i);

                    if (playerObject != null)
                    {
                        // Get the Player script component from the player object
                        player = playerObject.GetComponent<Player>();

                        Avatar.sprite = playerBodySprite[player.bodySpriteIndex].sprite;

                        // Find UIHolder by name in the "Nathan" scene
                        UIHolderObject = GameObject.Find("PlayerPoint" + i);

                        // Make PlayerPointUI a child of UIHolder
                        transform.SetParent(UIHolderObject.transform, false);

                        // Set the position to (0, 0, 0)
                        transform.localPosition = Vector3.zero;

                        // if (string.IsNullOrEmpty(PlayerName.text))
                        // {
                        //     PlayerName.text = "Player" + i;
                        // }
                        // else
                        // {
                            // Set the player name on the current UI
                            PlayerName.text = player.playerName;
                        //}

                        if (player == null)
                        {
                            Debug.LogError("Player script not found on the player object.");
                        }
                        else
                        {
                            // Player object found, exit the loop
                            playerFound = true;
                            break;
                        }
                    }
                }
            }

            // Wait for a short time before trying again
            yield return new WaitForSeconds(1f);
        }
    }

    void SetPointColor(Image pointImage, int pointNumber, int currentScore)
    {
        // Assuming you want to set the color to white for points equal to or less than the current score
        // You can customize this logic based on your requirements
        if (pointNumber <= currentScore)
        {
            pointImage.color = Color.white;
        }
        else
        {
            // Set the color to a different color if needed
            pointImage.color = Color.black;
        }
    }

    public void SetPlayerConnectionId(int connectionId)
    {
        playerConnectionId = connectionId;
        // Additional logic if needed
    }
}