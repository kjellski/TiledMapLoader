using System.Xml;
using UnityEngine;
using System.Collections;

public class Tile //: MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public Texture2D TileTexture { get; private set; }
    public int Gid { get; private set; }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override string ToString()
    {
        return "Tile{ X: " + X + ", Y: " + Y + " }";
    }

    /// <summary>
    /// Private to prevent instantiation
    /// </summary>
    private Tile() { }

    public Tile(int x, int y, Texture2D tileTexture, int gid)
    {
        X = x;
        Y = y;
        TileTexture = tileTexture;
        Gid = gid;
    }

    public static Tile ParseTile(XmlNode tileXmlNode, TileMap tileMap, int x, int y)
    {
        var gid = int.Parse(tileXmlNode.Attributes["gid"].Value);
        var texture = tileMap.GetTileSetByGid(gid).GetTilesTexture(gid);

        return new Tile(x, y, texture, gid);
    }
}
