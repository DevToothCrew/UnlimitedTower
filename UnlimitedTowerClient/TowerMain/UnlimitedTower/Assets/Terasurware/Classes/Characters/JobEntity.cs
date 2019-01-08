using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JobEntity : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public int jobNum;
		public string jobName;
		public double speed;
	}
}