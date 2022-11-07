using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeildedMace : MonoBehaviour
{
    public SkinnedMeshRenderer meshRenderer;
    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        rb  = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVisible(bool isVisible)
    {
        meshRenderer.enabled = isVisible;
        
    }
}
