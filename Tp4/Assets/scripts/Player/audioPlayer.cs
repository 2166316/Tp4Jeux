using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public enum ClipPlaying
{
    idle,walking,running
}
public class AudioPlayer : NetworkBehaviour
{
   
    public AudioSource source;
    public AudioClip playerFootStepsWalking;
    public AudioClip playerFootStepsRunning;
    public  ClipPlaying currentClipPlaying;

    void Start()
    {
        currentClipPlaying = ClipPlaying.idle;
        source = GetComponent<AudioSource>();
       
    }

    public void playWalkingStepsAudio()
    {
         source.clip = playerFootStepsWalking;
        currentClipPlaying = ClipPlaying.walking;
        source.Stop();
        source.Play();
         Debug.Log("walk audio");
    }

    public void playRunningStepsAudio()
    {
        source.clip = playerFootStepsRunning;
        currentClipPlaying = ClipPlaying.running;
        source.Stop();
        source.Play();
        Debug.Log("walk audio");
    }
    public void stopStepsAudio()
    {
        source.Stop();
        currentClipPlaying = ClipPlaying.idle;
        Debug.Log("stop audio");
    }
}
