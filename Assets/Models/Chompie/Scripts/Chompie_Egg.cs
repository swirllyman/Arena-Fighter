using UnityEngine;
using System.Collections;

public class Chompie_Egg : Photon.MonoBehaviour {

    public float open_Timer = 2.0f;
    public float destroy_Timer = 10.0f;
    public bool destroyed, opened, spawned;
    public Transform mother;
    Animation anim;
    Renderer mesh;
	// Use this for initialization
	void Start () {
        spawned = false;
        opened = false;
        anim = GetComponent<Animation>();
        mesh = GetComponentInChildren<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {

        open_Timer -= Time.deltaTime;
        destroy_Timer -= Time.deltaTime;

        if (open_Timer <= 0.0f)
        {
            anim.Play("Chompie_Egg_Open");
            if (!spawned)
                Open();
        }
        else
            anim.Play("Chompie_Egg_Close");

        if (destroy_Timer <= 0.0f &! destroyed) {
            StartCoroutine(DestroyEgg());
        }

        if (opened)
        {
            anim.Play("Chompie_Egg_Opened");
        }
	}

    public void SetMother(Transform t)
    {
        mother = t;
    }

    void Open()
    {
        spawned = true;
        StartCoroutine(OpenEgg(anim["Chompie_Egg_Open"].length));
        if(PhotonNetwork.isMasterClient){
            GameObject baby = PhotonNetwork.Instantiate("BabyChomp", transform.position, transform.rotation, 0);
            baby.GetComponent<Chomp>().SetMother(mother);
        }

    }

    IEnumerator OpenEgg(float waitTimer) {
        yield return new WaitForSeconds(waitTimer);
        opened = true;

    }


    IEnumerator DestroyEgg()
    {
        mesh.enabled = false;
        destroyed = true;
        yield return new WaitForSeconds(.15f);
        mesh.enabled = true;
        yield return new WaitForSeconds(.15f);
        mesh.enabled = false;
        yield return new WaitForSeconds(.15f);
        mesh.enabled = true;
        yield return new WaitForSeconds(.15f);
        mesh.enabled = false;
        yield return new WaitForSeconds(.15f);
        mesh.enabled = true;
        yield return new WaitForSeconds(.15f);
        mesh.enabled = false;
        if (PhotonNetwork.isMasterClient) {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
