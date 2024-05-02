using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2ActionsAI : MonoBehaviour
{
    private Animator Anim;
    private AnimatorStateInfo Player1Layer0;
    private bool _heavyMoving = false;
    public float punchSlideAmount = .2f;
    public float heavyReactAmount = 10f;
    public float DazedTime = 3.0f;

    private bool _heavyReact = false;
    private AudioSource MyPlayer;
    public static bool HitsAI = false;
    public static bool Dazed = false;
    public AudioClip punchSound;
    public AudioClip kickSound;
    public static bool FlyingJumpAI = false;

    private int AttackNumber = 1;
    private bool Attacking = true;
    public float AttackRate = 1.0f;
        
        
        
    [Header("Jump Attributes")]
    public float jumpSpeed = 0.05f;

    public float flipSpeed = 0.8f;

    public GameObject Player1;
    private static readonly int LightPunch = Animator.StringToHash("LightPunch");
    private static readonly int HeavyPunch = Animator.StringToHash("HeavyPunch");
    private static readonly int LightKick = Animator.StringToHash("LightKick");
    private static readonly int HeavyKick = Animator.StringToHash("HeavyKick");
    private static readonly int BlockOn = Animator.StringToHash("BlockOn");
    private static readonly int BlockOff = Animator.StringToHash("BlockOff");

    // Start is called before the first frame update
    void Start()
    {
        Anim = GetComponent<Animator>();
        MyPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //heavy punch slide
        if (_heavyMoving == true)
        {
            if (Player2MoveAI._facingRightAI == true)
            {
                Player1.transform.Translate(punchSlideAmount * Time.deltaTime,0,0);
            }
            if (Player2MoveAI._facingLeftAI == true)
            {
                Player1.transform.Translate(-punchSlideAmount * Time.deltaTime,0,0);
            }
        }
        
        //heavy react slide
        if (_heavyReact == true)
        {
            if (Player2MoveAI._facingRightAI == true)
            {
                Player1.transform.Translate(-heavyReactAmount * Time.deltaTime,0,0);
            }
            if (Player2MoveAI._facingLeftAI == true)
            {
                Player1.transform.Translate(heavyReactAmount * Time.deltaTime,0,0);
            }
        }
        
        
        AnimatorListener();

        if (Player1Layer0.IsTag("Motion"))
        {
            if (Attacking == true)
            {
                Attacking = false;
                if(AttackNumber == 1)
                {
                    Anim.SetTrigger(LightPunch);
                    HitsAI = false;
                }
                if(AttackNumber == 2)
                {
                    Anim.SetTrigger(HeavyPunch);
                    HitsAI = false;
                }
                if(AttackNumber == 3)
                {
                    Anim.SetTrigger(LightKick);
                    HitsAI = false;
                }
                if(AttackNumber == 4)
                {
                    Anim.SetTrigger(HeavyKick);
                    HitsAI = false;
                }
            }
           

            if (Input.GetButtonDown("BlockP2"))
            {
                Anim.SetTrigger(BlockOn);
            }
        }

        if (Player1Layer0.IsTag("Block"))
        {
            if (Input.GetButtonUp("BlockP2"))
            {
                Anim.SetTrigger(BlockOff);
            }
        }

        if (Player1Layer0.IsTag("Crouching"))
        {
           
                Anim.SetTrigger(LightKick);
                HitsAI = false;
                Anim.SetBool("Crouch", false);
            
        }
        
        //Air moves
        if (Player1Layer0.IsTag("Jumping"))
        {
            if(Input.GetButtonDown("Fire4P2"))
            {
                Anim.SetTrigger(HeavyKick);
                HitsAI = false;
            }
        }
    }

    private void AnimatorListener()
    {
        //Listen to the Animaior
        Player1Layer0 = Anim.GetCurrentAnimatorStateInfo(0);
    }
    public void JumpUp()
    {
        Player1.transform.Translate(0,jumpSpeed,0);
    }
    
    public void HeavyMove()
    {
        StartCoroutine(PunchSlide());
    }

    public void RandomAttack()
    {
            AttackNumber = Random.Range(1, 5);
            StartCoroutine(SetAttacking());

    }
    public void HeavyReaction()
    {
        StartCoroutine(HeavySlide());
        AttackNumber = 3;
    }
    public void FlipUp()
    {
        Player1.transform.Translate(0,flipSpeed,0);
        FlyingJumpAI = true;
    }
    public void FlipBack()
    {
        Player1.transform.Translate(0,flipSpeed,0);
        FlyingJumpAI = true;
    }
    public void IdleSpeed()
    {
        FlyingJumpAI = false;
    }
    public void PunchSoundEffect()
    {
        MyPlayer.clip = punchSound;
        MyPlayer.Play();
    }

    public void KickSoundEffect()
    {
        MyPlayer.clip = kickSound;
        MyPlayer.Play();
    }
    IEnumerator PunchSlide()
    {
        _heavyMoving = true;
        yield return new WaitForSeconds(0.05f);
        _heavyMoving = false;
    }
    
    IEnumerator HeavySlide()
    {
        _heavyReact = true;
        Dazed = true;
        yield return new WaitForSeconds(0.05f);
        _heavyReact = false;
        yield return new WaitForSeconds(DazedTime);
        Dazed = false;
    }

    IEnumerator SetAttacking()
    {
        yield return new WaitForSeconds(AttackRate);
        Attacking = true;
    }
}
