using UnityEngine;
using System.Collections;

public class Chompie : Player {

    float ability3_CD_Timer = 10.0f;
    bool ability3_CD = false;
    protected override void Start()
    {
        base.Start();
        attackingObject1 = Get_Child("Mouth_Trigger");
        attackingObject2 = Get_Child("Fire_Breath");
        attackingObject3 = Get_Child("Waist");
    }


    protected override void Update()
    {
        base.Update();
        if (ability3_CD)
        {
            ability3_CD_Timer -= Time.deltaTime;
        }
       
        if (ability3_CD_Timer <= 0.0f) {
            ability3_CD = false;
            ability3_CD_Timer = 10.0f;
        }
    }


    protected override void Ability1()
    {
        anim.SetBool("Ability1", true);
        StartCoroutine(MouthAttack());
    }

    protected override void Ability2()
    {
        anim.SetBool("Ability2", true);
        StartCoroutine(ChargeBreath());
    }

    protected override void Ability3()
    {
        if (!ability3_CD)
        {
            anim.SetBool("Ability3", true);
            StartCoroutine(BirthEgg());
        }
    }


    [PunRPC]
    public void Breath_Fire()
    {
        attackingObject2.GetComponent<ParticleSystem>().Play();
    }

    [PunRPC]
    public void Toggle_Collider(bool choice)
    {
        attackingObject2.GetComponent<Collider>().enabled = choice;
    }

    [PunRPC]
    public void Birth()
    {
        GameObject baby = PhotonNetwork.Instantiate("Egg", attackingObject3.position - (transform.up / 4), mesh.rotation, 0);
        baby.GetComponent<Chompie_Egg>().SetMother(transform);
    }


    IEnumerator MouthAttack()
    {
        yield return new WaitForSeconds(.3f);
        attackingObject1.GetComponent<Collider>().enabled = true;
        yield return new WaitForSeconds(.35f);
        attackingObject1.GetComponent<Collider>().enabled = false;
        anim.SetBool("Ability1", false);
    }

    IEnumerator ChargeBreath() {
        yield return new WaitForSeconds(.5f);
        StartCoroutine(Fire());
        photonView.RPC("Toggle_Collider", PhotonTargets.All, true);
        anim.SetBool("Ability2", false);
    }

    IEnumerator Fire()
    {
        photonView.RPC("Breath_Fire", PhotonTargets.All, null);
        yield return new WaitForSeconds(2.75f);
        photonView.RPC("Toggle_Collider", PhotonTargets.All, false);
    }

    IEnumerator BirthEgg()
    {
        ability3_CD = true;
        yield return new WaitForSeconds(1);
        anim.SetBool("Ability3", false);
        photonView.RPC("Birth", PhotonTargets.MasterClient, null);
    }
}
