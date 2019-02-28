public class StatusFrame {
    public int Hp;
    public int Defence;
    public int Ad;
    public int Critical;
    public int CriticalDamage;
    public int Speed;

    public StatusFrame(Status status, SERVANT_JOB Job)
    {
        Hp = (status.basicStr + status.plusStr) * 22;
        Defence = (status.basicDex + status.plusDex) * 1;

        if (Job == SERVANT_JOB.Archer || Job == SERVANT_JOB.Thief)
            Ad = (status.basicDex + status.plusDex) * 2;
        else if (Job == SERVANT_JOB.Magician || Job == SERVANT_JOB.Cleric)
            Ad = (status.basicInt + status.plusInt) * 2;
        else if (Job == SERVANT_JOB.Warrior)
            Ad = (status.basicStr + status.plusStr) * 2;

        Critical = (status.basicInt + status.plusInt) * 1;
        CriticalDamage = 100;

        switch (Job)
        {
            case SERVANT_JOB.Warrior:
                Speed = 34;
                break;
            case SERVANT_JOB.Thief:
                Speed = 50;
                break;
            case SERVANT_JOB.Archer:
                Speed = 42;
                break;
            case SERVANT_JOB.Magician:
                Speed = 29;
                break;
            case SERVANT_JOB.Cleric:
                Speed = 32;
                break;
            case SERVANT_JOB.WhiteHand:
                Speed = 30;
                break;
        }
    }
}
