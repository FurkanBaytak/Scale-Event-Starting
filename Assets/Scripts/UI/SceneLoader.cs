using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader: MonoBehaviour
{
    public Button startButton;
    public string sceneName;
    public float delay = 5.0f;

    void Start()
    {
        startButton.onClick.AddListener(() => StartCoroutine(LoadSceneWithDelay()));
    }

    IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(sceneName);
    }
}
