using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class itemEntity : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public int itemNum;
		public string itemName;
	}
}