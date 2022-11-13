using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuetilitySoftbody;

public class ShatteredEggPersonController : MonoBehaviour
{
    //public Vector3 InitialForce;
    public Vector3 ImpactPoint;
    public float ImpactSpeed;
    Rigidbody[] parts;
    public List<Vector3> partForce = new List<Vector3>();
    public bool enabled = false;
    YueSoftbodyPhysics softBody;
    // Start is called before the first frame update
    void Start()
    {

        softBody = GetComponentInChildren<YueSoftbodyPhysics>();
        softBody.enabled = false;
        parts = GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody part in parts)
        {
            partForce.Add(Vector3.zero);
            part.useGravity = false;
            part.isKinematic = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //InitialForce = Vector3.Lerp(InitialForce, targetForce, Time.deltaTime);
        if (enabled)
        {
            if (ImpactSpeed > 0.5f)
            {
                ImpactSpeed = Mathf.Lerp(ImpactSpeed, 0, Time.deltaTime * 4);
                for (var i = 0; i < parts.Length; i++)
                {
                    Rigidbody part = parts[i];
                    if (partForce[i] == Vector3.zero)
                    {

                        Vector3 away = (part.transform.position - ImpactPoint).normalized;


                        partForce[i] = away * ImpactSpeed;

                    }

                    part.AddForce(partForce[i]);
                }
            }
            else
            {
                ImpactSpeed = 0f;
            }
        }

    }

    public void EnableGravity()
    {
        foreach (Rigidbody part in parts)
        {
            part.useGravity = true;
            part.isKinematic = false;
        }

        softBody.enabled = true;
        enabled = true;

    }
}
