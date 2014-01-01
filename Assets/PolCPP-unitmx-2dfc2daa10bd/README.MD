UniTMX: A TMX importer for Unity3d
=======================================

UniTMX is a TMX file importer to load files from [tiled map editor] into Unity originally developed for [Twisted Potions: Double Trouble]. 

It does not support all the features from tiled but it may work fine for most of cases.

Features
--------

   - Imports TMX Files into a mesh
   - Supports extruded/padded sprites (see Example Assets/ExTexturePadEx.png / ExMapPadEx.tmx for an example)
   - Loads (optionally) mesh collisions into the same gameobject, or a different one.


Limitations
-----------

  - Only works with orthogonal sprites
  - You can only use one tile sheet per tmx file.


Installation
------------

   - Put this repo into your assets folder.


Usage
-----

### In Tiled Map editor:

#### Tiles

   - Load your tilemap
   - Paint some tiles in different layers

*Avoid naming your tilemap or layer Collision or collision_ as it will cause problems with the collision layers*

#### Collisions:
   - Create a new Tileset named Collisions **you can use Collision.png from the example assets**
   - Right click on each of the tiles you want to configure for collision and click tile properties.
   - Set a property named col with [one of the collision values like in this image] 
     - **Collision Values:** top,bottom,left,right. *Only use one of them per tile* 
   - Create a layer called collision\_1 (or collision\_N) and paint your collision tiles there.

#### Finally

   - Save. **Remember to set the tmx tile layer data format [as CSV like in this image]**
   - Rename the tmx file as xml.
   - To make this easier, you can use Tiled's "Edit Commands..." feature to add this command "cp %mapfile %mapfile.xml"
     Just make sure to check the "Save map before executing" check box.  Now each time you press the command button, it
     will save your map and make a copy with .xml at the end.

### Unity

   - Setup your 2d Scene for 2d sprites (orthographic camera etc), if you want take a look on how is the scene built on [Twisted potions: Double trouble].
   - Create and empty GameObject.
   - Now click on *Component*->*Scripts*->*TileMap*
   - On the GameObject inspector you'll [find a new set of options that look like this] 
   - Select the tilemap XML that you created earlier on tiled. And hit Import tiles or
     Generate colliders according to your needs.
   - Finally, remember to add a material with the tilemap texture to that GameObject, so it 
     displays correctly it's contents

*There are also some example assets that you can check if you have any doubts*

*If you want to extend the collision formats check the Code/Tileset.cs instructions*

*In case you want to run the importer from inside the game it's also possible, check how the Editor/TileMapEditor.cs is works, but be aware that the script is not optimized so you may have troubles with the GC*

  [tiled map editor]: http://mapeditor.org/
  [twisted potions: double trouble]: https://bitbucket.org/PolCPP/twisted-potions-double-trouble/
  [as csv like in this image]: https://dl.dropbox.com/u/69084784/tiledConfig.png
  [one of the collision values like in this image]: https://dl.dropbox.com/u/69084784/tiledConfig2.png
  [find a new set of options that look like this]: https://dl.dropbox.com/u/69084784/unityconfig.png 
