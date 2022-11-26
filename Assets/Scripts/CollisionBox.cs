using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionBox : MonoBehaviour
{
    MeshRenderer renderer;
    WorldManager manager;
    public bool suppressHide = false;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        manager = GameObject.FindObjectOfType<WorldManager>();
        if(manager != null && renderer != null)
        {
            if (!manager.development && !suppressHide)
            {
                renderer.enabled = false;
            }
        } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
