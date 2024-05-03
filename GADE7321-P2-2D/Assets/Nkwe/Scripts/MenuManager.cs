using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Canvas canvas1;
    public Canvas canvas2;
    public Canvas canvas3;

    public string scene1Name;
    public string scene2Name;

    public Slider volumeSlider;
    public AudioSource backgroundMusic;

    void Start()
    {
        // Disable all canvases initially
        canvas1.enabled = true;
        canvas2.enabled = false;
        canvas3.enabled = false;

        // Initialize volume slider value to current volume
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
        backgroundMusic.volume = volumeSlider.value;
    }

    public void LoadCanvas1()
    {
        // Disable other canvases and enable canvas1
        DisableAllCanvases();
        canvas1.enabled = true;
    }

    public void LoadCanvas2()
    {
        // Disable other canvases and enable canvas2
        DisableAllCanvases();
        canvas2.enabled = true;
    }

    public void LoadCanvas3()
    {
        // Disable other canvases and enable canvas3
        DisableAllCanvases();
        canvas3.enabled = true;
    }

    public void LoadScene0()
    {
        SceneManager.LoadScene(0);
    }
    public void LoadScene1()
    {
        SceneManager.LoadScene(1);
    }

    public void LoadScene2()
    {
        SceneManager.LoadScene(2);
    }

    public void SetVolume(float volume)
    {
        backgroundMusic.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
    }

    void DisableAllCanvases()
    {
        // Disable all canvases
        canvas1.enabled = false;
        canvas2.enabled = false;
        canvas3.enabled = false;
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}

