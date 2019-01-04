using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XMtileMap;

public class Test : MonoBehaviour {

    public Vector2 start = new Vector2(-5, -4);
    public Vector2 end = new Vector2(3, 3);

    void Awake()
    {
        Map.CreateTileMap(0);
    }

    // Use this for initialization
    void Start () {
        GameObject player = (GameObject)Instantiate(Resources.Load("Player"));
        player.transform.position = start;
        player.GetComponent<AStar>().Move(start, end);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
