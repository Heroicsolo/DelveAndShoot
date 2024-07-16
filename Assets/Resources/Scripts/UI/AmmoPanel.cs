using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Heroicsolo.Scripts.UI
{
    public class AmmoPanel : MonoBehaviour
    {
        [SerializeField] private Transform ammoHolder;
        [SerializeField] private GameObject ammoPrefab;
        [SerializeField] private Color emptyAmmoColor = Color.gray;

        private List<GameObject> ammoObjects = new List<GameObject>();

        private int maxAmmo;
        private int nextAmmoIdx;
        private Color defaultAmmoColor;

        public void SetMaxAmmo(int maxAmmo)
        {
            ClearAmmo();

            this.maxAmmo = maxAmmo;

            AddAmmo(maxAmmo);

            nextAmmoIdx = 0;
        }

        public void ClearAmmo()
        {
            foreach (var ammo in ammoObjects.ToArray())
            {
                Destroy(ammo);
            }

            ammoObjects.Clear();
        }

        public void AddAmmo(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                ammoObjects.Add(Instantiate(ammoPrefab, ammoHolder));
            }
        }

        public void RemoveAmmo()
        {
            ammoObjects[nextAmmoIdx++].GetComponent<Image>().color = emptyAmmoColor;
        }

        private void Start()
        {
            defaultAmmoColor = ammoPrefab.GetComponent<Image>().color;
        }
    }
}