using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Trigger : MonoBehaviour
{
    public Collider Col;
    public float DamageAmt = 0.1f;
    
    public bool EmitFX = false;
    public ParticleSystem Particles;
    public string ParticleType = "Player1Particles";
    private GameObject ChosenParticles;
    public bool Player2 = true;

    private void Start()
    {
        ChosenParticles = GameObject.Find(ParticleType);
        Particles = ChosenParticles.gameObject.GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (Player2 == true)
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
        else
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
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Player2 == true)
        {
            if (other.gameObject.CompareTag("Player1"))
            {
                if (EmitFX == true)
                {
                    Debug.Log("emit true p2");
                    Particles.Play();
                    Time.timeScale = 0.7f;
                }
                Player2Actions.Hits = true;
                SaveScript.Player1Health -= DamageAmt;
                if (SaveScript.Player1Timer < 2.0f)
                {
                    SaveScript.Player1Timer += 2.0f;
                }
            }
        }
        else if (Player2 == false)
        {
            if (other.gameObject.CompareTag("Player2"))
            {
                if (EmitFX == true)
                {
                    Debug.Log("emit true p1");
                    Particles.Play();
                    Time.timeScale = 0.7f;
                }
                Player2Actions.Hits = true;
                SaveScript.Player1Health -= DamageAmt;
                if (SaveScript.Player1Timer < 2.0f)
                {
                    SaveScript.Player1Timer += 2.0f;
                }
            }
        }
       
    }
}
