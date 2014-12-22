﻿using UnityEngine;
using System.Collections;
[RequireComponent(typeof(TrnthHVSCondition))]
public class TrnthHVSAction : TrnthHVS {
	public float delay=0;
	[ContextMenu("execute")]
	public void execute(){
		if(delay==0){
			_execute();
		}else {
			CancelInvoke();
			Invoke("_execute",delay);
		}
		// execute();
	}
	[ContextMenu("subscribe")]
	public void subscribe(){
		var conditions=GetComponents<TrnthHVSCondition>();
		foreach(var e in conditions){
			e.callback-=execute;
			e.callback+=execute;
			// log();
		}
	}
	public override string extraMsg{get{return "TrnthHVSAction";}}
	protected virtual void _execute(){
		log();
	}
	void Awake(){
		subscribe();
	}
}
