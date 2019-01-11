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
		public int Legend_CharNum;
		public int Legend_HairNum;
		public int Legend_HeadNum;
		public string Legend_Name;
		public int Legend_OriginalSTR;
		public int Legend_OriginalDEX;
		public int Legend_OriginalINT;
	}
}