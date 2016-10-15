using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MonoGame___Lab4
{
   public static class GeneralStaticHelper
   {
      public static int Mapsize;
      //Static methods for get and set Points
      public static bool Exists(this List<Point> points, Point point)
      {
         foreach (Point p in points)
            if ((p.X == point.X) && (p.Z == point.Z))
               return true;
         return false;
      }

      public static bool Exists(this List<Point> points, int x, int z)
      {
         foreach (Point p in points)
            if ((p.X == x) && (p.Z == z))
               return true;
         return false;
      }

      public static Point MinPoint(this List<Point> points)
      {
         points = points.OrderBy(p => p.F).ToList();
         return points[0];
      }
      public static void Add(this List<Point> points, int x, int z)
      {
         Point point = new Point(x, z);
         points.Add(point);
      }

      public static Point Get(this List<Point> points, Point point)
      {
         foreach (Point p in points)
            if ((p.X == point.X) && (p.Z == point.Z))
               return p;
         return null;
      }

      public static void Remove(this List<Point> points, int x, int z)
      {
         foreach (Point point in points)
         {
            if (point.X == x && point.Z == z)
               points.Remove(point);
         }
      }

      //Static Methods for convert Point Position from/to World Position
      public static Vector3 toVector3(this Point points)
      {
         var x = points.X.toWorldX();
         var z = points.Z.toWorldZ();
         return new Vector3(x, 0, z);
      }

      public static float toWorldX(this int points)
      {
         return points * 2 - MapData.MAPSIZE.X * 0.95f;
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
         return (int)(points + MapData.MAPSIZE.X * 0.95f) / 2;
      }

      public static int toPointY(this float points)
      {
         return (int)points / 2;
      }

   }
}
