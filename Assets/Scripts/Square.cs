using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square<T>
{
	public int positionX { get; protected set; }
	public int positionY { get; protected set; }
	//public Dictionary<string, Hexagon< HexObject>> neigbourHex;
	public List<Square<T>> neigbourHex { get; protected set; }
	public T Value { get; protected set; }
	public Vector3 worldPosition { get; protected set; }
	public Vector3[] corner { get; protected set; }
	public float size { get; protected set; }

	public Square(float size, Vector3 worldPosition = default(Vector3), T value = default(T), bool isVertical = false)
	{
		this.size = size;
		this.worldPosition = worldPosition;
		this.neigbourHex = new List<Square<T>>();
		this.corner = new Vector3[6];

		SetCorner();
	}
	/// <summary>
	/// Set position of corner points foundation on hexagon positiob=n
	/// </summary>
	public void SetCorner()
	{
		corner[0] = worldPosition + new Vector3(-size / 2, -size / 2);
		corner[1] = worldPosition + new Vector3(-size / 2, +size / 2);
		corner[2] = worldPosition + new Vector3(+size / 2, +size / 2);
		corner[3] = worldPosition + new Vector3(+size / 2, -size / 2);
	}
	public void SetGrid(int x, int y)
	{
		this.positionX = x; this.positionY = y;
	}
	public void SetValue(T val)
	{
		Value = val;
	}
	public Vector3 GetPosition()
	{
		return worldPosition;
	}
	public void AddNeigbourHex(Square<T> hex)
	{
		this.neigbourHex.Add(hex);
		hex.neigbourHex.Add(this);
	}
	public Mesh CreateHexMesh()
	{
		Mesh mesh = new Mesh();
		mesh.name = "Square";
		Vector3[] vertices = new Vector3[4];
		Vector3[] normals = new Vector3[4];
		Vector2[] uv = new Vector2[4];
		for (int i = 0; i < 4; i++)
		{
			vertices[i] = corner[i];
			normals[i] = Vector3.back;
			uv[i] = corner[i] / size + new Vector3(0.5f, 0.5f);
		}

		int[] triangles = new int[6];
		triangles[0] = 0;
		triangles[1] = 1;
		triangles[2] = 2;

		triangles[3] = 2;
		triangles[4] = 1;
		triangles[5] = 3;

		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uv;
		mesh.triangles = triangles;

		return mesh;
	}
}
