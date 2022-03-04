using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{

    //Declarations for Hero
    public CharacterController controller;
    public Transform cam;
    Animator my_Animator;
    public GameObject Axe;
    private AudioSource Audio;
    public AudioClip AxeSwing;
    private Vector3 playerVelocity;
    public LayerMask EnemyLayer;
    public Transform AttackPoint;
    //End Declorations for Hero

    //Hero movements Declarations
    public bool groundedplayer;
    private float speed = 3f;
    public float turnSmoothTime = 0.1f;
    private float JumpHeight = 2f;
    public float Gravity = -9f;
    private int AttackCounter = 0;
    float turnSmoothVelocity;
    //End Here movements Declarations

    //Hero hit box
    public float AttackRange = 0.7f;
    public int AttackDamage = 40;
    public float AttackRate = 1.3f;
    float NextAttackTime = 0f;
    //end hero hitbox declarations





    void Start()
    {
        my_Animator = GetComponent<Animator>();
        my_Animator.SetBool("isIdle", true);
        
    }
    private void Awake()
    {
        Audio = Axe.GetComponent<AudioSource>();
    
    }

    // Update is called once per frame
    void Update()
    {

        

        groundedplayer = controller.isGrounded;

        if(groundedplayer == true)
        {
            playerVelocity.y = -0.1f;
            my_Animator.SetBool("isJumping", false);
            my_Animator.SetBool("isIdle", true);
        }
     

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        //Don't forget to add an isMoving bool inside your Animator
       
       
         if (direction.magnitude >= 0.1f)
        {
           float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
           float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
           transform.rotation = Quaternion.Euler(0f, angle, 0f);

           Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
           controller.Move(moveDir.normalized * speed * Time.deltaTime);

            if (Input.GetKey(KeyCode.LeftShift) && direction.magnitude >= 0.1f)
            {
                Run();
                speed = 5.0f;
            }
        }
        
        if (!Input.GetKey(KeyCode.LeftShift) && direction.magnitude >= 0.1f)
        {
            Walk();
            speed = 2.5f;
        }
        else if(!Input.GetKey(KeyCode.LeftShift) && direction.magnitude == 0.0f)
        {
            Idle();
            speed = 2.5f;
        }

        if(Input.GetButtonDown("Jump") && groundedplayer)
        {
            playerVelocity.y += Mathf.Sqrt(JumpHeight * -2f * Gravity);
            my_Animator.SetBool("isIdle", false);
            my_Animator.SetBool("isJumping", true);
        }
        if (Time.time >= NextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                AttackCounter += 1;
                Attacking();
                NextAttackTime = Time.time + 1f / AttackRate;
            }
        }
           

            playerVelocity.y += Gravity * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        
    }







    void Attacking()
    {
        
        if(AttackCounter == 1){
            my_Animator.SetTrigger("Attack");
            Audio.PlayDelayed(0.3f);
        }
        else if(AttackCounter == 2)
        {
            my_Animator.SetTrigger("Attack2");
            Audio.PlayDelayed(0.3f);
        }
        else if(AttackCounter == 3)
        {
            my_Animator.SetTrigger("Attack3");
            Audio.PlayDelayed(0.35f);
        }
        else if(AttackCounter == 4)
        {
            my_Animator.SetTrigger("Attack4");
            AttackCounter = 0;
            Audio.PlayDelayed(0.3f);
        }


        StartCoroutine(ExecuteAfterTime(0.5f));
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Collider[] HitEnemies = Physics.OverlapSphere(AttackPoint.position, AttackRange, EnemyLayer);
        // Code to execute after the delay
        foreach (Collider Enemy in HitEnemies)
        {
            Enemy.GetComponent<Enemy>().TakeDamage(AttackDamage);
            Enemy.GetComponent<AudioSource>().Play();
        }
    }
    void Idle()
    {
        my_Animator.SetBool("isIdle", true);
        my_Animator.SetBool("isWalking", false);
        
    }

    void Walk()
    {
        my_Animator.SetBool("isWalking", true);
        my_Animator.SetBool("isIdle", false);
        my_Animator.SetBool("isRunning", false);
    }

    void Run()
    {
        my_Animator.SetBool("isRunning", true);
        my_Animator.SetBool("isWalking", false);
        my_Animator.SetBool("isIdle", false);
    }
    public void PlayAxceSwing()
    {
        Audio.clip = AxeSwing;
        Audio.Play();
    }
    private void OnDrawGizmosSelected()
    {
        if(AttackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(AttackPoint.position, AttackRange);
    }
}
