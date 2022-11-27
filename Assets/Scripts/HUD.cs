using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    private GameObject[] other;
    private bool NotFirst = false;
    WorldManager manager;

    //tutorial stuff
    public bool Tutorialized = false;
    private bool walked;
    private bool runned;
    private bool jumped;
    private bool primed = false;
    private bool swung = false;
    private bool reset = false;

    private bool hasWon = false;

    public GUISkin guiSkin;
    public GUIStyle hudGuiStyle;
    public GUIStyle promptGuiStyle;
    public GUIStyle yellGuiStyle;

    private float Timer = 0f;
    private float Cooldown = 4f;
    private bool isTiming;

    Rect tutorialSpace;
    Rect centralManifold;


    private float ResetTimer = 0f;
    private float ResetCooldown = 20f;

    string[] inBetween = new string[] { 
        "Cool",
        "Good job.",
        "Magnificent.",
        "Nice.", 
        "You can also spam click to swing in circles.", 
        "Perfect.", 
        "NOW GO BE A BAD EGG"};
    int inBetweenCounter = 0;


    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<WorldManager>();
        manager.killedEnemies = 0;
        hudGuiStyle = guiSkin.GetStyle("counter");
        promptGuiStyle = guiSkin.GetStyle("prompt");
        yellGuiStyle = guiSkin.GetStyle("yell");

        tutorialSpace = new Rect(Screen.width / 2 + 150, Screen.height / 3, 200, 200);
        centralManifold = new Rect(Screen.width / 2 - 100, Screen.height / 3, 200, 200);
    }

    // Update is called once per frame
    void Update()
    {
        if (isTiming && !Tutorialized)
        {
            Timer += Time.deltaTime;
            if (Timer >= Cooldown)
            {
                Timer = 0f;
                if(inBetweenCounter == 5)
                {
                    promptGuiStyle = yellGuiStyle;
                    tutorialSpace = centralManifold;
                    inBetweenCounter = 6;

                }
                else if(inBetweenCounter >= 6)
                {
                    isTiming = false;
                    Tutorialized = true;
                }
                else
                {
                    isTiming = false;
                }
            }
        }
    }

    void Awake()
    {

        other = GameObject.FindGameObjectsWithTag("WorldManager");

        foreach (GameObject oneOther in other)
        {
            if (oneOther.scene.buildIndex == -1)
            {
                NotFirst = true;
            }
        }

        if (NotFirst == true)
        {
            Destroy(gameObject);
        }


        DontDestroyOnLoad(this);
        if (manager != null)
        {
            manager.killedEnemies = 0;
        }

    }

    public void SetCounterBold()
    {
        hudGuiStyle = guiSkin.GetStyle("counterBold");
    }

    private void OnGUI()
    {
        GUI.skin = guiSkin;
        GUI.Label(new Rect(0, 0, 200, 200), manager.killedEnemies + " / " + manager.NumEnemies, hudGuiStyle);
        GUI.Label(new Rect(Screen.width - 200, 0, 200, 200), "\"R\"= Restart");

        if (!Tutorialized)
        {
            if (Timer < Cooldown && isTiming)
            {
                GUI.Label(tutorialSpace, inBetween[inBetweenCounter], promptGuiStyle);
            }
            else
            {
                if (!walked)
                {
                    GUI.Label(tutorialSpace, "WASD to walk", promptGuiStyle);
                }
                else if (!runned)
                {
                    GUI.Label(tutorialSpace, "Left SHIFT to running", promptGuiStyle);
                }
                else if (!jumped)
                {
                    GUI.Label(tutorialSpace, "SPACE to (sometimes) jump", promptGuiStyle);
                }
                else if (!primed)
                {

                    GUI.Label(tutorialSpace, "Left Click To Prime a Strike", promptGuiStyle);
                }
                else if (!swung)
                {

                    GUI.Label(tutorialSpace, "Release Left Click To Swing", promptGuiStyle);
                }
                else if (!reset)
                {
                    ResetTimer += Time.deltaTime;
                    if(ResetTimer >= ResetCooldown)
                    {
                        inBetween[5] = "Or dont. Doesnt matter to me.";
                        AckReset();
                    }
                    GUI.Label(tutorialSpace, "\"R\" to reset the level & progress.", promptGuiStyle);
                }
            }
        }
        
        if (hasWon)
        {
            Tutorialized = true;
            GUI.Label(centralManifold, "Congrats!! you're a rotten egg!", yellGuiStyle);
        }

    }

    public void AckWalk()
    {
        if (!isTiming && !walked)
        {
            walked = true;
            isTiming = true;
            Cooldown = 2f;
        }
    }
    public void AckRun()
    {
        if (!isTiming && walked && !runned)
        {
            runned = true;
            isTiming = true;
            inBetweenCounter++;
        }
    }
    public void AckJump()
    {
        if (!isTiming && walked && runned && !jumped)
        {
            jumped = true;
            isTiming = true;
            inBetweenCounter++;
        }
    }
    public void AckPrimed()
    {
        if (!isTiming && walked && runned && jumped && !primed)
        {
            primed = true;
            isTiming = true;
            Cooldown = 1f;
            inBetweenCounter++;
        }
    }

    public void AckSwing()
    {
        if (!isTiming && walked && runned && jumped && primed && !swung)
        {
            inBetweenCounter = 1;
            swung = true;
            isTiming = true;
            Cooldown = 6f;
            inBetweenCounter = 4;
        }
    }

    public void AckReset()
    {
        hudGuiStyle = guiSkin.GetStyle("counter");
        if (!isTiming && walked && runned && jumped && primed && swung && !reset)
        {
            inBetweenCounter = 2;
            swung = true;
            isTiming = true;
            Cooldown = 3f;
            inBetweenCounter = 5;
        }
    }

    public void SetWon()
    {
        hasWon = true;
    }
}
