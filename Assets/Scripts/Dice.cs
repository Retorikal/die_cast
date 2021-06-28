using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using TMPro;

public class Dice : MonoBehaviour, IPointedExecutor, IDamageable{
    // Config values
    public DamageProperty damageProperty;
    public float initialInjuryTime;
    public float stopRollThresh;
    public float collectCDMax;
    public int maxHitPt;

    // Variable values
    public int hitPt;
    public bool alive = true;
    public bool rolling = false;
    public float collectCD = 0;
    public int diceFaceValue = 1;

    public Rigidbody2D rb2D;
    public AudioClip diceSettle;
    public AudioClip diceRoll;
    public AudioClip diceThrow;
    public AudioClip diceHeal;

    public List<Sprite> landedDice;

    Animator anim;
    bool healer;
    float injury;
    SpriteRenderer sRenderer;
    AudioSource audioSrc;
    Transform luxTransform;
    Vector3 targetLuxScale;
    Rigidbody2D targetTow;
    PointedResponder ptResp;

    void Awake(){
        sRenderer = GetComponent<SpriteRenderer>();
        audioSrc = GetComponent<AudioSource>();
        luxTransform = transform.GetChild(0).GetComponent<Transform>();
        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        ptResp = GetComponent<PointedResponder>();
        ptResp.pointedExecutor = this;
    }

    // Update is called once per frame
    void FixedUpdate(){
        if(rb2D.velocity.magnitude <= stopRollThresh && rolling && injury <= 0)
            Settle();

        if(injury > 0) injury -= Time.fixedDeltaTime;
        if(collectCD > 0) collectCD -= Time.fixedDeltaTime;
    }

    void Update(){
        if(rolling){   
            diceFaceValue = Random.Range(1, 7);
        }else{
            sRenderer.sprite = landedDice[diceFaceValue-1];
        }

        luxTransform.localScale = Vector3.Lerp(luxTransform.localScale, targetLuxScale, 0.4f);
    }

    public void Damage(int damage){
        if(!rolling)
            hitPt ++;

        if(hitPt > maxHitPt) Die();
    }

    public void Heal(int heal){
        hitPt -= hitPt<=1 ? 0 : 1;

        RecalculateTorch();
    }

    public void PointExec(){
        // Clicked = collect dice
        if(collectCD > 0){
            Master.m.Warn("Not yet!");
            return;
        }

        Master.m.player.AddDice(diceFaceValue);
        Remove(false);
    }

    public void Spawn(Vector2 startLoc, Vector2 direction, float velocity){
        if(velocity <= .05f){
            healer = true;
            Debug.Log("Dice slammed at " + startLoc);
        }
        else{
            healer = false;
            Debug.Log("Dice thrown at " + startLoc + " to " + direction);
        }

        audioSrc.PlayOneShot(diceThrow, 1.0F);
        rb2D.simulated = true;
        luxTransform.localScale = new Vector3(1, 1, 0);
        targetLuxScale = new Vector3(1, 1, 0);
        rb2D.position = startLoc + direction*0.5f;
        collectCD = collectCDMax;
        injury = initialInjuryTime;
        hitPt = 1;
        alive = true;
        Roll(direction * velocity);
    }

    public void Hit(Vector2 direction){
        Damage(1);
        Roll(direction);
    }

    public void Roll(Vector2 velocity){
        anim.enabled = true;
        anim.SetBool("Rolling", true);
        audioSrc.PlayOneShot(diceRoll, 1.0F);
        ptResp.enabled = false;
        diceFaceValue = Random.Range(1, 7);
        rb2D.velocity = velocity;
        rb2D.isKinematic = false;
        rolling = true;
    }

    public void Remove(bool kill){
        if(kill) {
            Destroy(gameObject);
            return;
        }

        rb2D.simulated = false;
        alive = false;
        Timing.RunCoroutine(_Remove());
    }

    IEnumerator<float> _Remove(){
        targetLuxScale = new Vector3(0, 0, 0);
        yield return Timing.WaitForSeconds(0.3f);
        transform.position += new Vector3(-100, -100, 0);
    }

    public void Die(){
        Remove(false);
    }

    void RecalculateTorch(){
        float scale = 0.5f;
        scale += 2 * diceFaceValue/hitPt;

        targetLuxScale = new Vector3(scale, scale, 0);
    }

    void Settle(bool slammed = false){
        anim.SetBool("Rolling", false);
        anim.enabled = false;
        ptResp.enabled = true;
        rolling = false;
        rb2D.isKinematic = true;
        rb2D.velocity = Vector2.zero;

        if(healer){
            Master.m.HealDices(rb2D.position, 0.5f + 2*diceFaceValue/hitPt);
            audioSrc.PlayOneShot(diceHeal, 1.0F);
            healer = false;
        }
        else
            audioSrc.PlayOneShot(diceSettle, 1.0F);

        RecalculateTorch();
    }
}
