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

        private Dictionary<Type, Dictionary<string, IActor.Action>> _actionsCollection = new();

        public IActor.Action GetAction<T>(string name) where T : ManagedActor
        {
            return GetAction(typeof(T), name);
        }

        public IActor.Action GetAction(Type actorType, string name)
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
            if (_actionsCollection[actorType].Count > 0)
                _actionsCollection[actorType].Clear();
            var actionMethods = actorType
                                    .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                                    .Where(m => m.GetCustomAttribute<ActorActionAttribute>() != null);
            var actions = actionMethods.Select(am =>
            {
                var actDelegate = am.CreateDelegate(typeof(IActor.Act)) as IActor.Act;
                var actionAttr = am.GetCustomAttribute<ActorActionAttribute>();
                string name = actionAttr.name ?? am.Name;
                return new IActor.Action(name, actDelegate);
            });
            _actionsCollection[actorType].AddRange(
                actions.Select(i =>
                    new KeyValuePair<string, IActor.Action>(i.Name, i)
                    )
                );
        }

        public IEnumerable<IActor.Action> GetActions(Type actorType)
        {
            return _actionsCollection[actorType].Values;
        }
    }
}
