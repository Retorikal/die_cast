using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointedResponder : MonoBehaviour{
	public bool pointed;
	public string tooltip;

	public IPointedExecutor pointedExecutor;

	void Update(){
		var pointedTf = Master.m.player.pointedObject;
		pointed = (pointedTf != null && pointedTf == transform);

		if(pointed)
			Master.m.ShowTooltip(transform, tooltip); 
	}

	public void PointExec(){
		pointedExecutor.PointExec();
	}
}

public interface IPointedExecutor{
	public void PointExec();
}