using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace MonoGame___Lab4
{
   class Spawner
   {
      #region fields
      private float timeCount,snowCount;
      private Character target;
      private Game1 game;
      #endregion

      public List<Obstacles> Obstacles { get; set; }
      public List<Snowplow> Snowplows { get; set; }

      public Spawner(Game1 game, Character target)
      {
         this.target = target;
         this.game = game;

        Obstacles = new List<Obstacles>();
        Snowplows = new List<Snowplow>();
      }

      //make camera follow main character
      public void Update(GameTime gameTime)
      {
         float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
         timeCount += dt;
         snowCount += dt;

         if (timeCount > 0.5f && target.Position.Z < MapData.MAPSIZE.Y - 30)
         {  
            Instantiate(Game1.obs, target.Position, new Vector2(3, 5), new Vector2(20, 40), new Vector2(1, 2),
               new Vector2(180, 180), new Vector2(0.2f, 0.2f), ModelType.bullet);
            timeCount = 0; //reset timer
         }else if(snowCount > 1f && target.Position.Z < MapData.MAPSIZE.Y - 30)
         {
            var iniX = (float)Game1.random.Next(-(int)MapData.MAPSIZE.X, (int)MapData.MAPSIZE.X);
            var iniZ = (target.Position.Z + 30);
            if (MapData.MAPARRAY[iniZ.toPointY(),iniX.toPointX()] == '.')
            {
               Snowplows.Add(new Snowplow(game, Game1.car, new Vector3(iniX, 0,iniZ), 4f, 0.02f, Matrix.Identity, target));
            }
            snowCount = 0;
         }
         RemoveOutOfRange();  //remove bullets out of range
      }

      public void InstantiateStatic(
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
          //remove bullets out of range
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
            var randistanceX = Game1.random.Next((int)-MapData.MAPSIZE.X, (int)MapData.MAPSIZE.X);
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

         for(int i = 0; i< Snowplows.Count; i++)
         {
            if (target.Position.Z - 10 > Snowplows[i].Position.Z)
            {
               Snowplows.RemoveAt(i);
            }
         }
      }
   }
}
