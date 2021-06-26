using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable{

    // Config values
    public DamageProperty damageProperty;
    public float speed = 1f;
    public float throwVelocity = 5f;
    public float staggerTime = 0.5f;
    public int diceCapacity = 6;
    public int maxHitPt;

    // Variable

    public int hitPt;
    public float health = 100;
    public float diceMeter = 0;
    public float stagger = 0;

    public List<int> diceInventory;
    public Vector2 pointDir;
    public Rigidbody2D rb2D;

    public Transform pointedObject;
    bool changedPoint;

    Transform ptrTransform;
    Vector2 moveDir;

    void Awake(){
        ptrTransform = transform.GetChild(1).GetComponent<Transform>();
        rb2D = GetComponent<Rigidbody2D>();

        diceInventory = new List<int>(diceCapacity);
        for(int i = 0; i < 3; i++){
            diceInventory.Add(-1);
        }
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
        Master.m.ThrowDice(direction, thrown);
    }

    public void Damage(int damage){
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
        Master.m.player.diceInventory.Add(diceFaceValue);
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

