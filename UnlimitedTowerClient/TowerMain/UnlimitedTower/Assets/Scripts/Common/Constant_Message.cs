using System;

#region MessageList

public class MessageList
{
    public static string GetMessage(MESSAGE_IDX idx)
    {
        string msg = "";

        switch(idx)
        {
            case MESSAGE_IDX.UPGRADE_CONFIRM:
                msg = "장비 업그레이드시, 장비가 파괴될 가능성이 있습니다.\n그래도 진행 하시겠습니까?";
                break;
            default:
                break;
        }

        return msg;
    }
}

#endregion 