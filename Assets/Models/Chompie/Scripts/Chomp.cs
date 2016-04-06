using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Chomp : Photon.MonoBehaviour {

    Transform target;
    Animator anim;
    bool spawning = false;
    float spawnTimer = 4.5f;
    public float attackTimer = 2.5f;
    bool dead = false; 
    
    int forward = 20;
    public float sizeTimer = 0.0f;
    public Transform mother;
    public Player[] peopleInScene;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        anim.SetBool("Spawn", true);
        spawning = true;     
    }

    public void SetMother(Transform t)
    {
        mother = t;
        if (PhotonNetwork.isMasterClient)
        {
            peopleInScene = FindObjectsOfType<Player>();
            foreach (Player p in peopleInScene)
            {
                if (p.transform != mother)
                {
                    target = p.transform;
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {
        if (!dead) {
            if (transform.localScale.x < .2f)
            {
                transform.localScale *= Time.deltaTime / 20 + 1;
                sizeTimer += Time.deltaTime / 10;
            }
            if (photonView.isMine)
            {
                if (spawnTimer >= 0.0f)
                {
                    spawnTimer -= Time.deltaTime;
                }
                else
                {
                    anim.SetBool("Spawn", false);
                    spawning = false;
                }

                if (spawnTimer <= 2.0f)
                {
                    forward = 10;
                }
                if (spawning)
                {
                    transform.position += transform.forward / forward * Time.deltaTime;
                }
                else
                {
                    if (target == null)
                    {
                        Idle();
                    }
                    else if (Vector3.Distance(target.position, transform.position) > 1.5f)
                    {
                        Chase();
                    }
                    else
                    {
                        Attack();
                    }
                }


                if(attackTimer > 0.0f)
                {
                    attackTimer -= Time.deltaTime;
                }
            }
        }
	}

    void Idle()
    {
        anim.SetBool("Run", false);
        anim.SetBool("Attack", false);
    }

    void Chase()
    {
        transform.LookAt(target.position);
        transform.position += transform.forward * (5 * sizeTimer) * Time.deltaTime;
        anim.SetBool("Run", true);
        anim.SetBool("Attack", false);
    }

    void Attack()
    {
        if (attackTimer <= 0.0f) {
            anim.SetBool("Run", false);
            anim.SetBool("Attack", true);
            StartCoroutine(AttackTimer());
            attackTimer = 2.5f;
        }
    }

    public void Kill()
    {
        target = null;
        dead = true;

        anim.SetBool("Run", false);
        anim.SetBool("Attack", false);
        anim.SetBool("Spawn", false);
        anim.SetBool("Death", true);
        StartCoroutine(DeathTimer());
    }

    IEnumerator AttackTimer()
    {
        yield return new WaitForSeconds(.5f);
        anim.SetBool("Attack", false);
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(3.0f);
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }       
    }
}
