using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using static Heroicsolo.Logics.ManagedActor;

namespace Heroicsolo.Logics
{
    internal class ActionManager : MonoBehaviour, IActionManager
    {
        [AttributeUsage(AttributeTargets.Method)]
        public class ActorActionAttribute : Attribute
        {
            //Also method replacing can be implemented to add some code sugar.
            //Replacing target method with invokation of delegaate copyed from original
            //htd: https://stackoverflow.com/a/36415711/14474569
            public string name;

            public ActorActionAttribute(string name)
            {
                this.name = name;
            }

            public ActorActionAttribute()
            {
                name = null;
            }
        }

        private Dictionary<Type, Dictionary<string, ManagedAction>> _actionsCollection = new(); //TODO: try to store as HashSet

        public IAction GetAction<T>(string name) where T : ManagedActor
        {
            return GetAction(typeof(T), name);
        }

        public IAction GetAction(Type actorType, string name)
        {
            return _actionsCollection[actorType][name];
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void RegisterManagedActor(Type actorType)
        {
            if (!_actionsCollection.ContainsKey(actorType) || _actionsCollection[actorType] == null)
                _actionsCollection[actorType] = new();
            else if (_actionsCollection[actorType].Count > 0)
                _actionsCollection[actorType].Clear();

            var actionMethods = actorType
                                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                    .Where(m => m.GetCustomAttribute<ActorActionAttribute>() != null);

            var actions = actionMethods.Select(am =>
            {
                var actionAttr = am.GetCustomAttribute<ActorActionAttribute>();
                string name = actionAttr.name ?? am.Name;
                return new ManagedAction(name, am);
            });

            _actionsCollection[actorType].AddRange(
                actions.Select(i =>
                    new KeyValuePair<string, ManagedAction>(i.Name, i)
                    )
                );
        }

        public IEnumerable<IAction> GetActions(Type actorType)
        {
            return _actionsCollection[actorType].Values;
        }
    }
}
