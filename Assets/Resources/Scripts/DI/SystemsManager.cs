using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Heroicsolo.DI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectAttribute : Attribute
    {

    }

    public class SystemsManager : MonoBehaviour
    {
        private static List<ISystem> SystemsContainer = new List<ISystem>();

        public static void RegisterSystem<T>(T systemInstance) where T : MonoBehaviour, ISystem
        {
            if (!SystemsContainer.Contains(systemInstance))
            {
                SystemsContainer.Add(systemInstance);

                foreach (ISystem sys in SystemsContainer)
                {
                    InjectSystemsTo(sys);
                }
            }
        }

        public static void InjectSystemsTo<T>(T instance)
        {
            Type monoType = instance.GetType();

            FieldInfo[] objectFields = monoType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            for (int i = 0; i < objectFields.Length; i++)
            {
                InjectAttribute attribute = Attribute.GetCustomAttribute(objectFields[i], typeof(InjectAttribute)) as InjectAttribute;

                if (attribute != null)
                {
                    Type injectType = objectFields[i].FieldType;
                    ISystem system = SystemsContainer.Find(x => injectType.IsInstanceOfType(x));

                    if (system != null)
                    {
                        objectFields[i].SetValue(instance, system);
                    }
                }
            }
        }

        private void Awake()
        {
            InitializeSystems();
            DontDestroyOnLoad(gameObject);
        }

        private static void InitializeSystems()
        {
            SystemsContainer.AddRange(FindObjectsOfType<MonoBehaviour>(true).OfType<ISystem>());

            foreach (ISystem sys in SystemsContainer)
            {
                InjectSystemsTo(sys);
                DontDestroyOnLoad(sys.GetGameObject());
            }
        }
    }
}