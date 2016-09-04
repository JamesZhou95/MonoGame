using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;


namespace MonoGame___Lab4 {
   enum InputType { type1, type2, type3,type4 }
   class Character : GameComponent {
      private InputType type;
      private Model objModel;
      private Matrix worldMatrix, orientation;
      private Vector3 position;
      private Vector3 rotation;
      private Vector3 lookAt;
      private float scaleSize;
      private float moveSpeed, originalSpeed;
      private BoundingBox collider;
      private Game1 game;

      public Vector3 Position {
         get { return position; }
         set {
            position = value;
            UpdateLookAt();
         }
      }

      public Vector3 Rotation {
         get { return rotation; }
         set {
            rotation = value;
            UpdateLookAt();
         }
      }

      //create new character/obstacles
      public Character(Game1 game, Model model, Vector3 pos, float speed, float scale, Matrix world, InputType mType) : base(game) {
         objModel = model;
         position = pos;
         orientation = world;
         moveSpeed = speed;
         type = mType;
         scaleSize = scale;
         originalSpeed = speed;
         this.game = game;
         SoundEffect.MasterVolume = 0.1f;
      }

      //move character to position
      private void MoveTo(Vector3 pos, Vector3 rot) {
         Position = pos;
         Rotation = rot;
      }

      //Preview position to detect collision/boundary
      private Vector3 PreviewMove(Vector3 amount) {
         Matrix rotate = Matrix.CreateRotationY(rotation.Y);
         Vector3 movement = new Vector3(amount.X, amount.Y, amount.Z);
         movement = Vector3.Transform(movement, rotate);
         var previewPos = position + movement;
         previewPos.X = MathHelper.Clamp(previewPos.X, -Game1.MAPSIZE * 0.9f, Game1.MAPSIZE * 0.9f);
         return previewPos;
      }

      private void Move(Vector3 scale) {
         MoveTo(PreviewMove(scale), Rotation);
      }

      //change lookAt direction
      private void UpdateLookAt() {
         Matrix rotationMatrix = Matrix.CreateRotationY(rotation.Y);
         Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
         lookAt = position + lookAtOffset;
      }

