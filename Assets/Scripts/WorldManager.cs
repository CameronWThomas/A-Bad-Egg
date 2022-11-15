using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public float gravity = -12;
    public float mouseSensitivity = 10;
    public bool development = false;
    public int NumEnemies;
    public int killedEnemies;
    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity = new Vector3(0, 1 * gravity, 0);
        NumEnemies = GameObject.FindObjectsOfType<NpcController>().Length;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
