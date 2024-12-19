using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager1 : MonoBehaviour
{
    public GameObject gameOverUI;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Aseg¨²rate de que est¨¦ bloqueado al inicio
        Cursor.visible = false; // Cursor invisible al inicio
    }

    public void GameOver()
    {
        gameOverUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None; // Desbloquea el cursor
        Cursor.visible = true; // Haz visible el cursor
        Time.timeScale = 0; // Pausa el juego
    }


    public void Restart()
    {
        Time.timeScale = 1; // Reinicia el tiempo antes de cargar la escena
        Cursor.lockState = CursorLockMode.Locked; // Bloquea el cursor nuevamente
        Cursor.visible = false; // Haz que el cursor sea invisible
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

