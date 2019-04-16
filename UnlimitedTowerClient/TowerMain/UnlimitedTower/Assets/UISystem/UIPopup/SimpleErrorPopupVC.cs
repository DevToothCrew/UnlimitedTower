using UnityEngine;
using UnityEngine.UI;

public class SimpleErrorPopupVC : MonoBehaviour {

    public Text ErrorText;

    public void UpdateErrorText(string Text)
    {
        ErrorText.text = Text;
    }

    public void OnClickExitButton()
    {
        Destroy(this.gameObject);
    }
}
