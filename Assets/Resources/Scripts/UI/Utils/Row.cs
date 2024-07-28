using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Heroicsolo.Scripts.UI.Utils
{
    public class Row : ChildAnnotator
    {
        public Row(params VisualElement[] childs)
        {
            AddToClassList("row");
            for (int i = 0; i < childs.Length; i++)
                Add(childs[i]);
        }
    }
}
