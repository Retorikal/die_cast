using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Exit : MonoBehaviour{

    public bool locked = true;
    public bool vertical = false;

    SpriteMask sMask;
    Animator anim;
    AudioSource audioSrc;
    Collider2D triggerer;

    void Awake(){
        sMask = GetComponentInChildren<SpriteMask>(true);
        audioSrc = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        triggerer = GetComponent<Collider2D>();
        Lock();
    }

    void Update(){
        if(sMask.enabled){
            sMask.transform.localScale = Vector3.Lerp(sMask.transform.localScale, new Vector3(7,7,0), 0.4f);
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("Player") && !locked){
            Master.m.NextLevel();
            Lock();
        }
    }

    public void Unlock(){
        sMask.enabled = true;
        triggerer.isTrigger = true;
        anim.SetBool("Open", true);
        audioSrc.Play();
        locked = false;
        Master.m.Warn("EXIT UNLOCKED!");
    }

    public void Lock(){
        sMask.enabled = false;
        sMask.transform.localScale = new Vector3(0,0,0);
        triggerer.isTrigger = false;
        anim.SetBool("Open", false);
        audioSrc.Play();
        locked = true;
    }
}
