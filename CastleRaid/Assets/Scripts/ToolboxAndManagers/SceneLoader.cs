using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public delegate void LoadCompletedDelegate();
    public event LoadCompletedDelegate loadOperationComplete;

    private AsyncOperation loadOperation;
    private string targetSceneName;
    private bool loadingSceneBeingLoaded;

    private const string loadingSceneName = "Loading";
    private const float sceneChangingWaitTime = 1f;

    public void Begin(string sceneName, bool useLoadingScreen)
    {
        DontDestroyOnLoad(gameObject);
        targetSceneName = sceneName;       
        GameManager.levelStart += OnLevelStart;
        if (useLoadingScreen)
        {
            loadingSceneBeingLoaded = true;
            StartCoroutine(SceneLoadRoutine(loadingSceneName));
        }
        else
        {
            loadingSceneBeingLoaded = false;
            StartCoroutine(SceneLoadRoutine(targetSceneName));
        }
    }

    public bool IsLoading()
    {
        return loadOperation != null;
    }

    public float GetProgress()
    {
        if (IsLoading())
        {
            return loadOperation.progress;
        }
        else
        {
            return 0f;
        }
    }

    void OnLevelStart()
    {
        GameManager.levelInstance.ScreenFade(0f);
        GameManager.levelInstance.ScreenUnfade(sceneChangingWaitTime);

        if (loadingSceneBeingLoaded)
        {
            StartCoroutine(SceneLoadRoutine(targetSceneName));
            loadingSceneBeingLoaded = false;
        }
        else
        {
            OnLoadOperationComplete();
            Destroy(gameObject);
        }
    }

    void OnLoadOperationComplete()
    {
        if (loadOperationComplete != null)
        {
            loadOperationComplete.Invoke();
        }
    }

    void OnDestroy()
    {
        GameManager.levelStart -= OnLevelStart;
    }

    IEnumerator SceneLoadRoutine(string sceneToLoad)
    {
        loadOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        loadOperation.allowSceneActivation = false;
        while (true)
        {
            if (loadOperation.progress >= 0.9f)
            {
                break;
            }
            yield return null;
        }
        GameManager.levelInstance.ScreenFade(sceneChangingWaitTime);
        yield return new WaitForSeconds(sceneChangingWaitTime);
        loadOperation.allowSceneActivation = true;
		while (!loadOperation.isDone)
		{
			yield return null;
		}

		loadOperation = null;
	}
}
