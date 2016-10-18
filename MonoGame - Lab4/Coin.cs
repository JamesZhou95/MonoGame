using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame___Lab4
{
   class Coin : GameComponent
   {
      #region fields
      private Matrix worldMatrix, orientation;
      private Vector3 position;
      private Vector3 rotation;
      private float scaleSize;
      private float moveSpeed;
      private Character main;
      private BoundingSphere bSphere;
      private Model objModel;
      #endregion

      public Vector3 Position
      {
         get { return position; }
         set
         {
            position = value;
         }
      }

      //create new obstacles
      public Coin(Game1 game, Model model, Vector3 pos, float speed, float scale, Matrix world, Character target) : base(game)
      {
         position = pos;
         orientation = world;
         moveSpeed = speed;
         scaleSize = scale;
         main = target;
         objModel = model;
      }

      public override void Update(GameTime gameTime)
      {
         rotation.Y -= 0.02f;
         SetBoundingSphere();
         base.Update(gameTime);
      }

      private void SetBoundingSphere()
      {
         bSphere = new BoundingSphere();
         foreach (ModelMesh mesh in objModel.Meshes)
         {
            if (bSphere.Radius == 0)
               bSphere = mesh.BoundingSphere;
            else
               bSphere = BoundingSphere.CreateMerged(bSphere, mesh.BoundingSphere);
         }
         bSphere.Center = new Vector3(position.X, position.Y, position.Z);
         bSphere.Radius = 0.2f;

         if (position.Z < main.Position.Z + 3)
            if (main.onCollisionSphere(bSphere))
               Position = new Vector3(position.X, 0, position.Z - 15);
      }

      public void Draw(Camera camera)
      {
         Matrix[] transforms = new Matrix[objModel.Bones.Count];
         objModel.CopyAbsoluteBoneTransformsTo(transforms);

         foreach (var mesh in objModel.Meshes)
         {
            foreach (BasicEffect effect in mesh.Effects)
            {
               effect.DiffuseColor = Color.Gold.ToVector3();

               var scale = transforms[mesh.ParentBone.Index] * Matrix.CreateScale(scaleSize);
               worldMatrix = scale * orientation * Matrix.CreateRotationY(rotation.Y) * Matrix.CreateTranslation(Position);

               effect.World = worldMatrix;
               camera.Display(effect);
            }
            mesh.Draw();
         }
      }
   }
}

