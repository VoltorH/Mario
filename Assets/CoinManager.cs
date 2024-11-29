using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    // Start is called before the first frame update
    public float score = 0;
    public Text scoreText;
    // Update is called once per frame
    public void AddScore()
    {

        score = score + 1;
        scoreText.text =""+ score;
    }
}
