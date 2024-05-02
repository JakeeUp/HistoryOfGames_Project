using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Player2MoveAI : MonoBehaviour
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
    private float OppDistance;
    public float AttackDistance = 1.5f;
    private bool MoveAI = true;
    public static bool AttackState = false;


    [Header("Bools")]
    [SerializeField] private bool _canWalkLeft = true;
    [SerializeField] private bool _canWalkRight = true;
    public static bool _facingLeftAI = false;
    public static bool _facingRightAI = true;
    public static bool _walkLeftAI = true;
    public static bool _walkRightAI = true;
    private int Defend = 0;
    private bool isBlocking = false;
    

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
        _facingLeftAI = false;
        _facingRightAI = true;
        _walkLeftAI = true;
        _walkRightAI = true;
        opponent = GameObject.Find("Player1");
        Anim = GetComponentInChildren<Animator>();
        StartCoroutine(FaceRight());
        MyPlayer = GetComponentInChildren<AudioSource>();
        MoveSpeed = walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (SaveScript.TimeOut == true)
        {
            Anim.SetBool(Forward,false);
            Anim.SetBool(Backward,false);
        }

        if (SaveScript.TimeOut == false)
        {
            OppDistance = Vector3.Distance(opponent.transform.position, player1.transform.position);


            if (Player2Actions.FlyingJumpP2 == true)
            {
                walkSpeed = JumpSpeed;
            }
            else
            {
                walkSpeed = MoveSpeed;
            }

            //check if dead
            if (SaveScript.Player2Health <= 0)
            {
                Anim.SetTrigger("KnockedOut");
                player1.GetComponent<Player2ActionsAI>().enabled = false;
                //this.GetComponent<Player2Move>().enabled = false;
                StartCoroutine(KnockedOut());
            }

            if (SaveScript.Player1Health <= 0)
            {
                Anim.SetTrigger("Victory");
                player1.GetComponent<Player2ActionsAI>().enabled = false;
                this.GetComponent<Player2MoveAI>().enabled = false;
            }


            //Debug.Log(_walkLeft);
            AnimatorListener();
            CantExitScreenBounds();
            //PlayerMovement();
            PlayerJumpAndCrouch();

            //Get Opp Position
            oppPosition = opponent.transform.position;

            if (Player2ActionsAI.Dazed == false)
            {


                //facing left or right of opp
                if (oppPosition.x > player1.transform.position.x)
                {

                    StartCoroutine(FaceLeft());

                    if (Player1Layer0.IsTag("Motion"))
                    {
                        Time.timeScale = 1.0f;
                        Anim.SetBool("CanAttack", false);
                        if (OppDistance > AttackDistance)
                        {
                            if (MoveAI == true)
                            {
                                if (_canWalkRight == true)
                                {
                                    if (_walkRightAI == true)
                                    {
                                        Anim.SetBool(Forward, true);
                                        Anim.SetBool(Backward, false);
                                        AttackState = false;
                                        transform.Translate(walkSpeed, 0, 0);
                                    }
                                }
                            }
                        }
                    }

                    if (OppDistance < AttackDistance)
                    {
                        if (_canWalkRight == true)
                        {
                            if (MoveAI == true)
                            {
                                MoveAI = false;
                                Anim.SetBool(Forward, false);
                                Anim.SetBool(Backward, false);
                                Anim.SetBool("CanAttack", true);
                                StartCoroutine(ForwardFalse());
                            }
                        }
                    }
                }
            }

            if (oppPosition.x < player1.transform.position.x)
            {
                StartCoroutine(FaceRight());

                if (Player1Layer0.IsTag("Motion"))
                {
                    Time.timeScale = 1.0f;
                    Anim.SetBool("CanAttack", false);
                    if (OppDistance > AttackDistance)
                    {
                        if (MoveAI == true)
                        {
                            if (_canWalkLeft == true)
                            {
                                if (_walkLeftAI == true)
                                {
                                    Anim.SetBool(Forward, false);
                                    Anim.SetBool(Backward, true);
                                    AttackState = false;
                                    transform.Translate(-walkSpeed, 0, 0);
                                }
                            }
                        }
                    }
                }

                if (OppDistance < AttackDistance)
                {
                    if (_canWalkLeft == true)
                    {
                        if (MoveAI == true)
                        {
                            MoveAI = false;
                            Anim.SetBool(Forward, false);
                            Anim.SetBool(Backward, false);
                            Anim.SetBool("CanAttack", true);
                            StartCoroutine(ForwardFalse());
                        }
                    }
                }
            }

            //Reset the restrict
            if (Restrict.gameObject.activeInHierarchy == false)
            {
                _walkLeftAI = true;
                _walkRightAI = true;
            }

            if (Player1Layer0.IsTag("Block"))
            {
                RB.isKinematic = true;
                BoxCollider.enabled = false;
                CapsuleCollider.enabled = false;
            }
            else
            {
                BoxCollider.enabled = true;
                CapsuleCollider.enabled = true;
                RB.isKinematic = false;
            }
        }
    }

    private IEnumerator KnockedOut()
    {
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<Player2MoveAI>().enabled = false;
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

        if (Defend == 3)
        {
            Anim.SetBool(Crouch,true);
            Defend = 0;
        }

        if (Defend == 4)
        {
            Anim.SetTrigger("Jump");
            Defend = 0;
        }
        if (Defend == 2)
        {
            if (isBlocking == false)
            {
                isBlocking = true;
                Anim.SetTrigger("BlockOn");
                StartCoroutine(EndBlock());
            }
        }
        // if (Input.GetAxis("VerticalP2") == 0)
        // {
        //     Anim.SetBool(Crouch,false);
        // }
    }

    private void PlayerMovement()
    {
        //Moving left and right

        // if (Player1Layer0.IsTag("Motion"))
        // {
        //     if (Input.GetAxis("HorizontalP2") > 0)
        //     {
        //         if (_canWalkRight == true)
        //         {
        //             if (_walkRightAI == true)
        //             {
        //                 Anim.SetBool(Forward,true);
        //                 transform.Translate(walkSpeed,0,0);
        //             }
        //         }
        //     }
        //
        //     if (Input.GetAxis("HorizontalP2") < 0)
        //     {
        //         if (_canWalkLeft == true)
        //         {
        //             if (_walkLeftAI == true)
        //             {
        //                 Anim.SetBool(Backward, true);
        //                 transform.Translate(-walkSpeed,0,0);
        //             }
        //         }
        //     }
        // }
        //
        //
        // if (Input.GetAxis("HorizontalP2") == 0)
        // {
        //     Anim.SetBool(Forward , false);
        //     Anim.SetBool(Backward,false);
        // }
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
            Defend = Random.Range(0, 5);
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
             Defend = Random.Range(0, 5);
        }
    }


    IEnumerator JumpPause()
    {
        yield return new WaitForSeconds(1.0f);
        _isJumping = false;
    }

    IEnumerator FaceLeft()
    {
        if (_facingLeftAI == true)
        {
            _facingLeftAI = false;
            _facingRightAI = true;
            yield return new WaitForSeconds(0.15f);
            player1.transform.Rotate(0,180,0);
            Anim.SetLayerWeight(1, 0);
        }
    }
    IEnumerator FaceRight()
    {
        if (_facingRightAI == true)
        {
            _facingRightAI = false;
            _facingLeftAI = true;
            yield return new WaitForSeconds(0.15f);
            player1.transform.Rotate(0,-180,0);
            Anim.SetLayerWeight(1, 1);
        }
    }

    IEnumerator ForwardFalse()
    {
        yield return new WaitForSeconds(0.6f);
        MoveAI = true;
    }

    IEnumerator EndBlock()
    {
        yield return new WaitForSeconds(2.0f);
        isBlocking = false;
        Anim.SetTrigger("BlockOff");
        Defend = 0;
    }
}
