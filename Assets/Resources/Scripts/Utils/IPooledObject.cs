using UnityEngine;

namespace Heroicsolo.Utils
{
    public interface IPooledObject
    {
        string GetName();
        void SetName(string name);
        GameObject GetGameObject();
    }
}
