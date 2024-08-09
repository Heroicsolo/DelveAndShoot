using Heroicsolo.DI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heroicsolo.Logics
{
    public interface ITeamsManager : ISystem
    {
        void RegisterTeamMember(IHittable member);
        void RegisterTeamMember(IHittable member, TeamType team);
        void UnregisterTeamMember(IHittable member);
        void ChangeMemberTeam(IHittable member, TeamType newTeam);
        List<IHittable> GetTeamMembers(TeamType team);
        IHittable GetNearestTeamMember(TeamType team, Vector3 from);
        IHittable GetNearestTeamMember(TeamType team, IHittable fromHittable);
        IHittable GetNearestTeamMemberOfType(TeamType team, Vector3 from, HittableType unitType);
        IHittable GetNearestMemberOfOppositeTeams(IHittable fromHittable, float searchDistance = 0f);
        List<IHittable> GetEnemiesInRadius(IHittable fromHittable, float searchRadius = 20f);
        List<IHittable> GetEnemiesInRadius(Vector3 from, TeamType shooterTeam, float searchRadius = 20f);
        bool IsOppositeTeam(IHittable hittable, TeamType teamToCheck);
        void ActivateUnits();
        void DeactivateUnits();
    }
}