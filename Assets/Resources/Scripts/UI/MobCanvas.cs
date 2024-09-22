using Heroicsolo.Logics;
using Heroicsolo.Logics;
using Heroicsolo.Utils;
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
        [SerializeField] private FloatingText combatTextPrefab;

        private ICharacter owner;

        public void SetOwner(ICharacter owner, string name)
        {
            this.owner = owner;
            title.text = name;
            owner.SubscribeToDamageGot(OnDamageGot);
            owner.SubscribeToDamageDodged(OnDamageDodged);
        }

        private void OnDamageGot(float amount)
        {
            if (amount > 0f)
            {
                FloatingText ft = PoolSystem.GetInstanceAtPosition(combatTextPrefab, combatTextPrefab.GetName(), transform.position, Quaternion.identity, transform);
                ft.SetText($"-{Mathf.CeilToInt(amount)}");
                ft.SetCurved(true);
            }
            else
            {
                FloatingText ft = PoolSystem.GetInstanceAtPosition(combatTextPrefab, combatTextPrefab.GetName(), transform.position, Quaternion.identity, transform);
                ft.SetText("ABSORB");
                ft.SetCurved(true);
            }
        }

        private void OnDamageDodged()
        {
            FloatingText ft = PoolSystem.GetInstanceAtPosition(combatTextPrefab, combatTextPrefab.GetName(), transform.position, Quaternion.identity, transform);
            ft.SetText($"MISS");
            ft.SetCurved(true);
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