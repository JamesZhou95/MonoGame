using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGame___Lab4
{
   class PathFinding
   {
      public const int OBLIQUE = 14;
      public const int STEP = 10;
      public char[,] MapArray { get; private set; }
      List<Point> CloseList;
      List<Point> OpenList;

      public PathFinding(char[,] map)
      {
         this.MapArray = map;
         OpenList = new List<Point>(MapArray.Length);
         CloseList = new List<Point>(MapArray.Length);
      }

      //Find the path from start to end
      public Point FindPath(Point start, Point end, bool IsIgnoreCorner)
      {
         OpenList.Add(start);
         while (OpenList.Count != 0)
         {
            var tempStart = OpenList.MinPoint();
            OpenList.RemoveAt(0);
            CloseList.Add(tempStart);

            var surroundPoints = SurrroundPoints(tempStart, IsIgnoreCorner);
            foreach (Point point in surroundPoints)
            {
               if (OpenList.Exists(point))

                  FoundPoint(tempStart, point);
               else

                  NotFoundPoint(tempStart, end, point);
            }
            if (OpenList.Get(end) != null)
               return OpenList.Get(end);
         }
         return OpenList.Get(end);
      }

      //calculate G/F for already found point
      private void FoundPoint(Point tempStart, Point point)
      {
         var G = CalcG(tempStart, point);
         if (G < point.G)
         {
            point.ParentPoint = tempStart;
            point.G = G;
            point.CalcF();
         }
      }

      //calculate G/F for new point
      private void NotFoundPoint(Point tempStart, Point end, Point point)
      {
         point.ParentPoint = tempStart;
         point.G = CalcG(tempStart, point);
         point.H = CalcH(end, point);
         point.CalcF();
         OpenList.Add(point);
      }

      //Find the G value, current to start position.
      private int CalcG(Point start, Point point)
      {
         int G = (Math.Abs(point.X - start.X) + Math.Abs(point.Z - start.Z)) == 2 ? STEP : OBLIQUE;
         int parentG = point.ParentPoint != null ? point.ParentPoint.G : 0;
         return G + parentG;
      }

      //Find the H value, from current position to End position
      private int CalcH(Point end, Point point)
      {
         int step = Math.Abs(point.X - end.X) + Math.Abs(point.Z - end.Z);
         return step * STEP;
      }

      //Get all the surrounding points for the current point.
      public List<Point> SurrroundPoints(Point point, bool IsIgnoreCorner)
      {
         var surroundPoints = new List<Point>(9);

         for (int z = point.Z - 1; z <= point.Z + 1; z++)
            for (int x = point.X - 1; x <= point.X + 1; x++)
            {
               if (CanReach(point, x, z, IsIgnoreCorner))
                  surroundPoints.Add(x, z);
            }
         return surroundPoints;
      }

      //return ture if the point is walkable.
      private bool CanReach(int x, int z)
      {
         return MapArray[z, x] != 'R';
      }

      public bool CanReach(Point start, int x, int z, bool IsIgnoreCorner)
      {
         if (!CanReach(x, z) || CloseList.Exists(x, z))
            return false;
         else
         {
            if (Math.Abs(x - start.X) + Math.Abs(z - start.Z) == 1)
               return true;
            else
            {
               if (CanReach(Math.Abs(x - 1), z) && CanReach(x, Math.Abs(z - 1)))
                  return true;
               else
                  return IsIgnoreCorner;
            }
         }
      }
   }
}
