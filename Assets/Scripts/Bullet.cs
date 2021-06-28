using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Bullet : MonoBehaviour{
    public BulletType type;
    public int maxBounceCount;
    public float velocity;
    public float chaseChance;

    public int bounceCount;
    public bool alive;
    public bool dodged;

    AudioSource audioSrc;
    Rigidbody2D rb2D;
    SpriteRenderer sRenderer;
    LineRenderer lRenderer;
    string teleTag;

    void Awake(){
        teleTag = gameObject.GetInstanceID().ToString();
        audioSrc = GetComponent<AudioSource>();
        rb2D = GetComponent<Rigidbody2D>();
        sRenderer = GetComponent<SpriteRenderer>();
        lRenderer = GetComponent<LineRenderer>();
    }

    void FixedUpdate(){
        if(bounceCount > maxBounceCount && alive){ 
            Remove(false);
        }

        if(alive){
            rb2D.velocity = rb2D.velocity.normalized * velocity;
            rb2D.SetRotation(Vector2.SignedAngle(Vector2.up, rb2D.velocity));
        }
    }

    void OnCollisionEnter2D(Collision2D collision){
        Debug.Log("Coll'd");
        dodged = false;

        if(collision.transform.CompareTag("Wall")){
            bounceCount++;
            if(Random.Range(0f, 1f) <= chaseChance && bounceCount <= maxBounceCount){
                Chase();
            }
        }
        else if(collision.transform.CompareTag("Dice")){
            Master.m.DamageDice(collision.transform, transform);
        }
        else if(collision.transform.CompareTag("Player")){
            Master.m.player.Damage(1);
            Remove(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.transform.CompareTag("Player")){
            if(!dodged) {
                Master.m.player.CloseCall();
                Debug.Log("Bullet Dodged!");
            }
            audioSrc.Play();
            dodged = true;
        }
    }

    public void SetProp(float velocity, float chaseChance, int bounceCap){
        this.velocity = velocity;
        this.chaseChance = chaseChance;
        this.maxBounceCount = bounceCap;
    }

    public void Chase(){
        var direction = (Master.m.player.rb2D.position - rb2D.position).normalized;
        rb2D.velocity = direction * velocity;

        Timing.KillCoroutines(teleTag);
        Timing.RunCoroutine(_Telegraph(direction), teleTag);
    }

    IEnumerator<float> _Telegraph(Vector2 dir){
        var speed = 25f;
        var increment = (Vector3)dir * speed * Time.deltaTime;
        var target = (Vector3)rb2D.position + new Vector3(0, 0, 0.5f);
        var origin = target - 5 * (Vector3)dir;

        do {
            origin += increment;
            target += increment;
            var positions = new Vector3[2] {origin, target};
            lRenderer.SetPositions(positions);
            yield return Timing.WaitForOneFrame;
            Debug.Log("Chase tele:" + target + " " + increment);
        } while(Mathf.Abs(target.x) < 15 && Mathf.Abs(target.y) < 15);

        Debug.Log("Chase tele end");
    }

    public void Spawn(Vector2 startLoc){
        //transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, direction));
        
        rb2D.position = startLoc;
        bounceCount = 0;
        alive = true;
        Chase();
    }

    public void Remove(bool kill){
        if(kill) {
            Destroy(gameObject);
            return;
        }

        rb2D.position = new Vector2(100, -100);
        rb2D.velocity = Vector2.zero;
        alive = false;
    }
}
