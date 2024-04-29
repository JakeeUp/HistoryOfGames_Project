using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Serialization;

public class Player1Move : MonoBehaviour
{
    private Animator Anim;
    
    [Header("Player Attributes")]
    public float walkSpeed = 0.001f;

    private bool _isJumping = false;

    private AnimatorStateInfo Player1Layer0;

    [Header("Bools")]
    [SerializeField] private bool _canWalkLeft = true;
    [SerializeField] private bool _canWalkRight = true;
    [SerializeField] private bool _facingLeft = true;
    [SerializeField] private bool _facingRight = false;

    [Header("GameObjects")] 
    public GameObject player1;
    public GameObject opponent;


    public Vector3 oppPosition;
    
    


    private static readonly int Forward = Animator.StringToHash("Forward");
    private static readonly int Backward = Animator.StringToHash("Backward");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Crouch = Animator.StringToHash("Crouch");

    // Start is called before the first frame update
    void Start()
    {
       
        Anim = GetComponentInChildren<Animator>();
        StartCoroutine(FaceRight());
    }

    // Update is called once per frame
    void Update()
    {
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
            Anim.SetBool(Crouch,true);
        }

        if (Input.GetAxis("Vertical") == 0)
        {
            Anim.SetBool(Crouch,false);
        }
    }

    private void PlayerMovement()
    {
        //Moving left and right

        if (Player1Layer0.IsTag("Motion"))
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                if (_canWalkRight == true)
                {
                    Anim.SetBool(Forward,true);
                    transform.Translate(walkSpeed,0,0);
                }
            }

            if (Input.GetAxis("Horizontal") < 0)
            {
                if (_canWalkLeft == true)
                {
                    Anim.SetBool(Backward, true);
                    transform.Translate(-walkSpeed,0,0);
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
}
