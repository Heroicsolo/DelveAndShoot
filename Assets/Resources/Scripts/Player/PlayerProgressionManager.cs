using System;
using System.Collections.Generic;
using UnityEngine;

namespace Heroicsolo.Scripts.Logics
{
    public class PlayerProgressionManager : MonoBehaviour, IPlayerProgressionManager
    {
        private const string PlayerProgressionStateKey = "PlayerProgression";

        [Header("Progression Params")]
        [SerializeField][Min(1)] private int baseExpForLevel = 100;
        [SerializeField][Min(0f)] private float expForLevelDegreeCoef = 1.5f;
        [SerializeField][Min(0f)] private float expForLevelMultCoef = 2f;
        [SerializeField][Min(0)] private int baseSkillPoints = 1;
        [SerializeField][Min(0)] private int skillPointsPerLevel = 1;

        [Header("Currencies")]
        [SerializeField][Min(0)] private int startCurrencyAmount = 0;

        private Action<int> OnLevelUp;
        private Action<int, int, int> OnExperienceChanged;
        private PlayerSaves playerSaves;

        public (int, int, int) GetPlayerLevelState()
        {
            return (playerSaves.currentLevel, playerSaves.currentExp, GetNeededExpForLevelUp());
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void SubscribeToLevelUpEvent(Action<int> onLevelUpCallback)
        {
            OnLevelUp += onLevelUpCallback;
        }

        public void UnsubscribeFromLevelUpEvent(Action<int> onLevelUpCallback)
        {
            OnLevelUp -= onLevelUpCallback;
        }

        public void SubscribeToExpChangeEvent(Action<int, int, int> onExperienceChanged)
        {
            OnExperienceChanged += onExperienceChanged;
        }

        public void UnsubscribeFromExpChangeEvent(Action<int, int, int> onExperienceChanged)
        {
            OnExperienceChanged -= onExperienceChanged;
        }

        public void AddExperience(int amount)
        {
            playerSaves.currentExp += amount;

            int neededExp = GetNeededExpForLevelUp();

            int expTotal = playerSaves.currentExp;
            bool lvlChanged = false;

            while (expTotal >= neededExp)
            {
                playerSaves.currentLevel++;
                lvlChanged = true;
                expTotal -= neededExp;
                neededExp = GetNeededExpForLevelUp();
            }

            playerSaves.currentExp = expTotal;

            if (lvlChanged)
                OnLevelUp?.Invoke(playerSaves.currentLevel);

            OnExperienceChanged?.Invoke(playerSaves.currentLevel, playerSaves.currentExp, neededExp);

            SaveState();
        }

        public int GetNeededExpForLevelUp()
        {
            return GetCurrentLevelMaxExp() - playerSaves.currentExp;
        }

        public int GetCurrentLevelMaxExp()
        {
            return Mathf.CeilToInt(baseExpForLevel * (1f + expForLevelMultCoef * Mathf.Pow(playerSaves.currentLevel, expForLevelDegreeCoef)));
        }

        private void LoadState()
        {
            string playerSavesString = PlayerPrefs.GetString(PlayerProgressionStateKey, "");

            if (!string.IsNullOrEmpty(playerSavesString))
            {
                playerSaves = JsonUtility.FromJson<PlayerSaves>(playerSavesString);
            }
            else
            {
                playerSaves = new PlayerSaves
                {
                    currentLevel = 1,
                    currentExp = 0,
                    currencyAmount = startCurrencyAmount,
                };
            }
        }

        private void SaveState()
        {
            string playerSavesString = JsonUtility.ToJson(playerSaves);
            PlayerPrefs.SetString(PlayerProgressionStateKey, playerSavesString);
        }

        private void Awake()
        {
            LoadState();
        }

        private void OnApplicationQuit()
        {
            SaveState();
        }
    }

    [Serializable]
    public struct PlayerSaves
    {
        public int currentExp;
        public int currentLevel;
        public int currencyAmount;
    }
}