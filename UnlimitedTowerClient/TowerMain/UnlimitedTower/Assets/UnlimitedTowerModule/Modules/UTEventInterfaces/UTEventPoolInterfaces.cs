using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using CallbackList = System.Collections.Generic.Dictionary<string, System.Action<IJSONableData>>;

/// <summary>
/// JSON형태를 보장해야하는 데이터들의 경우 해당 인터페이스를 상속해야합니다.
/// </summary>
public interface IJSONableData
{
    string ToJson();
}

/// <summary>
/// 만약 이벤트에 실패를 리턴해야 하는 경우 해당 클래스를 사용합니다.
/// </summary>
public class UTFailedData : IJSONableData
{
    public UTFailedData(string msg)
    {
        this.msg = msg;
    }

    public string msg;
    public string ToJson()
    {
        return msg;
    }
}

/// <summary>
/// 모든 데이터들의 이벤트 전달 역할을 해주는 Static 클래스 입니다.
/// 추후 역할에 따라 형태가 변할 수 있습니다.
/// </summary>
public static class UTEventPoolInterface
{
    //등록된 event list입니다
    static public CallbackList.KeyCollection eventList { get { return callbackList.Keys; } }

    //callBack list 입니다.
    static public CallbackList callbackList = new CallbackList();

    /// <summary>
    /// 변경된 데이터는 해당 클레스를 통해 전달됩니다.
    /// </summary>
    /// <param name="eventName"> 이벤트 이름 </param>
    /// <param name="data"> 보낼 데이터 </param>
    static public void SendEventData(string eventName, IJSONableData data)
    {
        //Action<IJSONableData> cbs = null;
        //if (callbackList.TryGetValue(eventName, out cbs))
        //{
        //    if (cbs != null)
        //    {
        //        cbs(data);
        //    }
        //}

        try
        {
            Action<IJSONableData> cbs = null;
            if (callbackList.TryGetValue(eventName, out cbs))
            {
                if (cbs != null)
                {
                    cbs(data);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(eventName);
            //Debug.LogError("[UTEventPoolInterface.OnReceivedEventData]" + e.Message);
        }
    }

    /// <summary>
    /// 이 함수를 사용하여 모든 이벤트들을 등록할 수 있습니다.
    /// 등록된 이벤트들은 해당 이벤트 발생시, 상속한 클래스 내부에서 호출해 주어야 합니다.
    /// </summary>
    /// <param name="eventName"> 이벤트 이름 </param>
    /// <param name="callback"> 받은 데이터 </param>
    static public void AddEventListener(string eventName, Action<IJSONableData> callback)
    {
        try
        {
            Action<IJSONableData> cbs = null;

            if (callbackList.TryGetValue(eventName, out cbs))
            {
                cbs += callback;
                callbackList[eventName] = cbs;
            }
            else
            {
                callbackList.Add(eventName, callback);
            }
        }
        catch(Exception e)
        {
            Debug.LogError("[UTEventPoolInterface.AddEventListener]" + e.Message);
        }
    }

}
