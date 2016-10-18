using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace MonoGame___Lab4
{
   class Character : GameComponent
   {
      #region fields
      private Model objModel;
      private Matrix worldMatrix, orientation;
      private Vector3 position;
      private Vector3 rotation;
      private Vector3 lookAt;
      private float scaleSize;
      private float moveSpeed, originalSpeed;
      private float wheelRotate;
      private BoundingBox collider;
      private Game1 game;
      private Vector2 mapSize;
      #endregion

      public int Life { get; set; }

      public Vector3 Position
      {
         get { return position; }
         set
         {
            position = value;
         }
      }

      public Vector3 Rotation
      {
         get { return rotation; }
         set
         {
            rotation = value;
         }
      }

      //create new character/obstacles
      public Character(Game1 game, Model model, Vector3 pos, float speed, float scale, Matrix world) : base(game)
      {
         objModel = model;
         position = pos;
         orientation = world;
         moveSpeed = speed;
         scaleSize = scale;
         originalSpeed = speed;
         Life = 100;
         wheelRotate = 0f;
         mapSize = MapData.MAPSIZE;
         this.game = game;
      }

      //move character to position
      private void MoveTo(Vector3 pos, Vector3 rot)
      {
         Position = pos;
         Rotation = rot;
      }

      //Preview position to detect collision/boundary
      private Vector3 PreviewMove(Vector3 amount)
      {
         Matrix rotate = Matrix.CreateRotationY(rotation.Y);
         Vector3 movement = new Vector3(amount.X, amount.Y, amount.Z);
         movement = Vector3.Transform(movement, rotate);
         var previewPos = position + movement;
         previewPos.X = MathHelper.Clamp(previewPos.X, -mapSize.X * 0.9f, mapSize.X * 0.9f);
         return previewPos;
      }

      private void Move(Vector3 scale)
      {
         MoveTo(PreviewMove(scale), Rotation);
      }

      //keyboard input for main character(car)
      private void MainInput(float deltaTime)
      {
         KeyboardState ks = Keyboard.GetState();
         Vector3 moveVector = Vector3.Zero;

         //Up&Down for Speed control, Left&Right for Rotation(Limited 70*)
         #region Input
         moveVector.Z = 1f;

         if (ks.IsKeyDown(Keys.W))
         {
            Game1.accelerateSFX.Play();
            if (moveSpeed < originalSpeed * 2f)
               moveSpeed += 0.002f * moveSpeed;      //acceleration based on current speed
         }
         else if (ks.IsKeyDown(Keys.S))
         {
            Game1.stop.Play();
            if (moveSpeed > originalSpeed * 0.5f)
               moveSpeed -= 0.075f;
         }
         else if (moveSpeed > originalSpeed)
            moveSpeed -= 0.05f;
         else if (moveSpeed < originalSpeed)
            moveSpeed += 0.05f;

         if (ks.IsKeyDown(Keys.A) && rotation.Y < MathHelper.ToRadians(80f))
         {
            rotation.Y += 0.03f;
            wheelRotate += 0.02f;
         }
         else
         {
            if (wheelRotate > 0f)
               wheelRotate -= 0.05f;
         }

         if (ks.IsKeyDown(Keys.D) && rotation.Y > MathHelper.ToRadians(-80f))
         {
            rotation.Y -= 0.03f;
            wheelRotate -= 0.02f;
         }else
         {
            if (wheelRotate < 0f)
               wheelRotate += 0.05f;
         }

         moveVector.Normalize();    //constant speed
         moveVector *= deltaTime * moveSpeed;
         Move(moveVector);
         #endregion

         #region wheelsAnimation
         //left wheels rotation
         objModel.Bones[11].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(90) + wheelRotate) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateTranslation(objModel.Bones[11].Transform.Translation);
         objModel.Bones[10].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(90)) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateTranslation(objModel.Bones[10].Transform.Translation);
         //right wheels rotation
         objModel.Bones[9].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(-90) + wheelRotate) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateTranslation(objModel.Bones[9].Transform.Translation);
         objModel.Bones[8].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(-90)) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateTranslation(objModel.Bones[8].Transform.Translation);
         #endregion
      }

      public override void Update(GameTime gameTime)
      {
         float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
         MainInput(dt);
         SetCustomBoundingBox();

         base.Update(gameTime);
      }

      //Collision Detection between other obastacles, try to use delegate method later
      public void onCollisionBox(BoundingBox other)
      {
         if (collider.Intersects(other))
         {
            Life -= 3;      //lose Durability when hit by bullet
            Game1.hit.Play();
            if (Life <= 0)
            {
               MediaPlayer.Play(Game1.sFX);
               MediaPlayer.IsRepeating = false;
               game.GameOver = true;
            }
         }
      }

      public void onCollisionBoxCar(BoundingBox other)
      {
         if (collider.Intersects(other))
         {
            Life = 0;       //die immediately when hit rocks
            MediaPlayer.Play(Game1.sFX);
            MediaPlayer.IsRepeating = false;
            game.GameOver = true;
         }
      }

      public void onCollisionSphere(BoundingSphere other)
      {
         if (collider.Intersects(other))
         {
            Life = 0;       //die immediately when hit rocks
            MediaPlayer.Play(Game1.sFX);
            MediaPlayer.IsRepeating = false;
            game.GameOver = true;
         }
      }

      //create custom collider for collision detection
      private void SetCustomBoundingBox()
      {
         var boxMin = new Vector3(position.X - 0.6f, 0f, position.Z - 1.5f);
         var boxMax = new Vector3(position.X + 0.6f, 2f, position.Z + 1.5f);

         collider = new BoundingBox(boxMin, boxMax);
      }

      public void Draw(Camera camera)
      {
         Matrix[] transforms = new Matrix[objModel.Bones.Count];
         objModel.CopyAbsoluteBoneTransformsTo(transforms);

         foreach (var mesh in objModel.Meshes)
         {
            foreach (BasicEffect effect in mesh.Effects)
            {
               effect.EnableDefaultLighting();

               var scale = transforms[mesh.ParentBone.Index] * Matrix.CreateScale(scaleSize);
               worldMatrix = scale * orientation * Matrix.CreateRotationY(Rotation.Y)
                 * Matrix.CreateTranslation(Position);

               effect.World = worldMatrix;
               camera.Display(effect);
            }
            mesh.Draw();
         }
      }
   }
}