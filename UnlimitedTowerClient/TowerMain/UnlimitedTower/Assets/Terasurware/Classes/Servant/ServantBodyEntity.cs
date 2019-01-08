using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServantBodyEntity : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public int bodyNum;
		public string charName;
	}
}