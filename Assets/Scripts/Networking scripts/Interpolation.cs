using UnityEngine;
using System.Collections;

public class Interpolation : MonoBehaviour 
{
	private PhotonView PV;

	private Vector3 correctPlayerPos = Vector3.zero; // We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; // We lerp towards this
	private Quaternion correctShootingRot = Quaternion.identity; // We lerp towards this
    private Player myPlayer;

	public Animator anim;
	public Transform mesh;
    


	Quaternion realRotation = Quaternion.identity;

	void Awake ()
	{
        myPlayer = GetComponent<Player>();
		anim = GetComponent<Animator>();
	}

	void Start () 
	{
		PV = GetComponent<PhotonView> ();
	}

	void Update () 
	{
		if (!PV.isMine) 
		{
			SyncedMovement ();
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
        //We are sending this information   --  OUR player
		if (stream.isWriting)
		{
			stream.SendNext(transform.position);
			stream.SendNext (mesh.rotation);
			stream.SendNext (anim.GetBool ("Moving"));
            stream.SendNext(anim.GetBool("Jump"));
            stream.SendNext(anim.GetBool("Ability1"));
            stream.SendNext(anim.GetBool("Ability2"));
            stream.SendNext(anim.GetBool("Ability3"));
            stream.SendNext(anim.GetFloat("Speed"));
            stream.SendNext(myPlayer.invis);

        }
        //We are receiveing information     --   Player we are viewing
		else
		{
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			realRotation = (Quaternion)stream.ReceiveNext ();
			anim.SetBool ("Moving", (bool)stream.ReceiveNext ());
            anim.SetBool("Jump", (bool)stream.ReceiveNext());
            anim.SetBool("Ability1", (bool)stream.ReceiveNext());
            anim.SetBool("Ability2", (bool)stream.ReceiveNext());
            anim.SetBool("Ability3", (bool)stream.ReceiveNext());
            anim.SetFloat("Speed", (float)stream.ReceiveNext());
            myPlayer.invis = (bool)stream.ReceiveNext();
        }
	}
    

    //Lerping movement in rotation. Disable this if you need instant translating
	private void SyncedMovement()
	{
		transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
		mesh.rotation = Quaternion.Lerp (mesh.rotation, realRotation, 0.1f);
	}
}
