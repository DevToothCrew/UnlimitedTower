using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class levelEntity : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public int level;
		public double needExp;
	}
}