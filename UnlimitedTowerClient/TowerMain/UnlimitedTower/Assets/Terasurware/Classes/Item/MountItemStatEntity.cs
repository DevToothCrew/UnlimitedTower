using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MountItemStatEntity : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public int mountItemNum;
		public StatType statType;
		public double plusValue;
	}
}