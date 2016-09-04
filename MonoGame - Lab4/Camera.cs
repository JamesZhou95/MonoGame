using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace MonoGame___Lab4 {
   class Camera: GameComponent {
      private Vector3 camPosition;
      private Vector3 camRotation;
      private float camSpeed;
      private Vector3 camLookAt;
      private float offset;

      public Vector3 Position {
         get { return camPosition; }
         set {
            camPosition = value;
            UpdateLookAt();
         }
      }

      public Vector3 Rotation {
         get { return camRotation; }
         set {
            camRotation = value;
            UpdateLookAt();
         }
      }

      public Matrix Projection {
         get;
         protected set;
      }

      public Matrix View {
         get {
            return Matrix.CreateLookAt(camPosition, camLookAt, Vector3.UnitY);
         }
      }

      public Camera(Game game, Vector3 position, Vector3 rotation, float speed) : base(game) {
         camSpeed = speed;

         Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
            game.GraphicsDevice.Viewport.AspectRatio,
            1f, 1000f);

         offset = position.Z;

         MoveTo(position, rotation);
      }

      //move camera to target position
      public void MoveTo(Vector3 pos, Vector3 rot) {
         Position = pos;
         Rotation = rot;
      }

      //change camera look direction
      private void UpdateLookAt() {
         Matrix rotationMatrix = Matrix.CreateRotationX(MathHelper.ToRadians(45));
         Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
         camLookAt = camPosition + lookAtOffset;
      }

      //make camera follow main character
      public void Update(Vector3 target) {
         Position = new Vector3(target.X, Position.Y, target.Z - offset);
      }

      public void Display(BasicEffect effect) {
         effect.View = View;
         effect.Projection = Projection;
      }
   }
}
