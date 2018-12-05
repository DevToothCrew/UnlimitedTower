using System.Collections;
using System.Collections.Generic;

public class CharacterManager : MonoSingleton<CharacterManager> {

#region Character

	public Dictionary<int, Character> servantDic = new Dictionary<int, Character>();

    public void SetChar(Dictionary<int, Character> getCharcterDic)
	{
		servantDic = getCharcterDic;
	}

#endregion

	//public Dictionary<int, Character> battlePlayerDic;
	//public Dictionary<int, Character> battleEnemyDic;

	
	//public void SetBattleChar(Dictionary<int, Character> getPlayerDic, Dictionary<int, Character> getEnemyDic)
	//{
	//	battlePlayerDic = getPlayerDic;
	//	battleEnemyDic = getEnemyDic;
	//}

	//public bool GetBattleChar(CHAR_TYPE type, int myIndex, ref Character getChar)
	//{
	//	if(type == CHAR_TYPE.PLAYER)
	//	{
	//		if(battlePlayerDic.ContainsKey(myIndex) == false)
	//		{
	//			return false;
	//		}

	//		getChar = battlePlayerDic[myIndex];

	//		return true;
	//	}
	//	else if(type == CHAR_TYPE.ENEMY)
	//	{
	//		if(battleEnemyDic.ContainsKey(myIndex) == false)
	//		{
	//			return false;
	//		}

	//		getChar = battleEnemyDic[myIndex];

	//		return true;
	//	}

	//	return false;
	//}

}
