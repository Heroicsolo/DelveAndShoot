using Heroicsolo.DI;
using Heroicsolo.Logics;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenesLoader : MonoBehaviour, ISystem
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private TextMeshProUGUI loadingLabel;
    [SerializeField] private Image loadingBar;

    private bool sceneLoading = false;

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void LoadSceneAsync(string name, Action callback)
    {
        if (sceneLoading)
        {
            return;
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
        StartCoroutine(LevelLoader(asyncLoad, callback));
    }

    private IEnumerator LevelLoader(AsyncOperation asyncLoad, Action callback)
    {
        Time.timeScale = 1.0f;

        sceneLoading = true;

        loadingScreen.SetActive(true);

        while (!asyncLoad.isDone)
        {
            loadingLabel.text = $"LOADING: {Mathf.CeilToInt(100f * asyncLoad.progress)}%";
            loadingBar.fillAmount = asyncLoad.progress;
            yield return null;
        }

        callback?.Invoke();

        loadingScreen.SetActive(false);

        sceneLoading = false;
    }
}
