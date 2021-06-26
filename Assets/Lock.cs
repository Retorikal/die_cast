using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Exit : MonoBehaviour, IPointedExecutor{

    public int lockCode;
    public bool locked = true;

    TextMeshPro lockFaceText;
    PointedResponder ptResp;

    void Start(){
        lockFaceText = GetComponentInChildren<TextMeshPro>(true); 
        ptResp = GetComponent<PointedResponder>();
        ptResp.pointedExecutor = this;

        lockCode = Random.Range(1, 7);
        lockFaceText.text = lockCode.ToString();
    }

    bool checkDiceMatch(){
        var diceInventory = Master.m.player.diceInventory;
        for(var i = 0; i < diceInventory.Count; i++){
            if(diceInventory[i] == lockCode){
                diceInventory.RemoveAt(i);
                lockFaceText.text = "X";
                locked = false;
                return true;
            }
        }
        return false;
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
