using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterEntity : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public int monsterNum;
		public int typeNum;
		public string monsterName;
		public int mainstatTypenum;
		public int apearNum;
		public int sizeNum;
	}
}