using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public interface IRestartGameElement
{
 void RestartGame();
}

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;

    static GameManager m_GameManager;
    List<IRestartGameElement> m_RestartGameElements = new List<IRestartGameElement>();
    private void Start()
    {
        if (m_GameManager == null)
        {
            m_GameManager = GameManager.GetGameManager();
        }
    }
    private void Awake()
    {
        if (m_GameManager == null)
        {
            m_GameManager = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
        else
            GameManager.Destroy(gameObject);
    }
    static public GameManager GetGameManager()
    {
        return m_GameManager;
    }
    public void GameOver()
    {
        gameOverUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None; // Desbloquea el cursor
        Cursor.visible = true; // Haz visible el cursor
        Time.timeScale = 0; // Pausa el juego
    }
    public void AddRestartGameElement (IRestartGameElement RestartgameElement)
    {
        m_RestartGameElements.Add(RestartgameElement);
    }
    public void RestartGame()
    {
        foreach (IRestartGameElement l_RestartGameElement in m_RestartGameElements)
            l_RestartGameElement.RestartGame();
    }
    public void Quit()
    {
        Application.Quit();
    }

}
