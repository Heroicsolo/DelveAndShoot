using Heroicsolo.DI;
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
                case CursorState.Default:
                    Cursor.SetCursor(null, CursorOffset, CursorMode.ForceSoftware);
                    break;
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
            healthBarLabel.text = $"{currentVal}/{maxVal}";
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