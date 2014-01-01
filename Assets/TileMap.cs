using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Xml;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class TileMap : MonoBehaviour
{
    private readonly List<TileLayer> _tileLayers = new List<TileLayer>();
    private readonly List<TileSet> _tileSets = new List<TileSet>();

    public float tileSize = 0.5f;

    // Use this for initialization
    private void Start()
    {
        LoadFile("level002");
        BuildTexture();
        Debug.Log("Done generating TileMap.");
    }

    private void BuildTexture()
    {
        Texture2D texture = null;
        foreach (var tileLayer in _tileLayers)
        {
            int tilesCountX = tileLayer.Width;
            int tilesCountY = tileLayer.Height;

            texture = new Texture2D(tileLayer.Width * 32, tileLayer.Height * 32);
            Debug.Log(String.Format("Created texture: {0}x{1}", texture.width, texture.height));
            for (int y = 0; y < tilesCountY; y++)
            {
                for (int x = 0; x < tilesCountX; x++)
                {
                    Tile t = tileLayer.Tiles[x, y];
                    texture.SetPixels(x * 32, y * 32, 32, 32, t.TileTexture.GetPixels());
                    texture.Apply();
                    Debug.Log(String.Format("SetPixels({0}, {1}, {2}, {3}, {4})", x * 32, y * 32, 32, 32, t.TileTexture.GetPixel(0, 0)));
                }
            }
        }

        var spriteRenderer = GetComponent<MeshRenderer>();
        spriteRenderer.sharedMaterial.mainTexture = texture;
    }

    private void LoadFile(string path)
    {
        var textAsset = (TextAsset)Resources.Load(path);

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text);
        ParseFile(xmlDoc);
    }

    private void ParseFile(XmlDocument xmlData)
    {
        // there is only one map per map file, we'll parse this one
        XmlNodeList xmlNodeList = xmlData.SelectNodes("map");
        if (xmlNodeList != null) ParseMap(xmlNodeList.Item(0));
        else Debug.LogError("Unable to parse the map file, no <map> element found.");
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("TileMap{");
        foreach (TileSet tileset in _tileSets)
        {
            sb.Append(tileset);
        }
        foreach (TileLayer tilelayer in _tileLayers)
        {
            sb.Append(tilelayer);
        }
        sb.Append("}");
        return sb.ToString();
    }

    public TileSet GetTileSetByGid(int gid)
    {
        foreach (TileSet tileset in _tileSets.Where(tileset => tileset.Conains(gid)))
        {
            return tileset;
        }

        throw new NotSupportedException("Unknown tile gid, not found in any known TileSet.");
    }

    private void ParseMap(XmlNode mapNode)
    {
        XmlNodeList tilesetsXmlNodeList = mapNode.SelectNodes("tileset");
        if (tilesetsXmlNodeList != null) ParseTileSets(tilesetsXmlNodeList);
        else Debug.LogError("Unable to parse the map file, no <tileset> element found for <map>.");

        XmlNodeList tilellayerXmlNodeList = mapNode.SelectNodes("layer");
        if (tilellayerXmlNodeList != null) ParseTileLayers(tilellayerXmlNodeList);
        else Debug.LogError("Unable to parse the map file, no <layer> element found for <map>.");
        //Debug.Log("Parsed TileMap.");
    }

    private void ParseTileLayers(XmlNodeList tilelayersXmlNodeList)
    {
        foreach (XmlNode tileset in tilelayersXmlNodeList)
        {
            _tileLayers.Add(TileLayer.ParseTileLayer(tileset, this));
        }
        //Debug.Log("Parsed TileLayers.");
    }

    private void ParseTileSets(XmlNodeList tilesetsXmlNodeList)
    {
        foreach (XmlNode tileset in tilesetsXmlNodeList)
        {
            _tileSets.Add(TileSet.ParseTileSet(tileset));
        }
        //Debug.Log("Parsed TileSets.");
    }
}