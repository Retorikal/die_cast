using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    float injury;
    TextMeshPro diceFaceText;
    Rigidbody2D rb2D;
    Rigidbody2D targetTow;
    PointedResponder ptResp;

    void Awake(){
        rb2D = GetComponent<Rigidbody2D>();
        diceFaceText = GetComponentInChildren<TextMeshPro>(true); 
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
            diceFaceText.text = diceFaceValue.ToString();
        }
    }

    public void Damage(int damage){
        hitPt -= damage;
        if(hitPt<0) Die();
    }

    public void Heal(int heal){
        hitPt = (hitPt + heal > maxHitPt) ? maxHitPt : hitPt + heal;
    }

    public void PointExec(){
        // Clicked = collect dice
        if(collectCD > 0){
            Master.m.Warn("Not yet!");
            return;
        }

        Debug.Log("Dice clicked");
        Master.m.player.AddDice(diceFaceValue);
        Remove(false);
    }

    public void Spawn(Vector2 startLoc, Vector2 direction, float velocity){
        if(velocity <= .05f)
            Debug.Log("Dice slammed at " + startLoc);
        else
            Debug.Log("Dice thrown at " + startLoc + " to " + direction);

        collectCD = collectCDMax;
        injury = initialInjuryTime;
        rb2D.position = startLoc + direction;
        Roll(direction * velocity);
        alive = true;
    }

    public void Roll(Vector2 velocity){
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

        rb2D.position = new Vector2(-10, -10);
        alive = false;
    }

    public void Die(){
        Remove(true);
    }

    void Settle(){
        ptResp.enabled = true;
        rolling = false;
        rb2D.isKinematic = true;
        rb2D.velocity = Vector2.zero;
    }
}
