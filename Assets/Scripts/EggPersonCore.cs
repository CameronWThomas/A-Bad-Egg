using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggPersonCore : MonoBehaviour
{

    public EggPersonCoreCollider m_Collider;
    public Rigidbody rbody;
    // Start is called before the first frame update
    void Start()
    {
        m_Collider = GetComponentInChildren<EggPersonCoreCollider>();
        rbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
