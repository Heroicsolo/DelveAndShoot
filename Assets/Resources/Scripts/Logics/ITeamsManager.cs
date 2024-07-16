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
        IHittable GetNearestTeamMemberOfType(TeamType team, Vector3 from, HittableType unitType);
        IHittable GetNearestMemberOfOppositeTeams(IHittable fromHittable, float searchDistance = 0f);
        bool IsOppositeTeam(IHittable hittable, TeamType teamToCheck);
        void ActivateUnits();
        void DeactivateUnits();
    }
}