using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class ChangeScene : MonoBehaviour
{
    public void LoadScene(string Game)
    {
        NetworkManager.singleton.ServerChangeScene(Game);
    }
}
