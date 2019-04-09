using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageButton : MonoBehaviour {

    public GameObject languageList;
    public GameObject backButton;

    public Text languageName;
    public Image languageImage;

    private Sprite koreaImage;
    private Sprite chinaImage;
    private Sprite USImage;

    private void Awake()
    {
        languageList.SetActive(false);
        backButton.SetActive(false);

        koreaImage = Resources.Load<Sprite>("UI/LoginUI/ic_Korean_logo") as Sprite;
        chinaImage = Resources.Load<Sprite>("UI/LoginUI/ic_Chinese_logo") as Sprite;
        USImage = Resources.Load<Sprite>("UI/LoginUI/ic_Engligh_logo") as Sprite;

        languageImage.sprite = USImage;
    }

    public void OnClickLanguageButton()
    {
        languageList.SetActive(true);
        backButton.SetActive(true);
    }

    public void OnClickLanguage(int languageNum)
    {
        switch (languageNum)
        {
            case 0:
                languageName.text = "한국어";
                languageImage.sprite = koreaImage;
                break;

            case 1:
                languageName.text = "中文";
                languageImage.sprite = chinaImage;
                break;

            case 2:
                languageName.text = "English";
                languageImage.sprite = USImage;
                break;

            default:
                break;
        }

        languageList.SetActive(false);
        backButton.SetActive(false);
    }
}
