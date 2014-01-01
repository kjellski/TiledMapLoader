/*!
 * UniTMX: A tiled map editor file importer for Unity3d
 * https://bitbucket.org/PolCPP/unitmx
 * 
 * Copyright 2012 Pol Cámara
 * Released under the MIT license
 * Check LICENSE.MIT for more details.
 */

using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
public class TileMap : MonoBehaviour
{
	public TextAsset tilemap;
	public bool freezeLayersOnCollider = true;
		
	public Mesh CreateMesh ()
	{
		// We use the currentLayer ID to order them on the Z axis.
		int currentLayerID = 0;

		// UsedVertices is used to maintain a count of the vertices between 
		//layers so when you call renderTriangles, it nows where to start.
		int usedVertices = 0;

		TileSet tileset = null;		
		List<Vector3> vertices = new List<Vector3> ();
		List<Vector2> uv = new List<Vector2> ();
		List<int> triangles = new List<int> ();
		Mesh mesh = new Mesh ();
		

		// What we're doing here is simple: Load the xml file from the attributes
		// And start parsing. Once it finds a tileset element it creates the tileset object
		// (it will be the first thing it encounters)
		XmlDocument xmldoc = new XmlDocument ();
		xmldoc.Load (new StringReader (tilemap.text)); 
		XmlNodeList nodelist = xmldoc.DocumentElement.ChildNodes;		
		foreach (XmlNode outerNode in nodelist) {
			switch (outerNode.Name) {
			case "tileset":
				// Basically we just grab the data from the xml and build a Tileset object
				// To avoid problems with the collision tileset since we only store one tileset 
				// we ignore anything with Collision inside.  
				if (outerNode.Attributes ["name"].InnerText.IndexOf ("Collision") == -1) {
					XmlNode imageNode = outerNode.SelectSingleNode ("image");
					int firstGID = int.Parse (outerNode.Attributes ["firstgid"].InnerText);
					int width = int.Parse (outerNode.Attributes ["tilewidth"].InnerText);
					int height = int.Parse (outerNode.Attributes ["tileheight"].InnerText);
					int imageWidth = int.Parse (imageNode.Attributes ["width"].InnerText);
					int imageheight = int.Parse (imageNode.Attributes ["height"].InnerText);
					int tileBorder = 0;
					if (outerNode.Attributes ["spacing"] != null)
						tileBorder = int.Parse (outerNode.Attributes ["spacing"].InnerText);						
					tileset = new TileSet (firstGID, width, height, imageWidth, imageheight, tileBorder);	
				}
				break;
			case "layer":
				// First we build the layer object and then just call the renderVertices 
				// renderUV and renderTriangles to build our textured mesh. 
				// Finally we store the vertexcount and +1 to the currentLayerID. 
				// like in the tileset, we avoid using the "collision_" layers since 
				// they only contain collision info
				if (outerNode.Attributes ["name"].InnerText.IndexOf ("collision_") == -1) {
					XmlNode dataNode = outerNode.SelectSingleNode ("data");
					int layerWidth = int.Parse (outerNode.Attributes ["width"].InnerText);
					int layerHeight = int.Parse (outerNode.Attributes ["height"].InnerText);
					string csvData = dataNode.InnerText;
					Layer currentLayer = new Layer (tileset, csvData, currentLayerID, layerWidth, layerHeight);
					vertices.AddRange (currentLayer.renderVertices ());
					uv.AddRange (currentLayer.renderUv ());
					triangles.AddRange (currentLayer.renderTriangles (usedVertices, usedVertices+currentLayer.vertexCount));
					usedVertices += currentLayer.vertexCount;
				}
				currentLayerID += 1;
				break;
			}
		}

		mesh.vertices = vertices.ToArray ();
		mesh.uv = uv.ToArray ();
		mesh.triangles = triangles.ToArray ();
		return mesh;		
	}
	
