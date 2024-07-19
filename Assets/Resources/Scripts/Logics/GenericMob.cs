using Heroicsolo.Logics;
using Heroicsolo.Scripts.Logics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Resources.Scripts.Logics
{
    /// <summary>
    /// NOT FULLY IMPLEMENTED
    /// </summary>
    internal class GenericMob : Mob
    {
        [SerializeField]
        private string typeName;

        private string _typeName;

        private void Start()
        {
#if UNITY_EDITOR
            if(string.IsNullOrEmpty(typeName))
            {
                Debug.LogError($"Mob prefab {gameObject.name} spawned without proper pool name assigned");
                UnityEditor.EditorApplication.isPlaying = false;
            }
#endif
            _typeName = typeName;
        }

        public override void Activate()
        {
            throw new NotImplementedException();
        }

        public override void Deactivate()
        {
            throw new NotImplementedException();
        }

        public override void Die()
        {
            throw new NotImplementedException();
        }

        public override void GetDamage(float damage, DamageType damageType = DamageType.Physical)
        {
            throw new NotImplementedException();
        }

        public override GameObject GetGameObject()
        {
            return gameObject;
        }

        public override HittableType GetHittableType()
        {
            throw new NotImplementedException();
        }

        public override string GetName()
        {
            return _typeName;
        }

        public override TeamType GetTeamType()
        {
            throw new NotImplementedException();
        }

        public override Transform GetTransform()
        {
            throw new NotImplementedException();
        }

        public override void Heal(float amount)
        {
            throw new NotImplementedException();
        }

        public override bool IsDead()
        {
            throw new NotImplementedException();
        }

        public override void SetName(string name)
        {
            _typeName = name;
        }

        public override void SetTeam(TeamType team)
        {
            throw new NotImplementedException();
        }
    }
}