      //keyboard input for main character(car)
      private void MainInput(float deltaTime) {
         KeyboardState ks = Keyboard.GetState();
         Vector3 moveVector = Vector3.Zero;

         //Type 1 Up&Down for Speed control, Left&Right for Rotation(360*)
         #region InputType1
         if (type == InputType.type1) {
            moveVector.Z = 1;

            if (ks.IsKeyDown(Keys.W)) {
                //Game1.accelerateSFX.Play();
                if (moveSpeed < originalSpeed * 1.5f)
                        moveSpeed += 0.05f;
            }
            else if (ks.IsKeyDown(Keys.S)) {
               if (moveSpeed > originalSpeed * 0.5f)
                  moveSpeed -= 0.05f;
            }
            else if (moveSpeed > originalSpeed)
               moveSpeed -= 0.05f;
            else if (moveSpeed < originalSpeed)
               moveSpeed += 0.05f;

            if (ks.IsKeyDown(Keys.A))
               rotation.Y += 0.03f;

            if (ks.IsKeyDown(Keys.D))
               rotation.Y -= 0.03f;

            rotation.X -= 0.05f;    // rotation rate
            moveVector.Normalize();    //constant speed
            moveVector *= deltaTime * moveSpeed;
            Move(moveVector);

            #region wheelsAnimation
            //left wheels rotation
            objModel.Bones[10].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(90)) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateTranslation(objModel.Bones[10].Transform.Translation);
            objModel.Bones[11].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(90)) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateTranslation(objModel.Bones[11].Transform.Translation);
            //right wheels rotation
            objModel.Bones[9].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(-90)) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateTranslation(objModel.Bones[9].Transform.Translation);
            objModel.Bones[8].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(-90)) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateTranslation(objModel.Bones[8].Transform.Translation);
            #endregion
         }
         #endregion InputType1

         //Type 2 Up&Down for Speed control, Left&Right for Limited Rotation(15*)
         //Type 3 Up&Down for Speed control, Left&Right for Limited Rotation(15*) and with Left&Right shift
         #region InputType2and3
         if (type == InputType.type2) {
            moveVector.Z = 1;

            if (ks.IsKeyDown(Keys.W)) {
               //Game1.accelerateSFX.Play();
               if (moveSpeed < originalSpeed * 1.5f)
                  moveSpeed += 0.05f;
            }
            else if (ks.IsKeyDown(Keys.S)) {
               if (moveSpeed > originalSpeed * 0.5f)
                  moveSpeed -= 0.05f;
            }
            else if (moveSpeed > originalSpeed)
               moveSpeed -= 0.05f;
            else if (moveSpeed < originalSpeed)
               moveSpeed += 0.05f;

            if (ks.IsKeyDown(Keys.A)) {
               if(type == InputType.type3)
                  moveVector.X += 1f;
               if (rotation.Y < MathHelper.ToRadians(15f))
                  rotation.Y += 0.02f;

               Debug.WriteLine(rotation.Y);
            }

            if (ks.IsKeyDown(Keys.D)) {
               if (type == InputType.type3)
                  moveVector.X -= 1f;
               if(rotation.Y > MathHelper.ToRadians(-15f))
               rotation.Y -= 0.02f;
               Debug.WriteLine(rotation.Y);
            }

            moveVector.Normalize();
            moveVector *= deltaTime * moveSpeed;
            Move(moveVector);

            #region wheelsAnimation
            //left wheels rotation
            objModel.Bones[10].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(90)) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateTranslation(objModel.Bones[10].Transform.Translation);
            objModel.Bones[11].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(90)) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateTranslation(objModel.Bones[11].Transform.Translation);
            //right wheels rotation
            objModel.Bones[9].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(-90)) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateTranslation(objModel.Bones[9].Transform.Translation);
            objModel.Bones[8].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(-90)) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateTranslation(objModel.Bones[8].Transform.Translation);
            #endregion
         }
         #endregion

         //Type 4 Up&Down for Moving(No Speed Control), Left&Right for Rotation(360*),No limit At all.
         #region InputType2and3
         if (type == InputType.type4) {
            

            if (ks.IsKeyDown(Keys.W)) {
               moveVector.Z = 1;
            }
            if (ks.IsKeyDown(Keys.S)) {
               moveVector.Z = -1;
            }

            if (moveVector != Vector3.Zero) {
               if (ks.IsKeyDown(Keys.A)) {
                  //if (rotation.Y < MathHelper.ToRadians(30f))
                     rotation.Y += 0.02f;
               }

               if (ks.IsKeyDown(Keys.D)) {
                  //if (rotation.Y > MathHelper.ToRadians(-30f))
                     rotation.Y -= 0.02f;
               }

               moveVector.Normalize();
               moveVector *= deltaTime * moveSpeed;
               Move(moveVector);

               #region wheelsAnimation
               //left wheels rotation
               objModel.Bones[10].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(90)) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateTranslation(objModel.Bones[10].Transform.Translation);
               objModel.Bones[11].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(90)) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateTranslation(objModel.Bones[11].Transform.Translation);
               //right wheels rotation
               objModel.Bones[9].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(-90)) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateTranslation(objModel.Bones[9].Transform.Translation);
               objModel.Bones[8].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(-90)) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateTranslation(objModel.Bones[8].Transform.Translation);
               #endregion
            }
         }
         #endregion
      }

      public override void Update(GameTime gameTime) {
         float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
         MainInput(dt);
         SetCustomBoundingBox();

         base.Update(gameTime);
      }

      //Collision Detection between other obastacles, try to use delegate method later
      public void onCollision(BoundingBox other) {
         if (collider.Intersects(other)) {
            MediaPlayer.Volume = 1.0f;
            //MediaPlayer.Play(Game1.sFX);
            MediaPlayer.IsRepeating = false;
            game.GameOver = true;
         }
      }

      //create custom collider for collision detection
      private void SetCustomBoundingBox() {
         var boxMin = new Vector3(position.X - 0.8f, 0f, position.Z - 1.8f);
         var boxMax = new Vector3(position.X + 0.8f, 2f, position.Z + 1.8f);

         collider = new BoundingBox(boxMin, boxMax);
      }

      public void Draw(Camera camera) {
         Matrix[] transforms = new Matrix[objModel.Bones.Count];
         objModel.CopyAbsoluteBoneTransformsTo(transforms);

         foreach (var mesh in objModel.Meshes) {
            foreach (BasicEffect effect in mesh.Effects) {
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