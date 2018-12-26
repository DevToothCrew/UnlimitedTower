using System;

public class Cheat : MonoSingleton<Cheat>
{
    public Servant GetRandomServant()
    {
        Random rand = new Random();
        Servant servant = new Servant();
        servant.appear = 0;
        servant.exp = rand.Next(0, DEFINE.MAX_EXP);
        servant.state = 0;

        return servant;
    }
}