	public Mesh CreateColliderMesh ()
	{		
		// We use the currentLayer ID to order them on the Z axis.
		// Be aware that on the collider mesh it depends on the freezeColliderLayers
		// variable, this way you can avoid having colliders on different z axis.
		int currentLayerID = 0;
		
		// UsedVertices is used to maintain a count of the vertices between 
		// layers so when you call renderTriangles, it nows where to start.		
		int usedVertices = 0;

		TileSet tileset = null;		
		List<Vector3> vertices = new List<Vector3> ();
		List<int> triangles = new List<int> ();
		Mesh mesh = new Mesh ();
		
		XmlDocument xmldoc = new XmlDocument ();
		xmldoc.Load (new StringReader (tilemap.text)); 
		XmlNodeList nodelist = xmldoc.DocumentElement.ChildNodes;		

		// Works the almost the same way as the createMesh method (builds the
		// tileset and layer objects, then builds the mesh with those
		// but only checks on the collision names plus it doesn't build model
		// uv since its not neccesary. 
		foreach (XmlNode outerNode in nodelist) {
			switch (outerNode.Name) {
			case "tileset":	
				if (outerNode.Attributes ["name"].InnerText.IndexOf ("Collision") != -1) {
					XmlNode imageNode = outerNode.SelectSingleNode ("image");
					int firstGID = int.Parse (outerNode.Attributes ["firstgid"].InnerText);
					int width = int.Parse (outerNode.Attributes ["tilewidth"].InnerText);
					int height = int.Parse (outerNode.Attributes ["tileheight"].InnerText);
					int imageWidth = int.Parse (imageNode.Attributes ["width"].InnerText);
					int imageheight = int.Parse (imageNode.Attributes ["height"].InnerText);
					tileset = new TileSet (firstGID, width, height, imageWidth, imageheight);					
					
					// On the TMX editor, we set some attributes on our collider tiles so when parsing the
					// tileset, we'll be adding them to our tileset object.
					foreach (XmlNode innerNode in outerNode.ChildNodes) {
						if (innerNode.Name == "tile") {
							int tileID = 0;
							if (int.TryParse (innerNode.Attributes ["id"].InnerText, out tileID)) {
								XmlNode propertyNodes = innerNode.SelectSingleNode ("properties");
								foreach (XmlNode propertyNode in propertyNodes) {
									if (propertyNode.Attributes ["name"].InnerText == "col") {
										tileset.AddCollision (tileID, propertyNode.Attributes ["value"].InnerText);
									}
								}
							}
						}
					}
				}
				break;
			case "layer":
				if (outerNode.Attributes ["name"].InnerText.IndexOf ("collision_") != -1) {
					XmlNode dataNode = outerNode.SelectSingleNode ("data");
					int layerWidth = int.Parse (outerNode.Attributes ["width"].InnerText);
					int layerHeight = int.Parse (outerNode.Attributes ["height"].InnerText);
					string csvData = dataNode.InnerText;
					Layer currentLayer = new Layer (tileset, csvData, currentLayerID, layerWidth, layerHeight);
					vertices.AddRange (currentLayer.renderColVertices ());
					triangles.AddRange (currentLayer.renderTriangles (usedVertices, usedVertices + currentLayer.vertexCount));
					usedVertices += currentLayer.vertexCount;
					// In case we freeze the layers on collider they wont move on the z axis.
				}
				if (!freezeLayersOnCollider) 
					currentLayerID++;
				break;
			case "objectgroup":
				if (outerNode.Attributes ["name"].InnerText.IndexOf ("CollisionObjects") != -1) {
					foreach (XmlNode objectNode in outerNode.ChildNodes) {
						int colliderWidth = 0;
						int colliderHeight = 0;
						
						if (objectNode.Attributes.GetNamedItem ("width") != null) {
							colliderWidth = int.Parse (objectNode.Attributes ["width"].InnerText);
							colliderHeight = int.Parse (objectNode.Attributes ["height"].InnerText);	
						}
						
						int colliderX = int.Parse (objectNode.Attributes ["x"].InnerText);
						int colliderY = int.Parse (objectNode.Attributes ["y"].InnerText);
						colliderX += (colliderWidth / 2 + 16); 
						colliderY = - (colliderY + colliderHeight / 2);
						
						GameObject newCollider = null;
						
						if (objectNode.HasChildNodes) {
							// Can be polyline, ellipse or polygon, as boxes have no child.
							string objectType = objectNode.FirstChild.Name;
							Debug.Log ("Has children: " + objectType);
							switch (objectType) {
							case "ellipse":
								newCollider = CreateEllipseCollider (colliderX, colliderY, colliderWidth, colliderHeight);
								break;
							case "polyline":
								XmlNode polylineData = objectNode.FirstChild;
								newCollider = CreatePolylineCollider (polylineData, colliderX, colliderY);
								break;
							case "polygon":
							default:
								break;
							}
						} else {
							// Box
							newCollider = CreateBoxCollider (colliderX, colliderY, colliderWidth, colliderHeight);

						}
						if (newCollider != null) {
							if (transform.FindChild ("Colliders") == null) {
								GameObject colliderContainer = new GameObject ("Colliders");
								colliderContainer.transform.parent = gameObject.transform;
							}
							newCollider.transform.parent = transform.FindChild ("Colliders").transform;
						}
					}
				}
				break;
			}
		}
		mesh.vertices = vertices.ToArray ();
		mesh.triangles = triangles.ToArray ();
		return mesh;		
	}	
	private GameObject CreateBoxCollider (int colliderX, int colliderY, int colliderWidth, int colliderHeight)
	{
		Debug.Log ("Creating box at " + colliderX + ", " + colliderY);
		GameObject cube = new GameObject ("Collider_box_" + colliderX + colliderY);
		BoxCollider collider = cube.AddComponent (typeof(BoxCollider)) as BoxCollider;
		cube.transform.localPosition = new Vector3 (colliderX, colliderY, 0);
		collider.size = new Vector3 (colliderWidth, colliderHeight, 10.0f);
		
		return cube;
	}
	
