using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasRenderer))]
public class ButtonUIExtension : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{

    [Header("Button UI Extension")]
    public float transitionDuration = 0.5f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Sprite normalSprite;
    public Sprite highlightedSprite;
    public Sprite pressedSprite;
    public Sprite disabledSprite;

    [HideInInspector]
    public Image targetGraphic;

    [HideInInspector]
    private Image subGraphic;

    public UnityEvent<PointerEventData> OnClick;
    public UnityEvent<PointerEventData> OnEnter;
    public UnityEvent<PointerEventData> OnExit;

    private bool _isEnabled = true;
    public bool isEnabled
    {
        get
        {
            return _isEnabled;
        }
        set
        {
            if(_isEnabled != value)
            {
                _isEnabled = value;
                SetSpriteByState();
            }
        }
    }

    private bool _isPressed = false;
    public bool isPressed {
        get
        {
            return _isPressed;
        }
        private set
        {
            if(_isPressed != value)
            {
                _isPressed = value;
                SetSpriteByState();
            }
        }
    }

    private bool _isHovered = false;
    public bool isHovered
    {
        get
        {
            return _isHovered;
        }
        private set
        {
            if(_isHovered != value)
            {
                _isHovered = value;
                SetSpriteByState();
            }
        }
    }


    private float currDuration = 0.0f;

    protected void Awake()
    {
        //Create Sub Graphic Object
        GameObject go = (new GameObject("Sub Graphic"));
        go.transform.SetParent(transform, false);
        go.transform.SetSiblingIndex(0);

        subGraphic = go.AddComponent<Image>();

        SetSpriteByState();
    }

    protected void Update()
    {
        currDuration = Mathf.Clamp01(currDuration + Time.fixedUnscaledDeltaTime);
        float t = transitionCurve.Evaluate(currDuration / Mathf.Max(0.0001f, transitionDuration));

        subGraphic.GetComponent<CanvasRenderer>()?.SetAlpha(1.0f - t);
        targetGraphic.GetComponent<CanvasRenderer>()?.SetAlpha(t);
    }

    private void SetSpriteByState()
    {
        if(!isEnabled)
        {
            SetNextSprite(disabledSprite);
        }
        else
        {
            if(isPressed)
            {
                SetNextSprite(pressedSprite);
            }
            else
            {
                if(isHovered)
                {
                    SetNextSprite(highlightedSprite);
                }
                else
                {
                    if (normalSprite == null)
                        normalSprite = targetGraphic.sprite;

                    SetNextSprite(normalSprite);
                }
            }
        }
    }

    private void SetNextSprite(Sprite nextSprite)
    {
        if(targetGraphic.sprite != nextSprite)
        {
            currDuration = 1.0f - currDuration;
            subGraphic.sprite = targetGraphic.sprite;
            targetGraphic.sprite = nextSprite;
        }
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (!isEnabled) return;
        isPressed = true;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        if (!isEnabled) return;

        if (eventData.selectedObject == gameObject && isPressed)
            OnClick?.Invoke(eventData);

        isPressed = false;
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!isEnabled) return;
        isHovered = true;

        OnEnter?.Invoke(eventData);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!isEnabled) return;
        isHovered = false;

        OnExit?.Invoke(eventData);
    }
}
