public class SendValue {
    public int Attacker;
    public int Target;
    public bool isPlayer;
    public int Damage;

    public SendValue(int Attacker, int Target, bool isPlayer, int Damage)
    {
        this.Attacker = Attacker;
        this.Target = Target;
        this.isPlayer = isPlayer;
        this.Damage = Damage;
    }
}
