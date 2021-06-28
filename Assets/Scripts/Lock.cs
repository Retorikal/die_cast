using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Lock : MonoBehaviour, IPointedExecutor{

    public int lockCode;
    public bool locked = true;

    public Sprite altarEmpty;
    public List<Sprite> filledAltar;
    public List<Sprite> diceIndicatorEmpty;
    public List<Sprite> diceIndicatorFilled;
    public Vector3 targetLuxScale;

    SpriteRenderer altarSprite;
    SpriteRenderer tileSprite;
    PointedResponder ptResp;
    Transform luxTransform;

    void Awake(){
        altarSprite = GetComponent<SpriteRenderer>();
        tileSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        luxTransform = transform.GetChild(1).GetComponent<Transform>();
        ptResp = GetComponent<PointedResponder>();
        ptResp.pointedExecutor = this;

        targetLuxScale = Vector3.zero;
    }

    void Update(){
        luxTransform.localScale = Vector3.Lerp(luxTransform.localScale, targetLuxScale, 0.4f);
    }

    bool checkDiceMatch(){
        var diceInventory = Master.m.player.diceInventory;
        for(var i = 0; i < diceInventory.Count; i++){
            if(diceInventory[i] == lockCode){
                diceInventory.RemoveAt(i);

                altarSprite.sprite = filledAltar[lockCode-1];
                tileSprite.sprite = diceIndicatorFilled[lockCode-1];
                targetLuxScale = new Vector3(4, 4, 0);

                locked = false;
                Master.m.CheckLock();
                return true;
            }
        }
        return false;
    }

    public void Respawn(){
        var pos = transform.position;
        pos.x = Random.Range(-6, 7);
        pos.y = Random.Range(-6, 7);

        transform.position = pos;
        locked = true;
        lockCode = Random.Range(1, 7);
        altarSprite.sprite = altarEmpty;
        tileSprite.sprite = diceIndicatorEmpty[lockCode-1];

        targetLuxScale = Vector3.zero;
        luxTransform.localScale = Vector3.zero;
    }

    public void PointExec(){
        // Clicked = collect dice
        if(checkDiceMatch()){
            Master.m.Warn("+1");
            return;
        }
        else{
            Master.m.Warn("Mismatch!");
        }
    }
}
