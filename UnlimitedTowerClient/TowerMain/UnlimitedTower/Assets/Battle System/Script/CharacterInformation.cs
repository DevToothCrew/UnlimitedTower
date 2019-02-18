using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInformation : MonoBehaviour
{
    public float AttackDelay;
    public float AttackAfterDelay;
    public float Height;

    public CharacterInformation()
    {

    }

    public CharacterInformation(float AttackDelay, float AttackAfterDelay, float Height)
    {
        this.AttackDelay = AttackDelay;
        this.AttackAfterDelay = AttackAfterDelay;
        this.Height = Height;
    }
}
