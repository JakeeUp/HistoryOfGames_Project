using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P1MoveRestrict : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("P2Left"))
        {
            Player1Move._walkRightP1 = false;
        }
        if (other.gameObject.CompareTag("P2Right"))
        {
            Player1Move._walkLeftP1 = false;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("P2Left"))
        {
            Player1Move._walkRightP1 = true;
        }
        if (other.gameObject.CompareTag("P2Right"))
        {
            Player1Move._walkLeftP1 = true;
        }
    }
}
