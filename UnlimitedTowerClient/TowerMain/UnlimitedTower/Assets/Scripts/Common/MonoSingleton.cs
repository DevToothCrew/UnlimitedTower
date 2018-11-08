using UnityEngine;

// 유니티 전용
// 유니티 함수를 내부에서 사용할 수 있다.
// 웬만하면 이걸 쓰면 된다.
// 인스펙터에서 관찰 가능.
public class MonoSingleton<TSelfType> : MonoBehaviour where TSelfType : MonoBehaviour
{
	private void Awake()
	{
		if(instance == null)
		{
			CreateInstance();
		}
	}

	private static TSelfType instance = null;
	
	public static TSelfType Inst
	{
		get
		{
			if(instance == null)
			{
				CreateInstance();
            }
			return instance;
		}
	}

	private static void CreateInstance()
	{
		instance = (TSelfType)FindObjectOfType(typeof(TSelfType));

		if(instance == null)
		{
			instance = (new GameObject(typeof(TSelfType).Name)).AddComponent<TSelfType>();

			DontDestroyOnLoad(instance.gameObject);
		}
	}

}
