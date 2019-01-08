using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MountItemTeerEntity : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public int teerNum;
		public int minMountLevel;
	}
}