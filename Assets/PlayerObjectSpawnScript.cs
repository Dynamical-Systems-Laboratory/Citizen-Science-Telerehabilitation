using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerObjectSpawnScript : NetworkBehaviour {

	// Use this for initialization

	public GameObject playerObjPrefab;

	void Start () {
		if (!hasAuthority) {
			return;
		}

		GameObject go = Instantiate (playerObjPrefab);

		NetworkServer.SpawnWithClientAuthority (go, connectionToClient);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
