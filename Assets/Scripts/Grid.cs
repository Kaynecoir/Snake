using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<T>
{
	public int Height { get; protected set; }
	public int Width { get; protected set; }
	public float Size { get; protected set; }
	public float littleRadius { get; protected set; }
	public bool isVertical { get; private set; }
	public Vector3 zeroCoord { get; protected set; }
	public Vector3 PositionToCenter { get; protected set; }
	public Mesh gridMesh;
	protected TextMesh[,] textArray;
	public Square<T>[,] gridArray { get; protected set; }

	public delegate void VoidFunc();
	public event VoidFunc ChangeValue;
	public Grid(int height, int width, float radius, Vector3 worldPosition, bool isDebuging = false)
	{
		Height = height;
		Width = width;
		Size = radius;
		littleRadius = radius * Mathf.Sin(Mathf.PI / 3);
		zeroCoord = worldPosition;
		PositionToCenter = new Vector3(radius * (isVertical ? Mathf.Sin(Mathf.PI / 3) : 1), radius * (!isVertical ? Mathf.Sin(Mathf.PI / 3) : 1));
		this.isVertical = isVertical;
		gridArray = new Square<T>[Height, Width];
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				Square<T> h = new Square<T>(Size, GetPositionFromCenter(x, y) + zeroCoord, isVertical: this.isVertical);
				h.SetCorner();
				h.SetGrid(x, y);
				if (x - 1 >= 0) h.AddNeigbourHex(gridArray[y, x - 1]);
				if (y - 1 >= 0) h.AddNeigbourHex(gridArray[y - 1, x]);

				gridArray[y, x] = h;
				}
		}
	}
	
	public Square<T> this[int y, int x]
	{
		get
		{
			return gridArray[y, x];
		}
		private set
		{
			gridArray[y, x] = value;
		}
	}
	public Vector3 GetPositionFromCenter(int x, int y)
	{
		Vector3 pos = zeroCoord;

		pos = new Vector3(x, y) * Size + PositionToCenter;

		return pos;
	}
	public Vector3 GetPositionFromWorld(int x, int y)
	{
		Vector3 pos = zeroCoord;
		pos = gridArray[y, x].worldPosition;
		return pos;
	}
	public Vector3Int GetXY(Vector3 worldPosition)
	{
		return GetXY(worldPosition, out int x, out int y);
	}
	public Vector3Int GetXY(Vector3 CursorPosition, out int x, out int y)
	{
		x = 0; y = 0;
		Vector3 CursorToCenter = CursorPosition - zeroCoord;
		x = Mathf.RoundToInt(CursorToCenter.x / Size - 0.5f);
		y = Mathf.RoundToInt(CursorToCenter.y / Size - 0.5f);
		x = x < Width && x >= 0 ? x : 0;
		y = y < Height && y >= 0 ? y : 0;
		x = 0; y = 0;
		return new Vector3Int(x, y);
	}
	public void SetValue(Vector3 worldPosition, T val)
	{
		int x, y;
		GetXY(worldPosition, out x, out y);
		SetValue(x, y, val);
	}
	public void SetValue(int x, int y, T val)
	{
		if (x >= 0 && x < Width && y >= 0 && y < Height)
		{
			gridArray[y, x].SetValue(val);
			ChangeValue?.Invoke();
		}
	}
	public Square<T> GetValue(Vector3 worldPosition)
	{
		GetXY(worldPosition, out int x, out int y);
		return GetValue(x, y);
	}
	public Square<T> GetValue(int x, int y)
	{
		if (x >= 0 && x < Width && y >= 0 && y < Height)
		{
			return gridArray[y, x];
		}
		else return default(Square<T>);
	}
	public Mesh CreateMeshArray()
	{
		gridMesh = new Mesh();
		gridMesh.name = "HexGrid";
		Vector3[] vertices = new Vector3[(Width * Height) * 7];
		Vector3[] normals = new Vector3[(Width * Height) * 7];
		Vector2[] uv = new Vector2[(Width * Height) * 7];
		int[] triangles = new int[(Width * Height) * 18];
		for (int y = 0; y < Height; y++)
		{
			for (int x = 0; x < Width; x++)
			{
				int index = y * Width + x;
				AddMeshToArray(GetValue(x, y), vertices, normals, uv, triangles, index);
			}
		}
		gridMesh.vertices = vertices;
		gridMesh.normals = normals;
		gridMesh.uv = uv;
		gridMesh.triangles = triangles;
		return gridMesh;
	}
	public Mesh CreateMeshArray(out Vector3[] vertices, out Vector3[] normals, out Vector2[] uv, out int[] triangles)
	{
		gridMesh = new Mesh();
		gridMesh.name = "HexGrid";
		vertices = new Vector3[(Width * Height) * 7];
		normals = new Vector3[(Width * Height) * 7];
		uv = new Vector2[(Width * Height) * 7];
		triangles = new int[(Width * Height) * 18];
		for (int y = 0; y < Height; y++)
		{
			for (int x = 0; x < Width; x++)
			{
				int index = y * Width + x;
				AddMeshToArray(GetValue(x, y), vertices, normals, uv, triangles, index);
			}
		}
		gridMesh.vertices = vertices;
		gridMesh.normals = normals;
		gridMesh.uv = uv;
		gridMesh.triangles = triangles;
		return gridMesh;
	}
	public Mesh CreateMeshArray(Vector2 uvPoint)
	{
		gridMesh = new Mesh();
		gridMesh.name = "HexGrid";
		Vector3[] vertices = new Vector3[(Width * Height) * 7];
		Vector3[] normals = new Vector3[(Width * Height) * 7];
		Vector2[] uv = new Vector2[(Width * Height) * 7];
		int[] triangles = new int[(Width * Height) * 18];

		for (int y = 0; y < Height; y++)
		{
			for (int x = 0; x < Width; x++)
			{
				int index = y * Width + x;
				AddMeshToArray(GetValue(x, y), vertices, normals, uv, triangles, index, uvPoint);
			}
		}
		gridMesh.vertices = vertices;
		gridMesh.normals = normals;
		gridMesh.uv = uv;
		gridMesh.triangles = triangles;

		return gridMesh;
	}

	public void AddMeshToArray(Square<T> squr, Vector3[] vertices, Vector3[] normals, Vector2[] uv, int[] triangles, int index)
	{
		for (int i = 0; i < 6; i++)
		{
			vertices[index * 4 + i] = squr.corner[i] - zeroCoord;
			//UnityEngine.Debug.Log(vertices[index * 7 + i]);
			normals[index * 4 + i] = Vector3.back;
			uv[index * 4 + i] = (squr.corner[i] - squr.worldPosition) / squr.size + new Vector3(0.5f, 0.5f);
		}


		triangles[index * 6 + 0] = index * 4 + 0;
		triangles[index * 6 + 1] = index * 4 + 1;
		triangles[index * 6 + 2] = index * 4 + 2;

		triangles[index * 6 + 3] = index * 4 + 0;
		triangles[index * 6 + 4] = index * 4 + 2;
		triangles[index * 6 + 5] = index * 4 + 3;
	}
	public void AddMeshToArray(Square<T> squr, Vector3[] vertices, Vector3[] normals, Vector2[] uv, int[] triangles, int index, Vector2 uvPoint)
	{
		for (int i = 0; i < 6; i++)
		{
			vertices[index * 4 + i] = squr.corner[i] - zeroCoord;
			normals[index * 4 + i] = Vector3.back;
			uv[index * 4 + i] = uvPoint;
		}

		triangles[index * 6 + 0] = index * 6 + 0;
		triangles[index * 6 + 1] = index * 6 + 1;
		triangles[index * 6 + 2] = index * 6 + 2;

		triangles[index * 6 + 3] = index * 6 + 2;
		triangles[index * 6 + 4] = index * 6 + 1;
		triangles[index * 6 + 5] = index * 6 + 3;
	}
	public void ClearMeshArray()
	{
		for (int y = 0; y < Height; y++)
		{
			for (int x = 0; x < Width; x++)
			{
				int index = y * Width + x;
				for (int i = 0; i < 6; i++)
				{
					gridMesh.uv[index * 7 + i] = (GetValue(x, y).corner[i] - GetValue(x, y).worldPosition) / Size / 2 + new Vector3(0.5f, 0.5f);
				}
				gridMesh.uv[index * 7 + 6] = new Vector2(0.5f, 0.5f);
			}
		}
	}
}
