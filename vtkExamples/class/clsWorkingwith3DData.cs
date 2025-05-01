
#define  VTK_MAJOR_VERSION_5

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

using Kitware.VTK;

namespace vtkExamples
{
    class clsWorkingwith3DData
    {
        struct Frame
        {
            private float[] origin;
            private float[] xDirection;
            private float[] yDirection;
            private float[] zDirection;

            internal float[] Origin
            {
                get { return origin; }
            }

            internal float[] XDirection
            {
                get { return xDirection; }
            }

            internal float[] YDirection
            {
                get { return yDirection; }
            }

            internal float[] ZDirection
            {
                get { return zDirection; }
            }

            internal Frame(float[] origin, float[] xDirection, float[] yDirection, float[] zDirection)
            {
                this.origin = new float[3];
                this.xDirection = new float[3];
                this.yDirection = new float[3];
                this.zDirection = new float[3];
                origin.CopyTo(this.origin, 0);
                xDirection.CopyTo(this.xDirection, 0);
                yDirection.CopyTo(this.yDirection, 0);
                zDirection.CopyTo(this.zDirection, 0);

                Normalize(ref xDirection);
                Normalize(ref yDirection);
                Normalize(ref zDirection);

                Console.WriteLine("Origin: " +
                   this.origin[0] + " " +
                   this.origin[1] + " " +
                   this.origin[2]);
                Console.WriteLine("xDirection: " +
                   this.xDirection[0] + " " +
                   this.xDirection[1] + " " +
                   this.xDirection[2]);
                Console.WriteLine("yDirection: " +
                   this.yDirection[0] + " " +
                   this.yDirection[1] + " " +
                   this.yDirection[2]);
                Console.WriteLine("zDirection: " +
                   this.zDirection[0] + " " +
                   this.zDirection[1] + " " +
                   this.zDirection[2]);
            }


            private void Normalize(ref float[] vector)
            {
                IntPtr pDirection = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(float)) * 3);
                Marshal.Copy(vector, 0, pDirection, 3);
                vtkMath.Normalize(pDirection);
                Marshal.FreeHGlobal(pDirection);
            }


            internal void ApplyTransform(ref vtkTransform transform, string filename)
            {
                vtkPolyData polydata = vtkPolyData.New();
                CreatePolydata(ref polydata);

                vtkTransformFilter transformFilter = vtkTransformFilter.New();
#if VTK_MAJOR_VERSION_5
                transformFilter.SetInputConnection(polydata.GetProducerPort());
#else
                transformFilter.SetInput(polydata);
#endif
                transformFilter.SetTransform(transform);
                transformFilter.Update();

                vtkXMLPolyDataWriter writer = vtkXMLPolyDataWriter.New();
                writer.SetFileName(filename);
#if VTK_MAJOR_VERSION_5
            writer.SetInputConnection(transformFilter.GetOutputPort());
#else
                writer.SetInput(transformFilter);
#endif
                writer.Write();
            }


            internal void CreatePolydata(ref vtkPolyData polydata)
            {
                vtkPoints points = vtkPoints.New();

                points.InsertNextPoint(this.origin[0], this.origin[1], this.origin[2]);

                float[] x = new float[3];
                float[] y = new float[3];
                float[] z = new float[3];

                Add(this.origin, this.xDirection, ref x);
                Add(this.origin, this.yDirection, ref y);
                Add(this.origin, this.zDirection, ref z);

                points.InsertNextPoint(x[0], x[1], x[2]);
                points.InsertNextPoint(y[0], y[1], y[2]);
                points.InsertNextPoint(z[0], z[1], z[2]);

                polydata.SetPoints(points);

                vtkVertexGlyphFilter vertexGlyphFilter = vtkVertexGlyphFilter.New();
#if VTK_MAJOR_VERSION_5
            vertexGlyphFilter.AddInput(polydata);
#else
                vertexGlyphFilter.AddInputData(polydata);
#endif
                vertexGlyphFilter.Update();
                polydata.ShallowCopy(vertexGlyphFilter.GetOutput());
            }


