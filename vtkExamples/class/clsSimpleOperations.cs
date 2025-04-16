using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

using Kitware.VTK;

namespace vtkExamples
{
    class clsSimpleOperations
    {
        public static void UniformRandomNumber()
        {
            // Set the number of random numbers we wish to produce to 3.
            uint numRand = 3;
            // Without this line, the random numbers will be the same every iteration.
            vtkMath.RandomSeed((int)(DateTime.Now.Ticks & 0x0000FFFF));

            // Generate numRand random numbers from a uniform distribution between 0.0 and 2.0
            for (uint i = 0; i < numRand; i++)
            {
                double a = vtkMath.Random(0.0, 2.0);
                Console.WriteLine(a);
            }
        }

        public static void RandomSequence()
        {
            //Create a random sequence generator.
            vtkMinimalStandardRandomSequence sequence =
               vtkMinimalStandardRandomSequence.New();

            // initialize the sequence
            sequence.SetSeed((int)(DateTime.Now.Ticks & 0x0000FFFF));
            //Get 3 random numbers.
            double x = sequence.GetValue();
            sequence.Next();
            double y = sequence.GetValue();
            sequence.Next();
            double z = sequence.GetValue();

            // You can also use sequence.GetRangeValue(-1.0, 1.0); 
            // to set a range on the random values.

            // Output the resulting random numbers
            Console.WriteLine("x: " + x + " y: " + y + " z: " + z);
        }

        public static void ProjectPointPlane()
        {
            vtkPlane plane = vtkPlane.New();
            plane.SetOrigin(0.0, 0.0, 0.0);
            plane.SetNormal(0.0, 0.0, 1.0);

            double[] p = new double[] { 23.1, 54.6, 9.2 };
            double[] projected = new double[3];

            IntPtr pP = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * 3);
            IntPtr pProjected = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * 3);
            Marshal.Copy(p, 0, pP, 3);
            Marshal.Copy(projected, 0, pProjected, 3);

            // NOTE: normal assumed to have magnitude 1
            plane.ProjectPoint(pP, pProjected);
            Marshal.Copy(pProjected, projected, 0, 3);
            Marshal.FreeHGlobal(pP);
            Marshal.FreeHGlobal(pProjected);

