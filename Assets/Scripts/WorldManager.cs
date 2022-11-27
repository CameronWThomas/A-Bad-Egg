using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour
{
    public float gravity = -15;
    public float mouseSensitivity = 10;
    public bool development = false;
    public int NumEnemies;
    public int killedEnemies;
    public HUD hud;
    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity = new Vector3(0, 1 * gravity, 0);
        NumEnemies = GameObject.FindObjectsOfType<NpcController>().Length;
        hud = GetComponent<HUD>();
    }

    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            hud.AckReset();
            killedEnemies = 0;
            SceneManager.LoadScene(Application.loadedLevel);
        }
    }
    private void Awake()
    {

        killedEnemies = 0;
    }
    public void KilledEnemy()
    {
        killedEnemies++;
        if(killedEnemies >= NumEnemies / 2)
        {
            hud.SetCounterBold();
        }
        if(killedEnemies >= NumEnemies)
        {
            hud.SetWon();
        }
    }
}
