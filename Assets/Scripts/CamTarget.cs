using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTarget : MonoBehaviour
{
    public EggPersonCore core;
    Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - core.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = offset + core.transform.position;
    }
}
