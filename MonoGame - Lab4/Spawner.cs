using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonoGame___Lab4 {
   class Spawner {
      private float timeCount;
      private List<Obstacles> obstacles = new List<Obstacles>();
      private Character target;
      private Game1 game;

      public List<Obstacles> Obstacles {
         get { return obstacles; }
         set { obstacles = value; }
      }

      public Spawner(Game1 game, Character target)  {
         this.target = target;
         this.game = game;
      }

      //make camera follow main character
      public void Update(GameTime gameTime) {
         float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
         timeCount += dt; //start count
         
         if (timeCount > 0.5f) {   //bullet respwan rate 
            Instantiate(Game1.obs, target.Position, new Vector2(3,5),new Vector2(20,40),new Vector2(1,2),0.2f,ModelType.bullet);
            Instantiate(Game1.rock,target.Position, new Vector2(0, 4), Vector2.Zero, Vector2.Zero, 1f, ModelType.rock);
            timeCount = 0; //reset timer
         }
      
         RemoveOutOfRange();  //remove bullets out of range
      }

      private void Instantiate(
         Model model, 
         Vector3 targetPos, 
         Vector2 spawnNum,
         Vector2 moveSpeed, 
         Vector2 height, 
         float scale,
         ModelType type) {
         var randRate = Game1.random.Next((int)spawnNum.X, (int)spawnNum.Y);

         for (int i = 0; i < randRate; i++) {
            var randistanceX = Game1.random.Next(-Game1.MAPSIZE, Game1.MAPSIZE);
            var randistanceY = Game1.random.Next((int)height.X, (int)height.Y);
            var randistanceZ = Game1.random.Next(25,40);
            var randSpeed = Game1.random.Next((int)moveSpeed.X, (int)moveSpeed.Y);
            var iniPos = new Vector3(randistanceX, randistanceY, targetPos.Z + randistanceZ);
            //instantiate bullets
            Obstacles.Add(new Obstacles(game, type,model, iniPos, randSpeed, scale, Game1.rotation, target));
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
