using UnityEngine;
using UnityEngine.UI;

public class SimpleErrorPopupVC : MonoSingleton<SimpleErrorPopupVC>
{
    public GameObject ErrorPopup;
    public Text ErrorText;

    public void OnEnable()
    {
        ErrorPopup.SetActive(false);
    }

    public void UpdateErrorText(string Text)
    {
        ErrorPopup.SetActive(true);
        ErrorText.text = Text;
    }

    public void OnClickExitButton()
    {
        ErrorPopup.SetActive(false);
    }
}
