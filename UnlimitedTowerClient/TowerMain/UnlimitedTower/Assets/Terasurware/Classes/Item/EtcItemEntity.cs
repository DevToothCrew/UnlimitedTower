using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EtcItemEntity : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public int etcitemNum;
		public string itemName;
	}
}