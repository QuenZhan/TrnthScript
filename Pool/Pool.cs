﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;


namespace TRNTH.Pooling{
	[System.Serializable]
	public class Pool<T> {
	
		[SerializeField]List<T> _Instances=new List<T>();
		public IReadOnlyList<T> Instances{
			get{
				return _Instances;
			}
		}
		int SpawningIndex;
		public T Spawn(){
			var Limiation=_Instances.Count;
			SpawningIndex=++SpawningIndex%Limiation;
			var index=SpawningIndex;
			return _Instances[index];
		}
	}
	public class ComponentsPool<T> :Pool<T> where T:Component{
		public void UnparentAll(){
			for (int i = 0; i < Instances.Count; i++)
			{
				Instances[i].transform.parent=null;
				Instances[i].transform.position=-Vector2.up*10000;
			}
		}
	}
	[System.Serializable]public class GameObjectPool :Pool<GameObject>{
		public void UnparentAll(){
			for (int i = 0; i < Instances.Count; i++)
			{
				Instances[i].transform.parent=null;
				Instances[i].SetActive(false);
			}
		}
		public void Toggle(bool onOff){
			for (int i = 0; i < Instances.Count; i++)
			{
				Instances[i].SetActive(onOff);
			}
		}
	}
}