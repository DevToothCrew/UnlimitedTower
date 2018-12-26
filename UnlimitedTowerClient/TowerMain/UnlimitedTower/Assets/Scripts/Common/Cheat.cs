using System;

public class Cheat : MonoSingleton<Cheat>
{
    // Local 작동을 위한 Random Servant, Monster, Item, EOS, Money 추가 필요
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