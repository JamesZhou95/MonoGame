using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace MonoGame___Lab4
{
   class Spawner
   {
      private float timeCount;
      private Character target;
      private Game1 game;

      public List<Obstacles> Obstacles { get; set; }

      public Spawner(Game1 game, Character target)
      {
         this.target = target;
         this.game = game;

        Obstacles = new List<Obstacles>();
      }

      //make camera follow main character
      public void Update(GameTime gameTime)
      {
         //float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
         //timeCount += dt; //start count

         //if (timeCount > 0.5f)
         //{   //bullet respwan rate 
         //   Instantiate(Game1.obs, target.Position, new Vector2(3, 5), new Vector2(20, 40), new Vector2(1, 2),
         //      new Vector2(180, 180), new Vector2(0.2f, 0.2f), ModelType.bullet);
         //   Instantiate(Game1.rock, target.Position, new Vector2(0, 4), Vector2.Zero, Vector2.Zero,
         //      new Vector2(0, 180), new Vector2(0.5f, 1.5f), ModelType.rock);
         //   timeCount = 0; //reset timer
         //}
         //RemoveOutOfRange();  //remove bullets out of range
      }

      public void Generate(char[,] map, int sizeX, int sizeZ)
      {
         float xMin = -Game1.MAPSIZE.X * 0.95f;
         float xMax = Game1.MAPSIZE.X * 0.95f;
         float zMax = Game1.MAPSIZE.Y * 0.95f;
         for (int z = 0; z < map.GetLength(0); z++)
            for (int x = 0; x < map.GetLength(1); x++)
            {
               char number = map[z, x];

               if (number.Equals('r'))
               {
                  Instantiate2(Game1.rock, new Vector3(x * sizeX - xMax, 0, z * sizeZ), new Vector2(0, 180), new Vector2(0.5f, 1f), ModelType.rock);
               }
               else if (number.Equals('X'))
               {
                  Console.WriteLine("found X");
                  game.endP = new Point(x, z);
               }
               else if (number.Equals('S'))
               {
                  Console.WriteLine("found S");
                  game.startP = new Point(x, z);
               }
            }
      }

      private void Instantiate2(
         Model model,
         Vector3 iniPos,
         Vector2 rotate,
         Vector2 scale,
         ModelType type)
      {
         var randScale = Game1.random.NextDouble() * (scale.Y - scale.X) + scale.X;
         var randRotate = Game1.random.Next((int)rotate.X, (int)rotate.Y);
         //instantiate bullets
         Obstacles.Add(new Obstacles(game, type, model, iniPos, 0, (float)randScale,
            Matrix.CreateRotationY(MathHelper.ToRadians(randRotate)), target));
        RemoveOutOfRange();  //remove bullets out of range
      }

      private void Instantiate(
         Model model,
         Vector3 targetPos,
         Vector2 spawnNum,
         Vector2 moveSpeed,
         Vector2 height,
         Vector2 rotate,
         Vector2 scale,
         ModelType type)
      {
         var randRate = Game1.random.Next((int)spawnNum.X, (int)spawnNum.Y);

         for (int i = 0; i < randRate; i++)
         {
            var randistanceX = Game1.random.Next((int)-Game1.MAPSIZE.X, (int)Game1.MAPSIZE.X);
            var randistanceY = Game1.random.Next((int)height.X, (int)height.Y);
            var randistanceZ = Game1.random.Next(25, 40);
            var randSpeed = Game1.random.Next((int)moveSpeed.X, (int)moveSpeed.Y);
            var iniPos = new Vector3(randistanceX, randistanceY, targetPos.Z + randistanceZ);
            var randScale = Game1.random.NextDouble() * (scale.Y - scale.X) + scale.X;
            var randRotate = Game1.random.Next((int)rotate.X, (int)rotate.Y);
            //instantiate bullets
            Obstacles.Add(new Obstacles(game, type, model, iniPos, randSpeed, (float)randScale,
               Matrix.CreateRotationY(MathHelper.ToRadians(randRotate)), target));
         }
      }

      private void RemoveOutOfRange()
      {
         for (int i = 0; i < Obstacles.Count; i++)
         {
            if (target.Position.Z - 15 > Obstacles[i].Position.Z)
            {
               Obstacles.RemoveAt(i);
            }
         }
      }
   }
}
