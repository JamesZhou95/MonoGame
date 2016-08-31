using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame___Lab4 {
   class TexturePlane{
      private GraphicsDevice graphics;
      private VertexPositionTexture[] verts;
      private VertexBuffer vertexBuffer;
      private BasicEffect effect;
      private Matrix world;
      private Texture2D texture;
      private int size;
      private Vector3 translation;
      private float scale;

      public TexturePlane(GraphicsDevice graphics, Texture2D texture, int size, Matrix world,float scale) {
         this.graphics = graphics;
         this.texture = texture;
         this.world = world;
         this.size = size;
         this.scale = scale;

         verts = new VertexPositionTexture[4];
         verts[0] = new VertexPositionTexture(new Vector3(-size, 0, size), new Vector2(0, size));
         verts[1] = new VertexPositionTexture(new Vector3(-size, 0, -size), new Vector2(0, 0));
         verts[2] = new VertexPositionTexture(new Vector3(size, 0, size), new Vector2(size, 0));
         verts[3] = new VertexPositionTexture(new Vector3(size, 0, -size), new Vector2(size, size));

         vertexBuffer = new VertexBuffer(graphics, typeof(VertexPositionTexture), verts.Length, BufferUsage.None);
         vertexBuffer.SetData(verts);

         effect = new BasicEffect(graphics);
      }

      public void Update(Vector3 target) {
         //swap between another plane to create infinity view
         if(target.Z > translation.Z + size * scale)
            translation.Z += size*2;
      }

      public void Draw(Camera camera) {
         effect.World = world * Matrix.CreateTranslation(translation);
         effect.TextureEnabled = true;
         effect.Texture = texture;
         camera.Display(effect);

         graphics.SetVertexBuffer(vertexBuffer);

         effect.CurrentTechnique.Passes[0].Apply();
         graphics.DrawUserPrimitives(PrimitiveType.TriangleStrip, verts, 0, 2);
      }
   }
}
