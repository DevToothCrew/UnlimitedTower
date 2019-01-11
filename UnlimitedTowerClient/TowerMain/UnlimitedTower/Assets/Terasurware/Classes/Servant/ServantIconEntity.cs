using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServantIconEntity : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public int charNum;
		public int jobNum;
		public int iconNum;
	}
}