using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum LevelType
{
    MainMenu,
    InGame,
}

public class ASyncLoader : MonoBehaviour
{
    [SerializeField] private GameObject _loadingScreen;
    [SerializeField] private Slider _progressSlider;

    private IEnumerator LoadLevelASync(int levelType)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelType);
        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            if (_progressSlider) _progressSlider.value = progressValue;
            yield return null;
        }
    }

    /// <summary>
    /// <para>'levelType' behind the scenes is 'LevelType' enum.
    /// <br>0 = mainMenu, 1 = levelOne, 2 = levelTwo.</br></para>
    /// </summary>
    /// <param name="levelType"></param>
    public void LoadLevel(int levelType)
    {
        _loadingScreen.SetActive(true);
        StartCoroutine(LoadLevelASync(levelType));
    }

    /// <summary>
    /// <para><br>0 = mainMenu, 1 = levelOne, 2 = levelTwo.</br></para>
    /// </summary>
    /// <param name="levelType"></param>
    public void LoadLevel(LevelType levelType)
    {
        _loadingScreen.SetActive(true);
        StartCoroutine(LoadLevelASync((int)levelType));
    }
}