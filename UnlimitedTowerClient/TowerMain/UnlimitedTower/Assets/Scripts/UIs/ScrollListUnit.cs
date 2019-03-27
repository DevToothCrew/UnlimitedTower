using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollListUnit : MonoBehaviour {
    public RectTransform RectTr { get { return rect_tr; } }

    protected int main_idx;
    protected MonoBehaviour unit_controller;

    private RectTransform rect_tr;

    protected virtual void Awake()
    {
        rect_tr = GetComponent<RectTransform>();
    }

    public void Init (MonoBehaviour _unit_controller)
    {
        unit_controller = _unit_controller;
    }

    public virtual void UpdateUnit (int main_idx)
    {

    }

    public virtual void Selected (bool selected)
    {

    }

    public virtual int getUnitIdx()
    {
        return main_idx;
    }

    public virtual void OnClickUnit()
    {

    }
}
