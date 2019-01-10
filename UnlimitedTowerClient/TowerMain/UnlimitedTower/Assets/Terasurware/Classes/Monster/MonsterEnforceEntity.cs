using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterEnforceEntity : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public int gradeNum;
		public double startMultiVal;
		public double plusMultivalPerCount;
	}
}