using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatEntity : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public StatType statType;
		public string statName;
	}
}