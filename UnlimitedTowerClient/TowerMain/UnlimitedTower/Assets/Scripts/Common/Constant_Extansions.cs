using UnityEngine;

#region Extensions

static public class ActiveAnimationManager
{
    static public void SetActivateWithAnimation(this GameObject uiGO, bool value)
    {
        var animator = uiGO.GetComponent<Animator>();
        if (animator != null)
        {
            uiGO.SetActive(true);
            animator.SetTrigger(value ? "SetVisible" : "SetInvisible");
        }
        else
        {
            uiGO.SetActive(value);
        }
    }
}

static public class DebugLog
{
    static public void Log(bool error, string text)
    {
#if UNITY_EDITOR
        if(error == true)
        {
            Debug.LogError(text);
        }
        else
        {
            Debug.Log(text);
        }
#else
        return;
#endif
    }
}

#endregion