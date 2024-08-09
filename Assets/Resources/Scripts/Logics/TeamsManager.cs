using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Heroicsolo.Logics
{
    public class TeamsManager : MonoBehaviour, ITeamsManager
    {
        private readonly Dictionary<TeamType, List<TeamType>> oppositeTeams = new Dictionary<TeamType, List<TeamType>>
        {
            [TeamType.Player] = new List<TeamType> { TeamType.Enemies, TeamType.Neutral },
            [TeamType.Enemies] = new List<TeamType> { TeamType.Player, TeamType.Neutral },
            [TeamType.Neutral] = new List<TeamType> { TeamType.Enemies, TeamType.Player },
        };

        private Dictionary<TeamType, List<IHittable>> teamsMembers = new Dictionary<TeamType, List<IHittable>>();
        private List<IHittable> allUnits = new List<IHittable>();

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void RegisterTeamMember(IHittable member)
        {
            TeamType team = member.GetTeamType();

            if (!teamsMembers.ContainsKey(team))
            {
                teamsMembers.Add(team, new List<IHittable> { member });
            }
            else
            {
                teamsMembers[team].Add(member);
            }

            if (!allUnits.Contains(member))
            {
                allUnits.Add(member);
            }
        }

        public void RegisterTeamMember(IHittable member, TeamType team)
        {
            member.SetTeam(team);

            if (!teamsMembers.ContainsKey(team))
            {
                teamsMembers.Add(team, new List<IHittable> { member });
            }
            else
            {
                teamsMembers[team].Add(member);
            }

            if (!allUnits.Contains(member))
            {
                allUnits.Add(member);
            }
        }

        public void UnregisterTeamMember(IHittable member)
        {
            TeamType team = member.GetTeamType();

            if (teamsMembers.ContainsKey(team) && teamsMembers[team].Contains(member))
            {
                teamsMembers[team].Remove(member);
            }

            if (allUnits.Contains(member))
            {
                allUnits.Remove(member);
            }
        }

        public void ChangeMemberTeam(IHittable member, TeamType newTeam)
        {
            UnregisterTeamMember(member);
            RegisterTeamMember(member, newTeam);
        }

        public List<IHittable> GetTeamMembers(TeamType team)
        {
            if (teamsMembers.ContainsKey(team))
            {
                return teamsMembers[team];
            }
            else
            {
                return new List<IHittable>();
            }
        }

        public IHittable GetNearestTeamMember(TeamType team, IHittable fromHittable)
        {
            List<IHittable> members = GetTeamMembers(team);

            Vector3 from = fromHittable.GetTransform().position;

            if (members.Count > 0)
            {
                List<IHittable> selectedMembers = new List<IHittable>();

                members.Where(m => !m.IsDead() && m != fromHittable).ToList().ForEach(m => selectedMembers.Add(m));

                if (selectedMembers.Count > 0)
                {
                    selectedMembers.Sort((a, b) => from.DistanceXZ(a.GetTransform().position).CompareTo(from.DistanceXZ(b.GetTransform().position)));

                    return selectedMembers[0];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public IHittable GetNearestTeamMember(TeamType team, Vector3 from)
        {
            List<IHittable> members = GetTeamMembers(team);

            if (members.Count > 0)
            {
                List<IHittable> selectedMembers = new List<IHittable>();

                members.Where(m => !m.IsDead()).ToList().ForEach(m => selectedMembers.Add(m));

                if (selectedMembers.Count > 0)
                {
                    selectedMembers.Sort((a, b) => from.DistanceXZ(a.GetTransform().position).CompareTo(from.DistanceXZ(b.GetTransform().position)));

                    return selectedMembers[0];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public IHittable GetNearestTeamMemberOfType(TeamType team, Vector3 from, HittableType unitType)
        {
            List<IHittable> members = GetTeamMembers(team);

            if (members.Count > 0)
            {
                List<IHittable> selectedMembers = new List<IHittable>();

                members.Where(m => !m.IsDead() && m.GetHittableType() == unitType).ToList().ForEach(m => selectedMembers.Add(m));

                if (selectedMembers.Count > 0)
                {
                    selectedMembers.Sort((a, b) => from.DistanceXZ(a.GetTransform().position).CompareTo(from.DistanceXZ(b.GetTransform().position)));

                    return selectedMembers[0];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public bool IsOppositeTeam(IHittable hittable, TeamType teamToCheck)
        {
            return oppositeTeams[teamToCheck].Contains(hittable.GetTeamType());
        }

        public List<IHittable> GetEnemiesInRadius(IHittable fromHittable, float searchRadius = 20f)
        {
            TeamType myTeam = fromHittable.GetTeamType();
            Vector3 from = fromHittable.GetTransform().position;

            return GetEnemiesInRadius(from, myTeam, searchRadius);
        }

        public List<IHittable> GetEnemiesInRadius(Vector3 from, TeamType shooterTeam, float searchRadius = 20f)
        {
            List<IHittable> selectedMembers = new List<IHittable>();

            foreach (TeamType team in oppositeTeams[shooterTeam])
            {
                List<IHittable> members = GetTeamMembers(team);

                if (members.Count > 0)
                {
                    members.Where(m => !m.IsDead()
                        && from.DistanceXZ(m.GetTransform().position) < searchRadius).ToList().ForEach(m => selectedMembers.Add(m));
                }
            }

            return selectedMembers;
        }


        public IHittable GetNearestMemberOfOppositeTeams(IHittable fromHittable, float searchDistance = 0f)
        {
            TeamType myTeam = fromHittable.GetTeamType();
            Vector3 from = fromHittable.GetTransform().position;

            List<IHittable> selectedMembers = new List<IHittable>();

            foreach (TeamType team in oppositeTeams[myTeam])
            {
                List<IHittable> members = GetTeamMembers(team);

                if (members.Count > 0)
                {
                    if (searchDistance > 0f)
                    {
                        members.Where(m => !m.IsDead() 
                            && from.DistanceXZ(m.GetTransform().position) < searchDistance).ToList().ForEach(m => selectedMembers.Add(m));
                    }
                    else
                    {
                        members.Where(m => !m.IsDead()).ToList().ForEach(m => selectedMembers.Add(m));
                    }
                }
            }

            if (selectedMembers.Count > 0)
            {
                selectedMembers.Sort((a, b) => from.DistanceXZ(a.GetTransform().position).CompareTo(from.DistanceXZ(b.GetTransform().position)));

                return selectedMembers[0];
            }
            else
            {
                return null;
            }
        }

        public void ActivateUnits()
        {
            foreach (IHittable unit in allUnits)
            {
                unit.Activate();
            }
        }

        public void DeactivateUnits()
        {
            foreach (IHittable unit in allUnits)
            {
                unit.Deactivate();
            }
        }
    }
}