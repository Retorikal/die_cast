using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable{

    // Config values
    public DamageProperty damageProperty;
    public float speed = 1f;
    public float throwVelocity = 5f;
    public float staggerTime = 0.5f;
    public int diceMeterCap = 5;
    public int diceCapacity = 6;
    public int maxHitPt;

    // Variable
    public int hitPt;
    public float diceMeter = 0;
    public float stagger = 0;

    public List<int> diceInventory;
    public Vector2 pointDir;
    public Rigidbody2D rb2D;

    public Transform pointedObject;

    public AudioClip hurt;
    public bool tutorialMode = false;

    bool changedPoint;

    AudioSource audioSrc;
    Transform luxTransform;
    Transform ptrTransform;
    Vector3 targetLuxScale;
    Vector2 moveDir;

    void Awake(){
        ptrTransform = transform.GetChild(1).GetComponent<Transform>();
        luxTransform = transform.GetChild(2).GetComponent<Transform>();
        audioSrc = GetComponent<AudioSource>();
        rb2D = GetComponent<Rigidbody2D>();
        hitPt = maxHitPt;

        diceInventory = new List<int>(diceCapacity);
        for(int i = 0; i < 3; i++){
            diceInventory.Add(-1);
        }

        RecalculateTorch();
    }

    void Update(){
        var moveX = Input.GetAxisRaw("X");
        var moveY = Input.GetAxisRaw("Y");
        moveDir = new Vector2(moveX, moveY);
        pointDir = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - rb2D.position).normalized;

        ptrTransform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(new Vector2(1, 1), pointDir));

        RaycastHit2D hit = Physics2D.Raycast(transform.position, pointDir, 1f, 1);
        Debug.DrawRay(transform.position, pointDir*1f);

        if(hit.collider != null){
            pointedObject = hit.transform;
            changedPoint = pointedObject != hit.transform;
        }
        else{
            pointedObject = null;
        }

        // Input processing
        if(Input.GetButtonDown("Fire1")){
            ThrowDice(pointDir, true);
        }
        if(Input.GetButtonDown("Fire2")){
            ThrowDice(pointDir, false);
        }
        if(Input.GetButtonDown("Grab") && pointedObject != null){
            var pResp = pointedObject.gameObject.GetComponent<PointedResponder>();
            if(pResp != null) pResp.PointExec();
        }
        if(Input.GetKey(KeyCode.R)){
            string output = "";
            foreach(var i in diceInventory){
                output += i.ToString() + ",";
            }
            Debug.Log(output);
        }
        if(Input.GetKey(KeyCode.Q)){
            Master.m.BeginTrial();
        }

        luxTransform.localScale = Vector3.Lerp(luxTransform.localScale, targetLuxScale, 0.4f);
    }

    void FixedUpdate(){
        if(stagger <= 0){
            stagger = 0;
            rb2D.MovePosition(rb2D.position + (speed * moveDir.normalized * Time.fixedDeltaTime));
        }
        else
            stagger -= Time.fixedDeltaTime;
    }

    void ThrowDice(Vector2 direction, bool thrown){
        if(diceInventory.Count == 0) {
            Master.m.Warn("Artifact depleted!");
            return;
        }

        Stagger(staggerTime);
        if(thrown){
            rb2D.velocity = -direction * throwVelocity;
        }
        else{
            rb2D.velocity = Vector2.zero;
        }

        diceInventory.RemoveAt(0);
        RecalculateTorch();
        Master.m.ThrowDice(direction, thrown);
    }

    void RecalculateTorch(){
        float scale = 1.5f;
        foreach(var i in diceInventory){
            scale += i == -1 ? 1:0;
        }

        targetLuxScale = new Vector3(scale, scale, 0);
    }

    public void Damage(int damage){
        audioSrc.PlayOneShot(hurt, 1f);
        hitPt -= damage;
        if(hitPt<0) Die();
    }

    public void Heal(int heal){
        hitPt = (hitPt + heal > maxHitPt) ? maxHitPt : hitPt + heal;
    }

    public void Die(){
        Debug.Log("ded");
        Master.m.Lose();
    }

    public void AddDice(int diceFaceValue){
        if(diceInventory.Count == diceCapacity){
            diceInventory.RemoveAt(0);
        }
        diceInventory.Add(diceFaceValue);

        if(diceFaceValue == -1){
            Heal(1);
        }

        RecalculateTorch();
    }   

    public void CloseCall(){
        diceMeter++;
        if(diceMeter == diceMeterCap){
            diceMeter = 0;
            AddDice(-1);
        }

        // Animate Close Call
    }

    public bool hasChangedPoint(){
        var tmp = changedPoint;
        changedPoint = false;
        return tmp;
    }

    public void Stagger(float duration){
        stagger = staggerTime;
    }
}

