using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomAnimationStart : MonoBehaviour
{
    public List<Animator> animators;
    public Vector2 minMaxSpeed;
    private void Start()
    {
        foreach (Animator animator in animators)
        {
            animator.speed = Random.Range(minMaxSpeed.x, minMaxSpeed.y);
            animator.Play(0, -1, Random.value);
        }
    }
}
