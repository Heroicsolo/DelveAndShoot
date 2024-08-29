using Heroicsolo.DI;
using Heroicsolo.Logics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Heroicsolo.Scripts.UI
{
    public class LevelCompleteScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI expLabel;

        [Inject] private ScenesLoader scenesLoader;
        [Inject] private IPlayerProgressionManager playerProgressionManager;

        public void OnButtonNextClicked()
        {
            scenesLoader.LoadSceneAsync("GameScene", null);
        }

        private void Start()
        {
            SystemsManager.InjectSystemsTo(this);

            expLabel.text = $"+{playerProgressionManager.GetExpPerCurrentLevel()}";
        }
    }
}