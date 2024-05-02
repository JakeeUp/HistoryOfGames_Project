using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBars : MonoBehaviour
{
    public Image Player1Green;
    public Image Player2Green;
    public Image Player1Red;
    public Image Player2Red;

    public TextMeshProUGUI TimerText;

    public float LevelTime = 90;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (LevelTime > 0)
        {
            LevelTime -= 1 * Time.deltaTime;
        }

        if (LevelTime <= .9)
        {
            SaveScript.TimeOut = true;
        }
        TimerText.text = Mathf.Round(LevelTime).ToString();
        
        Player1Green.fillAmount = SaveScript.Player1Health;
        Player2Green.fillAmount = SaveScript.Player2Health;

        if (SaveScript.Player2Timer > 0)
        {
            SaveScript.Player2Timer -= 2.0f * Time.deltaTime;
        }


        if (SaveScript.Player2Timer <= 0)
        {
            if (Player2Red.fillAmount > SaveScript.Player2Health)
            {
                Player2Red.fillAmount -= 0.003f;
            }
        }
        
        
        if (SaveScript.Player1Timer > 0)
        {
            SaveScript.Player1Timer -= 2.0f * Time.deltaTime;
        }


        if (SaveScript.Player1Timer <= 0)
        {
            if (Player1Red.fillAmount > SaveScript.Player1Health)
            {
                Player1Red.fillAmount -= 0.003f;
            }
        }
    }
}
