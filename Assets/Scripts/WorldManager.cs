using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public float gravity = -12;
    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity = new Vector3(0, 1 * gravity, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
