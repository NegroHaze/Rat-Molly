using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    public float timer = 180;
    public Text timerText;
    public GameObject bomb;
    private AudioSource bombAudio;
    public GameObject player;

    public AudioClip boom;

    private Vector3 startPos = Vector3.zero;

    private bool explosion = false;

    void Start()
    {
        this.bombAudio = this.bomb.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.timer <= 0 && !explosion)
        {
            explosion = true;
            this.player.GetComponent<AudioSource>().Stop();
            GetComponent<Tremblement>().enabled = true;
            this.bombAudio.Stop();
            this.bombAudio.loop = false;
            this.bombAudio.volume = 0.5f;
            this.bombAudio.clip = boom;
            this.bombAudio.Play();
            this.timerText.text = "";
        }
        else
        {
            if (!explosion)
            {
                this.calculateText();
            }
        }
    }

    private void calculateText()
    {
        timer -= Time.deltaTime;
        var calculatedMinute = Mathf.Floor(timer / 60);
        var temp = (int)calculatedMinute;
        var temp2 = (int)timer % 60;
        this.timerText.text = temp + ":" + temp2;
    }
}
