using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Resources.Scripts.Utils
{
    public interface IHideable
    {
        public float HidedPercentage { get; }
        public void Hide(float percentage);
        public void Unhide();
    }
}
