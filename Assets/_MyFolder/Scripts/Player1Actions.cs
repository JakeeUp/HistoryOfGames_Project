using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1Actions : MonoBehaviour
{
    private Animator Anim;
    private AnimatorStateInfo Player1Layer0;
    private bool _heavyMoving = false;
    public float punchSlideAmount = .2f;
    private AudioSource MyPlayer;
    public AudioClip punchSound;
    public AudioClip kickSound;
    
    [Header("Jump Attributes")]
    public float jumpSpeed = 0.05f;

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
            if (Player1Move._facingRight == true)
            {
                Player1.transform.Translate(punchSlideAmount * Time.deltaTime,0,0);
            }
            if (Player1Move._facingLeft == true)
            {
                Player1.transform.Translate(-punchSlideAmount * Time.deltaTime,0,0);
            }
        }
        AnimatorListener();

        if (Player1Layer0.IsTag("Motion"))
        {
            if(Input.GetButtonDown("Fire1"))
            {
                Anim.SetTrigger(LightPunch);
            }
            if(Input.GetButtonDown("Fire2"))
            {
                Anim.SetTrigger(HeavyPunch);
            }
            if(Input.GetButtonDown("Fire3"))
            {
                Anim.SetTrigger(LightKick);
            }
            if(Input.GetButtonDown("Jump"))
            {
                Anim.SetTrigger(HeavyKick);
            }

            if (Input.GetButtonDown("Block"))
            {
                Anim.SetTrigger(BlockOn);
            }
        }

        if (Player1Layer0.IsTag("Block"))
        {
            if (Input.GetButtonUp("Block"))
            {
                Anim.SetTrigger(BlockOff);
            }
        }

        if (Player1Layer0.IsTag("Crouching"))
        {
            if(Input.GetButtonDown("Fire3"))
            {
                Anim.SetTrigger(LightKick);
            }
        }
        
        //Air moves
        if (Player1Layer0.IsTag("Jumping"))
        {
            if(Input.GetButtonDown("Jump"))
            {
                Anim.SetTrigger(HeavyKick);
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
}
