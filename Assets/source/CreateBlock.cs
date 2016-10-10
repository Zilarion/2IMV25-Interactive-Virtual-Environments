using UnityEngine;
using System.Collections;

public class CreateBlock : MonoBehaviour {
	public float x, z;

	// Use this for initialization
	void Start () {
		GameObject obj = this.gameObject;

		float width = 1.0f;
		float height = 1.0f;
		float depth = 1.0f;

		x = x / obj.transform.lossyScale.x;
		z = z / obj.transform.lossyScale.z;

		// Left side
		createCube (obj.transform, 
			new Vector3(-width / 2, 0, 0), 
			new Vector3(0.01f, height, depth));
		// Right side
		createCube (obj.transform, 
			new Vector3(width / 2, 0, 0), 
			new Vector3(0.01f, height, depth));


		// Back side
		createCube (obj.transform, 
			new Vector3(0, 0, depth / 2), 
			new Vector3(width, height, 0.01f));
		// Front side
		createCube (obj.transform, 
			new Vector3(0, 0, -depth / 2), 
			new Vector3(width, height, 0.01f));

		// Bottom side
		createCube (obj.transform, 
			new Vector3(0, - height / 2, 0), 
			new Vector3(width, 0.01f, depth));


		createTop (obj.transform, width, height, depth);
	}

	GameObject createCube(Transform parent, Vector3 position, Vector3 scale) {
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.transform.parent = parent;
		cube.transform.localPosition = position;
		cube.transform.localScale = scale;
		return cube;
	}

	GameObject createTop (Transform parent, float width, float height, float depth) {
		GameObject top = new GameObject ();
		top.transform.parent = parent;
		top.transform.localPosition = new Vector3(0, height / 2, 0);
		top.transform.localScale = new Vector3(1, 1, 1);

		float xLeftRight = width / 2 - x / 2;

		// Left of hole
		createCube (top.transform, 
			new Vector3(- width / 2 + xLeftRight / 2, 0, 0),
			new Vector3(xLeftRight, 0.01f, depth));

		// Right of hole
		createCube (top.transform, 
			new Vector3(width / 2 - xLeftRight / 2, 0, 0),
			new Vector3(xLeftRight, 0.01f, depth));

		float zTopBottom = depth / 2 - z / 2;

		// Top of hole
		createCube (top.transform, 
			new Vector3(0, 0, - depth / 2 + zTopBottom / 2),
			new Vector3(x, 0.01f, zTopBottom));

		// Bottom of hole
		createCube (top.transform, 
			new Vector3(0, 0, + depth / 2 - zTopBottom / 2),
			new Vector3(x, 0.01f, zTopBottom));

		// Right of hole
		return top;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
