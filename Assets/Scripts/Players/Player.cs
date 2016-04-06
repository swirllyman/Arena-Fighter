using UnityEngine;
using System.Collections;

public enum PlayerState { run, idle, attack1, attack2, attack3, dead, takeDamage }

public abstract class Player : Photon.MonoBehaviour {

    private int health = 1;
    private int maxHealth = 1;
    private float speed = 1;
    private int defense = 1;
    private int damage = 1;

    public float jumpAmount = 1000;
    private bool justJumped = false;

    protected Transform attackingObject1;
	protected Transform attackingObject2;
	protected Transform attackingObject3;

    GameObject createdObject1;
    GameObject createdObject2;
    GameObject createdObject3;

    public Animator anim;
    CapsuleCollider col;
    Rigidbody body;
    protected Transform mesh;

    Quaternion destRot;

    bool left, right, forward, backward, moving, grounded;
    public Transform cam;

    PlayerState currentState = PlayerState.idle;

	//Nextworking variables//
	private PhotonView PV;
	Vector3 realPosition = Vector3.zero;
	Quaternion realRotation = Quaternion.identity;

	Vector3 spawn_position = new Vector3(-20, 20, 20);
	float death_timer = 5.0f;
	float active_timer = 0.0f;

    protected virtual void Start() 
	{
		PV = GetComponent<PhotonView> ();
        col = GetComponent<CapsuleCollider>();
        body = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
		anim.SetBool ("Moving", false);
		mesh = GetComponentInChildren<Renderer> ().transform;
        StopMovement();
    }


    void FixedUpdate()
    {
		if (PV.isMine) {
            //if (currentState != PlayerState.dead && (Input.GetAxis ("Horizontal") != 0 || Input.GetAxis ("Vertical") != 0)) {
            //	transform.Translate (Input.GetAxis ("Horizontal") * speed / 10, 0, Input.GetAxis ("Vertical") * speed / 10);
            //}

            if (currentState != PlayerState.dead && (Input.GetAxis("Horizontal") >= .33f || Input.GetAxis("Vertical") >= .33f))
            {
                transform.Translate(Input.GetAxis("Horizontal") / 10, 0, Input.GetAxis("Vertical") / 10);
            }
            else if (currentState != PlayerState.dead && (Input.GetAxis("Horizontal") <= -.33f || Input.GetAxis("Vertical") <= -.33f))
            {
                transform.Translate(Input.GetAxis("Horizontal") / 10, 0, Input.GetAxis("Vertical") / 10);
            }
        } 
		else 
		{
			//transform.position = Vector3.Lerp (transform.position, realPosition, 0.1f);
			transform.rotation = Quaternion.Lerp (transform.rotation, realRotation, 0.1f);
			//anim.SetBool("Moving", true);
		}
    }

	// Update is called once per frame
	protected virtual void Update () {
        
        //Our player
        if (PV.isMine)
        {
            if (currentState != PlayerState.dead)
            {

                Move();

                //Jump
                if (Input.GetButtonDown("Jump"))
                {
                    Jump();
                }


                //Attack
                if (Input.GetButtonDown("Ability1") & !AbilityInProgress())
                {
                    currentState = PlayerState.attack1;
                    Ability1();
                }
                if (Input.GetButtonDown("Ability2") & !AbilityInProgress())
                {
                    currentState = PlayerState.attack2;
                    Ability2();
                }
                if (Input.GetButtonDown("Ability3") & !AbilityInProgress())
                {
                    currentState = PlayerState.attack3;
                    Ability3();
                }
            }
            else
            {
                active_timer += Time.deltaTime;
                if (active_timer >= death_timer)
                {
                    active_timer = 0.0f;
                    transform.position = spawn_position;
                    currentState = PlayerState.idle;
                }
            }
        }
    }


    bool AbilityInProgress() {
        return anim.GetCurrentAnimatorStateInfo(1).IsName("Ability1") || anim.GetCurrentAnimatorStateInfo(1).IsName("Ability2") || anim.GetCurrentAnimatorStateInfo(1).IsName("Ability3");
    }


