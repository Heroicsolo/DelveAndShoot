using System.Collections.Generic;
using UnityEngine;

namespace Heroicsolo.Utils
{
    public class PoolSystem : MonoBehaviour
    {
        public static PoolSystem instance;

        private readonly Dictionary<string, Queue<IPooledObject>> pool = new Dictionary<string, Queue<IPooledObject>>();
        private readonly Dictionary<string, IPooledObject> prefabs = new Dictionary<string, IPooledObject>();
        private readonly List<GameObject> poolGOs = new List<GameObject>();

        private void Awake()
        {
            if (!instance)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public static void ResetPools(bool destroyImmediately = true)
        {
            foreach (var go in instance.poolGOs.ToArray())
            {
                if (destroyImmediately)
                {
                    Object.DestroyImmediate(go);
                }
                else
                {
                    Object.Destroy(go);
                }
            }

            instance.poolGOs.Clear();

            foreach (var queue in instance.pool.Values)
            {
                queue.Clear();
            }
        }

        public static T GetInstanceAtPosition<T>(T prefab, string name, Vector3 pos, Transform parent = null) where T : MonoBehaviour, IPooledObject
        {
            Vector3 scale = prefab.transform.localScale;
            T ins = GetInstance(prefab, name);
            if (parent != null)
            {
                ins.transform.SetParent(parent);
            }
            ins.transform.position = pos;
            ins.transform.localScale = scale;
            ins.transform.rotation = Quaternion.identity;
            return ins;
        }

        public static T GetInstanceAtPosition<T>(T prefab, string name, Vector3 pos, Quaternion rotation, Transform parent = null) where T : MonoBehaviour, IPooledObject
        {
            Vector3 scale = prefab.transform.localScale;
            T ins = GetInstance(prefab, name);
            if (parent != null)
            {
                ins.transform.SetParent(parent);
            }
            ins.transform.position = pos;
            ins.transform.localScale = scale;
            ins.transform.rotation = rotation;
            return ins;
        }

        public static T GetInstance<T>(T prefab, string name) where T : MonoBehaviour, IPooledObject
        {
            return instance.GetOrSpawnInstance(prefab, name);
        }

        public static T GetObj<T>(string name) where T : MonoBehaviour, IPooledObject
        {
            return instance.GetObject<T>(name);
        }

        public static void ReturnToPool<T>(T obj) where T : MonoBehaviour, IPooledObject
        {
            if (!Application.isPlaying)
            {
                Object.Destroy(obj.gameObject);
                return;
            }

            obj.gameObject.transform.SetParent(null);
            obj.gameObject.SetActive(false);

            string name = obj.GetName();

            if (instance.pool.ContainsKey(name) == false)
            {
                instance.pool.Add(name, new Queue<IPooledObject>());
            }

            instance.poolGOs.Add(obj.GetGameObject());
            instance.pool[obj.GetName()].Enqueue(obj);
        }

        private T GetOrSpawnInstance<T>(T prefab, string name) where T : MonoBehaviour, IPooledObject
        {
            if (pool.ContainsKey(name) == false)
            {
                pool.Add(name, new Queue<IPooledObject>());
            }

            var ins = pool[name].Count > 0 ? pool[name].Dequeue() : null;

            if (ins == null)
            {
                ins = SpawnObject(prefab);
            }
            else
            {
                instance.poolGOs.Remove(ins.GetGameObject());
            }

            var result = (T)ins;

            PrepareObj(result, name);

            return result;
        }

        private T GetObject<T>(string name) where T : MonoBehaviour, IPooledObject
        {
            var ins = pool[name].Count > 0 ? pool[name].Dequeue() : null;

            if (ins == null)
            {
                ins = SpawnObject((T)prefabs[name]);
            }
            else
            {
                instance.poolGOs.Remove(ins.GetGameObject());
            }

            var result = (T)ins;

            PrepareObj(result, name);

            return result;
        }

        private void PrepareObj<T>(T obj, string name) where T : MonoBehaviour, IPooledObject
        {
            obj.gameObject.SetActive(true);
            obj.SetName(name);
        }

        private T SpawnObject<T>(T prefab) where T : MonoBehaviour, IPooledObject
        {
            var ins = Object.Instantiate(prefab);
            ins.gameObject.SetActive(false);
            return ins;
        }
    }
}
