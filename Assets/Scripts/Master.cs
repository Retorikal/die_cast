using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : MonoBehaviour{

    public static Master m;

    public GameObject diceFab;
    public GameObject bulletFab;
    public GameObject altarFab;

    public List<Vector3> bulletSpawnPoint;
    public List<Bullet> bullets;
    public List<Dice> dices;
    public List<Lock> locks;
    public Player player;
    public Exit exit;

    // Level property
    public float bulletVelocity;
    public float bulletChaseChance;
    public float bulletRespawnDelay;
    public int bulletBounceCap;
    public int bulletLack; // Below lack, bullets respawn 3 times as fast.
    public int bulletTypical; // Below typical, bullets respawn twice as fast.
    public int bulletCap;
    public int altarCount;

    public int currentLevel;
    public bool trialBegun;

    float sinceLastSpawn;
    AudioSource audioSrc;
    Tooltip tooltip;
    Warner warner;

    void Awake(){
        DontDestroyOnLoad(gameObject);
        audioSrc = GetComponent<AudioSource>();
        dices = new List<Dice>();
        bullets = new List<Bullet>();
        locks = new List<Lock>();
        sinceLastSpawn = 0;
        trialBegun = false;

        m = this;
    }

    // Update is called once per frame
    void Update(){
        if(player.pointedObject == null) HideTooltip();
        if(trialBegun){
            var activeBulletCount = 0;
            foreach(var b in bullets){
                if(b.alive) activeBulletCount ++;
            }

            if(activeBulletCount < bulletCap){
                int div = 1;
                if(activeBulletCount < bulletTypical) div = 2;
                if(activeBulletCount < bulletLack) div = 3;

                var adjustedDelay = bulletRespawnDelay/div;
                if(sinceLastSpawn > adjustedDelay) SpawnBullet(Vector2.zero);
            }

            sinceLastSpawn += Time.fixedDeltaTime;
        }
    }

    public void HideTooltip(){

    }

    // Tooltip over object faced by player. Disappears as soon as facing away 
    public void ShowTooltip(Transform t, string message){
        Debug.Log(message);
    }

    // Floats for n sec on player before disappearing
    public void Warn(string message){
        Debug.Log(message);
    }

    public void SpawnBullet(Vector2 startLoc){
        var bullet = bullets.Find(d => !d.alive);

        if(bullet == null){
            var bulletGO = Instantiate(bulletFab);
            bullet = bulletGO.GetComponent<Bullet>();
            bullets.Add(bullet);
        }

        sinceLastSpawn = 0;
        bullet.SetProp(bulletVelocity, bulletChaseChance, bulletBounceCap);
        bullet.Spawn(startLoc);
    }

    public void CheckLock(){
        if(locks.Count == 0) return;
        Debug.Log("Checking lock..");
        foreach(var l in locks){
            if(l.locked) return;
        }

        Debug.Log("Unlocked!");
        exit.Unlock();
    }

    public void ThrowDice(Vector2 direction, bool thrown){
        var dice = dices.Find(d => !d.alive);

        if(dice == null){
            var diceGO = Instantiate(diceFab);
            dice = diceGO.GetComponent<Dice>();
            dices.Add(dice);
        }

        dice.Spawn(player.rb2D.position, direction, thrown ? player.throwVelocity:0);
    }

    public void HealDices(Vector2 origin, float diameter){
        var radius = diameter/2;
        foreach(var d in dices){
            var distance = (origin - d.rb2D.position).magnitude;
            if(distance <= radius){
                d.Heal(1);
            }
        }

    }

    public void DamageDice(Transform hitDice, Transform bullet){
        foreach(var d in dices){
            if(hitDice == d.transform){
                var direction = (Vector2)(d.transform.position - bullet.transform.position).normalized * 3f;
                d.Hit(direction);
            }
        }
    }

    public bool DodgeBullet(Transform dodgedBullet){
        foreach(var b in bullets){
            if(dodgedBullet == b.transform){
                b.dodged = true;
                return true;
            }
        }

        return false;
    }

    public void BeginTrial(){
        bulletVelocity = 3.5f;
        bulletChaseChance = 0.3f;
        bulletBounceCap = 3;
        bulletRespawnDelay = 3f;
        bulletLack = 1;
        bulletTypical = 1; 
        bulletCap = 1;
        altarCount = 1;

        trialBegun = true;
        audioSrc.Play();

        NextLevel();
    }

    public void Lose(){

    }

    public void NextLevel(){
        currentLevel++;

        bulletVelocity += 0.3f;
        bulletChaseChance += 0.05f;

        if(currentLevel % 6 == 0){
            bulletLack += 1;
        }
        if(currentLevel % 5 == 0){
            bulletBounceCap += 1;
            bulletTypical += 1;
        }
        if(currentLevel % 3 == 0){
            altarCount += 1;
            bulletCap += 1;
        }

        while(locks.Count < altarCount){
            var go = Instantiate(altarFab);
            var altar = go.GetComponent<Lock>();
            locks.Add(altar);
        }
        while(locks.Count > altarCount){
            var altar = locks[0];
            locks.RemoveAt(0);
            Destroy(altar.gameObject);
        }

        foreach(var l in locks){
            l.Respawn();
        }
        foreach(var b in bullets){
            b.Remove(false);
        }
        foreach(var d in dices){
            d.Remove(false);
        }

        player.rb2D.position = new Vector2(0, 7);
        sinceLastSpawn = 0;
    }
}