	private GameObject CreateEllipseCollider (int colliderX, int colliderY, int colliderWidth, int colliderHeight)
	{
		Debug.Log ("Creating ellypse collider at " + colliderX + ", " + colliderY);
		if (colliderWidth != colliderHeight) {
			// We are simulating the ellipse with a capsule collider, which cannot 
			// be as accurate as a real ellypse...
			Debug.LogWarning ("Ellypse colliders are not really accurate when not round.");
		}
		GameObject ellipse = new GameObject ("Collider_ellipse_" + colliderX + colliderY);
		CapsuleCollider collider = ellipse.AddComponent (typeof(CapsuleCollider)) as CapsuleCollider;
		ellipse.transform.localPosition = new Vector3 (colliderX, colliderY, 0);
		if (colliderWidth >= colliderHeight) {
			collider.direction = 0;
			collider.radius = colliderHeight / 2.0f;
			collider.height = colliderWidth;
		} else {
			collider.direction = 1;
			collider.radius = colliderWidth / 2.0f;
			collider.height = colliderHeight;
		}

		return ellipse;		
	}
	
	private GameObject CreatePolylineCollider (XmlNode data, int colliderX, int colliderY)
	{
		GameObject polyline = new GameObject ("Collider_polyline" + colliderX + colliderY);
		Mesh mesh = new Mesh ();
		polyline.AddComponent<MeshCollider> ();
//		polyline.AddComponent<MeshFilter> ();
//		polyline.AddComponent<MeshRenderer> ();
		
		ArrayList points = new ArrayList ();
		List<Vector3> vertices = new List<Vector3> ();
		List<int> verticesOrder = new List<int> ();
		
		string pointsString = data.Attributes ["points"].InnerText;
		string [] pointsTuples = pointsString.Split (' ');
		
		foreach (string p in pointsTuples) {
			Vector3 point = new Vector3 ();
			string[] tmp = p.Split (',');
			point.x = colliderX + int.Parse (tmp [0]);
			point.y = colliderY - int.Parse (tmp [1]);
			point.z = 0.0f;
			points.Add (point);
			Debug.Log ("Adding point: " + point.ToString ());
		}
		
		Vector3 firstPoint = (Vector3)points [0];
		for (int idx=1; idx < points.Count; idx++) {
			Vector3 secondPoint = (Vector3)points [idx];
			Vector3 firstFront = new Vector3 (firstPoint.x, firstPoint.y, -10.0f);
			Vector3 firstBack = new Vector3 (firstPoint.x, firstPoint.y, 10.0f);
			Vector3 secondFront = new Vector3 (secondPoint.x, secondPoint.y, -10.0f);
			Vector3 secondBack = new Vector3 (secondPoint.x, secondPoint.y, 10.0f);
			vertices.Add (firstFront); // 0
			vertices.Add (firstBack); // 1
			vertices.Add (secondFront); // 2
			vertices.Add (secondBack); // 3
			
			verticesOrder.Add ((idx - 1) * 4 + 0);
			verticesOrder.Add ((idx - 1) * 4 + 1);
			verticesOrder.Add ((idx - 1) * 4 + 3);

			verticesOrder.Add ((idx - 1) * 4 + 3);
			verticesOrder.Add ((idx - 1) * 4 + 2);
			verticesOrder.Add ((idx - 1) * 4 + 0);

			firstPoint = secondPoint;
		}
		
		mesh.vertices = vertices.ToArray ();
		mesh.triangles = verticesOrder.ToArray ();
		
		mesh.RecalculateNormals ();
		
		polyline.GetComponent<MeshCollider> ().sharedMesh = mesh;
//		polyline.GetComponent<MeshFilter> ().mesh = mesh;
		
		return polyline;
	}
}