using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class AudioTest : MonoBehaviour
{
    private AudioSource ass;
    private void Awake()
    {
        ass = GetComponent<AudioSource>();
    }
    [YarnCommand("playAudio")]
    public void Play()
    {
        ass.Play();
        Debug.Log("playing");
    }
}
