/*!
 * UniTMX: A tiled map editor file importer for Unity3d
 * https://bitbucket.org/PolCPP/unitmx
 * 
 * Copyright 2012 Pol Cámara
 * Released under the MIT license
 * Check LICENSE.MIT for more details.
 */

using System;
using UnityEngine;
using System.Collections.Generic;

// Manages all the magic. It builds both the tile mesh with it's uv's 
// And collision layer mesh 

public class Layer
{
	TileSet _tileset;
	string[]  _data;
	int _currentLayerID;
	int _width;
	int _height;
	int _vertexCount = 0;

	public int vertexCount {
		get {
			return this._vertexCount;
		}
	}	
	
	public Layer (TileSet tileset, string data, int currentLayerID, int width, int height)
	{
		this._tileset = tileset;			
		this._data = data.Split (',');
		this._currentLayerID = currentLayerID;
		this._width = width;
		this._height = height;
	}

	// Renders the tile vertices.
	// Basically, it reads the entire CSV file cells, and creates a 
	// 4 vertexes (forming a rectangle or square according to settings) 
	// when a value different than 0 is found  
	public List<Vector3> renderVertices ()
	{
		int dataIndex = 0;
		//_currentLayerID = _currentLayerID * 10;
		float z = _currentLayerID * -10;
		List<Vector3> vertices = new List<Vector3> ();
		for (int i = 1; i <= _height; i++) {
			for (int j = 1; j <= _width; j++) {
				string dataValue = _data [dataIndex].ToString ().Trim ();
				if (dataValue != "0") {
					vertices.AddRange (new Vector3[] {
							new Vector3 (_tileset.width * (j + 1), _tileset.height * (-i + 1), z),
							new Vector3 (_tileset.width * (j + 1), _tileset.height * -i, z),							
							new Vector3 (_tileset.width * j, _tileset.height * (-i + 1), z),								
							new Vector3 (_tileset.width * j, _tileset.height * -i, z),
						});	
					_vertexCount += 4;
				}
				dataIndex++;
			}
		}
		return vertices;
	}

	// Renders the collision vertices.
	// Basically, it works the same way as renderVertices but in this case
	// it checks the value to see what kind of collision mesh we need to draw.
	public List<Vector3> renderColVertices ()
	{
		int dataIndex = 0;
		float z2 = _currentLayerID * -10;
		float z = _currentLayerID * -10 + 5; // - _tileset.height;
		//_currentLayerID = _currentLayerID * 10;
		TileSet.CollisionFormat collision = TileSet.CollisionFormat.none;
		List<Vector3> vertices = new List<Vector3> ();
		for (int i = 1; i <= _height; i++) {
			for (int j = 1; j <= _width; j++) {
				string dataValue = _data [dataIndex].ToString ().Trim ();
				if (dataValue != "0") {
    				collision = _tileset.GetCollision (int.Parse (dataValue));
    				if (collision != TileSet.CollisionFormat.none) {
						if ((collision & TileSet.CollisionFormat.top) != 0) {
							vertices.AddRange (new Vector3[] {
									new Vector3 (_tileset.width * (j + 1), _tileset.height * (-i + 1), z),
									new Vector3 (_tileset.width * (j + 1), _tileset.height * (-i + 1), z2),							
									new Vector3 (_tileset.width * j, _tileset.height * (-i + 1), z),								
									new Vector3 (_tileset.width * j, _tileset.height * (-i + 1), z2),
								});								
                            _vertexCount += 4;
                        }
						if ((collision & TileSet.CollisionFormat.bottom) != 0) {
							vertices.AddRange (new Vector3[] {
									new Vector3 (_tileset.width * j, _tileset.height * -i, z),	
									new Vector3 (_tileset.width * j, _tileset.height * -i, z2),
									new Vector3 (_tileset.width * (j + 1), _tileset.height * -i, z),
									new Vector3 (_tileset.width * (j + 1), _tileset.height * -i, z2),							
								});															
                            _vertexCount += 4;
                        }
						if ((collision & TileSet.CollisionFormat.left) != 0) {
							vertices.AddRange (new Vector3[] {
									new Vector3 (_tileset.width * j, _tileset.height * (-i + 1), z),
									new Vector3 (_tileset.width * j, _tileset.height * (-i + 1), z2),							
									new Vector3 (_tileset.width * j, _tileset.height * (-i + 0), z),								
									new Vector3 (_tileset.width * j, _tileset.height * (-i + 0), z2),
								
								});															
                            _vertexCount += 4;
                        }
						if ((collision & TileSet.CollisionFormat.right) != 0) {
							vertices.AddRange (new Vector3[] {
									new Vector3 (_tileset.width * (j + 1), _tileset.height * (-i + 0), z),
									new Vector3 (_tileset.width * (j + 1), _tileset.height * (-i + 0), z2),							
									new Vector3 (_tileset.width * (j + 1), _tileset.height * (-i + 1), z),								
									new Vector3 (_tileset.width * (j + 1), _tileset.height * (-i + 1), z2),
								});															
                            _vertexCount += 4;
                        }
					}
				}
				dataIndex++;
			}
		}
//		_vertexCount = vertices.Length();
		return vertices;
	}		
	
	// Creates the Face UV for the faces according to the tile represented on the TMX.
	public List<Vector2> renderUv ()
	{

		List<Vector2> uv = new List<Vector2> (); 
		int horizontalCellCount = _tileset.materialWidth / (_tileset.width + _tileset.tileBorder);
		int verticalCellCount = _tileset.materialHeight / (_tileset.height + _tileset.tileBorder);		
		float cellWidth = ((float)_tileset.width / _tileset.materialWidth);
		float cellHeight = ((float)_tileset.height / _tileset.materialHeight);		
		float borderWidth = ((float)_tileset.tileBorder / _tileset.materialWidth);
		float borderHeight = ((float)_tileset.tileBorder / _tileset.materialHeight);
		int totalCells = _width * _height;
		int dataValue;
		for (int i = 0; i < totalCells; i++) {
				dataValue = int.Parse(_data [i].ToString ().Trim ());
				if (dataValue != 0) {
					dataValue = dataValue - _tileset.firstGID;
					int posY = dataValue / verticalCellCount;
					int posX = dataValue % horizontalCellCount; 					
					float u = ((cellWidth + borderWidth) * posX) + borderWidth/2;
					float v = 1.0f - ((cellHeight + borderHeight) * posY) - borderHeight/2;				
					uv.AddRange (new Vector2[] {
						new Vector2 (u + cellWidth, v),
						new Vector2 (u + cellWidth, v - cellHeight),
						new Vector2 (u, v),
						new Vector2 (u, v - cellHeight),					
					});
				}
		}
		return uv;
	}
	
	// Creates the triangles given the ammount of the Used Vertices until now (including other layers).
	public List<int> renderTriangles (int start, int end)
	{
		List<int> triangles = new List<int> ();
        int currentTri = start;
		while(currentTri < end) {
				triangles.AddRange (new int[] {
						currentTri, currentTri + 1, currentTri + 2,
			            currentTri + 2, currentTri + 1, currentTri + 3,
					});						
				currentTri += 4;
		}
		return triangles;
	}
}