using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRestartGameElement
{
 void RestartGame();
}
public class GameManager : MonoBehaviour
{
    static GameManager m_GameManager;
    List<IRestartGameElement> m_RestartGameElements = new List<IRestartGameElement>();
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
    public void AddRestartGameElement (IRestartGameElement RestartgameElement)
    {
        m_RestartGameElements.Add(RestartgameElement);
    }
    public void RestartGame()
    {
        foreach (IRestartGameElement l_RestartGameElement in m_RestartGameElements)
            l_RestartGameElement.RestartGame();
    }

}