            Console.WriteLine("Projected: "
               + projected[0] + " "
               + projected[1] + " "
               + projected[2]);
        }

        public static void PerspectiveTransform()
        {
            vtkMatrix4x4 m = vtkMatrix4x4.New();
            m.SetElement(0, 0, 1);
            m.SetElement(0, 1, 2);
            m.SetElement(0, 2, 3);
            m.SetElement(0, 3, 4);
            m.SetElement(1, 0, 2);
            m.SetElement(1, 1, 2);
            m.SetElement(1, 2, 3);
            m.SetElement(1, 3, 4);
            m.SetElement(2, 0, 3);
            m.SetElement(2, 1, 2);
            m.SetElement(2, 2, 3);
            m.SetElement(2, 3, 4);
            m.SetElement(3, 0, 4);
            m.SetElement(3, 1, 2);
            m.SetElement(3, 2, 3);
            m.SetElement(3, 3, 4);

            vtkPerspectiveTransform perspectiveTransform = vtkPerspectiveTransform.New();
            perspectiveTransform.SetMatrix(m);

            vtkTransform transform = vtkTransform.New();
            transform.SetMatrix(m);

            double[] p = new double[] { 1.0, 2.0, 3.0 };

            double[] normalProjection = transform.TransformPoint(p[0], p[1], p[2]);

            Console.WriteLine("Standard projection: "
               + normalProjection[0] + " "
               + normalProjection[1] + " "
               + normalProjection[2]);

            double[] perspectiveProjection = perspectiveTransform.TransformPoint(p[0], p[1], p[2]);

            Console.WriteLine("Perspective  projection: "
               + perspectiveProjection[0] + " "
               + perspectiveProjection[1] + " "
               + perspectiveProjection[2]);
        }

        public static void GaussianRandomNumber()
        {
            // Set the number of random numbers we wish to produce to 3.
            uint numRand = 3;
            // Without this line, the random numbers will be the same every iteration.
            vtkMath.RandomSeed((int)(DateTime.Now.Ticks & 0x0000FFFF));

            // Generate numRand random numbers from a Gaussian distribution with mean 0.0 and standard deviation 2.0
            for (uint i = 0; i < numRand; i++)
            {
                double a = vtkMath.Gaussian(0.0, 2.0);
                Console.WriteLine(a);
            }
        }

        public static void DistancePointToLine()
        {
            double[] lineP0 = new double[] { 0.0, 0.0, 0.0 };
            double[] lineP1 = new double[] { 2.0, 0.0, 0.0 };

            double[] p0 = new double[] { 1.0, 0, 0 };
            double[] p1 = new double[] { 1.0, 2.0, 0 };

            // Don't worry, fortunately only a few functions in ActiViz.NET need 
            // Marshaling between Managed and Unmanaged Code
            IntPtr pP0 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * 3);
            IntPtr pP1 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * 3);
            IntPtr pLineP0 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * 3);
            IntPtr pLineP1 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * 3);
            Marshal.Copy(p0, 0, pP0, 3);
            Marshal.Copy(p1, 0, pP1, 3);
            Marshal.Copy(lineP0, 0, pLineP0, 3);
            Marshal.Copy(lineP1, 0, pLineP1, 3);

            double dist0 = vtkLine.DistanceToLine(pP0, pLineP0, pLineP1);
            Console.WriteLine("Dist0: " + dist0);

            double dist1 = vtkLine.DistanceToLine(pP1, pLineP0, pLineP1);
            Console.WriteLine("Dist1: " + dist1);

            double parametricCoord = 0.0; // must be initialized because this var is passed by reference
            double[] closest = new double[3];
            IntPtr pClosest = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * 3);
            Marshal.Copy(closest, 0, pClosest, 3);

            dist0 = vtkLine.DistanceToLine(pP0, pLineP0, pLineP1, ref parametricCoord, pClosest);
            Marshal.Copy(pClosest, closest, 0, 3);
            Console.WriteLine(
               "Dist0: " + dist0
               + " closest point: " + closest[0] + " " + closest[1] + " " + closest[2]
               + " parametricCoord: " + parametricCoord);

            dist1 = vtkLine.DistanceToLine(pP1, pLineP0, pLineP1, ref parametricCoord, pClosest);
            Marshal.Copy(pClosest, closest, 0, 3);
            Console.WriteLine(
               "Dist1: " + dist1
               + " closest point: " + closest[0] + " " + closest[1] + " " + closest[2]
               + " parametricCoord: " + parametricCoord);

            Marshal.FreeHGlobal(pP0);
            Marshal.FreeHGlobal(pP1);
            Marshal.FreeHGlobal(pLineP0);
            Marshal.FreeHGlobal(pLineP1);
            Marshal.FreeHGlobal(pClosest);
        }

        static public void DistanceBetweenPoints()
        {
            // Create two points.
            double[] p0 = new double[] { 0.0, 0.0, 0.0 };
            double[] p1 = new double[] { 1.0, 1.0, 1.0 };

            IntPtr pP0 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * 3);
            IntPtr pP1 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * 3);
            Marshal.Copy(p0, 0, pP0, 3);
            Marshal.Copy(p1, 0, pP1, 3);

            // Find the squared distance between the points.
            double squaredDistance = vtkMath.Distance2BetweenPoints(pP0, pP1);

            // Take the square root to get the Euclidean distance between the points.
            double distance = Math.Sqrt(squaredDistance);

            // Output the results.
            Console.WriteLine("SquaredDistance = " + squaredDistance);
            Console.WriteLine("Distance = " + distance);
            Marshal.FreeHGlobal(pP0);
            Marshal.FreeHGlobal(pP1);
        }

    }
}
