using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelManager : NetworkSceneManagerBase
{
    Scene loadedScene = default;

    public void LoadLevel(int index)
    {
        FusionCallbacks.runner.SetActiveScene(index);
    }

    protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
    {
        LoadSceneParameters loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Single);
        if (loadedScene != default)
        {
            yield return SceneManager.UnloadSceneAsync(loadedScene);
        }
        loadedScene = default;
        List<NetworkObject> sceneObjects = new List<NetworkObject>();
        if (newScene > 0)
        {
            yield return SceneManager.LoadSceneAsync(newScene, loadSceneParameters);
            loadedScene = SceneManager.GetSceneByBuildIndex(newScene);
            sceneObjects = FindNetworkObjects(loadedScene, disable: false);
        }
        finished(sceneObjects);
    }
}
