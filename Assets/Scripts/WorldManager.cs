using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public float gravity = -15;
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

    private void OnGUI()
    {

        GUI.Label(new Rect(0, 0, 100, 50), "GOAL: " + killedEnemies + " / " + NumEnemies);
        GUI.Label(new Rect(Screen.width - 100, 0, 100, 50), "\"R\" to restart level.");
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
    }
}
