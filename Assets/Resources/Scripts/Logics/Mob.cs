using Heroicsolo.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Heroicsolo.Scripts.Logics
{
    internal abstract class Mob : MonoBehaviour, IPooledObject
    {
        public abstract GameObject GetGameObject();

        public abstract string GetName();

        public abstract void SetName(string name);
    }
}
