using Heroicsolo.DI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Heroicsolo.Scripts.UI
{
    public class GameUIView : MonoBehaviour, ISystem
    {
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

        [Header("Aim")]
        [SerializeField] private Transform aimTransform;
        [SerializeField] private Image aimImage;

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void SetAimPos(Vector3 pos)
        {
            aimTransform.position = pos;
        }

        public void SetAmmo(int ammo)
        {
            ammoPanel.SetMaxAmmo(ammo);
        }

        public void RemoveAmmo()
        {
            ammoPanel.RemoveAmmo();
        }

        public void SetAimState(UIAimState aimState)
        {
            switch (aimState)
            {
                case UIAimState.Default:
                    aimTransform.gameObject.SetActive(true);
                    aimImage.color = Color.white;
                    Cursor.visible = false;
                    break;
                case UIAimState.Targeted:
                    aimTransform.gameObject.SetActive(true);
                    aimImage.color = Color.red;
                    Cursor.visible = false;
                    break;
                case UIAimState.Hidden:
                    aimTransform.gameObject.SetActive(false);
                    Cursor.visible = true;
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
    }
}