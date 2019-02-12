using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FormIndexTransTable : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public int arrayIndex;
		public int realIndex;
	}
}