
namespace MonoGame___Lab4
{
   public class Point
   {
      public Point ParentPoint { get; set; }
      public int F { get; set; }  //F=G+H
      public int G { get; set; }
      public int H { get; set; }
      public int X { get; set; }
      public int Z { get; set; }

      public Point(int x, int z)
      {
         this.X = x;
         this.Z = z;
      }
      public void CalcF()
      {
         this.F = this.G + this.H;
      }
   }

}
