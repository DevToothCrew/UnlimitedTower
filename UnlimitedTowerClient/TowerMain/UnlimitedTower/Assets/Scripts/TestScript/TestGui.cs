using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class TestGui : MonoBehaviour {

    int m_nWindowId = 0;
    Rect m_WindowRect = new Rect(100, 10, 300, 100);


    // Use this for initialization
    private void Awake()
    {

    }


    private void OnGUI()
    {
        m_WindowRect = GUI.Window(m_nWindowId, m_WindowRect, WindowFunction, "Status");
        
    }
    void WindowFunction(int nWindowId)
    {
        GUI.DragWindow();
        //

        if (Picking.Inst.SelectedCharObject)
        {
            CharController charCtrl = Picking.Inst.SelectedCharObject.GetComponent<CharController>();

            //if (charCtrl.charType == CHAR_TYPE.PLAYER)
            if (charCtrl)
            {
                //ChartController chartCtrl = pickedObject.GetComponent<ChartController>();
                GUI.Label(new Rect(20, 15, 200, 25), "Name : " + charCtrl.name);
                //GUI.Label(new Rect(20, 30, 200, 40), "Hp : " + BattleManager.Inst.GetChar(charCtrl.charType, charCtrl.battleDicIndex).nowHp

                //    + " / " + BattleManager.Inst.GetChar(charCtrl.charType, charCtrl.battleDicIndex).maxHp);
                //GUI.Label(new Rect(20, 45, 200, 55), "Dam : " + BattleManager.Inst.GetChar(charCtrl.charType, charCtrl.battleDicIndex).damage.ToString());
                //GUI.Label(new Rect(20, 60, 200, 70), "TurnSpeed : " + BattleManager.Inst.GetChar(charCtrl.charType, charCtrl.battleDicIndex).speed);

            }

            


        }
            
    }
}
