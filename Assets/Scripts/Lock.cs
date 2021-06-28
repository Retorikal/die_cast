using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Lock : MonoBehaviour, IPointedExecutor{

    public int lockCode;
    public bool locked = true;

    TextMeshPro lockFaceText;
    PointedResponder ptResp;

    void Awake(){
        lockFaceText = GetComponentInChildren<TextMeshPro>(true); 
        ptResp = GetComponent<PointedResponder>();
        ptResp.pointedExecutor = this;
    }

    bool checkDiceMatch(){
        var diceInventory = Master.m.player.diceInventory;
        for(var i = 0; i < diceInventory.Count; i++){
            if(diceInventory[i] == lockCode){
                diceInventory.RemoveAt(i);
                lockFaceText.text = "X";
                locked = false;
                Master.m.CheckLock();
                return true;
            }
        }
        return false;
    }

    public void Respawn(){
        var pos = transform.position;
        pos.x = Random.Range(-7, 8);
        pos.y = Random.Range(-7, 8);

        transform.position = pos;
        locked = true;
        lockCode = Random.Range(1, 7);
        lockFaceText.text = lockCode.ToString();
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