            internal void Write(string filename)
            {
                vtkPolyData polydata = vtkPolyData.New();
                CreatePolydata(ref polydata);

                vtkXMLPolyDataWriter writer = vtkXMLPolyDataWriter.New();
                writer.SetFileName(filename);
#if VTK_MAJOR_VERSION_5
            writer.SetInputConnection(polydata.GetProducerPort());
#else
                writer.SetInputData(polydata);
#endif
                writer.Write();
            }
        }


        public static void AlignFramesMain()
        {
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();

            float[] frame1origin = new float[] { 0, 0, 0 };
            float[] frame1XDirection = new float[] { 1, 0, 0 };
            float[] frame1YDirection = new float[] { 0, 1, 0 };
            Console.WriteLine(frame1YDirection[0] + " " + frame1YDirection[1] + " " + frame1YDirection[2]);
            float[] frame1ZDirection = new float[] { 0, 0, 1 };
            Frame frame1 = new Frame(frame1origin, frame1XDirection, frame1YDirection, frame1ZDirection);
            Console.WriteLine("\nWriting frame1.vtp...");
            // adjust path
            string filePath = System.IO.Path.Combine(root, @"Data\frame1.vtp");
            frame1.Write(filePath);

            float[] frame2origin = new float[] { 0, 0, 0 };
            float[] frame2XDirection = new float[] { .707f, .707f, 0 };
            float[] frame2YDirection = new float[] { -.707f, .707f, 0 };
            float[] frame2ZDirection = new float[] { 0, 0, 1 };
            Frame frame2 = new Frame(frame2origin, frame2XDirection, frame2YDirection, frame2ZDirection);
            Console.WriteLine("\nWriting frame2.vtp...");
            // adjust path
            filePath = System.IO.Path.Combine(root, @"Data\frame2.vtp");
            frame1.Write(filePath);

            vtkTransform transform = vtkTransform.New();
            AlignFrames(frame2, frame1, ref transform); // Brings frame2 to frame1

            Console.WriteLine("\nWriting transformed.vtp...");
            // adjust path
            filePath = System.IO.Path.Combine(root, @"Data\transformed.vtp");
            frame2.ApplyTransform(ref transform, filePath);

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }


        static void AlignFrames(Frame sourceFrame, Frame targetFrame, ref vtkTransform transform)
        {
            // This function takes two frames and finds the matrix M between them.
            vtkLandmarkTransform landmarkTransform = vtkLandmarkTransform.New();

            // Setup source points
            vtkPoints sourcePoints = vtkPoints.New();

            sourcePoints.InsertNextPoint(
               sourceFrame.Origin[0],
               sourceFrame.Origin[1],
               sourceFrame.Origin[2]);

            float[] sourceX = new float[3];
            float[] sourceY = new float[3];
            float[] sourceZ = new float[3];

            Add(sourceFrame.Origin, sourceFrame.XDirection, ref sourceX);
            Add(sourceFrame.Origin, sourceFrame.YDirection, ref sourceY);
            Add(sourceFrame.Origin, sourceFrame.ZDirection, ref sourceZ);

            sourcePoints.InsertNextPoint(sourceX[0], sourceX[1], sourceX[2]);
            sourcePoints.InsertNextPoint(sourceY[0], sourceY[1], sourceY[2]);
            sourcePoints.InsertNextPoint(sourceZ[0], sourceZ[1], sourceZ[2]);

            // Setup target points
            vtkPoints targetPoints = vtkPoints.New();
            targetPoints.InsertNextPoint(targetFrame.Origin[0], targetFrame.Origin[1], targetFrame.Origin[2]);

            float[] targetX = new float[3];
            float[] targetY = new float[3];
            float[] targetZ = new float[3];

            Add(targetFrame.Origin, targetFrame.XDirection, ref targetX);
            Add(targetFrame.Origin, targetFrame.YDirection, ref targetY);
            Add(targetFrame.Origin, targetFrame.ZDirection, ref targetZ);

            targetPoints.InsertNextPoint(targetX[0], targetX[1], targetX[2]);
            targetPoints.InsertNextPoint(targetY[0], targetY[1], targetY[2]);
            targetPoints.InsertNextPoint(targetZ[0], targetZ[1], targetZ[2]);

            landmarkTransform.SetSourceLandmarks(sourcePoints);
            landmarkTransform.SetTargetLandmarks(targetPoints);
            landmarkTransform.SetModeToRigidBody();
            landmarkTransform.Update();

            vtkMatrix4x4 M = landmarkTransform.GetMatrix();
            transform.SetMatrix(M);
        }

        // helper function
        static void Add(float[] vec1, float[] vec2, ref float[] vec)
        {
            vec[0] = vec1[0] + vec2[0];
            vec[1] = vec1[1] + vec2[1];
            vec[2] = vec1[2] + vec2[2];
        }

        public static void ContoursFromPolyData(RenderWindowControl renderWindowControl1)
        {
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = string.Empty; // System.IO.Path.Combine(root, @"Data\foot\foot.mha");

            vtkPolyData inputPolyData;
            //if (filePath != null)
            //{
            //    vtkXMLPolyDataReader reader = vtkXMLPolyDataReader.New();
            //    reader.SetFileName(filePath);
            //    reader.Update();
            //    inputPolyData = reader.GetOutput();
            //}
            //else
            //{
                vtkSphereSource sphereSource = vtkSphereSource.New();
                sphereSource.SetThetaResolution(30);
                sphereSource.SetPhiResolution(15);
                sphereSource.Update();
                inputPolyData = sphereSource.GetOutput();
            //}

            vtkPolyDataMapper inputMapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
            inputMapper.SetInput(inputPolyData);
#else
         inputMapper.SetInputData(inputPolyData);
#endif

            // Create a plane to cut
            vtkPlane plane = vtkPlane.New();
            double[] center = inputPolyData.GetCenter();
            double[] bounds = inputPolyData.GetBounds();
            plane.SetOrigin(center[0], center[1], center[2]);
            plane.SetNormal(1, 1, 1);


            float[] centerf = new float[] { (float)center[0], (float)center[1], (float)center[2] };
            float[] minBoundf = new float[] { (float)bounds[0], (float)bounds[2], (float)bounds[4] };
            float[] maxBoundf = new float[] { (float)bounds[1], (float)bounds[3], (float)bounds[5] };
            IntPtr pCenter = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(float)) * 3);
            IntPtr pMinBound = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(float)) * 3);
            IntPtr pMaxBound = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(float)) * 3);
            Marshal.Copy(centerf, 0, pCenter, 3);
            Marshal.Copy(minBoundf, 0, pMinBound, 3);
            Marshal.Copy(maxBoundf, 0, pMaxBound, 3);

            // vtkMath.Distance2BetweenPoints accepts floats only
            double distanceMin = Math.Sqrt(vtkMath.Distance2BetweenPoints(pMinBound, pCenter));
            double distanceMax = Math.Sqrt(vtkMath.Distance2BetweenPoints(pMaxBound, pCenter));

            Marshal.FreeHGlobal(pCenter);
            Marshal.FreeHGlobal(pMinBound);
            Marshal.FreeHGlobal(pMaxBound);
            // Create cutter
            vtkCutter cutter = vtkCutter.New();
            cutter.SetCutFunction(plane);
