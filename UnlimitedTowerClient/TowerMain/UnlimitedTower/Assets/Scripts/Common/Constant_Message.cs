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
                msg = "If the upgrade fails, ability of the main monster will be recuded by a random number. And the sub monster will be deleted.";
                break;
            case MESSAGE_IDX.UPGRADE_EQUIPMENT_CONFIRM:
                msg = "If the upgrade fails, the equipment will be destroyed.";
                break;
            default:
                break;
        }

        return msg;
    }
}

#endregion 