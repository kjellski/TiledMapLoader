using System.Collections.Generic;
using UnityEngine;

public class SpriteLoader
{
    private static readonly object SingletonLock = new object();
    private static volatile SpriteLoader _singleton;
    private readonly Dictionary<string, Sprite[]> _cache = new Dictionary<string, Sprite[]>();
    private readonly object _cacheLock = new object();

    private SpriteLoader()
    {
    }

    public static SpriteLoader GetInstance()
    {
        if (_singleton != null) return _singleton;

        lock (SingletonLock)
        {
            if (_singleton == null)
            {
                _singleton = new SpriteLoader();
            }
        }

        return _singleton;
    }

    public Sprite GetSprite(string terrain, int i)
    {
        Sprite tmp = LoadSprites(terrain)[i];
        return tmp;
    }

    private Sprite[] LoadSprites(string textureName)
    {
        if (!_cache.ContainsKey(textureName))
        {
            lock (_cacheLock)
            {
                if (!_cache.ContainsKey(textureName))
                    _cache[textureName] = LoadSpritesFromTexture(textureName);

                return _cache[textureName];
            }
        }
        return _cache[textureName];
    }

    private Sprite[] LoadSpritesFromTexture(string textureName)
    {
        return Resources.LoadAll<Sprite>(textureName);
    }
}