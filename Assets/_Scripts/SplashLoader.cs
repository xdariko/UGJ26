using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashLoader : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "SampleScene";
    [SerializeField] private float minSplashTime = 2f;

    private IEnumerator Start()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextSceneName);

        if (op == null)
            yield break;

        op.allowSceneActivation = false;

        yield return new WaitForSeconds(minSplashTime);

        while (op.progress < 0.9f)
        {
            yield return null;
        }

        op.allowSceneActivation = true;
    }
}
