using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;

public class player_movement : MonoBehaviour
{
    private Rigidbody2D body;
    [SerializeField] private float speed;
    [SerializeField] private float jumppower;
    private Animator anim;
    //private bool Grounded;
    Camera_cube Camera_Cube;
    public OpenCvSharp.Rect MyFace;
    public RawImage Image;

    [SerializeField] private LayerMask groundlayer;
    [SerializeField] private LayerMask walllayer;

    private BoxCollider2D boxcolliderplayer;

    private float walljumpcooldown;

    private float hztalInput;
    private float HorizontalInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxcolliderplayer = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        //hztalInput = Input.GetAxis("Horizontal");
        //// con camara
        MyFace.X = Image.GetComponent<Camera_cube>().MyFace.X;
        MyFace.Y = Image.GetComponent<Camera_cube>().MyFace.Y;
        //float hztalInput = GetComponent<Camera_cube>().MyFace.X;
        if (MyFace.X > 250)
        {
            hztalInput = -1.1f;
        }
        else if (MyFace.X < 250)
        {
            hztalInput = 1.1f;
        }
        else
        {
            hztalInput = 0f;
        }

        //flip towards the direction ---
        if (hztalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (hztalInput<0.01f)
        {
            transform.localScale = new Vector3(-1,1,1);
        }
        //---

        // jump ---
        
        //// with Camera
        //if (MyFace.Y < 100 && Grounded)
        //{
        //    Jump();
        //}
        // ---

        anim.SetBool("Run", hztalInput != 0);
        anim.SetBool("Grounded",IsGrounded());

        //print(OnWall()); //detect wall

        ////Walljump Logic
        if (walljumpcooldown >0.2f)
        {


            //// with Camera
            if (MyFace.Y < 100 && IsGrounded())
            {
                Jump();
            }
            //// ---
            body.velocity = new Vector2(hztalInput * speed, body.velocity.y);
            if (OnWall() && IsGrounded())
            {
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }
            else
            {
                body.gravityScale = 3;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }
        }
        else
        {
            walljumpcooldown += Time.deltaTime;
        }
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumppower); //with camera speed +3
            anim.SetTrigger("Jump");
        }
        else if (OnWall() && !IsGrounded())
        {
            if (HorizontalInput == 0)
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 20, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 10);
            }
            walljumpcooldown = 0;
            body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x)*3, 10); //return 1 or -1
        }
        //IsGrounded() = false;
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "Ground")
    //    {
    //        IsGrounded() = true;
    //    }
    //}

    private bool IsGrounded()
    {
        // Raycast virtual line intersects with an object with a collider return true
        //boxcast is the same but with a box
        RaycastHit2D raycasthit = Physics2D.BoxCast(boxcolliderplayer.bounds.center, boxcolliderplayer.bounds.size, 0, Vector2.down, 0.1f, groundlayer);
        return raycasthit.collider!=null; //
    }

    private bool OnWall()
    {
        RaycastHit2D raycasthit = Physics2D.BoxCast(boxcolliderplayer.bounds.center, boxcolliderplayer.bounds.size, 0, new Vector2(transform.localScale.x,0), 0.1f, walllayer);
        return raycasthit.collider != null; //
    }
}
