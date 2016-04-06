using UnityEngine;
using System.Collections;

public class Chomp_Interpolate : MonoBehaviour {

    private PhotonView PV;

    private Vector3 correctPlayerPos = Vector3.zero; // We lerp towards this
    private Quaternion correctPlayerRot = Quaternion.identity; // We lerp towards this
    private Quaternion correctShootingRot = Quaternion.identity; // We lerp towards this

    public Animator anim;

    Quaternion realRotation = Quaternion.identity;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (!PV.isMine)
        {
            SyncedMovement();
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(anim.GetBool("Spawn"));
            stream.SendNext(anim.GetBool("Run"));
            stream.SendNext(anim.GetBool("Attack"));
            stream.SendNext(anim.GetBool("Death"));

        }
        else
        {
            correctPlayerPos = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
            anim.SetBool("Spawn", (bool)stream.ReceiveNext());
            anim.SetBool("Run", (bool)stream.ReceiveNext());
            anim.SetBool("Attack", (bool)stream.ReceiveNext());
            anim.SetBool("Death", (bool)stream.ReceiveNext());
        }
    }

    private void SyncedMovement()
    {
        transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
        transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
    }
}
