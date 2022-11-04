using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeildedMace : MonoBehaviour
{
    SkinnedMeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
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
