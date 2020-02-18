using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class KeyListener : MonoBehaviour
{
    private VideoClip videos;
    public GameObject screen;

    private VideoPlayer screenVideoPlayer;
    void Start()
    {
        print(this.screen);
        if (this.screen)
        {
            screenVideoPlayer = this.screen.GetComponent<VideoPlayer>();
            var videos = Resources.Load("Assets/Scenes/Environnement 1/Ressources/HEYYEYAAEYAAAEYAEYAA.mp4");
            Debug.Log("coucou");
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
