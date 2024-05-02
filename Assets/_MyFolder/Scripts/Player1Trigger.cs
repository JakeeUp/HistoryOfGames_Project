using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Trigger : MonoBehaviour
{
    public Collider Col;
    public float DamageAmt = 0.1f;

    public bool EmitFX = false;
    public ParticleSystem Particles;

    public string ParticleType = "Player2Particles";
    public bool Player1 = true;


    private GameObject ChosenParticles;


    private void Start()
    {
        ChosenParticles = GameObject.Find(ParticleType);
        Particles = ChosenParticles.gameObject.GetComponent<ParticleSystem>();
    }


    private void Update()
    {
        if (Player1 == true)
        {
            if (Player1Actions.Hits == false)
            {
                Col.enabled = true;
            }
            else
            {
                Col.enabled = false;
            }
        }
        else
        {
            if (Player2Actions.Hits == false)
            {
                Col.enabled = true;
            }
            else
            {
                Col.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Player1 == true)
        {
            if (other.gameObject.CompareTag("Player2"))
            {
                if (EmitFX == true)
                {
                    Debug.Log("emit true p1");
                    Particles.Play();
                    Time.timeScale = 0.7f;
                }
                Player1Actions.Hits = true;
                SaveScript.Player2Health -= DamageAmt;
                if (SaveScript.Player2Timer < 2.0f)
                {
                    SaveScript.Player2Timer += 2.0f;
                }
            }
        }
        else if (Player1 == false)
        {
            if (other.gameObject.CompareTag("Player1"))
            {
                if (EmitFX == true)
                {
                    Debug.Log("emit true p2");
                    Particles.Play();
                    Time.timeScale = 0.7f;
                }
                Player1Actions.Hits = true;
                SaveScript.Player2Health -= DamageAmt;
                if (SaveScript.Player2Timer < 2.0f)
                {
                    SaveScript.Player2Timer += 2.0f;
                }
            }
        }
        
    }
}
