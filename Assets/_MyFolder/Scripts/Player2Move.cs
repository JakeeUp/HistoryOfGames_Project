using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Serialization;

public class Player2Move : MonoBehaviour
{
    private Animator Anim;
    
    [Header("Player Attributes")]
    public float walkSpeed = 0.001f;

    private bool _isJumping = false;

    private AnimatorStateInfo Player1Layer0;

    [Header("Bools")]
    [SerializeField] private bool _canWalkLeft = true;
    [SerializeField] private bool _canWalkRight = true;
    public static bool _facingLeftP2 = false;
    public static bool _facingRightP2 = true;
    public static bool _walkLeft = true;
    public static bool _walkRight = true;
    

    [Header("GameObjects")] 
    public GameObject player1;
    public GameObject opponent;
    public GameObject Restrict;


    public Vector3 oppPosition;

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
    //private static readonly int KnockedOut = Animator.StringToHash("KnockedOut");
    private static readonly int Knockout = Animator.StringToHash("KnockOut");

    // Start is called before the first frame update
    void Start()
    {
       
        Anim = GetComponentInChildren<Animator>();
        StartCoroutine(FaceRight());
        MyPlayer = GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //check if dead
        if (SaveScript.Player2Health <= 0)
        {
            Anim.SetTrigger(Knockout);
            player1.GetComponent<Player2Actions>().enabled = false;
            //this.GetComponent<Player2Move>().enabled = false;
            StartCoroutine(KnockedOut());
        }
        //Debug.Log(_walkLeft);
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
            _walkLeft = true;
            _walkRight = true;
        }
    }

    private IEnumerator KnockedOut()
    {
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<Player2Move>().enabled = false;
    }

    private void PlayerJumpAndCrouch()
    {
        //Jumping and crouch
        if (Input.GetAxis("VerticalP2") > 0)
        {
            if (_isJumping == false)
            {
                _isJumping = true;
                Anim.SetTrigger(Jump);
                StartCoroutine(JumpPause());
            }
            
        }

        if (Input.GetAxis("VerticalP2") < 0)
        {
            Anim.SetBool(Crouch,true);
        }

        if (Input.GetAxis("VerticalP2") == 0)
        {
            Anim.SetBool(Crouch,false);
        }
    }

    private void PlayerMovement()
    {
        //Moving left and right

        if (Player1Layer0.IsTag("Motion"))
        {
            if (Input.GetAxis("HorizontalP2") > 0)
            {
                if (_canWalkRight == true)
                {
                    if (_walkRight == true)
                    {
                        Anim.SetBool(Forward,true);
                        transform.Translate(walkSpeed,0,0);
                    }
                }
            }

            if (Input.GetAxis("HorizontalP2") < 0)
            {
                if (_canWalkLeft == true)
                {
                    if (_walkLeft == true)
                    {
                        Anim.SetBool(Backward, true);
                        transform.Translate(-walkSpeed,0,0);
                    }
                }
            }
        }


        if (Input.GetAxis("HorizontalP2") == 0)
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
        if (_facingLeftP2 == true)
        {
            _facingLeftP2 = false;
            _facingRightP2 = true;
            yield return new WaitForSeconds(0.15f);
            player1.transform.Rotate(0,180,0);
            Anim.SetLayerWeight(1, 0);
        }
    }
    IEnumerator FaceRight()
    {
        if (_facingRightP2 == true)
        {
            _facingRightP2 = false;
            _facingLeftP2 = true;
            yield return new WaitForSeconds(0.15f);
            player1.transform.Rotate(0,-180,0);
            Anim.SetLayerWeight(1, 1);
        }
    }
}
