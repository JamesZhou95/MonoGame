using System.IO;
using Microsoft.Xna.Framework;

namespace MonoGame___Lab4
{
   class MapData
   {
      public static Vector2 MAPSIZE;
      public static char[,] MAPARRAY;

      private Spawner sp;

      public MapData(Game1 game, string mapName, Character main)
      {
         sp = new Spawner(game, main);
         SetMap(mapName);
         Generate(MAPARRAY, 2, 2);
      }

      private void SetMap(string mapName)
      {
         string path = mapName + ".txt";

         using (StreamReader sr = new StreamReader(path))
         {
            int currentZ = 0;
            while (sr.Peek() >= 0)
            {
               string line = sr.ReadLine();

               if (currentZ == 0)
               {
                  string[] deminsions = line.Split(',');
                  MAPSIZE.X = int.Parse(deminsions[0]);
                  MAPSIZE.Y = int.Parse(deminsions[1]);

                  MAPARRAY = new char[(int)MAPSIZE.Y, (int)MAPSIZE.X];
                  for (int z = 0; z < MAPSIZE.Y; z++)
                     for (int x = 0; x < MAPSIZE.X; x++)
                     {
                        MAPARRAY[z, x] = '.';
                     }
               }
               else
               {
                  int currentX = 0;
                  foreach (char c in line.ToCharArray())
                  {
                     MAPARRAY[currentZ - 1, currentX] = char.Parse(c.ToString());
                     currentX++;
                  }
               }
               currentZ++;
            }
         }
      }

      //Gnerate the static Map
      private void Generate(char[,] map, int sizeX, int sizeZ)
      {
         float xMin = -MAPSIZE.X * 0.95f;
         float xMax = MAPSIZE.X * 0.95f;
         float zMax = MAPSIZE.Y * 0.95f;
         for (int z = 0; z < map.GetLength(0); z++)
            for (int x = 0; x < map.GetLength(1); x++)
            {
               char number = map[z, x];

               if (number.Equals('R'))
               {
                  sp.InstantiateStatic(Game1.rock, new Vector3(x.toWorldX(), 0, z.toWorldZ()), new Vector2(0, 180), new Vector2(0.5f, 1f), ModelType.rock);
               }
            }
      }

      public void Update(GameTime gameTime)
      {
         sp.Update(gameTime);
         foreach (var ob in sp.Obstacles)
            ob.Update(gameTime);
         foreach (var ob in sp.Snowplows)
            ob.Update(gameTime);
      }

      public void Draw(Camera cam)
      {
         foreach (var ob in sp.Obstacles)
            ob.Draw(cam);
         foreach (var ob in sp.Snowplows)
            ob.Draw(cam);
      }
   }
}
