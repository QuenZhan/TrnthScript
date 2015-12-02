﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public abstract class TrnthPositionPicker : MonoBehaviour,ITrnthPositionPicker {
	[SerializeField]Transform _locator;
	[SerializeField]protected Transform _group;
	public Vector3 position{get{
		if(_locator==null)return transform.position;
		return _locator.position;
	}}
	public event System.Action<ITrnthPositionPicker,ITrnthPositionPickee> onPicked=delegate{};
	public void onDragEnd(){
		Invoke("_delayScrollTo",1);
	}
	public void onDragStart(){
		scrollStop();
		CancelInvoke("_delayScrollTo");
	}
		void _delayScrollTo(){
			if(gameObject.activeInHierarchy)scrollTo(_pickee);
		}
	public virtual void onScrollValueChange(Vector2 vec){
		if(!cooled)return;
		pick();
		// StartCoroutine(_cooldown());
	}
		IEnumerator _cooldown(){
			cooled=false;
			yield return new WaitForSeconds(0.1f);
			cooled=true;
		}bool cooled=true;
	public void pick(){
		if(pickees.Count<1 || _locator==null)return;
		pickees.Sort((a,b)=>{
			return  (_locator.position - a.positionWorld).magnitude < (_locator.position - b.positionWorld).magnitude ?-1:1;
		});
		var pickee=pickees[0];
		if(pickee==_pickee)return;
		StartCoroutine(_cooldown());
		if(_pickee!=null)_pickee.onAwayPosition(this);
		_pickee=pickee;
		_pickee.onPosition(this);
		onPicked(this,pickee);
	}
	public void scrollTo(ITrnthPositionPickee data){
		if(_scroll)_scroll.inertia=false;
		var thePickee=pickees.Find(t=>{return t==data;});
		if(thePickee==null)return;
		var _posTarget=_group.localPosition;
		_posTarget.y=-thePickee.positionLocal.y;
		scrollStop();
		cooled=false;
		StartCoroutine(_scrollUpdate(_posTarget,0.5f));
	}
		IEnumerator _scrollUpdate(Vector3 _posTarget,float duration){
			var timeStart=Time.time;
			var _vel=Vector3.zero;
			while(Time.time-timeStart<duration){
				// _group.localPosition=Vector3.SmoothDamp(_group.localPosition,_posTarget, ref _vel, duration);
				_group.localPosition=Vector3.Lerp(_group.localPosition,_posTarget,0.2f);
				yield return new WaitForSeconds(0);				
			}
			scrollStop();
		}
	public void scrollStop(){
		StopCoroutine("_scrollUpdate");
		cooled=true;
		if(_scroll)_scroll.inertia=true;
	}
	// protected Transform _group{get{if(_scroll==null)return null;return _scroll.content;}}
	protected abstract List<ITrnthPositionPickee> pickees{get;}
	protected virtual void OnEnable(){
		cooled=true;
	}
	protected virtual void OnDisable(){
		CancelInvoke("_delayScrollTo");
	}
	protected virtual void Awake(){
		_scroll=_group.GetComponentInParent<ScrollRect>();
		_eventTriggerStuff(_scroll);
	}	
	void _eventTriggerStuff(ScrollRect scroll){
		if(scroll==null)return;		
		EventTrigger trigger = _scroll.gameObject.AddComponent<EventTrigger>();
		trigger.delegates.Add(entry(EventTriggerType.BeginDrag),onDragStart);
		trigger.delegates.Add(entry(EventTriggerType.EndDrag),onDragEnd);
		trigger.delegates.Add(entry(EventTriggerType.Scroll),onScrollValueChange);
	}
	EventTrigger.Entry entry(EventTriggerType type,System.Action<BaseEventData> callback){
		EventTrigger.Entry entry = new EventTrigger.Entry(){
			eventID = type
			,callback = new EventTrigger.TriggerEvent()
		};
		// var call = new UnityEngine.Events.UnityAction<BaseEventData>(TestFunc);
		entry.callback.AddListener(callback);
		return entry;
	}
	ScrollRect _scroll;
	ITrnthPositionPickee _pickee;
}
