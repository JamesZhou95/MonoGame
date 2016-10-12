using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace MonoGame___Lab4
{
   enum ModelType { bullet, rock }
   class Obstacles : GameComponent
   {
      private ModelType type;
      private Model objModel;
      private Matrix worldMatrix, orientation;
      private Vector3 position;
      private Vector3 rotation;
      private Vector3 lookAt;
      private float scaleSize;
      private float moveSpeed;
      private Character main;
      private BoundingBox collider;
      private BoundingSphere bSphere;

      public Vector3 Position
      {
         get { return position; }
         set
         {
            position = value;
            UpdateLookAt();
         }
      }

      public Vector3 Rotation
      {
         get { return rotation; }
         set
         {
            rotation = value;
            UpdateLookAt();
         }
      }

      //create new obstacles
      public Obstacles(Game1 game, ModelType mType, Model model, Vector3 pos, float speed, float scale, Matrix world, Character target) : base(game)
      {
         objModel = model;
         position = pos;
         orientation = world;
         moveSpeed = speed;
         scaleSize = scale;
         main = target;
         type = mType;
      }

      //move character to position
      private void MoveTo(Vector3 pos, Vector3 rot)
      {
         Position = pos;
         Rotation = rot;
      }

      //Preview position to detect collision
      private Vector3 PreviewMove(Vector3 amount)
      {
         Matrix rotate = Matrix.CreateRotationY(rotation.Y);
         Vector3 movement = new Vector3(amount.X, amount.Y, amount.Z);
         movement = Vector3.Transform(movement, rotate);
         var previewPos = position + movement;
         previewPos.X = MathHelper.Clamp(previewPos.X, -Game1.MAPSIZE.X * 0.95f, Game1.MAPSIZE.X * 0.95f);
         return previewPos;
      }

      private void Move(Vector3 scale)
      {
         MoveTo(PreviewMove(scale), Rotation);
      }

      //change look at direction
      private void UpdateLookAt()
      {
         Matrix rotationMatrix = Matrix.CreateRotationY(rotation.Y);
         Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
         lookAt = position + lookAtOffset;
      }

      //moving obstacles(bullets)
      private void Fire(float deltaTime)
      {
         if (type == ModelType.bullet)
         {
            Vector3 moveVector = new Vector3();
            moveVector.Z = -0.1f;
            moveVector *= deltaTime * moveSpeed;
            Move(moveVector);
         }
      }

      public override void Update(GameTime gameTime)
      {
         float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
         Fire(dt);

         SetBoundingBox();
         if (type == ModelType.bullet)
            main.onCollisionBox(collider);
         else
            main.onCollisionSphere(bSphere);

         base.Update(gameTime);
      }

      private void SetBoundingBox()
      {
         if (type == ModelType.bullet)
         {
            var boxMin = new Vector3(position.X - 0.05f, 0f, position.Z - 0.15f);
            var boxMax = new Vector3(position.X + 0.05f, 2f, position.Z + 0.20f);

            collider = new BoundingBox(boxMin, boxMax);
         }
         else
         {
            bSphere = new BoundingSphere();
            foreach (ModelMesh mesh in objModel.Meshes)
            {
               if (bSphere.Radius == 0)
                  bSphere = mesh.BoundingSphere;
               else
                  bSphere = BoundingSphere.CreateMerged(bSphere, mesh.BoundingSphere);
            }
            bSphere.Center = new Vector3(position.X, position.Y + 0.05f, position.Z);
            bSphere.Radius *= scaleSize * 0.25f;
         }
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
               effect.DiffuseColor = Color.White.ToVector3();

               var scale = Matrix.CreateScale(scaleSize);
               worldMatrix = transforms[mesh.ParentBone.Index] * scale * orientation * Matrix.CreateRotationY(Rotation.Y)
                 * Matrix.CreateTranslation(Position);
               effect.World = worldMatrix;

               camera.Display(effect);
            }
            mesh.Draw();
         }
      }
   }
}