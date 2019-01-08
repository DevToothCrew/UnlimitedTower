using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterGradeEntity : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public int gradeNum;
		public string name;
		public int min_original_Stat;
		public int max_original_Stat;
	}
}