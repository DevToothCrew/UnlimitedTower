using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISystem : MonoBehaviour {
    private GameObject Pause;
    private Image PauseBack;
    private Animator PauseAni;
    private int temp;

    private void Start()
    {
        Pause = GameObject.Find("PauseBack");
        PauseBack = Pause.transform.GetChild(0).GetComponent<Image>();
        PauseAni = Pause.GetComponent<Animator>();
    }

    public void SpeedX2()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 2;
            BattleSystem.Inst.TimeScale = 2;
        }
        else
        {
            Time.timeScale = 1;
            BattleSystem.Inst.TimeScale = 1;
        }
    }

    public void PauseOnAction()
    {
        PauseAni.SetTrigger("Stop");
        PauseBack.raycastTarget = true;
        Invoke("Stop", 0.5f);
    }

    public void Stop()
    {
        temp = BattleSystem.Inst.TimeScale;
        Time.timeScale = 0;
        BattleSystem.Inst.TimeScale = 0;
    }

    public void PauseOffAction()
    {
        Time.timeScale = temp;
        BattleSystem.Inst.TimeScale = temp;
        PauseAni.SetTrigger("Play");
        Invoke("Play", 0.5f);
    }

    public void Play()
    {
        PauseBack.raycastTarget = false;
    }
}
