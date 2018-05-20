using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IO : MonoBehaviour {

    private float speed = 5f;
    private bool facingRight = true;
    private Animator anim;
    


    [SerializeField]
    private float jumpForce = 300f;

    public Rigidbody2D rb { get; set; }

    // Use this for initialization
    void Start () {
        GetComponent<Rigidbody2D>().freezeRotation = true;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
		
	}
	
	// Update is called once per frame
	void Update () {
        HandleInput();
	}

    void FixedUpdate ()
    {
        if (rb.velocity.y > 0) anim.SetInteger("Y", 1);
        else if (rb.velocity.y == 0) anim.SetInteger("Y", 0);
        else anim.SetInteger("Y", -1);

        

        
        if (rb.velocity.x == 0) anim.SetBool("move", false);
        else anim.SetBool("move", true);

        float horizontal = Input.GetAxis("Horizontal");
        //rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);



        if (horizontal > 0 & !facingRight)
        {
            Flip(horizontal);
        }
        else if(horizontal < 0 && facingRight)
        {
            Flip(horizontal);
        }
    }

    private void HandleInput()
    {
        if(rb.velocity.y == 0 && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector2(0, jumpForce));
        }

    }

    private void Flip(float horizontal)
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

}
