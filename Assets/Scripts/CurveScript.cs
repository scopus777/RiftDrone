using UnityEngine;
using System.Collections;

public class CurveScript : MonoBehaviour {

	public int xCountVertices = 11;
	public int yCountVertices = 11;

	// Use this for initialization
	void Start () {
		var mesh = GetComponent<MeshFilter>().mesh;
		var vertices = mesh.vertices;
		
		for (var i = 0; i < yCountVertices; i++) {
			vertices [i * yCountVertices] = vertices [i * yCountVertices] + new Vector3 (0, 4, 0);
			vertices [(xCountVertices-1) + i * yCountVertices] = vertices [(xCountVertices-1) + i * yCountVertices] + new Vector3 (0, 4, 0);
		}
		for (var i = 0; i < yCountVertices; i++) {
			vertices [1 + i * yCountVertices] = vertices [1 + i * yCountVertices] + new Vector3 (0, 2.5f, 0);
			vertices [(xCountVertices-2) + i * yCountVertices] = vertices [(xCountVertices-2) + i * yCountVertices] + new Vector3 (0, 2.5f, 0);
		}
		for (var i = 0; i < yCountVertices; i++) {
			vertices [2 + i * yCountVertices] = vertices [2 + i * yCountVertices] + new Vector3 (0, 1.5f, 0);
			vertices [(xCountVertices-3) + i * yCountVertices] = vertices [(xCountVertices-3) + i * yCountVertices] + new Vector3 (0, 1.5f, 0);
		}
		for (var i = 0; i < yCountVertices; i++) {
			vertices [3 + i * yCountVertices] = vertices [3 + i * yCountVertices] + new Vector3 (0, 0.5f, 0);
			vertices [(xCountVertices-4) + i * yCountVertices] = vertices [(xCountVertices-4) + i * yCountVertices] + new Vector3 (0, 0.5f, 0);
		}
		
		mesh.vertices = vertices;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
