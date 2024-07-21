using Heroicsolo.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Heroicsolo.Scripts.UI
{
    public class WorldItemInfoPopup : MonoBehaviour, IPooledObject
    {
        [SerializeField] private TextMeshProUGUI label;

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

        public void SetText(string text)
        {
            label.text = text;
        }
    }
}