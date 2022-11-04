using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceKeeper : MonoBehaviour
{
    public GameObject target;
    Vector3 offset;
    public bool active = false;
    // Start is called before the first frame update
    void Start()
    {

        offset = transform.position - target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            transform.position = offset + target.transform.position;
        }
    }
}
