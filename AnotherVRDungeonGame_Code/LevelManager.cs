using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject loadingScreen;
    [SerializeField] Slider loadingBar;

    private void Awake()
    {
        Time.timeScale = 1f;
        //loadingScreen.SetActive(false);
    }

    public void ReloadScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenu()
    {
        Debug.Log("Menu");
        LoadScene(0);
    }

    public void LoadMission()
    {
        LoadScene(1);
    }

    public void LoadArena()
    {
        LoadScene(3);
    }

    public void LoadSettings()
    {
        Debug.Log("Settings");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadScene(int sceneID)
    {
        Debug.Log("load scene");
        StartCoroutine(LoadSceneAsync(sceneID));
    }

    IEnumerator LoadSceneAsync(int sceneId)
    {
        Debug.Log("loading");
        FindObjectOfType<TimeControl>().enabled = false;
        Time.timeScale = 0;

        loadingScreen.transform.position = Camera.main.transform.position;
        loadingScreen.transform.rotation = Camera.main.transform.rotation;
        loadingScreen.SetActive(true);
        PlayerHealth player = FindObjectOfType<PlayerHealth>();
        player.deathCanvas.SetActive(false);
        //player.damageUI.color = Color.black;
        

        yield return new WaitForSecondsRealtime(2f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.value = progressValue;
            yield return null;
        }
    }
}
