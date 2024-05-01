using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2Trigger : MonoBehaviour
{
    public Collider Col;
    public float DamageAmt = 0.1f;
    private void Update()
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player1"))
        {
            Player2Actions.Hits = true;
            SaveScript.Player1Health -= DamageAmt;
            if (SaveScript.Player1Timer < 2.0f)
            {
                SaveScript.Player1Timer += 2.0f;
            }
        }
    }
}
