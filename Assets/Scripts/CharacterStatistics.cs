using System;
public class CharacterStatistics
{
    public int HealthMax { get; private set; }
    public int Attack { get; private set; }
    public int Defense { get; private set; }
    public float Speed { get; private set; }
    public float Range { get; private set; }
    public float Accuracy { get; private set; }

    public CharacterStatistics(int healthMax, int attack, int defense, float speed, float range, float accuracy)
    {
        HealthMax = healthMax;
        Attack = attack;
        Defense = defense;
        Speed = speed;
        Range = range;
        Accuracy = accuracy;
    }

}
