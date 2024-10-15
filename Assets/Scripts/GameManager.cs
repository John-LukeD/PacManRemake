using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject leftWarpNode;
    public GameObject rightWarpNode;

    public AudioSource Siren;

    // Start is called before the first frame update
    void Awake()
    {
        Siren.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
