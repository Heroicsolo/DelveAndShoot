using Assets.Resources.Scripts.Logics;
using Heroicsolo.Logics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Heroicsolo.Scripts.UI
{
    public class MobCanvas : MonoBehaviour
    {
        [SerializeField] private Image hpBar;
        [SerializeField] private TextMeshProUGUI title;

        private ICharacter owner;

        public void SetOwner(ICharacter owner, string name)
        {
            this.owner = owner;
            title.text = name;
        }

        private void Update()
        {
            if (owner != null && !owner.IsDead())
            {
                hpBar.fillAmount = owner.GetCharacterStat(Logics.CharacterStatType.Health).Percent;
            }
        }
    }
}