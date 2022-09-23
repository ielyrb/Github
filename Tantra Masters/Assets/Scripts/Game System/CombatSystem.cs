using System;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public static bool HitAndDodgeSystem(int hit, int dodge)
    {
        bool isDodged = false;

        double dodgeChance;
        dodgeChance = dodge + 100;
        dodgeChance = 100 / dodgeChance;
        dodgeChance = (hit + 50) * dodgeChance;
        dodgeChance = dodgeChance-100;
        dodgeChance = dodgeChance * -2;
        if (dodgeChance > 90)
        {
            dodgeChance = 90;
        }


        int rand = UnityEngine.Random.Range(0, 100);
        if (rand <= dodgeChance)
        {
            isDodged = true;
        }

        return isDodged;
    }

    public static Tuple<float, bool> CriticalSystem(int crit, int critEva, float critDamage, float critReduc)
    {
        float damageMod = 0f;
        bool isCrit = false;
        float critChance = 0f;
        critChance = 100 + (crit - critEva);
        critChance = 100 / critChance;
        critChance = 100 * critChance;
        critChance = critChance - 100;
        critChance = critChance * -1;
        critChance += 15;

        critChance = Mathf.Round(critChance);

        if (critChance > 95)
        {
            critChance = 95;
        }

        int rand = UnityEngine.Random.Range(0, 100);
        if (rand <= critChance)
        {
            damageMod = (critDamage - critReduc) / 100;
            isCrit = true;
        }

        return new Tuple<float, bool>(damageMod, isCrit);
    }
}
