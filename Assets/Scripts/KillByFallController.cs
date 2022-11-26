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
    int frame = 0;

    public float terrainHeight;
    // Start is called before the first frame update
    void Start()
    {
        me = GetComponentInParent<EggPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        frame++;
        heightCounter += Time.deltaTime;
        if(heightCounter > heightTimer)
        {
            heightCounter = 0f;
            initHeight = me.epc.transform.position.y;
        }

        currHeight = me.epc.transform.position.y;
        if(currHeight - initHeight >= 1)
        {
            initHeight = currHeight;
            heightCounter = 0f;
        }

        if (initHeight - currHeight >= fallHeight && !me.fallingToDeath)
        {
            me.fallingToDeath = true;
            if (!me.IsRagdolling())
            {
                if (me.playerController != null)
                {
                    //me.ToggleRagdoll(true);
                    me.ToggleRoll();
                }
                else
                {
                    me.ToggleRagdoll(true);
                }
            }
        }

        if (me.controller.enabled && !me.isRagdolled && !me.rolling && !me.ragdolled)
        {
            if (me.controller.isGrounded)
            {
                heightCounter = 0;
                initHeight = me.epc.transform.position.y;
            }
        }
        if (me.navMeshAgent != null && !me.IsRagdolling()) 
        {
            initHeight = me.epc.transform.position.y;
        }

        if (me.fallingToDeath && me.health != 0)
        {
            //Vector3 pos = me.epc.transform.position;
            terrainHeight = Terrain.activeTerrain.SampleHeight(transform.position);

            if (Mathf.Abs(currHeight - terrainHeight) <= 2.0f)
            {
                Vector3 forcePos = me.epc.transform.position;
                forcePos.y += 2;
                me.OnHit(20f, forcePos, true);
            }

            timer += Time.deltaTime;
            if(timer > counter && me.fallingToDeath)
            {
                me.fallingToDeath = false;
                timer = 0f;
                initHeight = me.epc.transform.position.y;
                if (me.IsRagdolling())
                {
                    if (me.playerController != null)
                    {
                        //me.ToggleRagdoll(false);
                        me.ToggleRoll();
                    }
                    else
                    {
                        me.ToggleRagdoll(false);
                    }
                }

            }
            
        }
    }

    
}