    void Move() {
        //Movement
        if (Input.GetAxis("Horizontal") >= .33f)
        {
            anim.SetFloat("Speed", Input.GetAxis("Horizontal"));
        }
        else if (Input.GetAxis("Horizontal") <= -.33f)
        {
            anim.SetFloat("Speed", -(Input.GetAxis("Horizontal")));
        }

        if (Input.GetAxis("Vertical") >= .33f)
        {
            anim.SetFloat("Speed", Input.GetAxis("Vertical"));
        }
        else if (Input.GetAxis("Vertical") <= -.33f)
        {
            anim.SetFloat("Speed", -(Input.GetAxis("Vertical")));
        }

        if ((Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0) || ((Input.GetAxis("Vertical") > -.33f && Input.GetAxis("Vertical") < .33f) && (Input.GetAxis("Horizontal") > -.33f && Input.GetAxis("Horizontal") < .33f)))
        {
            moving = false;
            anim.SetBool("Moving", false);
            anim.SetFloat("Speed", 0);
            StopMovement();
        }



        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
           // moving = true;
            //anim.SetBool("Moving", true);

            //Setting rotation states
            if (Input.GetAxis("Horizontal") < 0)
                left = true;
            else
                left = false;
            if (Input.GetAxis("Horizontal") > 0)
                right = true;
            else
                right = false;
            if (Input.GetAxis("Vertical") < 0)
                backward = true;
            else
                backward = false;
            if (Input.GetAxis("Vertical") > 0)
                forward = true;
            else
                forward = false;

        }
        else if (moving)
        {
            anim.SetBool("Moving", false);
            StopMovement();
        }


        //Check to see if we are on the ground
        Ray ray = new Ray(col.bounds.center, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, col.height / 2 + .05f))
        {
            //Hitting ground, didn't just jump, and we are currently jumping
            if (!hit.collider.isTrigger & !justJumped && anim.GetBool("Jump"))
            {
                anim.SetBool("Jump", false);
            }
        }


                //Rotations
        if (left)
            Rotate(270);
        if (right)
            Rotate(90);
        if (forward)
            Rotate(0);
        if (backward)
            Rotate(180);

        if (left && forward)
            Rotate(315);
        if (left && backward)
            Rotate(225);
        if (right && forward)
            Rotate(45);
        if (right && backward)
            Rotate(135);


        mesh.rotation = Quaternion.Slerp(mesh.rotation, destRot, .25f);

        //if not idle snap forward based on camera position
        if (anim.GetFloat("Speed") >= .33f &! cam.GetComponent<CameraRotate>().selectionMode)
        {

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, cam.eulerAngles.y, transform.eulerAngles.z);
            destRot = transform.rotation;
        }
    }

    void StopMovement() {
		moving = false;
        left = false;
        right = false;
        forward = false;
        backward = false;
    }


    void Jump()
    {
        Ray ray = new Ray(col.bounds.center, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, col.height / 2 + .05f))
        {
            if (!hit.collider.isTrigger)
            {
                anim.SetBool("Jump", true);
                StartCoroutine(JustJumpedTimer());
                body.AddForce(0, jumpAmount, 0);
            }
        }
    }


    void Rotate(int degree)
    {
        destRot = transform.rotation;
        destRot *= Quaternion.Euler(Vector3.up * degree);
    }

    public void TakeDamage(int dmg) {
        if(dmg - defense > 0) health -= dmg - defense;
        if (health <= 0) Death();
		Debug.Log ("Ouch");
    }

    private void Death() 
	{
        currentState = PlayerState.dead;
		//col.enabled = false;

    }

    IEnumerator JustJumpedTimer()
    {
        justJumped = true;
        yield return new WaitForSeconds(.5f);
        justJumped = false;
    }

    protected abstract void Ability1();

    protected abstract void Ability2();

    protected abstract void Ability3();

	protected Transform Get_Child(string child_name)
	{
		return Get_Child (transform, child_name);
	}

	protected Transform Get_Child(Transform child_transform, string child_name)
	{
		if (child_transform.name == child_name) 
		{
			return child_transform;
		} 
		else 
		{
			foreach (Transform child in child_transform) 
			{
				Transform returned_child = Get_Child (child, child_name);
				if (returned_child != null) 
				{
					return returned_child;
				}
			}
		}
		return null;
	}
}
