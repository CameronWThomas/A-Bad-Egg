using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
public class IntroManager : MonoBehaviour
{
    // Start is called before the first frame update
    public VideoPlayer VideoPlayer; // Drag & Drop the GameObject holding the VideoPlayer component
    public string SceneName;
    void Start()
    {
        VideoPlayer = GetComponent<VideoPlayer>();

        SceneName = "SampleScene";

        VideoPlayer.loopPointReached += LoadScene;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void LoadScene(VideoPlayer vp)
    {
        VideoPlayer.SetDirectAudioVolume(0, 0f);
        VideoPlayer.isLooping = true;
        SceneManager.LoadScene(SceneName);
    }
}
