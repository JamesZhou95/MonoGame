using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace MonoGame___Lab4 {
   class Spawner {
      private float timeCount;
      private List<Obstacles> obstacles = new List<Obstacles>();
      private Character target;
      private Model model;
      private Matrix rotation;
      private Game1 game;

      public List<Obstacles> Obstacles {
         get { return obstacles; }
         set { obstacles = value; }
      }

      public Spawner(Game1 game, Character target, Model model,Matrix rotation)  {
         this.target = target;
         this.model = model;
         this.rotation = rotation;
         this.game = game;
      }

      //make camera follow main character
      public void Update(GameTime gameTime) {
         float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
         timeCount += dt; //start count
         
         if (timeCount > 0.5f) {   //bullet respwan rate 
            Instantiate(model,0.2f, rotation, new Vector2(3,5),new Vector2(20,40),new Vector2(1,2));
            //Instantiate(Game1.rock,1f, rotation, new Vector2(1,3),Vector2.Zero,Vector2.Zero);
            timeCount = 0; //reset timer
         }
      
         RemoveOutOfRange();  //remove bullets out of range
      }

      private void Instantiate(Model model, float scale, Matrix rotation, Vector2 rate,Vector2 speed,Vector2 height) {
         var randRate = Game1.random.Next((int)rate.X, (int)rate.Y);

         for (int i = 0; i < randRate; i++) {
            var randistanceX = Game1.random.Next(-Game1.MAPSIZE, Game1.MAPSIZE);
            var randistanceY = Game1.random.Next((int)height.X, (int)height.Y);
            var randistanceZ = Game1.random.Next(25, 40);
            var randSpeed = Game1.random.Next((int)speed.X, (int)speed.Y);
            var iniPos = new Vector3(randistanceX, randistanceY, target.Position.Z + randistanceZ);
            //instantiate obstacles
            Obstacles.Add(new Obstacles(game, model, iniPos, randSpeed, scale, rotation, target));
         }
      }

      private void RemoveOutOfRange() {
         for (int i = 0; i < obstacles.Count; i++) {
            if (target.Position.Z - 15 > obstacles[i].Position.Z) {
               Obstacles.RemoveAt(i);
            }
         }
      }
   }
}