#if VTK_MAJOR_VERSION_5
            cutter.SetInput(inputPolyData);
#else
         cutter.SetInputData(inputPolyData);
#endif
            cutter.GenerateValues(20, -distanceMin, distanceMax);
            vtkPolyDataMapper cutterMapper = vtkPolyDataMapper.New();
            cutterMapper.SetInputConnection(cutter.GetOutputPort());
            cutterMapper.ScalarVisibilityOff();

            // Create plane actor
            vtkActor planeActor = vtkActor.New();
            planeActor.GetProperty().SetColor(1.0, 0.0, 0.0);
            planeActor.GetProperty().SetLineWidth(3);
            planeActor.SetMapper(cutterMapper);

            // Create input actor
            vtkActor inputActor = vtkActor.New();
            inputActor.GetProperty().SetColor(1.0, 0.8941, 0.7686); // bisque
            inputActor.SetMapper(inputMapper);

            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(.2, .3, .4);
            renderer.AddActor(inputActor);
            renderer.AddActor(planeActor); //display the contour lines
        }

        public  static void FindAllArrayNamesMain()
        {
            FindAllArrayNames(null);
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }


        private static void FindAllArrayNames(string filePath)
        {
            vtkPolyData polydata = vtkPolyData.New();

            if (filePath == null)
            {
                vtkSphereSource sphereSource = vtkSphereSource.New();
                sphereSource.Update();
                vtkXMLPolyDataWriter writer = vtkXMLPolyDataWriter.New();
                writer.SetFileName(@"c:\vtk\vtkdata-5.8.0\Data\testFindAllArrayNames.vtp");
                writer.SetInputConnection(sphereSource.GetOutputPort());
                writer.Write();
                polydata = sphereSource.GetOutput();
            }
            else
            {
                vtkXMLPolyDataReader reader = vtkXMLPolyDataReader.New();
                reader.SetFileName(filePath);
                reader.Update();
                polydata = reader.GetOutput();
            }
            FindAllData(ref polydata);
        }


        private static void FindAllData(ref vtkPolyData polydata)
        {
            Console.WriteLine("Normals: " + polydata.GetPointData().GetNormals());

            int numberOfPointArrays = polydata.GetPointData().GetNumberOfArrays();
            Console.WriteLine("Number of PointData arrays: " + numberOfPointArrays);

            int numberOfCellArrays = polydata.GetCellData().GetNumberOfArrays();
            Console.WriteLine("Number of CellData arrays: " + numberOfCellArrays);

            Console.WriteLine(
               Environment.NewLine +
               "Type table/key: " +
               Environment.NewLine +
               "-------------------------");
            //more values can be found in <VTK_DIR>/Common/vtkSetGet.h

            Console.WriteLine(3 + " unsigned char");
            Console.WriteLine(7 + " unsigned int");
            Console.WriteLine(10 + " float");
            Console.WriteLine(11 + " double" + Environment.NewLine);

            for (int i = 0; i < numberOfPointArrays; i++)
            {
                // The following two lines are equivalent
                //arrayNames.push_back(polydata.GetPointData().GetArray(i).GetName());
                //arrayNames.push_back(polydata.GetPointData().GetArrayName(i));
                int dataTypeID = polydata.GetPointData().GetArray(i).GetDataType();
                string dataTypeAsString = polydata.GetPointData().GetArray(i).GetDataTypeAsString();
                Console.WriteLine("Array " + i + ": "
                   + polydata.GetPointData().GetArrayName(i)
                   + " (type: " + dataTypeID + ")"
                   + " (type as string: " + dataTypeAsString + ")" + Environment.NewLine);
            }

            for (int i = 0; i < numberOfCellArrays; i++)
            {
                // The following two lines are equivalent
                //polydata.GetPointData().GetArray(i).GetName();
                //polydata.GetPointData().GetArrayName(i);
                int dataTypeID = polydata.GetCellData().GetArray(i).GetDataType();
                string dataTypeAsString = polydata.GetPointData().GetArray(i).GetDataTypeAsString();
                Console.WriteLine("Array " + i + ": "
                   + polydata.GetCellData().GetArrayName(i)
                   + " (type: " + dataTypeID + ")"
                   + " (type as string: " + dataTypeAsString + ")");
            }
        }

        public static void ImplicitBoolean(RenderWindowControl renderWindowControl1)
        {
            vtkSphere sphere1 = vtkSphere.New();
            sphere1.SetCenter(.9, 0, 0);
            vtkSphere sphere2 = vtkSphere.New();
            sphere2.SetCenter(-.9, 0, 0);

            vtkImplicitBoolean implicitBoolean = vtkImplicitBoolean.New();
            implicitBoolean.AddFunction(sphere1);
            implicitBoolean.AddFunction(sphere2);
            implicitBoolean.SetOperationTypeToUnion();
            //implicitBoolean.SetOperationTypeToIntersection();

            // Sample the function
            vtkSampleFunction sample = vtkSampleFunction.New();
            sample.SetSampleDimensions(50, 50, 50);
            sample.SetImplicitFunction(implicitBoolean);
            double value = 3.0;
            double xmin = -value, xmax = value,
                   ymin = -value, ymax = value,
                   zmin = -value, zmax = value;
            sample.SetModelBounds(xmin, xmax, ymin, ymax, zmin, zmax);

            // Create the 0 isosurface
            vtkContourFilter contours = vtkContourFilter.New();
#if VTK_MAJOR_VERSION_5
            contours.SetInputConnection(sample.GetOutputPort());
#else
         contours.SetInputData(sample);
#endif
            contours.GenerateValues(1, 1, 1);

            // Map the contours to graphical primitives
            vtkPolyDataMapper contourMapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
            contourMapper.SetInputConnection(contours.GetOutputPort());
#else
         contourMapper.SetInputData(contours);
#endif
            contourMapper.ScalarVisibilityOff();

            // Create an actor for the contours
            vtkActor contourActor = vtkActor.New();
            contourActor.SetMapper(contourMapper);
            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(.2, .3, .4);
            renderer.AddActor(contourActor);
        }

        public static void IterateOverLines()
        {
            double[] origin = new double[] { 0.0, 0.0, 0.0 };
            double[,] p = new double[,] {
            {1.0, 0.0, 0.0},
            {0.0, 1.0, 0.0},
            {0.0, 1.0, 2.0},
            {1.0, 2.0, 3.0}};

            // Create a vtkPoints object and store the points in it
            vtkPoints points = vtkPoints.New();
            points.InsertNextPoint(origin[0], origin[1], origin[2]);
            for (int i = 0; i < 4; i++)
                points.InsertNextPoint(p[i, 0], p[i, 1], p[i, 2]);

            // Create a cell array to store the lines in and add the lines to it
            vtkCellArray lines = vtkCellArray.New();

            // Create four lines
            for (int i = 0; i < 4; i++)
            {
                vtkLine line = vtkLine.New();
                line.GetPointIds().SetId(0, i);
                line.GetPointIds().SetId(1, i + 1);
                lines.InsertNextCell(line);
            }

            // Create a polydata to store everything in
            vtkPolyData linesPolyData = vtkPolyData.New();

            // Add the points to the dataset
            linesPolyData.SetPoints(points);

            // Add the lines to the dataset
            linesPolyData.SetLines(lines);

            Console.WriteLine("There are " + linesPolyData.GetNumberOfLines() + " lines.");
            linesPolyData.GetLines().InitTraversal();
            vtkIdList idList = vtkIdList.New();
            while (linesPolyData.GetLines().GetNextCell(idList) != 0)
            {
                Console.WriteLine("Line has " + idList.GetNumberOfIds() + " points.");

                for (int pointId = 0; pointId < idList.GetNumberOfIds(); pointId++)
                {
                    Console.Write(idList.GetId(pointId) + " ");
                }
                Console.Write(Environment.NewLine);
            }
        }


        public static void MultiBlockMergeFilter()
        {
            vtkSphereSource sphereSource1 = vtkSphereSource.New();
            sphereSource1.Update();

            vtkSphereSource sphereSource2 = vtkSphereSource.New();
            sphereSource2.SetCenter(10, 10, 10);
            sphereSource2.Update();

            vtkMultiBlockDataSet multiBlockDataSet1 = vtkMultiBlockDataSet.New();
            multiBlockDataSet1.SetNumberOfBlocks(1);
            multiBlockDataSet1.SetBlock(0, sphereSource1.GetOutput());
#if VTK_MAJOR_VERSION_5
            multiBlockDataSet1.Update();
#endif

            vtkMultiBlockDataSet multiBlockDataSet2 = vtkMultiBlockDataSet.New();
            multiBlockDataSet2.SetNumberOfBlocks(1);
            multiBlockDataSet2.SetBlock(0, sphereSource2.GetOutput());
#if VTK_MAJOR_VERSION_5
            multiBlockDataSet2.Update();
#endif

            vtkMultiBlockMergeFilter multiBlockMergeFilter = vtkMultiBlockMergeFilter.New();
#if VTK_MAJOR_VERSION_5
            multiBlockMergeFilter.AddInput(multiBlockDataSet1);
            multiBlockMergeFilter.AddInput(multiBlockDataSet2);
#else
         multiBlockMergeFilter.AddInputData(multiBlockDataSet1);
         multiBlockMergeFilter.AddInputData(multiBlockDataSet2);
#endif
            multiBlockMergeFilter.Update();
        }

        public static void NullPoint()
        {
            vtkPoints points = vtkPoints.New();
            points.InsertNextPoint(1, 1, 1);
            points.InsertNextPoint(2, 2, 2);
            points.InsertNextPoint(3, 3, 3);

            vtkPolyData polydata = vtkPolyData.New();
            polydata.SetPoints(points);

            vtkFloatArray floatArray = vtkFloatArray.New();
            floatArray.SetNumberOfValues(3);
            floatArray.SetNumberOfComponents(1);
            floatArray.SetName("FloatArray");
            for (int i = 0; i < 3; i++)
            {
                floatArray.SetValue(i, 2);
            }
            polydata.GetPointData().AddArray(floatArray);

            vtkIntArray intArray = vtkIntArray.New();
            intArray.SetNumberOfValues(3);
            intArray.SetNumberOfComponents(1);
            intArray.SetName("IntArray");
            for (int i = 0; i < 3; i++)
            {
                intArray.SetValue(i, 2);
            }

            polydata.GetPointData().AddArray(intArray);

            Console.WriteLine("PointIdx   x y z " + "floatArray" + " " + "intArray");
            Console.WriteLine("----------------------------------------");
            for (int i = 0; i < 3; i++)
            {
                double[] p = polydata.GetPoint(i);
                vtkFloatArray pointsFloatArray = vtkFloatArray.SafeDownCast(polydata.GetPointData().GetArray("FloatArray"));
                vtkIntArray pointsIntArray = vtkIntArray.SafeDownCast(polydata.GetPointData().GetArray("IntArray"));
                Console.WriteLine("   " + i + "       " + p[0] + " " + p[1] + " " + p[2] + "    "
                          + pointsFloatArray.GetValue(i) + "          " + pointsIntArray.GetValue(i));
            }

            polydata.GetPointData().NullPoint(1);
            polydata.Modified();
            Console.WriteLine("");

            for (int i = 0; i < 3; i++)
            {
                double[] p = polydata.GetPoint(i);
                vtkFloatArray pointsFloatArray = vtkFloatArray.SafeDownCast(polydata.GetPointData().GetArray("FloatArray"));
                vtkIntArray pointsIntArray = vtkIntArray.SafeDownCast(polydata.GetPointData().GetArray("IntArray"));
                Console.WriteLine("   " + i + "       " + p[0] + " " + p[1] + " " + p[2] + "    "
                          + pointsFloatArray.GetValue(i) + "          " + pointsIntArray.GetValue(i));

            }
        }


        public static void PolyDataGetPoint()
        {
            // Create a sphere
            vtkSphereSource sphereSource = vtkSphereSource.New();
            sphereSource.Update();
            vtkPolyData polydata = sphereSource.GetOutput();

            // Write all of the coordinates of the points in the vtkPolyData to the console.
            for (int i = 0; i < polydata.GetNumberOfPoints(); i++)
            {
                double[] p = polydata.GetPoint(i);
                // This is identical to:
                // double[] p = polydata.GetPoints().GetPoint(i);
                Console.WriteLine("Point " + i + " : (" + p[0] + " " + p[1] + " " + p[2] + ")");
            }
        }


        public static void ShrinkPolyData(RenderWindowControl renderWindowControl1)
        {
            vtkSphereSource sphereSource = vtkSphereSource.New();
            sphereSource.SetRadius(10);
            sphereSource.SetPhiResolution(12);
            sphereSource.SetThetaResolution(12);
            sphereSource.Update();

            vtkShrinkPolyData shrinkFilter = vtkShrinkPolyData.New();
            shrinkFilter.SetInputConnection(sphereSource.GetOutputPort());
            shrinkFilter.Update();

            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(shrinkFilter.GetOutputPort());

            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);

            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(.2, .3, .4);
            //Add the actors to the renderer, set the background and size 
            renderer.AddActor(actor);
        }

        public static void VectorFieldNonZeroExtraction(RenderWindowControl renderWindowControl1)
        {
            // Create an image
            vtkImageData image = vtkImageData.New();
            CreateVectorField(ref image);

            // This filter produces a vtkImageData with an array named "Magnitude"
            vtkImageMagnitude magnitudeFilter = vtkImageMagnitude.New();
            magnitudeFilter.SetInputConnection(image.GetProducerPort());
            magnitudeFilter.Update();

            image.GetPointData().AddArray(magnitudeFilter.GetOutput().GetPointData().GetScalars());
            image.GetPointData().SetActiveScalars("Magnitude");

            vtkThresholdPoints thresholdVector = vtkThresholdPoints.New();
            thresholdVector.SetInput(image);
            thresholdVector.SetInputArrayToProcess(
               0,
               0,
               (int)vtkDataObject.FieldAssociations.FIELD_ASSOCIATION_POINTS,
               (int)vtkDataSetAttributes.AttributeTypes.SCALARS,
               "Magnitude");
            thresholdVector.ThresholdByUpper(0.00001);
            thresholdVector.Update();

            // in case you want to save imageData
            //vtkXMLPolyDataWriter writer = vtkXMLPolyDataWriter.New();
            //writer.SetFileName("output.vtp");
            //writer.SetInputConnection(thresholdPoints.GetOutputPort());
            //writer.Write();

            // repesents the pixels
            vtkCubeSource cubeSource = vtkCubeSource.New();
            cubeSource.SetXLength(2.0);
            cubeSource.SetYLength(2.0);
            cubeSource.SetZLength(2.0);
            vtkGlyph3D glyph = vtkGlyph3D.New();
            glyph.SetInput(image);
            glyph.SetSourceConnection(cubeSource.GetOutputPort());
            // don't scale glyphs according to any scalar data
            glyph.SetScaleModeToDataScalingOff();

            vtkPolyDataMapper glyphMapper = vtkPolyDataMapper.New();
            glyphMapper.SetInputConnection(glyph.GetOutputPort());
            // don't color glyphs according to scalar data
            glyphMapper.ScalarVisibilityOff();
            glyphMapper.SetScalarModeToDefault();

            vtkActor actor = vtkActor.New();
            actor.SetMapper(glyphMapper);

            // represent vector field
            vtkGlyph3D vectorGlyph = vtkGlyph3D.New();
            vtkArrowSource arrowSource = vtkArrowSource.New();
            vtkPolyDataMapper vectorGlyphMapper = vtkPolyDataMapper.New();

            int n = image.GetPointData().GetNumberOfArrays();
            for (int i = 0; i < n; i++)
            {
                Debug.WriteLine("name of array[" + i + "]: " + image.GetPointData().GetArrayName(i));
            }

            vtkPolyData tmp = thresholdVector.GetOutput();
            Debug.WriteLine("number of thresholded points: " + tmp.GetNumberOfPoints());
            vectorGlyph.SetInputConnection(thresholdVector.GetOutputPort());

            // in case you want the point glyphs to be oriented according to 
            // scalar values in array "ImageScalars" uncomment the following line
            image.GetPointData().SetActiveVectors("ImageScalars");

            vectorGlyph.SetSourceConnection(arrowSource.GetOutputPort());
            vectorGlyph.SetScaleModeToScaleByVector();
            vectorGlyph.SetVectorModeToUseVector();
            vectorGlyph.ScalingOn();
            vectorGlyph.OrientOn();
            vectorGlyph.SetInputArrayToProcess(
               1,
               0,
               (int)vtkDataObject.FieldAssociations.FIELD_ASSOCIATION_POINTS,
               (int)vtkDataSetAttributes.AttributeTypes.SCALARS,
               "ImageScalars");

            vectorGlyph.Update();

            vectorGlyphMapper.SetInputConnection(vectorGlyph.GetOutputPort());
            vectorGlyphMapper.Update();

            vtkActor vectorActor = vtkActor.New();
            vectorActor.SetMapper(vectorGlyphMapper);


            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(.2, .6, .3);
            //Add the actors to the renderer, set the background and size 
            renderer.AddActor(actor);
            renderer.AddActor(vectorActor);
        }


        static void CreateVectorField(ref vtkImageData image)
        {
            // Specify the size of the image data
            image.SetDimensions(3, 3, 3);
            image.SetNumberOfScalarComponents(3);
            image.SetScalarTypeToFloat();
            image.AllocateScalars();
            image.SetSpacing(10.0, 10.0, 10.0);
            int[] dims = image.GetDimensions();

            float[] pixel = new float[] { 0.0f, 0.0f, 0.0f };
            IntPtr pPixel;

            // Zero the vectors
            for (int z = 0; z < dims[2]; z++)
            {
                for (int y = 0; y < dims[1]; y++)
                {
                    for (int x = 0; x < dims[0]; x++)
                    {
                        pPixel = image.GetScalarPointer(x, y, 0);
                        Marshal.Copy(pixel, 0, pPixel, 3);
                    }
                }
            }

            // Set two of the pixels to non zero values
            pixel[0] = 8.0f;
            pixel[1] = 8.0f;
            pixel[2] = -8.0f;
            pPixel = image.GetScalarPointer(0, 2, 0);
            Marshal.Copy(pixel, 0, pPixel, 3);

            pixel[0] = 8.0f;
            pixel[1] = -8.0f;
            pixel[2] = 8.0f;
            pPixel = image.GetScalarPointer(2, 0, 2);
            Marshal.Copy(pixel, 0, pPixel, 3);
        }

        public static void WarpVector(RenderWindowControl renderWindowControl1)
        {
            vtkPoints points = vtkPoints.New();
            points.InsertNextPoint(0.0, 0.0, 0.0);
            points.InsertNextPoint(1.0, 0.0, 0.0);
            points.InsertNextPoint(2.0, 0.0, 0.0);
            points.InsertNextPoint(3.0, 0.0, 0.0);
            points.InsertNextPoint(4.0, 0.0, 0.0);

            vtkCellArray lines = vtkCellArray.New();
            vtkLine line = vtkLine.New();
            line.GetPointIds().SetId(0, 0);
            line.GetPointIds().SetId(1, 1);
            lines.InsertNextCell(line);
            line.GetPointIds().SetId(0, 1);
            line.GetPointIds().SetId(1, 2);
            lines.InsertNextCell(line);
            line.GetPointIds().SetId(0, 2);
            line.GetPointIds().SetId(1, 3);
            lines.InsertNextCell(line);
            line.GetPointIds().SetId(0, 3);
            line.GetPointIds().SetId(1, 4);
            lines.InsertNextCell(line);

            vtkDoubleArray warpData = vtkDoubleArray.New();
            warpData.SetNumberOfComponents(3);
            warpData.SetName("warpData");
            double[] warp = new double[] { 0.0, 0.0, 0.0 };
            warp[1] = 0.0;
            warpData.InsertNextTuple3(warp[0], warp[1], warp[2]);
            warp[1] = 0.1;
            warpData.InsertNextTuple3(warp[0], warp[1], warp[2]);
            warp[1] = 0.3;
            warpData.InsertNextTuple3(warp[0], warp[1], warp[2]);
            warp[1] = 0.0;
            warpData.InsertNextTuple3(warp[0], warp[1], warp[2]);
            warp[1] = 0.1;
            warpData.InsertNextTuple3(warp[0], warp[1], warp[2]);

            vtkPolyData polydata = vtkPolyData.New();
            polydata.SetPoints(points);
            polydata.SetLines(lines);
            polydata.GetPointData().AddArray(warpData);
            polydata.GetPointData().SetActiveVectors(warpData.GetName());

            //WarpVector will use the array marked as active vector in polydata
            //it has to be a 3 component array
            //with the same number of tuples as points in polydata
            vtkWarpVector warpVector = vtkWarpVector.New();
#if VTK_MAJOR_VERSION_5
            warpVector.SetInput(polydata);
#else
         warpVector.SetInputData(polydata);
#endif
            warpVector.Update();

            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
            mapper.SetInput(warpVector.GetPolyDataOutput());
#else
         mapper.SetInputData(warpVector.GetPolyDataOutput());
#endif
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(.2, .6, .3);
            renderer.AddActor(actor);
        }

    }
}
