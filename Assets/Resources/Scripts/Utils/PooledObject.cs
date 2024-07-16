using UnityEngine;

namespace Heroicsolo.Utils
{
    public class PooledObject : MonoBehaviour, IPooledObject
    {
        private string poolName;
    
        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void SetName(string name)
        {
            poolName = name;
        }

        public string GetName()
        {
            return poolName;
        }
    }
}
