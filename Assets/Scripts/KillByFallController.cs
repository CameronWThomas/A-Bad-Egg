using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillByFallController : MonoBehaviour
{
    public EggPersonController me;
    public float timer = 0f;
    float counter = 5f;


    //20 frame loop to check height
    float heightCounter = 0f;
    float heightTimer =20f;

    public float initHeight;
    public float currHeight;

    float fallHeight = 8f;
    // Start is called before the first frame update
    void Start()
    {
        me = GetComponentInParent<EggPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        heightCounter += Time.deltaTime;
        if(heightCounter > heightTimer)
        {
            heightCounter = 0f;
            initHeight = me.epc.transform.position.y;
        }
        currHeight = me.epc.transform.position.y;
        if(currHeight > initHeight)
        {
            initHeight = currHeight;
            heightCounter = 0f;
        }
        if (initHeight - currHeight >= fallHeight)
        {
            me.fallingToDeath = true;
            me.ToggleRagdoll(true);
        }

        if (me.controller.enabled)
        {
            if (me.controller.isGrounded)
            {
                heightCounter = 0;
                initHeight = me.epc.transform.position.y;
            }
        }

        if (me.fallingToDeath && me.health != 0)
        {
            timer += Time.deltaTime;
            if(timer > counter)
            {
                me.fallingToDeath = false;
                timer = 0f;
            }
            Vector3 pos = transform.position;
            float terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position);
            
            if(pos.y - terrainHeight <= Mathf.Abs(1.2f))
            {
                me.OnHit(0f, me.epc.transform.position, true);
            }
        }
    }

    
}
