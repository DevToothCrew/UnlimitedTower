using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LegendServantEntity : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public int legendServantNum;
		public int bodyNum;
		public string name;
		public int originalSTR;
		public int originalDEX;
		public int originalINT;
		public int jobNum;
	}
}