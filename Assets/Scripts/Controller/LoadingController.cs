using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingController : MonoBehaviour
{
    [SerializeField]
    public static string sceneName;

    [SerializeField]
    FadeController fadeController;
    [SerializeField]
    TextMeshProUGUI progressText;

    AsyncOperation operation;

    void StartLoadScene()
    {
        StartCoroutine(LoadAsync());
    }

    void ChangeScene()
    {
        operation.allowSceneActivation = true;
    }

    IEnumerator LoadAsync()
    {
        operation = SceneManager.LoadSceneAsync(sceneName);
        // When the scene loading completes, don't change the scene instantly
        operation.allowSceneActivation = false; 

        while(operation.progress < 0.9f)
        {
            progressText.text = (int)(operation.progress / 0.009f) + " %";
            yield return null;
        }

        progressText.text = "100 %";

        fadeController.OnFadeOutComplete.AddListener(ChangeScene);
        fadeController.FadeOut();
    }

    // Start is called before the first frame update
    void Start()
    {
        progressText.text = "";
        ResultData.missionName = "";
        ResultData.elapsedTime = 0;
        MissionManager.phase = 1;
        fadeController.OnFadeInComplete.AddListener(StartLoadScene);
    }
}
