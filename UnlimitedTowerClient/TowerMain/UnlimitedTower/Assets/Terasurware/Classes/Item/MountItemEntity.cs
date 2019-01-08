using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MountItemEntity : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public int mountItemNum;
		public MountitemType mountitemType;
		public string itemName;
		public bool isSpeJobPossible;
		public int possibleJobNum;
		public int teerNum;
		public int gradeNum;
	}
}