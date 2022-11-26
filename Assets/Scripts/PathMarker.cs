using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMarker : MonoBehaviour
{
    public int family;

    public int order;
    WorldManager manager;
    MeshRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindObjectOfType<WorldManager>();
        renderer = GetComponent<MeshRenderer>();
        if (manager != null && renderer != null)
        {
            if (!manager.development) 
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
