using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System;

namespace MonoGame___Lab4
{
   class Snowplow : GameComponent
   {
      private Matrix worldMatrix, orientation;
      private Vector3 position;
      private Vector3 rotation;
      private Vector3 lookAt;
      private Vector3 endPos;
      private float scaleSize;
      private float moveSpeed;
      private Character main;
      private BoundingBox collider;

      private PathFinding map;
      private bool findPoints = true;
      private Point points;
      private Model objModel;

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
      public Snowplow(Game1 game, Model model, Vector3 pos, Vector3 end, float speed, float scale, Matrix world, Character target) : base(game)
      {
         position = pos;
         orientation = world;
         moveSpeed = speed;
         scaleSize = scale;
         main = target;
         objModel = model;
         endPos = end;
         map = new PathFinding(Game1.MapArry);
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
         // Matrix rotate = Matrix.CreateRotationY(rotation.Y);
         var targetPos = (points != null) ? points.toVector3() : new Vector3(position.X, 0, position.Z - 10);
         Matrix rotate = RotateToFace(position, targetPos, Vector3.Up);
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
         // Matrix rotationMatrix = Matrix.CreateRotationY(rotation.Y);
         var targetPos = (points != null) ? points.toVector3() : new Vector3(position.X, 0, position.Z - 10);
         Matrix rotationMatrix = RotateToFace(position, targetPos, Vector3.Up);
         Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
         lookAt = position + lookAtOffset;
      }

      Matrix RotateToFace(Vector3 O, Vector3 P, Vector3 U)
      {
         Vector3 D = (O - P);
         Vector3 Right = Vector3.Cross(U, D);
         Vector3.Normalize(ref Right, out Right);
         Vector3 Backwards = Vector3.Cross(Right, U);
         Vector3.Normalize(ref Backwards, out Backwards);
         Vector3 Up = Vector3.Cross(Backwards, Right);
         Matrix rot = new Matrix(Right.X, Right.Y, Right.Z, 0, Up.X, Up.Y, Up.Z, 0, Backwards.X, Backwards.Y, Backwards.Z, 0, 0, 0, 0, 1);
         return rot;
      }

      public void Update(GameTime gameTime, Spawner sp)
      {
         float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

         Vector3 moveVector = Vector3.Zero;
         moveVector.Z = -1f;

         if (findPoints)
         {
            points = map.FindPath(endPos.toPoint(), position.toPoint(), true);
            findPoints = false;
         }

        if (points != null && position.Z <= points.Z.toWorldZ())
               points = points.ParentPoint;

         moveVector.Normalize();    //constant speed
         moveVector *= dt * moveSpeed;
         Move(moveVector);

         SetBoundingBox();
         main.onCollisionBox(collider);
         base.Update(gameTime);
      }

      private void SetBoundingBox()
      {
         var boxMin = new Vector3(position.X - 0.05f, 0f, position.Z - 0.15f);
         var boxMax = new Vector3(position.X + 0.05f, 2f, position.Z + 0.20f);

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
               var targetPos = (points != null) ? points.toVector3() : new Vector3(position.X, 0, position.Z - 10);
               worldMatrix = scale * orientation * RotateToFace(position, targetPos, Vector3.Up)
                 * Matrix.CreateTranslation(Position);

               effect.World = worldMatrix;
               camera.Display(effect);
            }
            mesh.Draw();
         }
      }
   }
   public static class ConvertHelper
   {
      public static Vector3 toVector3(this Point points)
      {
         var x = points.X.toWorldX();
         var z = points.Z.toWorldZ();
         return new Vector3(x, 0, z);
      }

      public static float toWorldX(this int points)
      {
         return points * 2 - Game1.MAPSIZE.X * 0.95f;
      }

      public static float toWorldZ(this int points)
      {
         return points * 2;
      }

      public static Point toPoint(this Vector3 points)
      {
         var x = points.X.toPointX();
         var z = points.Z.toPointY();
         return new Point(x, z);
      }

      public static int toPointX(this float points)
      {
         return (int)(points + Game1.MAPSIZE.X * 0.95f) / 2;
      }

      public static int toPointY(this float points)
      {
         return (int)points / 2;
      }
   }
}

