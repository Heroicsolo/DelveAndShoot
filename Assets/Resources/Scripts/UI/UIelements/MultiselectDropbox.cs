using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Heroicsolo.UI.UIelements
{
    internal class MultiselectDropbox<T> : BindableElement, INotifyValueChanged<List<T>>
    {
        private List<T> list;
        private List<T> m_Value = new();
        public List<T> value
        {
            get => m_Value;
            set
            {
                if (value.Except(list).Count() <= 0)
                    this.value = value;
                else
                    throw new ArgumentException("vales to select not exists in list");
                if (panel != null)
                    using (ChangeEvent<List<T>> evt = ChangeEvent<List<T>>.GetPooled(m_Value, value))
                    {
                        evt.target = this;
                        SetValueWithoutNotify(value);
                        SendEvent(evt);
                    }
            }
        }

        public void SetValueWithoutNotify(List<T> newValue)
        {
            m_Value = newValue;
        }
    }
}
