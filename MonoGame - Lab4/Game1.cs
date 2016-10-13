using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.IO;
using System;
using System.Diagnostics;

namespace MonoGame___Lab4
{
   public class Game1 : Game
   {
      #region fields
      public static Random random = new Random();
      public static Model car, obs, rock;
      public static Vector2 MAPSIZE;
      public static Song sFX;
      public static SoundEffect accelerateSFX;
      public static SoundEffect hit;
      public static SoundEffect stop;
      public static Matrix rotation = Matrix.CreateRotationY(MathHelper.ToRadians(180));

      public static char[,] MapArry;

      public Point startP { get; set; }
      public Point endP { get; set; }

      private GraphicsDeviceManager graphics;
      private SpriteBatch spriteBatch;
      private Camera cam;
      private Character main;
      private TexturePlane plane, plane2;
      private Snowplow snowplow;
      private Spawner spawner;
      private Texture2D ground;
      private SpriteFont font;
      private Song bgm;
      #endregion
      public bool GameOver { get; set; }

      public Game1()
      {
         graphics = new GraphicsDeviceManager(this);
         Content.RootDirectory = "Content";
      }

      protected override void Initialize()
      {
         #region test
         //MapArry = new char[,]
         //{
         //   {'r','r','r','r','r','r','r','r','r','r','r','r','r','r'},
         //   {'r','-','-','-','-','-','-','-','-','-','-','-','-','r'},
         //   {'r','-','-','-','-','-','-','-','-','-','-','-','-','r'},
         //   {'r','-','-','-','-','-','-','-','-','-','-','-','-','r'},
         //   {'r','-','-','-','-','-','-','-','-','-','-','-','-','r'},
         //   {'r','-','-','-','-','-','-','-','-','-','-','-','-','r'},
         //   {'r','-','-','-','-','-','-','-','-','-','-','-','-','r'},
         //   {'r','-','r','-','-','-','X','-','-','-','-','r','-','r'},
         //   {'r','-','-','r','-','-','-','-','-','-','-','-','r','r'},
         //   {'r','-','-','-','r','r','-','-','r','r','-','-','-','r'},
         //   {'r','-','-','-','-','r','-','-','r','-','r','r','-','r'},
         //   {'r','-','-','-','-','r','-','-','-','-','-','-','-','r'},
         //   {'r','-','-','-','r','-','-','-','-','r','-','-','-','r'},
         //   {'r','-','-','-','-','-','-','-','-','-','-','-','-','r'},
         //   {'r','-','-','-','-','-','-','-','-','-','r','-','-','r'},
         //   {'r','-','-','-','-','-','-','r','r','-','r','-','-','r'},
         //   {'r','-','-','-','-','-','-','-','r','-','r','-','-','r'},
         //   {'r','-','-','-','-','-','-','r','r','-','r','-','-','r'},
         //   {'r','-','-','-','-','-','r','r','-','-','r','r','-','r'},
         //   {'r','-','-','-','-','r','r','-','-','-','-','r','-','r'},
         //   {'r','-','-','r','r','r','-','-','-','-','-','r','-','r'},
         //   {'r','-','r','r','r','-','-','-','r','-','-','r','-','r'},
         //   {'r','r','r','r','-','-','-','r','r','-','-','r','-','r'},
         //   {'r','-','r','-','-','-','-','-','-','-','-','r','-','r'},
         //   {'r','-','-','r','-','-','-','-','-','-','-','-','r','r'},
         //   {'r','-','-','-','r','r','-','-','r','r','-','-','-','r'},
         //   {'r','-','-','-','-','r','-','-','r','-','r','r','-','r'},
         //   {'r','-','-','-','-','r','-','-','r','-','-','-','-','r'},
         //   {'r','-','-','-','r','-','-','r','-','r','-','-','-','r'},
         //   {'r','-','r','-','r','-','-','-','-','-','-','r','-','r'},
         //   {'r','-','-','r','-','r','-','-','-','-','-','-','r','r'},
         //   {'r','-','-','-','r','r','-','-','r','r','-','-','-','r'},
         //   {'r','-','-','-','-','r','-','r','r','-','r','r','-','r'},
         //   {'r','-','-','-','-','r','-','r','r','-','-','-','-','r'},
         //   {'r','-','-','-','r','-','-','-','-','r','-','-','-','r'},
         //   {'r','-','r','-','-','-','-','r','-','-','-','r','-','r'},
         //   {'r','-','-','-','-','-','-','-','-','-','-','-','r','r'},
         //   {'r','-','-','-','-','r','-','r','r','r','-','-','-','r'},
         //   {'r','-','-','-','-','r','r','r','r','-','r','r','-','r'},
         //   {'r','-','-','-','-','r','-','r','r','-','-','-','-','r'},
         //   {'r','-','-','-','r','r','-','-','-','r','-','-','-','r'},
         //   {'r','-','r','-','-','r','-','-','-','-','-','r','-','r'},
         //   {'r','-','-','r','-','r','r','-','-','-','-','-','r','r'},
         //   {'r','-','-','-','r','r','r','-','r','r','-','-','-','r'},
         //   {'r','-','-','-','-','r','-','-','r','-','r','r','-','r'},
         //   {'r','-','-','-','-','r','-','-','r','-','-','-','-','r'},
         //   {'r','-','-','-','r','-','-','-','-','r','-','-','-','r'},
         //   {'r','-','r','-','-','-','-','r','-','-','-','r','-','r'},
         //   {'r','-','-','-','-','-','-','-','-','-','-','-','r','r'},
         //   {'r','-','-','-','-','-','-','-','r','r','-','-','-','r'},
         //   {'r','-','-','r','r','r','-','-','r','-','r','r','-','r'},
         //   {'r','-','-','r','r','r','r','-','r','-','-','-','-','r'},
         //   {'r','-','-','r','-','-','-','-','-','-','-','r','-','r'},
         //   {'r','-','-','r','-','-','-','-','-','-','-','-','r','r'},
         //   {'r','-','-','r','r','r','r','-','-','-','-','-','-','r'},
         //   {'r','r','-','-','-','r','r','r','r','-','-','r','-','r'},
         //   {'r','r','r','-','-','r','-','-','-','-','-','-','-','r'},
         //   {'r','r','-','-','r','r','-','-','-','-','-','-','-','r'},
         //   {'r','-','-','-','r','-','-','-','-','r','-','-','-','r'},
         //   {'r','-','-','-','r','-','-','-','r','r','-','-','-','r'},
         //   {'r','-','-','-','-','-','-','r','r','-','-','r','-','r'},
         //   {'r','r','-','-','-','-','S','-','-','r','-','-','r','r'},
         //   {'r','r','r','-','r','r','-','r','r','-','-','-','-','r'},
         //   {'r','-','r','-','-','r','-','r','-','-','-','r','-','r'},
         //   {'r','-','-','-','-','r','-','-','-','-','-','-','-','r'},
         //   {'r','-','-','-','r','-','-','-','-','r','-','-','-','r'},
         //   {'r','-','-','-','r','-','-','-','-','r','-','-','-','r'},
         //   {'r','-','-','r','-','-','-','-','r','r','r','-','-','r'},
         //   {'r','-','-','-','-','-','-','r','r','-','-','r','-','r'},
         //   {'r','-','-','-','-','-','S','-','-','r','-','-','r','r'},
         //   {'r','-','-','-','r','r','-','-','r','r','-','-','-','r'},
         //   {'r','-','-','-','-','r','-','r','r','-','r','r','-','r'},
         //   {'r','-','-','-','-','r','-','-','r','-','-','-','-','r'},
         //   {'r','-','-','-','r','-','-','-','-','r','-','-','-','r'},
         //   {'r','-','-','-','r','-','-','-','-','r','-','-','-','r'},
         //   {'r','-','-','r','-','-','-','-','r','r','r','-','-','r'},
         //   {'r','-','r','-','-','-','-','r','r','-','-','r','-','r'},
         //   {'r','r','-','-','-','-','S','-','-','r','-','-','r','r'},
         //   {'r','r','r','r','r','r','r','r','r','r','r','r','r','r'}
         //};
         #endregion
         SetMap("MapData");

         base.Initialize();
      }

