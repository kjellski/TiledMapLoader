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

// Simply a tileset storage class. It's basically used to access the tile
// width and height and texture width and height
//
// The only exception are the collisions, which also stores the tiles with
// collision and it's format.
//
// In case you want to EXTEND them formats with more interesting options 
// like diagonal collisions or square collisions.
// You just need to update the CollisionFormat enum, the Addcollision method
// and in the Layer class the renderColVertices method.

public class TileSet
{
	public enum CollisionFormat
	{
		none=0,
		top=1,
		bottom=2,
		left=4,
		right=8
	};

	private int _firstGID;
	private int _width;
	private int _height;
	private int _materialWidth;
	private int _materialHeight;
	private int _tileBorder;
	
	// Stores the tile id and Collision format. 
	// Note that on the tile id it uses it's absolute number (Tile id + FirstGID) 
	private Dictionary<int, CollisionFormat> _collisions = new Dictionary<int, CollisionFormat> ();

	public int firstGID {
		get {
			return this._firstGID;
		}
	}

	public int height {
		get {
			return this._height;
		}
	}

	public int materialHeight {
		get {
			return this._materialHeight;
		}
	}

	public int materialWidth {
		get {
			return this._materialWidth;
		}
	}

	public int width {
		get {
			return this._width;
		}
	}

	public int tileBorder {
		get {
			return this._tileBorder;
		}
	}
	
	public TileSet (int firstGID, int width, int height, int materialWidth, int materialHeight, int tileBorder = 0)
	{
		this._firstGID = firstGID;
		this._width = width;
		this._height = height;		
		this._materialWidth = materialWidth;
		this._materialHeight = materialHeight;
		this._tileBorder = tileBorder;		
	}
	
	public void AddCollision (int tile_id, string collisionText)
	{
		CollisionFormat collisionFormat = CollisionFormat.none;
		string[] words = collisionText.Split(',');
    	foreach (string word in words) {
    		switch (word) {
    		case "top":
    			collisionFormat |= CollisionFormat.top;		
    			break;
    		case "bottom":
    			collisionFormat |= CollisionFormat.bottom;		
    			break;				
    		case "left":
    			collisionFormat |= CollisionFormat.left;		
    			break;
    		case "right":
    			collisionFormat |= CollisionFormat.right;	
    			break;
    		}
    	}
		if (collisionFormat != CollisionFormat.none)
			_collisions.Add (tile_id + firstGID, collisionFormat);	
	}
	
	public CollisionFormat GetCollision (int tile_id)
	{
		if (_collisions.ContainsKey (tile_id))
			return _collisions [tile_id];
		else
			return CollisionFormat.none;
	}
}

