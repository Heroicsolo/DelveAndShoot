using Heroicsolo.DI;
using Heroicsolo.Scripts.Logics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heroicsolo.Scripts.UI
{
    public class GameUIController : MonoBehaviour, IGameUIController
    {
        [SerializeField] [Min(0f)] private float healthChangeTime = 1f;

        [Inject] private GameUIView uiView;
        [Inject] private IPlayerProgressionManager playerProgressionManager;

        private bool uiElementSelected;

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void SetAmmo(int ammo)
        {
            uiView.SetAmmo(ammo);
        }

        public void RemoveAmmo()
        {
            uiView.RemoveAmmo();
        }

        public void SetUIElementSelected(bool value)
        {
            uiElementSelected = value;
        }

        public void SetCursorState(CursorState cursorState, bool forced = false)
        {
            if (forced || !uiElementSelected)
            {
                uiView.SetCursorState(cursorState);
            }
        }

        public void OnHealthChanged(float oldVal, float newVal, float maxVal)
        {
            StartCoroutine(HealthChanger(oldVal, newVal, maxVal));
        }

        public void OnStaminaChanged(float oldVal, float newVal, float maxVal)
        {
            uiView.SetStaminaBarValue(newVal, maxVal);
        }

        public void OnPlayerLevelUp(int level)
        {
            uiView.ShowLevelUpIndicator();
        }

        public void OnPlayerExperienceChanged(int level, int currExp, int neededExp)
        {
            uiView.SetPlayerLevelInfo(level, currExp, neededExp + currExp);
        }

        private IEnumerator HealthChanger(float oldVal, float newVal, float maxVal)
        {
            float currVal = oldVal;

            float changePeriod = healthChangeTime / Mathf.Abs(newVal - oldVal);

            int sign = (int)Mathf.Sign(newVal - oldVal);

            uiView.SetHealthBarValue(oldVal, newVal, maxVal);

            while (currVal != newVal)
            {
                yield return new WaitForSeconds(changePeriod);

                currVal += sign;

                uiView.SetHealthBarValue(currVal, newVal, maxVal);
            }

            uiView.SetHealthBarValue(newVal, newVal, maxVal);
        }

        private void Start()
        {
            playerProgressionManager.SubscribeToLevelUpEvent(OnPlayerLevelUp);
            playerProgressionManager.SubscribeToExpChangeEvent(OnPlayerExperienceChanged);

            (int currLvl, int currExp, int neededExp) = playerProgressionManager.GetPlayerLevelState();

            OnPlayerExperienceChanged(currLvl, currExp, neededExp);
        }
    }
}