using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Heroicsolo.Scripts.UI
{
    public class DialogPopup : MonoBehaviour, IDialogPopup
    {
        [SerializeField] private Image avatar;
        [SerializeField] private TextMeshProUGUI messageLabel;
        [SerializeField] [Min(0f)] private float maxTextAppearanceTime = 3f;
        [SerializeField] [Min(1f)] private float showTime = 3f;
        [SerializeField] [Min(1)] private int textAppearanceSpeed = 10;

        private bool isAppearing = false;
        private float timeToHide;

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void ShowMessage(string message, Sprite avatarSprite)
        {
            gameObject.SetActive(true);
            avatar.sprite = avatarSprite;
            StopAllCoroutines();
            StartCoroutine(TextAppearance(message));
        }

        private IEnumerator TextAppearance(string text)
        {
            float minInterval = maxTextAppearanceTime / text.Length;
            float interval = Mathf.Min(minInterval, 1f / textAppearanceSpeed);

            messageLabel.text = "";

            int symbolsPrinted = 0;

            isAppearing = true;

            do
            {
                messageLabel.text += text[symbolsPrinted];

                symbolsPrinted++;

                yield return new WaitForSeconds(interval);
            }
            while(symbolsPrinted < text.Length && isAppearing);

            messageLabel.text = text;

            timeToHide = showTime;

            isAppearing = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isAppearing)
            {
                isAppearing = false;
                timeToHide = showTime;
            }
            else
            {
                timeToHide = 0f;
                gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (timeToHide > 0f)
            {
                timeToHide -= Time.deltaTime;

                if (timeToHide <= 0f)
                {
                    timeToHide = 0f;
                    gameObject.SetActive(false);
                }
            }
        }
    }
}