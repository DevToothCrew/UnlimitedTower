using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MountItemEnforceEntity : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public int enforceCount;
		public double successProb;
		public double Cost;
	}
}