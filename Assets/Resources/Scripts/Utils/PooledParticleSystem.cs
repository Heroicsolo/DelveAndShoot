using UnityEngine;

namespace Heroicsolo.Utils
{
    public class PooledParticleSystem : MonoBehaviour, IPooledObject
    {
        private ParticleSystem particleSystem;

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public string GetName()
        {
            return gameObject.name;
        }

        public void SetName(string name)
        {
            gameObject.name = name;
        }

        private void Awake()
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            if (!particleSystem.IsAlive())
            {
                PoolSystem.ReturnToPool(this);
            }
        }
    }
}