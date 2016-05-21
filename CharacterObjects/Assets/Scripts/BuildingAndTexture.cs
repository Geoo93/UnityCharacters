﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class BuildingAndTexture : MonoBehaviour {


	public GameObject sphere = null;
	private List <GameObject> newSpheres = new List<GameObject>();
	private List <GameObject> floorCheck = new List<GameObject>();


	bool pickTexture = false;

	private int xlength, ylength, zlength, row = 0;

	private int xSize = 12;
	private int ySize = 40;
	private int zSize = 6;

	private int roundness = 0;
	private bool roundTop = false;
	private bool roundFront = false;
	private bool roundBack = false;
	private bool roundSides = false;


	private int offset = 0;
	private int midY = 0;

	private Vector3 topPoint = new Vector3 ();
	private List<int[]> rows = new List<int[]>();
	private List<int> listOfVerticesIndexes = new List<int>();
	private List <Vector3> verticesCopy = new List<Vector3> ();


	private List<int> allControlPoint= new List<int>();
	private List<int> topControlPointIndexes = new List<int>();
	private List<int> frontControlPointIndexes = new List<int>();
	private List<int> backControlPointIndexes = new List<int>();
	private List<int> sidesControlPointIndexes = new List<int>();

	private BoxCollider boxCollider;
	private MeshCollider meshCollider;
	private MeshFilter meshFilter;
	private Renderer meshRenderer;
	private Mesh mesh;


	private List<Vector3> vertices = new List<Vector3>();

	private List<Vector3> normals = new List<Vector3>();

	private List<Vector2> uv = new List<Vector2>();

	private int[] triangles; 

	private static int
	SetQuad (int[] triangles, int i, int v00, int v10, int v01, int v11) {

		triangles[i] = v00;
		triangles[i + 4] = v01;
		triangles[i + 1] = triangles[i + 4];
		triangles[i + 3] = v10;
		triangles[i + 2] = triangles[i + 3];
		triangles[i + 5] = v11;



		return i + 6;
	}


	private GameObject plane;


	private void RandomBuildingProperties()
	{

		plane = GameObject.Find("Plane");

		float xScale = Random.Range (1.0f,6.0f);
		float zScale = Random.Range (1.0f,6.0f);

		plane.transform.localScale = new Vector3 (xScale, plane.transform.localScale.y, zScale);

		xSize = (int)plane.GetComponent<MeshRenderer> ().bounds.size.x;
		zSize = (int)plane.GetComponent<MeshRenderer> ().bounds.size.z;

		float xx = plane.transform.position.x - ((float)xSize/2.0f);
		float zz = plane.transform.position.z - ((float)zSize/2.0f);
		Vector3 pivotCenter = new Vector3 (xx,plane.transform.position.y, zz);

	
		GameObject center = (GameObject)Instantiate (sphere, pivotCenter, Quaternion.identity);
		center.GetComponent<MeshRenderer> ().material.color = Color.blue;
		floorCheck.Add (center);

		int splitSize = 25;

		if (xSize > splitSize || zSize > splitSize) {

			//print (xSize);

			int xCount = 1;
			while (xSize / xCount > splitSize) {
				
				xCount++;
			}
			float xOffset = xSize / xCount;
			print ("x Offset: " + xOffset + "   x count: " + xCount);


			int zCount = 1;
			while (zSize / zCount > splitSize) {
				zCount++;
			}
			float zOffset = zSize / zCount;

			print ("z Offset: " + zOffset + "   z count: " + zCount);


			for (int s = 0; s < xCount; s++) {

				float xP = center.transform.position.x + (s * xOffset);

				for (int z = 0; z < zCount; z++) {

					float zP = center.transform.position.z + (z * zOffset);
					//float zP = center.transform.position.z + zOffset + (z * zOffset);

					Vector3 xz = new Vector3 (xP, center.transform.position.y, zP);

					GameObject a = (GameObject)Instantiate (sphere, xz, Quaternion.identity);
					floorCheck.Add (a);
				}

			}
			print ("all point: " + floorCheck.Count);


			xSize = (int)xOffset - 6;
			ySize = Random.Range (20, 60);
			zSize = (int)zOffset - 6;


			print ("bounds: " + plane.GetComponent<MeshRenderer> ().bounds);
			print ("size:  " + plane.GetComponent<MeshRenderer> ().bounds.size);

			roundTop = true;//(Random.Range (0, 2) == 0);
			roundFront = (Random.Range (0, 2) == 0);
			roundBack = (Random.Range (0, 2) == 0);
			roundSides = (Random.Range (0, 2) == 0);


			int i = 0;
			for (int r = 0; r < 20; r++) {
			
				if (i < xSize && i < zSize && i < 5) {
					i++;
				}
			}
			roundness = Random.Range (0, i);

			print ("top: " + roundTop + "    front: " + roundFront + "   back: " + roundBack + "   sides: " + roundSides);
			print ("x: " + xSize + "    y: " + ySize + "   z: " + zSize + "   roundness: " + roundness);

			//this.transform.position = new Vector3 (-xSize / 2, this.transform.position.y, -zSize / 2);
			this.transform.position = new Vector3 (
				floorCheck [1].transform.localPosition.x + 3,
				this.transform.position.y, 
				floorCheck [1].transform.localPosition.z + 3);
		} else {

			xSize -= 4;
			ySize = Random.Range (30, 60);
			zSize -= 4;
			this.transform.position = new Vector3(
				floorCheck[0].transform.localPosition.x + 2,
				this.transform.position.y, 
				floorCheck[0].transform.localPosition.z + 2);
		}
			
	}
	private void CreateControllPointsIndexes ()
	{
		//get all rows
		row = (xSize + zSize) * 2;

		xlength = xSize + 1;
		ylength = ySize + 1;
		zlength = zSize + 1;

		int zExtra = zSize - 2;
		offset = ((xlength * 2 ) + (zSize - 1 + zExtra)) ;

		//Debug.Log (" offset " + offset);
	
		for (int x = 0; x < offset + 1; x++) {

			for (int z = 0; z < zlength; z++)
			{

				List<int> innerArray = new List<int>();

				for (int y = 0; y < ylength; y++)
				{
					int myPos = (((offset * y) + x) + y);
					innerArray.Add (myPos);
					//print(innerArray[y]);

				}
				rows.Insert(x, innerArray.ToArray());
			}
		}
		//print ("rows points:  " + rows.Count);
	}

//	void createSpheresInvertices(){
//
//		for (int v = 0; v < vertices.Count; v++) {
//			createSphere (vertices [v] + this.transform.localPosition, newSpheres);
//
//		}
//
//		//backControlPointIndexes//frontControlPointIndexes
//		for (int x = 0; x < row  ; x++) {
//			newSpheres [rows [x] [4]].GetComponent<MeshRenderer> ().material.color = Color.green;
//			//print("v: "+x+"   "+rows [x][0]);
//
////			for (int z = 0; z < rows [x].Length; z++) 
////			{
////				newSpheres [rows [x] [z]].GetComponent<MeshRenderer> ().material.color = Color.green;
////
////			}
//		}
//	}
//
	private void CreateMesh()
	{

		meshFilter = GetComponent<MeshFilter>();
		if (meshFilter == null){
			Debug.LogError("MeshFilter not found!");
			return;
		}

		mesh = meshFilter.sharedMesh;
		if (mesh == null){
			meshFilter.mesh = new Mesh();
			mesh = meshFilter.sharedMesh;
		}
		mesh.name = "building mesh";

		mesh.Clear();
	}
		
	private void CreateVertices() {


		int cornerVertices = 8;
		int edgeVertices = (xSize + ySize + zSize - 3) * 4;

		int faceVertices = (
			(xSize - 1) * (ySize - 1) +
			(xSize - 1) * (zSize - 1) +
			(ySize - 1) * (zSize - 1)) * 2;

		int verticesLength = cornerVertices + edgeVertices + faceVertices;
		//vertices = new Vector3[verticesLength];
		//normals = new Vector3[verticesLength];
		//uv = new Vector2[verticesLength];

		//print ("vertices Length: "+verticesLength);

		int v = 0;
		// sides
		for (int y = 0; y <= ySize; y++) {
			for (int x = 0; x <= xSize; x++) {
				SetVertex(v++, x, y, 0);
			}
			for (int z = 1; z <= zSize; z++) {
				SetVertex(v++, xSize, y, z);
			}
			for (int x = xSize - 1; x >= 0; x--) {
				SetVertex(v++, x, y, zSize);
			}

			for (int z = zSize - 1; z > 0; z--) {
				SetVertex(v++, 0, y, z);
			}
		}


		// top 
		for (int z = 1; z < zSize; z++) {
			for (int x = 1; x < xSize; x++) {
				SetVertex(v++, x, ySize, z);
			}
		}
		//bottom
		for (int z = 1; z < zSize; z++) {
			for (int x = 1; x < xSize; x++) {
				SetVertex(v++, x, 0, z);
			}
		}



	}
	private void SetVertex (int i, int x, int y, int z) {
		
		Vector3 vect = new Vector3 (x, y, z);
		Vector3 inner = vect;

			
		////sides
		if (x < roundness) {
			if (roundSides) {
				inner.x = roundness;
			} else {
				inner.x = 0;
			}
		}
		else if (x > xSize - roundness) {

			if (roundSides) {
				inner.x = xSize - roundness;
			} else {
				inner.x = xSize; 
			}
		}

		////top and bottom
		if (y < roundness) {
			//bottom rounder
			//inner.y = roundness;
		}
		else if (y > ySize - roundness) {
			// top rounder
			//inner.y = 0;
			if (roundTop) {
				inner.y = ySize - roundness;
			} else {
				inner.y = ySize; 
			}
		}

		////front and back
		if (z < roundness) {
			// add or disable front rounder
			if (roundFront) {
				inner.z = roundness;
			} else {
				inner.z = 0;
			}
		}
		else if (z > zSize - roundness) {
			//add or disable back rounder
			if (roundBack) {
				inner.z = zSize - roundness;
			} else {
				inner.z = zSize;
			}
		}

		normals.Add((vect - inner).normalized);
		vertices.Add(inner + normals[i] * roundness);
		uv.Add(new Vector2((float)x / ( xSize), (float)y / (ySize) ));


	}

	private void CreateTriangles () {
		
		int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
		int triLength = quads * 6;
		triangles = new int[triLength];
		int ring = (xSize + zSize) * 2;
		int t = 0, v = 0;

		for (int y = 0; y < ySize; y++, v++) {
			for (int q = 0; q < ring - 1; q++, v++) {
				t = SetQuad(triangles, t, v, v + 1, v + ring, v + ring + 1);
			}
			t = SetQuad(triangles, t, v, v - ring + 1, v + ring, v + 1);
		}

		t = CreateTopFace(triangles, t, ring);
		t = CreateBottomFace(triangles, t, ring);

	
	}

	private int CreateTopFace (int[] triangles, int t, int ring) {
		int v = ring * ySize;
		for (int x = 0; x < xSize - 1; x++, v++) {
			t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
		}
		t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

		int vMin = ring * (ySize + 1) - 1;
		int vMid = vMin + 1;
		int vMax = v + 2;

		for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++) {
			t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + xSize - 1);
			for (int x = 1; x < xSize - 1; x++, vMid++) {
				t = SetQuad(
					triangles, t,
					vMid, vMid + 1, vMid + xSize - 1, vMid + xSize);
			}
			t = SetQuad(triangles, t, vMid, vMax, vMid + xSize - 1, vMax + 1);
		}

		int vTop = vMin - 2;
		t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);
		for (int x = 1; x < xSize - 1; x++, vTop--, vMid++) {
			t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
		}
		t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);

		return t;
	}

	private int CreateBottomFace (int[] triangles, int t, int ring) {
		int v = 1;
		int vMid = vertices.Count - (xSize - 1) * (zSize - 1);

		t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
		for (int x = 1; x < xSize - 1; x++, v++, vMid++) {
			t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
		}
		t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);

		int vMin = ring - 2;
		vMid -= xSize - 2;
		int vMax = v + 2;

		for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++) {
			t = SetQuad(triangles, t, vMin, vMid + xSize - 1, vMin + 1, vMid);
			for (int x = 1; x < xSize - 1; x++, vMid++) {
				t = SetQuad(
					triangles, t,
					vMid + xSize - 1, vMid + xSize, vMid, vMid + 1);
			}
			t = SetQuad(triangles, t, vMid + xSize - 1, vMax + 1, vMid, vMax);
		}

		int vTop = vMin - 1;
		t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
		for (int x = 1; x < xSize - 1; x++, vTop--, vMid++) {
			t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
		}
		t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);

		return t;
	}

	private void AddPropertiesToMesh()
	{
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles;

		mesh.normals = normals.ToArray();
		mesh.uv = uv.ToArray();

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();




	}
	private void CreateColliders () {

		//Destroy(boxCollider);
		//boxCollider = gameObject.AddComponent<BoxCollider>();
		//boxCollider.size = new Vector3(xSize, ySize, zSize);
		//boxCollider.center = new Vector3 ((float)xSize/2, (float)ySize/2, (float)zSize/2);
		Destroy(meshCollider);
		meshCollider = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
		meshCollider.sharedMesh = mesh; // Give it your mesh here.

	}

	private void GetIndexes () {

		int p = 0;
		for (int i = 0; i < vertices.Count; i++) {

			Vector3 vertexPos = new Vector3 (vertices [i].x + this.transform.position.x, vertices [i].y + this.transform.position.y, vertices [i].z + this.transform.position.z);
			verticesCopy.Add(vertexPos);

			listOfVerticesIndexes.Add (p);
			p++;

			//top vertices
			if (vertices [i].y == ySize) {
				topControlPointIndexes.Add (i);

			}
		}
		//Debug.Log ("vertices length: "+vertices.Length +"   list of indexes length: "+ listOfVerticesIndexes.Count+"  vertex points copied: "+verticesCopy.Count);


	}

	private GameObject createSphere(Vector3 pos , List <GameObject> objectArr){

		GameObject a = (GameObject) Instantiate(sphere, pos, Quaternion.identity);
		a.transform.localScale = new Vector3 (0.4f, 0.4f, 0.4f);
		a.GetComponent<Renderer> ().material.color = Color.red;
		a.transform.parent = this.transform;
		objectArr.Add (a);

		return a;
	}

	private void CreatePivotControlPoints (){

		midY = (int)(Mathf.Round (ySize / 2));
		int pivot = 0;

		for (int s = 0; s < listOfVerticesIndexes.Count; s++) {

			//print (listOfIndexes.ToArray());
			for (int a = 0; a < offset + 1; a++) {

				if (  listOfVerticesIndexes[s].Equals(rows [a] [midY])   ) {

					//createSphere (verticesCopy [listOfVerticesIndexes [s]], newSpheres);
					allControlPoint.Add (pivot);
					//print (listOfIndexes[s]+"   "+newSpheres.Count);
					pivot++;
				} 

			}

		}

		////create top sphere
		topPoint = new Vector3 (this.transform.localPosition.x + (float)xSize / 2, this.transform.localPosition.y + ySize, this.transform.localPosition.z + (float)zSize / 2);
		//createSphere (topPoint, newSpheres);


		////front
		int front = xlength  ;
		for (int f = 0; f < front; f++) {

			frontControlPointIndexes.Add (f);
		}

		////back
		int backFrom = xSize + zSize;
		int backTo = allControlPoint.Count - (zSize - 1);//row - zSize + 1; 
		for (int b = backFrom; b < backTo; b++) {

			backControlPointIndexes.Add (b);
		}


		//first side
		for (int s = 0; s < zSize - 1; s++) {

			sidesControlPointIndexes.Add (s + xlength);
		}

		//second side
		for (int ss = (xlength * 2) + (zSize - 1); ss < allControlPoint.Count; ss++) {

			sidesControlPointIndexes.Add (ss);
		}



	}

	private void RandomOutlinesGeneration()
	{

		////front calcutations
		int fFromLoop = Random.Range (0, frontControlPointIndexes.Count - 1);
		int fToLoop = Mathf.Clamp (Random.Range (fFromLoop, frontControlPointIndexes.Count - 1), 0, frontControlPointIndexes.Count - 1);
		float fRandOffset = Random.Range (-4, 4);

		//print ("front from: " + fFromLoop + "   front too: " + fToLoop + "   all front:" + frontControlPointIndexes.Count);

		if (fToLoop > fFromLoop + 1) {
			
			for (int i = fFromLoop; i < fToLoop; i++) {

				for (int z = 0; z < rows [frontControlPointIndexes [i]].Length; z++) {

					vertices [rows [frontControlPointIndexes [i]] [z]] = new Vector3 (
						vertices [rows [frontControlPointIndexes [i]] [z]].x,
						vertices [rows [frontControlPointIndexes [i]] [z]].y,
						vertices [rows [frontControlPointIndexes [i]] [z]].z + fRandOffset);
				}

			}

		}
			

		////back calcutations
		int bFrom = Random.Range (0, backControlPointIndexes.Count - 1);
		int bTo = Mathf.Clamp (Random.Range (bFrom, backControlPointIndexes.Count - 1), 0, backControlPointIndexes.Count - 1);
		float bRandOffset = Random.Range (-4, 4);

		//print ("back from: " + bFrom + "   back too: " + bTo + "   all backs:" + backControlPointIndexes.Count);

		if (bTo > bFrom + 1) {
			for (int i = bFrom; i < bTo; i++) {

				for (int z = 0; z < rows [backControlPointIndexes [i]].Length; z++) {

					vertices [rows [backControlPointIndexes [i]] [z]] = new Vector3 (
						vertices [rows [backControlPointIndexes [i]] [z]].x,
						vertices [rows [backControlPointIndexes [i]] [z]].y,
						vertices [rows [backControlPointIndexes [i]] [z]].z + fRandOffset);
				}
			}
		}


		////sides calcutations
		int sType = Random.Range (0, 3);
		int sFrom = 0;
		int sTo = 0;
		int sFrom2 = 0;
		int sTo2 = 0;
		float sRandOffset = Random.Range (-4, 4);

		switch (sType) {

		case 0:
			sFrom = 1;
			sTo = (sidesControlPointIndexes.Count / 2) - 1;
			break;
		case 1:
			sFrom = (sidesControlPointIndexes.Count / 2) + 1;
			sTo = (sidesControlPointIndexes.Count) - 1;
			break;
		case 2:
			sFrom = 1;
			sTo = (sidesControlPointIndexes.Count/2) - 1;
			sFrom2 = sTo + 2;
			sTo2 = sidesControlPointIndexes.Count - 1;
			break;

		}

		print (" Sides Type" + sType + "   sides from: " + sFrom + "   sides too: " + bTo + "   all sides:" + sidesControlPointIndexes.Count);

		if (sType != 2) {
			
			for (int i = sFrom; i < sTo; i++) {

				//if (sType != 2 || sType == 2 && i != (sidesControlPointIndexes.Count / 2) && i != ((sidesControlPointIndexes.Count / 2) - 1)) {

				for (int z = 0; z < rows [sidesControlPointIndexes [i]].Length; z++) {

					vertices [rows [sidesControlPointIndexes [i]] [z]] = new Vector3 (
						vertices [rows [sidesControlPointIndexes [i]] [z]].x + sRandOffset,
						vertices [rows [sidesControlPointIndexes [i]] [z]].y,
						vertices [rows [sidesControlPointIndexes [i]] [z]].z);
				}

			}
		}else{
			
			for (int a = sFrom; a < sTo; a++) {

				for (int z = 0; z < rows [sidesControlPointIndexes [a]].Length; z++) {

					vertices [rows [sidesControlPointIndexes [a]] [z]] = new Vector3 (
						vertices [rows [sidesControlPointIndexes [a]] [z]].x + sRandOffset,
						vertices [rows [sidesControlPointIndexes [a]] [z]].y,
						vertices [rows [sidesControlPointIndexes [a]] [z]].z);
				}
			}
			for (int aa = sFrom2; aa < sTo2; aa++) {

				for (int z = 0; z < rows [sidesControlPointIndexes [aa]].Length; z++) {

					vertices [rows [sidesControlPointIndexes [aa]] [z]] = new Vector3 (
						vertices [rows [sidesControlPointIndexes [aa]] [z]].x - sRandOffset,
						vertices [rows [sidesControlPointIndexes [aa]] [z]].y,
						vertices [rows [sidesControlPointIndexes [aa]] [z]].z);
				}

			}
		}


		////top calcutations
		int topPivotIndex = newSpheres.Count - 1;
		float tRandOffset = Random.Range (-4, 2);

		for (int y = 0; y < topControlPointIndexes.Count; y++) {

			vertices [topControlPointIndexes [y]] = new Vector3 (
				vertices [topControlPointIndexes [y]].x,
				vertices [topControlPointIndexes [y]].y + +tRandOffset,
				vertices [topControlPointIndexes [y]].z);
		
		}

		print (" top offset:  " + tRandOffset);

	}
	private void CreateColorAndtexture() {

		meshRenderer = GetComponent<MeshRenderer> ();
	
//		Material material = new Material (Shader.Find (".ShaderExample/TextureSplatting"));


		Texture[] smallStripes = new Texture[] {
			Resources.Load ("TextureStripe2") as Texture,
			Resources.Load ("TextureStripe7") as Texture,
			Resources.Load ("TextureStripe8") as Texture,
			Resources.Load ("TextureStripe9") as Texture
		};
		int pickSmallStripes = (int)Mathf.Floor (Random.value * smallStripes.Length);


		Texture[] bigStripes = new Texture[] {

			Resources.Load ("TextureStripe1") as Texture,
			Resources.Load ("TextureStripe3") as Texture,
			Resources.Load ("TextureStripe4") as Texture,
			Resources.Load ("TextureStripe5") as Texture,
			Resources.Load ("TextureStripe6") as Texture,
			Resources.Load ("TextureStripe10") as Texture
		};
		int pickBigStripes = (int)Mathf.Floor (Random.value * bigStripes.Length);

		Texture[] bigStripesInverted = new Texture[] {

			Resources.Load ("TextureStripe11") as Texture,
			Resources.Load ("TextureStripe33") as Texture,
			Resources.Load ("TextureStripe44") as Texture,
			Resources.Load ("TextureStripe55") as Texture,
			Resources.Load ("TextureStripe66") as Texture
		};
		int pickbigStripesInverted = (int)Mathf.Floor (Random.value * bigStripesInverted.Length);

		Texture small = smallStripes [pickSmallStripes] as Texture;
		Texture big = bigStripes [pickBigStripes] as Texture;
		Texture inverted = bigStripesInverted [pickbigStripesInverted] as Texture;
//
//		material.SetTexture ("_Texture1", small);
//		material.SetTextureScale ("_Texture1", new Vector2(1,1));
//
//		material.SetTexture ("_Texture2", big);
//		material.SetTextureScale ("_Texture2", new Vector2(1,1));
//
//
//		//material.color = Color.Lerp(Color.white, ExtensionMethods.RandomColor(), 1f);
//		meshRenderer.material = material;
//
		//plane.GetComponent<MeshRenderer> ().material = material;




//		Material mat = new Material (Shader.Find (".ShaderExample/VertexWaveAnimationY"));
//		mat.SetFloat ("_Frequency", 2f);
//		mat.SetFloat ("_Noise", 13.0f);
//		mat.SetFloat ("_Speed", 6.0f);
//		meshRenderer.material = mat;


		Material mat = new Material (Shader.Find (".ShaderExample/GradientThreeColor(Texture)"));
		mat.SetTexture ("_MainTex", small);
		mat.SetColor ("_ColorTop", Color.white);
		mat.SetColor ("_ColorMid", Color.white);
		mat.SetColor ("_ColorBot", ExtensionMethods.RandomColor());
		mat.SetFloat ("_Middle", Random.Range(0.2f,0.6f));
		meshRenderer.material = mat;



	}

	private void UpdateVerticesAndPositions() {


		for (int v = 0; v < vertices.Count ; v++) {
			vertices [v] = newSpheres [v].transform.localPosition; 

		}


	}


	void Awake ()
	{

		this.name = "dynamic object";

		RandomBuildingProperties ();
		CreateControllPointsIndexes ();
		CreateMesh ();
		CreateVertices();

		GetIndexes ();
		CreatePivotControlPoints ();
		RandomOutlinesGeneration ();


		CreateTriangles();
		AddPropertiesToMesh ();
		CreateColliders();
		CreateColorAndtexture ();

//		print("vertices: "+vertices.Length);
//		print("normals: "+normals.Length);
//		print("uv: "+uv.Length);


		//createSpheresInvertices ();



	}

	void Update()
	{
//		UpdateVerticesAndPositions ();
//		CreateMesh ();
//		CreateTriangles();
//		AddPropertiesToMesh ();
//
//
		if (Input.GetKeyDown ("space")) 
		{
			removeAll ();
			addAgain ();

		}



	}

	void removeAll(){

		meshFilter.sharedMesh = null;
		meshCollider.sharedMesh = null;
		mesh.Clear();
		vertices.Clear (); 
		triangles = null; 
		normals.Clear ();
		uv.Clear ();


		foreach (GameObject spheres in newSpheres) {
			DestroyImmediate (spheres);

		}
		newSpheres.Clear ();

		foreach (GameObject floorPoints in floorCheck) {
			DestroyImmediate (floorPoints);
		}
		floorCheck.Clear ();

		rows.Clear();
		listOfVerticesIndexes.Clear();
		verticesCopy.Clear();

		allControlPoint.Clear();
		topControlPointIndexes.Clear (); 
		frontControlPointIndexes.Clear();
		backControlPointIndexes.Clear (); 
		sidesControlPointIndexes.Clear();
	
	
	}


	void addAgain(){

		RandomBuildingProperties ();
		CreateControllPointsIndexes ();
		CreateMesh ();
		CreateVertices();

		GetIndexes ();
		CreatePivotControlPoints ();
		RandomOutlinesGeneration ();


		CreateTriangles();
		AddPropertiesToMesh ();
		CreateColliders();
		CreateColorAndtexture ();

	}


}
