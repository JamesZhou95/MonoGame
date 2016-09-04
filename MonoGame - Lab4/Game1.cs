﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace MonoGame___Lab4 {
   public class Game1 : Game {
      public static Random random = new Random();
      public readonly static int MAPSIZE = 20;
      private static Matrix rotation = Matrix.CreateRotationY(MathHelper.ToRadians(180));
      private bool gameOver;
      private GraphicsDeviceManager graphics;
      private SpriteBatch spriteBatch;
      private Camera cam;
      private Character main;
      private TexturePlane plane, plane2;
      private Spawner spawner;

      public Game1() {
         graphics = new GraphicsDeviceManager(this);
         Content.RootDirectory = "Content";
      }

      public bool GameOver { get { return gameOver; } set { gameOver = value; } }

      protected override void Initialize() {
         base.Initialize();
      }

      protected override void LoadContent() {
         spriteBatch = new SpriteBatch(GraphicsDevice);
         var texture = Content.Load<Texture2D>("grass");
         var model = Content.Load<Model>("car2");
         var bullet = Content.Load<Model>("bullet");

         //create new objects
         main = new Character(this,model, Vector3.Zero,12f,0.02f,rotation, InputType.type1);
         plane = new TexturePlane(GraphicsDevice, texture, MAPSIZE, Matrix.Identity,1);
         plane2 = new TexturePlane(GraphicsDevice, texture, MAPSIZE, Matrix.CreateTranslation(new Vector3(0,0,MAPSIZE)),2);
         cam = new Camera(this, new Vector3(0f, 15f, 12f), Vector3.Zero, 10);
         spawner = new Spawner(this, main, bullet, rotation, new Vector2(3, 5));
      }

      protected override void UnloadContent() {

      }

      protected override void Update(GameTime gameTime) {
         if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

         if (!gameOver) {
            foreach (var ob in spawner.Obstacles)
               ob.Update(gameTime);
            cam.Update(main.Position);
            main.Update(gameTime);
            plane.Update(main.Position);
            plane2.Update(main.Position);
            spawner.Update(gameTime);
            base.Update(gameTime);
         }
      }

      protected override void Draw(GameTime gameTime) {
         GraphicsDevice.Clear(Color.BlanchedAlmond);
      
         foreach (var ob in spawner.Obstacles)
            ob.Draw(cam);        
         main.Draw(cam);
         plane.Draw(cam);
         plane2.Draw(cam);
         base.Draw(gameTime);
      }
   }
}
