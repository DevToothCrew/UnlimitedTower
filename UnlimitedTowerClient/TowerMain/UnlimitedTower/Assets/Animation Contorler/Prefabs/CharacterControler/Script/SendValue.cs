public class SendValue {
    public int Attacker;
    public int Target;
    public bool isPlayer;

    public SendValue(int Attacker, int Target, bool isPlayer)
    {
        this.Attacker = Attacker;
        this.Target = Target;
        this.isPlayer = isPlayer;
    }
}
