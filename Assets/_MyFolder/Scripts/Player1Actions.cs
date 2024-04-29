﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Actions : MonoBehaviour
{
    [Header("Jump Attributes")]
    public float jumpSpeed = 0.05f;

    public GameObject Player1;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JumpUp()
    {
        Player1.transform.Translate(0,jumpSpeed,0);
    }
    public void FlipUp()
    {
        Player1.transform.Translate(0,jumpSpeed,0);
        Player1.transform.Translate(0.1f,0,0);
    }
    public void FlipBack()
    {
        Player1.transform.Translate(0,jumpSpeed,0);
        Player1.transform.Translate(-0.1f,0,0);
    }
}
