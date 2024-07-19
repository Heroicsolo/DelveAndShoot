using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heroicsolo.Scripts.Utils
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
    public class WeightedChoser<T> : IEnumerable<ValueWeight<T>>
    {

        private List<ValueWeight<T>> valuesWeights;
        private List<float> comulateWeights;
        private bool isReady = false;

        public WeightedChoser()
        {
            valuesWeights = new List<ValueWeight<T>>();
        }
        public WeightedChoser(IEnumerable<ValueWeight<T>> valuesWeights)
        {
            this.valuesWeights = new List<ValueWeight<T>>(valuesWeights);
        }
        private void Prepare()
        {
            valuesWeights = valuesWeights.OrderByDescending(i=>i.Weight).ToList();
            comulateWeights = valuesWeights.Select(kv=>kv.Weight).ComulativeSum().ToList();
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
                    return valuesWeights.ElementAt(guess).Value;
                float hopDist = target - comulateWeights[guess];
                guess += 1 + (int)(hopDist / valuesWeights.ElementAt(guess).Weight);
            }
        }

        public IEnumerator<ValueWeight<T>> GetEnumerator()
        {
            return valuesWeights.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
