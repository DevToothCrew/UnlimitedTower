using System;

#region MessageList

public class MessageList
{
    public static string GetMessage(MESSAGE_IDX idx)
    {
        string msg = "";

        switch(idx)
        {
            case MESSAGE_IDX.UPGRADE_MONSTER_CONFIRM:
                msg = "If reinforcement is fails, the enhancement phase will be reduced by a random number.";
                break;
            case MESSAGE_IDX.UPGRADE_EQUIPMENT_CONFIRM:
                msg = "If reinforcement is fails, the equipment will be destroyed.";
                break;
            default:
                break;
        }

        return msg;
    }
}

#endregion 