      public void SetMap(string mapName)
      {
         string path = mapName + ".txt";

         using(StreamReader sr = new StreamReader(path))
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

                  MapArry = new char[(int)MAPSIZE.Y, (int)MAPSIZE.X];
                  for (int z = 0; z < MAPSIZE.Y; z++)
                     for (int x = 0; x < MAPSIZE.X; x++)
                     {
                        MapArry[z, x] = '.';
                     }
               }
               else
               {
                  int currentX = 0;
                  foreach(char c in line.ToCharArray())
                  {
                     MapArry[currentZ-1, currentX] = char.Parse(c.ToString());
                     currentX++;
                  }
               }
               currentZ++;
            }
         }
      }

      protected override void LoadContent()
      {
         spriteBatch = new SpriteBatch(GraphicsDevice);
         ground = Content.Load<Texture2D>("Textures/snow");
         car = Content.Load<Model>("Models/car");
         obs = Content.Load<Model>("Models/bullet");
         rock = Content.Load<Model>("Models/rock");
         sFX = Content.Load<Song>("Sounds/carExplodeSFX");
         accelerateSFX = Content.Load<SoundEffect>("Sounds/carAccelerateSFX");
         hit = Content.Load<SoundEffect>("Sounds/Hit");
         stop = Content.Load<SoundEffect>("Sounds/stop");
         bgm = Content.Load<Song>("Sounds/BGM");
         font = Content.Load<SpriteFont>("Fonts/Arial");

         //create new objects
         ResetGame();

         //var path = new PathFinding(MapArry);

         //Console.WriteLine("start: " + startP.Z + " End: " + endP.Z);
         //var parent = path.FindPath(startP, endP, false);

         //Console.WriteLine("Print path:" + MapArry[startP.Z, startP.X]);

         //while (parent != null)
         //{
         //   Console.WriteLine("ok");
         //   Console.WriteLine(parent.X + ", " + parent.Z);
         //   parent = parent.ParentPoint;
         //}
      }

      protected override void UnloadContent()
      {

      }

      protected override void Update(GameTime gameTime)
      {
         if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

         if (Keyboard.GetState().IsKeyDown(Keys.Enter) && GameOver)
            ResetGame();

         if (!GameOver)
         {
            foreach (var ob in spawner.Obstacles)
            ob.Update(gameTime);
            cam.Update(main.Position);
            main.Update(gameTime);
            plane.Update(main.Position);
            snowplow.Update(gameTime,spawner);
            //plane2.Update(main.Position);
            //spawner.Update(gameTime);
         }
         base.Update(gameTime);
      }

      private void ResetGame()
      {

         main = new Character(this, car, new Vector3(0, 0, 4), 5f, 0.02f, rotation);
         plane = new TexturePlane(GraphicsDevice, ground, MAPSIZE, Matrix.Identity, 1);
         //plane2 = new TexturePlane(GraphicsDevice, ground, MAPSIZE, Matrix.CreateTranslation(new Vector3(0, 0, MAPSIZE.Y)), 2);
         cam = new Camera(this, new Vector3(0f, 15f, 12f), Vector3.Zero, 10);
         spawner = new Spawner(this, main);
         spawner.Generate(MapArry,
         2,
         2);

        
         snowplow = new Snowplow(this, car, startP.toVector3(), endP.toVector3(), 3f, 0.02f, Matrix.Identity, main);
         GameOver = false;
         MediaPlayer.Volume = 0.6f;
         MediaPlayer.Play(bgm);
         MediaPlayer.IsRepeating = true;
         SoundEffect.MasterVolume = 0.05f;
      }

      protected override void Draw(GameTime gameTime)
      {
         GraphicsDevice.Clear(Color.WhiteSmoke);

         foreach (var ob in spawner.Obstacles)
         ob.Draw(cam);
         main.Draw(cam);
         plane.Draw(cam);
         snowplow.Draw(cam);
         //plane2.Draw(cam);

         spriteBatch.Begin();
         spriteBatch.DrawString(font, "Distance: " + main.Position.Z.ToString().Split('.')[0] + "M", new Vector2(10, 10), Color.Black, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
         spriteBatch.DrawString(font, "Durability: " + main.Life.ToString().Split('.')[0] + "%", new Vector2(650, 10), Color.Black, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
         spriteBatch.End();

         GraphicsDevice.BlendState = BlendState.Opaque;
         GraphicsDevice.DepthStencilState = DepthStencilState.Default;
         GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

         base.Draw(gameTime);
      }
   }
}
