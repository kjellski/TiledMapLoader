using System.Text;
using System.Xml;
using UnityEngine;
using System.Collections;
using Debug = System.Diagnostics.Debug;

public class TileLayer // : MonoBehaviour
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public string Name { get; private set; }
    public Tile[,] Tiles { get; private set; }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// private to prevent instantiation
    /// </summary>
    private TileLayer() { }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("TileLazer{");
        sb.Append(" Name: " + Name);
        sb.Append(", Width: " + Width);
        sb.Append(", Height: " + Height);
        sb.Append(", Data: [");
        for (int z = 0; z < Height; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                sb.Append(Tiles[x,z]);
            }
        }   
        sb.Append("] ");
        sb.Append("}");
        return sb.ToString();
    }

    public TileLayer(int width, int height, string name, Tile[,] data)
    {
        Width = width;
        Height = height;
        Name = name;
        Tiles = data;
    }

    /// <summary>
    /// Parses the example xml element structure to an object of this type:
    /// <note>
    ///     <layer name="background" width="20" height="20">
    ///         <data>
    ///             <tile gid="1"/>
    ///             ...
    ///         </data>
    ///     </layer>
    /// </note>
    /// </summary>
    /// <param name="tileLayerXmlNode"></param>
    /// <param name="map"></param>
    /// <returns></returns>
    public static TileLayer ParseTileLayer(XmlNode tileLayerXmlNode, TileMap map)
    {
        int widht = int.Parse(tileLayerXmlNode.Attributes["width"].Value);
        int height = int.Parse(tileLayerXmlNode.Attributes["height"].Value);
        string name = tileLayerXmlNode.Attributes["name"].Value;
        Tile[,] data = ParseTiles(tileLayerXmlNode.SelectNodes("data/tile"), map, widht, height);

        return new TileLayer(widht, height, name, data);
    }

    public static Tile[,] ParseTiles(XmlNodeList tilesXmlNodeList, TileMap map, int width, int height)
    {
        var tiles = new Tile[width, height];

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                tiles[x, z] = Tile.ParseTile(tilesXmlNodeList.Item(x * width + z), map, x, z);
            }
        }

        return tiles;
    }
}
