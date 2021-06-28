using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Exit : MonoBehaviour, IPointedExecutor{

    public bool locked = true;
    public bool vertical = false;

    PointedResponder ptResp;

    void Awake(){
        ptResp = GetComponent<PointedResponder>();
        ptResp.pointedExecutor = this;
    }

    public void RandomizePosition(){
        ptResp.tooltip = "";
        locked = false;

    }

    public void Unlock(){
        locked = false;
        Master.m.Warn("Unlocked!");
        ptResp.tooltip = "Press space to escape";
    }

    public void PointExec(){
        if(!locked){
            Master.m.NextLevel();
            ptResp.tooltip = "";
        }
    }
}
