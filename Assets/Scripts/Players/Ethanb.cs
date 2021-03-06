﻿using UnityEngine;
using System.Collections;

public class Ethanb : Player 
{

    public float timer;
    public float invisTimer = 5.0f;

    Renderer myBody;


	protected override void Start ()
	{
		base.Start();
		attackingObject1 = Get_Child("Ethan_Light");
		attackingObject2 = Get_Child ("EthanBody");

        myBody = Get_Child("EthanBody").GetComponent<Renderer>();

	}


    protected override void Update()
    {
        base.Update();

        if (invis)
        {
            myBody.enabled = false;

            if (photonView.isMine)
            {
                timer -= Time.deltaTime;

                if (timer <= 0.0f)
                {
                    invis = false;
                }
            }
        }
        else
        {
            myBody.enabled = true;
        }
    }

    protected override void Ability1() 
	{
        base.Ability1();
		attackingObject1.GetComponent<Light> ().enabled = !attackingObject1.GetComponent<Light> ().enabled;
    }

    protected override void Ability2()
    {
        base.Ability2();
		GameObject projectile_object = PhotonNetwork.Instantiate ("Ethan_Projectile", attackingObject2.position + mesh.forward, mesh.rotation, 0);
    }

    protected override void Ability3()
    {
        base.Ability3();
        timer = invisTimer;
        invis = true;

    }

}
