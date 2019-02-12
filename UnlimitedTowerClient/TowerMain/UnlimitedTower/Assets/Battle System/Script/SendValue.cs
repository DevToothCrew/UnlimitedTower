public class SendValue {
    public int Attacker;
    public int Target;
    public int Damage;
    public bool isCritical;
    public bool isAvoid;
    public bool isPlayer;

    public SendValue(int Attacker, int Target, int Damage, bool isCritical, bool isAvoid, bool isPlayer)
    {
        this.Attacker = Attacker;
        this.Target = Target;
        this.Damage = Damage;
        this.isCritical = isCritical;
        this.isAvoid = isAvoid;
        this.isPlayer = isPlayer;
    }
}
