using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServantJobEntity : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public int jobNum;
		public float speed;
		public string jobName;
		public int getPossibleLevel;
	}
}