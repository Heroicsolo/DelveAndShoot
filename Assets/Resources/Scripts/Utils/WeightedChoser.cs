using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Heroicsolo.Utils
{
    [Serializable]
    public struct ValueWeight<T> : IComparable<ValueWeight<T>>
    {
        public T Value;
        public float Weight;

        public int CompareTo(ValueWeight<T> other)
        {
            return Weight.CompareTo(other.Weight);
        }
    }

    [Serializable]
    public class WeightedChoser<T> : IDictionary<T,float>
    {

        private Dictionary<T,float> valuesWeights;
        private List<float> comulateWeights;
        private bool isReady = false;

        public ICollection<T> Keys => valuesWeights.Keys;

        public ICollection<float> Values => valuesWeights.Values;

        public int Count => valuesWeights.Count;

        public bool IsReadOnly => false;

        public float this[T key] 
        {
            get => valuesWeights[key]; 
            set 
            {
                valuesWeights[key] = value;
                isReady = false;
            } 
        }

        public WeightedChoser()
        {
            valuesWeights = new Dictionary<T, float>();
        }
        public WeightedChoser(IEnumerable<KeyValuePair<T,float>> valuesWeights)
        {
            this.valuesWeights = new Dictionary<T, float>(valuesWeights);
        }
        public WeightedChoser(Dictionary<T, float> valuesWeights)
        {
            this.valuesWeights = valuesWeights;
        }

        private void Prepare()
        {
            valuesWeights = new Dictionary<T, float>(valuesWeights.OrderByDescending(kv=>kv.Value));
            comulateWeights = valuesWeights.Select(kv=>kv.Value).ComulativeSum().ToList();
            isReady = true;
        }
        public T Chose()
        {
            if (!isReady)
                Prepare();
            float target = UnityEngine.Random.Range(0, comulateWeights.Last());
            int guess = 0;
            while (true)
            {
                if (comulateWeights[guess] > target)
                    return valuesWeights.Keys.ToArray()[guess];
                float hopDist = target - comulateWeights[guess];
                guess += 1 + (int)(hopDist / valuesWeights.Values.ToArray()[guess]);
            }
        }

        public void Add(T key, float value)
        {
            valuesWeights.Add(key, value);
            isReady = false;
        }

        public bool ContainsKey(T key)
        {
            return valuesWeights.ContainsKey(key);
        }

        public bool Remove(T key)
        {
            isReady = false;
            return valuesWeights.Remove(key);
        }

        public bool TryGetValue(T key, out float value)
        {
            return valuesWeights.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<T, float> item)
        {
            isReady = false;
            ((ICollection<KeyValuePair<T, float>>)valuesWeights).Add(item);
        }

        public void Clear()
        {
            valuesWeights.Clear();
            isReady = false;
        }

        public bool Contains(KeyValuePair<T, float> item)
        {
            return valuesWeights.Contains(item);
        }

        public void CopyTo(KeyValuePair<T, float>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<T, float>>)valuesWeights).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<T, float> item)
        {
            isReady = false;
            return ((ICollection<KeyValuePair<T, float>>)valuesWeights).Remove(item);
        }

        public IEnumerator<KeyValuePair<T, float>> GetEnumerator()
        {
            return valuesWeights.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)valuesWeights).GetEnumerator();
        }
    }
}
