﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace MonoGame___Lab4 {
   public class Game1 : Game {
      public static Random random = new Random();
      public readonly static int MAPSIZE = 20;
      public static Matrix rotation = Matrix.CreateRotationY(MathHelper.ToRadians(180));
      private bool gameOver;
      private GraphicsDeviceManager graphics;
      private SpriteBatch spriteBatch;
      private Camera cam;
      private Character main;
      private TexturePlane plane, plane2;
      private Spawner spawner;
      private Texture2D ground;
      private SpriteFont font;
      public static Model car, obs,rock;
      private Song bgm;
      public static Song sFX;
      public static SoundEffect accelerateSFX;

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
         ground = Content.Load<Texture2D>("snow");
         car = Content.Load<Model>("car2");
         obs = Content.Load<Model>("bullet");
         rock = Content.Load<Model>("rock");
         sFX = Content.Load<Song>("Sounds/carExplodeSFX");
         accelerateSFX = Content.Load<SoundEffect>("Sounds/carAccelerateSFX");
         bgm = Content.Load<Song>("Sounds/BGM");
         font = Content.Load<SpriteFont>("Fonts/Arial");

         //create new objects
         ResetGame();
      }

      protected override void UnloadContent() {

      }

      protected override void Update(GameTime gameTime) {
         if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

         if (Keyboard.GetState().IsKeyDown(Keys.Enter) && gameOver)
            ResetGame();
            
         if (!gameOver) {
            foreach (var ob in spawner.Obstacles)
               ob.Update(gameTime);
            cam.Update(main.Position);
            main.Update(gameTime);
            plane.Update(main.Position);
            plane2.Update(main.Position);
            spawner.Update(gameTime);
         }
         base.Update(gameTime);
      }

      private void ResetGame() {
         main = new Character(this, car, Vector3.Zero, 12f, 0.02f, rotation);
         plane = new TexturePlane(GraphicsDevice, ground, MAPSIZE, Matrix.Identity, 1);
         plane2 = new TexturePlane(GraphicsDevice, ground, MAPSIZE, Matrix.CreateTranslation(new Vector3(0, 0, MAPSIZE)), 2);
         cam = new Camera(this, new Vector3(0f, 15f, 12f), Vector3.Zero, 10);
         spawner = new Spawner(this, main);
         gameOver = false;
         MediaPlayer.Volume = 0.6f;
         MediaPlayer.Play(bgm);
         MediaPlayer.IsRepeating = true;
         SoundEffect.MasterVolume = 0.05f;
      }

      protected override void Draw(GameTime gameTime) {
         GraphicsDevice.Clear(Color.WhiteSmoke);

         foreach (var ob in spawner.Obstacles)
            ob.Draw(cam);        
         main.Draw(cam);
         plane.Draw(cam);
         plane2.Draw(cam);
         
         spriteBatch.Begin();
         spriteBatch.DrawString(font, "Distance: "+ main.Position.Z.ToString().Split('.')[0] + "M", new Vector2(10, 10), Color.Black,0f,Vector2.Zero,0.5f,SpriteEffects.None,0f);
         spriteBatch.End();

         GraphicsDevice.BlendState = BlendState.Opaque;
         GraphicsDevice.DepthStencilState = DepthStencilState.Default;
         GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

         base.Draw(gameTime);
      }
   }
}
