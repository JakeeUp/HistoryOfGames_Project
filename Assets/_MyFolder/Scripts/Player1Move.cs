﻿using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Player1Move : MonoBehaviour
{
    private Animator Anim;
    
    [Header("Player Attributes")]
    public float walkSpeed = 0.001f;

    private bool _isJumping = false;

    private AnimatorStateInfo Player1Layer0;
    public Rigidbody RB;
    public Collider BoxCollider;
    public Collider CapsuleCollider;
    public float JumpSpeed = 0.05f;
    private float MoveSpeed;
    private float Timer = 2.0f;
    private float CrouchTime = 0.0f;
    

    [Header("Bools")]
    [SerializeField] private bool _canWalkLeft = true;
    [SerializeField] private bool _canWalkRight = true;
    public static bool _facingLeft = false;
    public static bool _facingRight = true;
    [SerializeField]public static bool _walkLeftP1 = true;
    [SerializeField]public static bool _walkRightP1 = true;
    

    [Header("GameObjects")] 
    public GameObject player1;
    public GameObject opponent;
    public GameObject Restrict;

    public Vector3 oppPosition;
    public GameObject WinCondition;

    [Header("AudioClips")] 
    public AudioClip LightPunch;
    public AudioClip HeavyPunch;
    public AudioClip LightKick;
    public AudioClip HeavyKick;
    private AudioSource MyPlayer;


    private static readonly int Forward = Animator.StringToHash("Forward");
    private static readonly int Backward = Animator.StringToHash("Backward");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Crouch = Animator.StringToHash("Crouch");
    private static readonly int HeadReact = Animator.StringToHash("HeadReact");
    private static readonly int BigReact = Animator.StringToHash("BigReact");
    private static readonly int Knockout = Animator.StringToHash("KnockOut");

    // Start is called before the first frame update
    void Start()
    {
        _facingLeft = false;
        _facingRight = true;
        _walkLeftP1 = true;
        _walkRightP1 = true;
        
        
        
        WinCondition = GameObject.Find("WinCondition");
        WinCondition.gameObject.SetActive(false);
        opponent = GameObject.Find("Player2");
        Anim = GetComponentInChildren<Animator>();
        StartCoroutine(FaceRight());
        MyPlayer = GetComponentInChildren<AudioSource>();
        MoveSpeed = walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
        if (SaveScript.TimeOut == true)
        {
            Anim.SetBool(Forward,false);
            Anim.SetBool(Backward,false);
            
            //Get Opp Position
            oppPosition = opponent.transform.position;
        
            //facing left or right of opp
            if (oppPosition.x > player1.transform.position.x)
            {
                StartCoroutine(FaceLeft());
            }
            if (oppPosition.x < player1.transform.position.x)
            {
                StartCoroutine(FaceRight());
            }
        }
        if (SaveScript.TimeOut == false)
        {
            if (Player1Actions.FlyingJumpP1 == true)
            {
                walkSpeed = JumpSpeed;
            }
            else
            {
                walkSpeed = MoveSpeed;
            }
        
            //check if dead
            if (SaveScript.Player1Health <= 0)
            {
                Anim.SetTrigger("KnockedOut");
                player1.GetComponent<Player1Actions>().enabled = false;
                StartCoroutine(KnockedOut());
                //this.GetComponent<Player1Move>().enabled = false;
                WinCondition.gameObject.SetActive(true);
                WinCondition.gameObject.GetComponent<LoseWIn>().enabled = true;
            }

            if (SaveScript.Player2Health <= 0)
            {
                Anim.SetTrigger("Victory");
                player1.GetComponent<Player1Actions>().enabled = false;
                this.GetComponent<Player1Move>().enabled = false;
                WinCondition.gameObject.SetActive(true);
                WinCondition.gameObject.GetComponent<LoseWIn>().enabled = true;
            }
        
        
        
            AnimatorListener();
            CantExitScreenBounds();
            PlayerMovement();
            PlayerJumpAndCrouch();
        
            //Get Opp Position
            oppPosition = opponent.transform.position;
        
            //facing left or right of opp
            if (oppPosition.x > player1.transform.position.x)
            {
                StartCoroutine(FaceLeft());
            }
            if (oppPosition.x < player1.transform.position.x)
            {
                StartCoroutine(FaceRight());
            }
        
            //Reset the restrict
            if (Restrict.gameObject.activeInHierarchy == false)
            {
                _walkLeftP1 = true;
                _walkRightP1 = true;
            }

            if (Player1Layer0.IsTag("Block"))
            {
                RB.isKinematic = true;
                BoxCollider.enabled = false;
                CapsuleCollider.enabled = false;
            }
            else if(Player1Layer0.IsTag("Motion"))
            {
                BoxCollider.enabled = true;
                CapsuleCollider.enabled = true;
                RB.isKinematic = false;
            }

            if (Player1Layer0.IsTag("Crouching"))
            {
                BoxCollider.enabled = false;
            }
            if (Player1Layer0.IsTag("Sweep"))
            {
                BoxCollider.enabled = false;
            }
        }
    }

    private void PlayerJumpAndCrouch()
    {
        //Jumping and crouch
        if (Input.GetAxis("Vertical") > 0)
        {
            if (_isJumping == false)
            {
                _isJumping = true;
                Anim.SetTrigger(Jump);
                StartCoroutine(JumpPause());
            }
            
        }

        if (Input.GetAxis("Vertical") < 0)
        {
            if (CrouchTime < Timer)
            {
                CrouchTime += 1.0f * Time.deltaTime;
                Anim.SetBool(Crouch,true);
            }
            else if (CrouchTime > Timer)
            {
                Anim.SetBool(Crouch,false);
                StartCoroutine(ResetCrouchTime());
            }
        }

        if (Input.GetAxis("Vertical") == 0)
        {
            Anim.SetBool(Crouch,false);
            CrouchTime = 0.0f;
        }
    }

    private void PlayerMovement()
    {
        //Moving left and right

        if (Player1Layer0.IsTag("Motion"))
        {
            Time.timeScale = 1.0f;
            if (Input.GetAxis("Horizontal") > 0)
            {
                if (_canWalkRight == true)
                {
                    if (_walkRightP1 == true)
                    {
                        Anim.SetBool(Forward,true);
                        transform.Translate(walkSpeed,0,0);
                    }
                }
            }

            if (Input.GetAxis("Horizontal") < 0)
            {
                if (_canWalkLeft == true)
                {
                    if (_walkLeftP1 == true)
                    {
                        Anim.SetBool(Backward, true);
                        transform.Translate(-walkSpeed,0,0);
                    }
                }
            }
        }


        if (Input.GetAxis("Horizontal") == 0)
        {
            Anim.SetBool(Forward , false);
            Anim.SetBool(Backward,false);
        }
    }

    private void CantExitScreenBounds()
    {
        //cannot exit screen
        Vector3 screenBounds = Camera.main.WorldToScreenPoint(this.transform.position);

        if (screenBounds.x > Screen.width - 200)
        {
            _canWalkRight = false;
        }
        if (screenBounds.x < 200)
        {
            _canWalkLeft = false;
        }
        else if (screenBounds.x > 200 && screenBounds.x < Screen.width - 200)
        {
            _canWalkRight = true;
            _canWalkLeft = true;
        }
    }

    private void AnimatorListener()
    {
        //Listen to the Animaior
        Player1Layer0 = Anim.GetCurrentAnimatorStateInfo(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("FistLight"))
        {
            Anim.SetTrigger("BigReact");
            MyPlayer.clip = LightPunch;
            MyPlayer.Play();
        }
        if (other.gameObject.CompareTag("FistHeavy"))
        {
            Anim.SetTrigger("HeadReact");
            MyPlayer.clip = HeavyPunch;
            MyPlayer.Play();
        }
        if (other.gameObject.CompareTag("KickHeavy"))
        {
            Anim.SetTrigger("BigReact");
            MyPlayer.clip = HeavyKick;
            MyPlayer.Play();
        }

        if (other.gameObject.CompareTag("KickLight"))
        {
            Anim.SetTrigger("HeadReact");
            MyPlayer.clip = LightKick;
            MyPlayer.Play();
        }
    }


    IEnumerator JumpPause()
    {
        yield return new WaitForSeconds(1.0f);
        _isJumping = false;
    }

    IEnumerator FaceLeft()
    {
        if (_facingLeft == true)
        {
            _facingLeft = false;
            _facingRight = true;
            yield return new WaitForSeconds(0.15f);
            player1.transform.Rotate(0,180,0);
            Anim.SetLayerWeight(1, 0);
        }
    }
    IEnumerator FaceRight()
    {
        if (_facingRight == true)
        {
            _facingRight = false;
            _facingLeft = true;
            yield return new WaitForSeconds(0.15f);
            player1.transform.Rotate(0,-180,0);
            Anim.SetLayerWeight(1, 1);
        }
    }

    IEnumerator KnockedOut()
    {
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<Player1Move>().enabled = false;
    }

    IEnumerator ResetCrouchTime()
    {
        yield return new WaitForSeconds(2.0f);
        CrouchTime = 0.0f;
    }
}
