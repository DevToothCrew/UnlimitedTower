using System;
using System.Reflection;

// 유니티 함수 사용하지 않을 때 사용
public class Singleton<T> where T : class 
{
	private static object syncObj = new object();
	private static volatile T instance = null;

	public static T Instance{
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
		lock(syncObj)
		{
			if(instance == null)
			{
				Type t = typeof(T);

				ConstructorInfo[] cConstr = t.GetConstructors();

                if (cConstr.Length > 0)
                {
                    throw new InvalidOperationException(String.Format("{0} has at least one accesible cConstr making it impossible to enforce singleton behaviour", t.Name));
                }

                instance = (T)Activator.CreateInstance(t, true);
			}
		}
	}
}