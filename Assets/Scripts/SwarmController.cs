/*Shaun Yonkers
 * 1650692
 * CMPT430
 * Lab 5: Swarm AI  
 * 
 * Swarm Controller script is used to generate the two swarms, at set locations, withing a set radius, using the prefab and color desired.
 * 
 * ****** Credit to Brian Brookwell for implementation of Swarm Controller ******
 * */

using UnityEngine;
using System.Collections;

public class SwarmController : MonoBehaviour {
	public Swarm x, y;
	public GameObject prefab;
	public Color Team1, Team2;

	// Create the two new swarms and set the tag of each swarm to distinguish between teams internally
	void Awake () {
		x = new Swarm (new Vector3 (-45, 0, -45), 15f, 30, prefab, Team1);
		y = new Swarm (new Vector3 (30, 0, 30), 15f, 30, prefab, Team2);

		x.setTag ("Team1");
		y.setTag ("Team2");
	}
}
