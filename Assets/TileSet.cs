using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using System.Xml;
using UnityEngine;

public class TileSet 
{
    public int FirstGid { get; private set; }

    public Texture2D Texture { get; private set; }

    public int ImageHeight { get; private set; }
    public int ImageWidth { get; private set; }
    public String Name { get; private set; }
    public int TileHeight { get; private set; }
    public int TileWidth { get; private set; }

    private readonly Dictionary<int, Texture2D> _textureParts = new Dictionary<int, Texture2D>();

    public TileSet(int firstgid,
        Texture2D texture,
        int imageheight,
        int imagewidth,
        String name,
        int tileheight,
        int tilewidth)
    {
        FirstGid = firstgid;
        Texture = texture;
        ImageHeight = imageheight;
        ImageWidth = imagewidth;
        TileHeight = tileheight;
        Name = name;
        TileWidth = tilewidth;
        Debug.Log(this);
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("TileSet{");
        sb.Append(" FirstGid: " + FirstGid);
        sb.Append(", UvRect: " + Texture.GetHashCode());
        sb.Append(", ImageHeight: " + ImageHeight);
        sb.Append(", ImageWidth: " + ImageWidth);
        sb.Append(", Name: " + Name);
        sb.Append(", TileHeight: " + TileHeight);
        sb.Append(", TileWidth: " + TileWidth);
        sb.Append("}");
        return sb.ToString();
    }

    public Texture2D GetTilesTexture(int gid)
    {
        if (_textureParts.ContainsKey(gid))
            return _textureParts[gid];

        int tileNumber = gid - 1;
        int x = (tileNumber % TileWidth);
        var y = TileHeight - 1 - (int)Math.Floor((double)(tileNumber / TileWidth));
        
        var texture = new Texture2D(TileWidth, TileHeight);
        texture.SetPixels(Texture.GetPixels(x, y, TileWidth, TileHeight));
        texture.Apply();

        _textureParts[gid] = texture;

        Debug.Log("GetTilesRect(" + gid + " => " + x + ", " + y + ") => " + texture);
        return texture;
    }

    public bool Conains(int gid)
    {
        int upper = (ImageHeight / TileHeight) * (ImageHeight / TileHeight);
        int lower = FirstGid;
        return gid <= upper && gid >= lower;
    }

    public static TileSet ParseTileSet(XmlNode tilesetXmlNode)
    {
        // read the tileset node itself
        int firstgid = int.Parse(tilesetXmlNode.Attributes["firstgid"].Value);
        string name = tilesetXmlNode.Attributes["name"].Value;
        int tileheight = int.Parse(tilesetXmlNode.Attributes["tileheight"].Value);
        int tilewidth = int.Parse(tilesetXmlNode.Attributes["tileheight"].Value);

        // read the image node
        XmlNode imageXmlNode = tilesetXmlNode.SelectNodes("image").Item(0);
        int imageheight = GetImageHeight(imageXmlNode);
        int imagewidth = GetImageWidth(imageXmlNode);
        Texture2D texture = ParseImage(imageXmlNode);

        return new TileSet(firstgid, texture, imageheight, imagewidth, name, tileheight, tilewidth);
    }

    private static Texture2D ParseImage(XmlNode imageXmlNode)
    {
        string path = imageXmlNode.Attributes["source"].Value;
        path = "file://" + Application.dataPath + "/Resources/" + path;
        //Debug.Log(path);
        var www = new WWW(path);

        return www.texture;
    }

    private static int GetImageHeight(XmlNode imageXmlNode)
    {
        return int.Parse(imageXmlNode.Attributes["height"].Value);
    }

    private static int GetImageWidth(XmlNode imageXmlNode)
    {
        return int.Parse(imageXmlNode.Attributes["width"].Value);
    }
}