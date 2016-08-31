using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace MonoGame___Lab4 {
   public class Game1 : Game {
      public static Random random = new Random();
      public static int MAPSIZE = 40;
      private static Matrix rotation = Matrix.CreateRotationY(MathHelper.ToRadians(180));
      private GraphicsDeviceManager graphics;
      private SpriteBatch spriteBatch;
      private Camera cam;
      private Character main,test;
      private TexturePlane plane,plane2;
      private List<Character> obstacles = new List<Character>();
      private Model bullet;
      private float timeCount;

      public Game1() {
         graphics = new GraphicsDeviceManager(this);
         Content.RootDirectory = "Content";
      }

      protected override void Initialize() {
         base.Initialize();
      }

      protected override void LoadContent() {
         spriteBatch = new SpriteBatch(GraphicsDevice);
         var texture = Content.Load<Texture2D>("grass");
         var model = Content.Load<Model>("car2");
         bullet = Content.Load<Model>("bullet");

         //create new objects,main character,2 planes and 1 cam.
         main = new Character(this,model, Vector3.Zero,10f,0.02f,rotation, ModelType.main);
         plane = new TexturePlane(GraphicsDevice, texture, MAPSIZE, Matrix.Identity,1);
         plane2 = new TexturePlane(GraphicsDevice, texture, MAPSIZE, Matrix.CreateTranslation(new Vector3(0,0,MAPSIZE)),2);
         cam = new Camera(this, new Vector3(0f, 15f, -15f), Vector3.Zero, 10);
      }

      protected override void UnloadContent() {

      }

      protected override void Update(GameTime gameTime) {
         //get frame rate and count time
         float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
         timeCount += dt;

         if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

         //bullet respwan rate 
         if (timeCount > 0.5) {
            Spawner(bullet, main.Position, rotation,new Vector2(3,5));
            timeCount = 0; //reset timer
         }

         //moving bullets
         foreach (var ob in obstacles) {
            ob.Update(gameTime);
         }

         RemoveOutOfRange(); //remove bullets out of range

         cam.Update(main.Position);
         main.Update(gameTime);
         plane.Update(main.Position);
         plane2.Update(main.Position);
         base.Update(gameTime);
      }

      protected override void Draw(GameTime gameTime) {
         GraphicsDevice.Clear(Color.CornflowerBlue);
         plane.Draw(cam);
         plane2.Draw(cam);
         main.Draw(cam);
         foreach(var ob in obstacles) {
            ob.Draw(cam);
         }
         
         base.Draw(gameTime);
      }

      //obstacles Spwaner
      private void Spawner(Model model,Vector3 targetPos,Matrix rotation,Vector2 rate) {
         var randRate = random.Next((int)rate.X, (int)rate.Y);
         
         for (int i = 0; i < randRate; i++) {
            var randistanceX = random.Next(-20, 20);
            var randistanceY = random.Next(1, 2);
            var randistanceZ = random.Next(25, 40);
            var randSpeed = random.Next(20, 40);
            var iniPos = new Vector3(targetPos.X + randistanceX, randistanceY, targetPos.Z + randistanceZ);
            //instantiate bullets
            obstacles.Add(new Character(this, model, iniPos, randSpeed, 0.2f, rotation, ModelType.obstacle));
         }
      }

      //remove out of range bullets(behind the main character)
      public void RemoveOutOfRange() {
         for(int i = 0; i<obstacles.Count-1;i++){
            if (main.Position.Z - 15 > obstacles[i].Position.Z) {            
               obstacles.RemoveAt(i);
            }
         }
      }
   }
}
