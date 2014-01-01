/*!
 * UniTMX: A tiled map editor file importer for Unity3d
 * https://bitbucket.org/PolCPP/unitmx
 * 
 * Copyright 2012 Pol Cámara
 * Released under the MIT license
 * Check LICENSE.MIT for more details.
 */
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TileMap))]
public class TileMapEditor : Editor
{
	public override void OnInspectorGUI ()
	{
	    TileMap TMap = (TileMap) target;
		DrawDefaultInspector ();		
		if (GUILayout.Button ("Import Tiles")) {
			MeshFilter filter = TMap.GetComponent<MeshFilter>();
			if (filter)
				DestroyImmediate(filter, true);
			filter = TMap.gameObject.AddComponent<MeshFilter>();
			filter.mesh = TMap.CreateMesh();
		}
		if (GUILayout.Button ("Generate colliders")) {
			MeshCollider collider = TMap.GetComponent<MeshCollider>();
			if (collider)
				DestroyImmediate(collider, true);
			if(TMap.transform.FindChild("Colliders") != null){
				DestroyImmediate(TMap.transform.FindChild("Colliders").gameObject, true);
			}
			collider = TMap.gameObject.AddComponent<MeshCollider>();
			collider.sharedMesh = TMap.CreateColliderMesh();		
		}
	}
}