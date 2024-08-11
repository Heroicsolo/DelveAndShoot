using Heroicsolo.Logics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heroicsolo.Logics
{
    public interface IHittable
    {
        void Heal(float amount);
        void GetDamage(float damage, DamageType damageType = DamageType.Physical);
        void DodgeDamage();
        void Die();
        bool IsDead();
        GameObject GetGameObject();
        Transform GetTransform();
        Transform GetHitPivot();
        HittableType GetHittableType();
        void SetTeam(TeamType team);
        TeamType GetTeamType();
        void Activate();
        void Deactivate();
    }

    public enum DamageType
    {
        Physical = 0,
        Siege = 10,
        Fire = 20,
        Electrical = 30,
        Frost = 40,
        Poison = 50,
        Curse = 60
    }

    public enum HittableType
    {
        Humanoid = 0,
        Beast = 10,
        Elemental = 20,
        Mech = 30,
        Ghost = 40,
        Giant = 50,
        Props = 60
    }

    public enum TeamType
    {
        Player = 0,
        Enemies = 10,
        Neutral = 20
    }
}