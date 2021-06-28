using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Exit : MonoBehaviour{

    public bool locked = true;
    public bool vertical = false;

    Animator anim;
    AudioSource audioSrc;
    Collider2D triggerer;

    void Awake(){
        audioSrc = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        triggerer = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("Player") && !locked){
            Master.m.NextLevel();
            Lock();
        }
    }

    public void Unlock(){
        triggerer.isTrigger = true;
        anim.SetBool("Open", true);
        audioSrc.Play();
        locked = false;
        Master.m.Warn("EXIT UNLOCKED!");
    }

    public void Lock(){
        triggerer.isTrigger = false;
        anim.SetBool("Open", false);
        audioSrc.Play();
        locked = true;
    }
}
