using Heroicsolo.DI;
using Heroicsolo.Utils;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Heroicsolo.Scripts.UI
{
    public class GameUIView : MonoBehaviour, ISystem
    {
        private readonly Vector2 CursorOffset = new Vector2(0, 0);

        [Header("Player stats indication")]
        [SerializeField] private Image healthBar;
        [SerializeField] private Image healthBarOldValue;
        [SerializeField] private Image staminaBar;
        [SerializeField] private TextMeshProUGUI healthBarLabel;
        [SerializeField] private TextMeshProUGUI staminaBarLabel;
        [SerializeField] private GameObject playerLvlUpIndicator;
        [SerializeField] private Image expBar;
        [SerializeField] private TextMeshProUGUI expBarLabel;
        [SerializeField] private TextMeshProUGUI levelLabel;

        [Header("Ammo")]
        [SerializeField] private AmmoPanel ammoPanel;

        [Header("Cursor Params")]
        [SerializeField] private Texture2D aimCursorTexture;
        [SerializeField] private Texture2D aimCursorTextureTargeted;
        [SerializeField] private Texture2D pickUpCursorTexture;

        [Header("Info Popups")]
        [SerializeField] private WorldItemInfoPopup worldItemInfoPopupPrefab;

        private Dictionary<Guid, WorldItemInfoPopup> worldItemInfoPopups = new();

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void SetAmmo(int ammo)
        {
            ammoPanel.SetMaxAmmo(ammo);
        }

        public void RemoveAmmo()
        {
            ammoPanel.RemoveAmmo();
        }

        public void SetCursorState(CursorState aimState)
        {
            switch (aimState)
            {
                case CursorState.Aim:
                    Cursor.SetCursor(aimCursorTexture, CursorOffset, CursorMode.ForceSoftware);
                    break;
                case CursorState.Targeted:
                    Cursor.SetCursor(aimCursorTextureTargeted, CursorOffset, CursorMode.ForceSoftware);
                    break;
                case CursorState.PickUp:
                    Cursor.SetCursor(pickUpCursorTexture, CursorOffset, CursorMode.ForceSoftware);
                    break;
                case CursorState.Default:
                    Cursor.SetCursor(null, CursorOffset, CursorMode.ForceSoftware);
                    break;
            }
        }

        public void ShowWorldItemDesc(Guid itemID, string desc, Vector3 worldPos)
        {
            if (!worldItemInfoPopups.ContainsKey(itemID))
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

                WorldItemInfoPopup worldItemInfoPopup = PoolSystem.GetInstanceAtPosition(worldItemInfoPopupPrefab, worldItemInfoPopupPrefab.GetName(), screenPos, transform);

                worldItemInfoPopup.SetText(desc);

                worldItemInfoPopups.Add(itemID, worldItemInfoPopup);
            }
        }

        public void HideWorldItemDesc(Guid itemID)
        {
            if (worldItemInfoPopups.ContainsKey(itemID))
            {
                WorldItemInfoPopup worldItemInfoPopup = worldItemInfoPopups[itemID];

                worldItemInfoPopups.Remove(itemID);

                PoolSystem.ReturnToPool(worldItemInfoPopup);
            }
        }

        public void ShowLevelUpIndicator()
        {
            playerLvlUpIndicator.SetActive(true);
        }

        public void SetHealthBarValue(float oldVal, float currentVal, float maxVal)
        {
            healthBarOldValue.fillAmount = oldVal / maxVal;
            healthBar.fillAmount = currentVal / maxVal;
            healthBarLabel.text = $"{Mathf.CeilToInt(currentVal)}/{Mathf.CeilToInt(maxVal)}";
        }

        public void SetStaminaBarValue(float currentVal, float maxVal)
        {
            staminaBar.fillAmount = currentVal / maxVal;
            staminaBarLabel.text = $"{Mathf.CeilToInt(currentVal)}/{Mathf.CeilToInt(maxVal)}";
        }

        public void SetPlayerLevelInfo(int level, int currExp, int maxExp)
        {
            expBar.fillAmount = (float)currExp / maxExp;
            expBarLabel.text = $"{currExp}/{maxExp}";
            levelLabel.text = $"{level}";
        }

        private void Start()
        {
            Cursor.SetCursor(null, CursorOffset, CursorMode.ForceSoftware);
        }
    }
}