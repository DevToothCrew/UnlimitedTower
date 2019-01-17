using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifyList<T> : List<T> {

    public System.Action<T> addevent;
    public System.Action<T> removeevent;

	public void Add(T elem)
    {
        base.Add(elem);

        if (addevent != null)
        {
            addevent(elem);
        }
    }

    public void Remove(T elem)
    {
        base.Remove(elem);

        if (removeevent != null)
        {
            removeevent(elem);
        }
    }
}
