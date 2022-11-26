using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public List<AudioSource> splats;
    public List<AudioSource> screams;

    //float numSplats = 1;
    public float screamPercentChance = 10;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] splatObs = GameObject.FindGameObjectsWithTag("splatSound");
        foreach (GameObject go in splatObs)
        {
            AudioSource splat = go.GetComponent<AudioSource>();
            if (go != null)
            {
                splats.Add(splat);
            }
        }

        GameObject[] screamObs = GameObject.FindGameObjectsWithTag("scream_sound");
        foreach (GameObject go in screamObs)
        {
            AudioSource scream = go.GetComponent<AudioSource>();
            if (go != null)
            {
                screams.Add(scream);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool ShouldScream()
    {
        return Random.Range(0, 101) <= screamPercentChance;
    }

    public void PlayRandomSplat()
    {
        AudioSource splat1 = splats[Random.Range(0, splats.Count)];
        AudioSource splat2 = splats[Random.Range(0, splats.Count)];

        if (ShouldScream())
        {
            AudioSource scream = screams[Random.Range(0, screams.Count)];
            splat1.Play();
            splat2.Play();
            scream.Play();
        }
        else
        {
            splat1.Play();
            splat2.Play();
        }
    }

}
