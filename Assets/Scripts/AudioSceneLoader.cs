using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioSceneLoader : MonoBehaviour
{
    private const string AudioSceneName = "AudioSystem";

    private void Awake()
    {
        if (AudioManager.Instance == null)
        {
            bool alreadyLoadingOrLoaded = false;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == AudioSceneName)
                {
                    alreadyLoadingOrLoaded = true;
                    break;
                }
            }

            if (!alreadyLoadingOrLoaded)
            {
                // In Editor, check if it's already in the hierarchy but not loaded yet
                #if UNITY_EDITOR
                bool found = false;
                for (int i = 0; i < UnityEditor.SceneManagement.EditorSceneManager.sceneCount; i++)
                {
                    if (UnityEditor.SceneManagement.EditorSceneManager.GetSceneAt(i).name == AudioSceneName)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    SceneManager.LoadScene(AudioSceneName, LoadSceneMode.Additive);
                }
                #else
                SceneManager.LoadScene(AudioSceneName, LoadSceneMode.Additive);
                #endif
            }
        }
    }
}
