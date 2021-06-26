using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : MonoBehaviour{

    public static Master m;

    public GameObject diceFab;
    public GameObject bulletFab;

    public List<Bullet> bullets;
    public List<Dice> dices;
    public List<Lock> Locks;
    public Player player;

    public bool cycleDone;
    public bool levelStarted;

    public Level activeLevel;
    public List<Level> levels;

    void Awake(){
        dices = new List<Dice>();
        m = this;
    }

    // Update is called once per frame
    void Update(){
        if(player.pointedObject == null) HideTooltip();
        if(levelStarted){}
    }


    public void HideTooltip(){

    }

    // Tooltip over object faced by player. Disappears as soon as facing away 
    public void ShowTooltip(Transform t, string message){
        //Debug.Log(message);
    }

    // Floats for n sec before disappearing
    public void Warn(string message){
        Debug.Log(message);
    }

    public void SpawnBullet(){

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

    public void Lose(){

    }

    public void NextLevel(){

    }
}
