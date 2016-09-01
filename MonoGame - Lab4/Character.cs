using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;


namespace MonoGame___Lab4 {
   enum ModelType { main, obstacle }
   class Character : GameComponent {
      private Model objModel;
      private Matrix worldMatrix, orientation;
      private Vector3 position;
      private Vector3 rotation;
      private Vector3 lookAt;
      private float scaleSize;
      private float moveSpeed;
      private ModelType type;
      private BoundingSphere collider = new BoundingSphere();
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
      public Character(Game1 game, Model model, Vector3 pos, float speed, float scale, Matrix world, ModelType mType) : base(game) {
         objModel = model;
         position = pos;
         orientation = world;
         moveSpeed = speed;
         type = mType;
         scaleSize = scale;
         this.game = game;
      }

      //move character to position
      private void MoveTo(Vector3 pos, Vector3 rot) {
         Position = pos;
         Rotation = rot;
      }

      //Preview position to detect collision
      private Vector3 PreviewMove(Vector3 amount) {
         Matrix rotate = Matrix.CreateRotationY(rotation.Y);
         Vector3 movement = new Vector3(amount.X, amount.Y, amount.Z);
         movement = Vector3.Transform(movement, rotate);
         var previewPos = position + movement;
         previewPos.X = MathHelper.Clamp(previewPos.X, -Game1.MAPSIZE * 0.95f, Game1.MAPSIZE * 0.95f);
         return previewPos;
      }

      private void Move(Vector3 scale) {
         MoveTo(PreviewMove(scale), Rotation);
      }

      //change look at direction
      private void UpdateLookAt() {
         Matrix rotationMatrix = Matrix.CreateRotationY(rotation.Y);
         Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
         lookAt = position + lookAtOffset;
      }

      //keyboard input for main character
      private void MainInput(float deltaTime) {
         KeyboardState ks = Keyboard.GetState();
         Vector3 moveVector = Vector3.Zero;

         if (ks.IsKeyDown(Keys.W))
            moveVector.Z = 1;
         if (ks.IsKeyDown(Keys.S))
            moveVector.Z = -1;

         if (moveVector != Vector3.Zero) {
            if (ks.IsKeyDown(Keys.A))
               rotation.Y += 0.03f;
            //moveVector.X = 1;  //not in use for car model

            if (ks.IsKeyDown(Keys.D))
               rotation.Y -= 0.03f;
            //moveVector.X = -1; //not in use for car model

            rotation.X -= 0.05f;    // rotation rate
            moveVector.Normalize();    //constant speed
            moveVector *= deltaTime * moveSpeed;
            Move(moveVector);
         }

         //left wheels rotation
         objModel.Bones[10].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(90)) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateTranslation(objModel.Bones[10].Transform.Translation);
         objModel.Bones[11].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(90)) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateTranslation(objModel.Bones[11].Transform.Translation);
         //right wheels rotation
         objModel.Bones[9].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(-90)) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateTranslation(objModel.Bones[9].Transform.Translation);
         objModel.Bones[8].Transform = Matrix.CreateRotationY(MathHelper.ToRadians(-90)) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateTranslation(objModel.Bones[8].Transform.Translation);
      }

      //moving obstacles(bullets)
      private void Fire(float deltaTime) {
         Vector3 moveVector = new Vector3();
         moveVector.Z = -0.1f;
         moveVector *= deltaTime * moveSpeed;
         Move(moveVector);
      }

      public override void Update(GameTime gameTime) {
         float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            MainInput(dt);

            //create custom collider for collision detection
            //collider = new BoundingBox(new Vector3(position.X + 1.5f, position.Y, position.Z - 0.5f),
            //   new Vector3(position.X - 0.5f, position.Y + 2f, position.Z + 1.5f));
        
         base.Update(gameTime);
      }

      public void onCollision(BoundingSphere other) {
         if (collider.Intersects(other)) {
           // game.GameOver = true;
         }
      }

      public void Draw(Camera camera) {
         Matrix[] transforms = new Matrix[objModel.Bones.Count];
         objModel.CopyAbsoluteBoneTransformsTo(transforms);

         foreach (var mesh in objModel.Meshes) {
            foreach (BasicEffect effect in mesh.Effects) {
               effect.EnableDefaultLighting();

               var scale = Matrix.CreateScale(scaleSize);
               worldMatrix = transforms[mesh.ParentBone.Index] * scale * orientation * Matrix.CreateRotationY(Rotation.Y)
                 * Matrix.CreateTranslation(Position);

               effect.World = worldMatrix;

               if (collider.Radius == 0)
                  collider = mesh.BoundingSphere;
               else
                  collider = BoundingSphere.CreateMerged(collider, mesh.BoundingSphere);

               camera.Display(effect);
            }
            mesh.Draw();
         }
         collider.Center = position;
         //collider.Radius *= scaleSize;
         collider.Transform(worldMatrix);
         //Debug.WriteLine(collider.Center);
      }
   }
}