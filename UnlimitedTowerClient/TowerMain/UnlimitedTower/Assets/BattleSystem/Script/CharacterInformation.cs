using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInformation : MonoBehaviour
{
    public float AttackDelay;
    public float AttackAfterDelay;
    public float Height;
    public float AttackRange;
    
    // public CharacterInformation(float AttackDelay, float AttackAfterDelay, float Height, float AttackRange)
    // {
    //     this.AttackDelay = AttackDelay;
    //     this.AttackAfterDelay = AttackAfterDelay;
    //     this.Height = Height;
    //     this.AttackRange = AttackRange;
    // }

    public void SetValue(float AttackDelay, float AttackAfterDelay, float Height, float AttackRange)
    {
        this.AttackDelay = AttackDelay;
        this.AttackAfterDelay = AttackAfterDelay;
        this.Height = Height;
        this.AttackRange = AttackRange;
    }
}