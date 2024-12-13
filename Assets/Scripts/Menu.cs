using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    //If click play => enter game (scene 1)
    public void OnPlayButton ()
    {
        SceneManager.LoadScene(1);
    }
}
