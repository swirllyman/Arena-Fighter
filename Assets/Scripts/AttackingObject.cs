using UnityEngine;
using System.Collections;

public class AttackingObject : MonoBehaviour {

    Player player;
	// Use this for initialization
	void Start () {
        player = transform.root.GetComponent<Player>();
	}

    void SetPlayer(Player p)
    {
        player = p;
    }

	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider c) 
	{
        if (c.tag == "Enemy" && c.transform != player.transform) 
		{
			c.transform.GetComponent<Player>().TakeDamage(50);
        }
        else if(c.tag == "Baby_Chomp")
        {
            if (PhotonNetwork.isMasterClient)
            {
                c.transform.GetComponent<Chomp>().Kill();
            }
        }
    }
}
