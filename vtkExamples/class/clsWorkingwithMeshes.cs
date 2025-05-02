

#define VTK_MAJOR_VERSION_5


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
    class clsWorkingwithMeshes
    {

        public static void BoundaryEdges(RenderWindowControl renderWindowControl1)
        {
            vtkDiskSource diskSource = vtkDiskSource.New();
            diskSource.Update();

            vtkFeatureEdges featureEdges = vtkFeatureEdges.New();
#if VTK_MAJOR_VERSION_5
         featureEdges.SetInputConnection(diskSource.GetOutputPort());
#else
            featureEdges.SetInputData(diskSource);
#endif
            featureEdges.ColoringOff();
            featureEdges.BoundaryEdgesOn();
            featureEdges.FeatureEdgesOff();
            featureEdges.ManifoldEdgesOff();
            featureEdges.NonManifoldEdgesOff();
            featureEdges.Update();

            // Visualize
            vtkPolyDataMapper edgeMapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
         edgeMapper.SetInputConnection(featureEdges.GetOutputPort());
#else
            edgeMapper.SetInputData(featureEdges);
#endif
            vtkActor edgeActor = vtkActor.New();
            edgeActor.GetProperty().SetLineWidth(3);
            edgeActor.GetProperty().SetColor(0, 0, 255);
            edgeActor.SetMapper(edgeMapper);

            vtkPolyDataMapper diskMapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
         diskMapper.SetInputConnection(diskSource.GetOutputPort());
#else
            diskMapper.SetInputData(diskSource);
#endif
            vtkActor diskActor = vtkActor.New();
            diskActor.SetMapper(diskMapper);

            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.3, 0.6, 0.3);
            // add our actor to the renderer
            renderer.AddActor(diskActor);
            renderer.AddActor(edgeActor);
        }

        public static void CapClip(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            // string filePath = System.IO.Path.Combine(root, @"Data\SyntheticPolyline.vtp");
            // string filePath = System.IO.Path.Combine(root, @"Data\uniform-001371-5x5x5.vtp");
            // string filePath = System.IO.Path.Combine(root, @"Data\political.vtp");
            // string filePath = System.IO.Path.Combine(root, @"Data\filledContours.vtp");
            // string filePath = System.IO.Path.Combine(root, @"Data\disk_out_ref_surface.vtp");
            string filePath = null; // System.IO.Path.Combine(root, @"Data\cow.vtp");
            // PolyData to process
            vtkPolyData polyData;

            if (filePath != null)
            {
                vtkXMLPolyDataReader reader = vtkXMLPolyDataReader.New();
                reader.SetFileName(filePath);
                reader.Update();
                polyData = reader.GetOutput();
            }
            else
            {
                // Create a sphere
                vtkSphereSource sphereSource = vtkSphereSource.New();
                sphereSource.SetThetaResolution(20);
                sphereSource.SetPhiResolution(11);

                vtkPlane plane = vtkPlane.New();
                plane.SetOrigin(0, 0, 0);
                plane.SetNormal(1.0, -1.0, -1.0);

                vtkClipPolyData clipper = vtkClipPolyData.New();
                clipper.SetInputConnection(sphereSource.GetOutputPort());
                clipper.SetClipFunction(plane);
                clipper.SetValue(0);
                clipper.Update();

                polyData = clipper.GetOutput();
            }

            vtkDataSetMapper clipMapper = vtkDataSetMapper.New();
#if VTK_MAJOR_VERSION_5
            clipMapper.SetInput(polyData);
#else
         clipMapper.SetInputData(polyData);
#endif

            vtkActor clipActor = vtkActor.New();
            clipActor.SetMapper(clipMapper);
            clipActor.GetProperty().SetColor(1.0000, 0.3882, 0.2784);
            clipActor.GetProperty().SetInterpolationToFlat();

            // Now extract feature edges
            vtkFeatureEdges boundaryEdges = vtkFeatureEdges.New();
#if VTK_MAJOR_VERSION_5
            boundaryEdges.SetInput(polyData);
#else
         boundaryEdges.SetInputData(polyData);
#endif
            boundaryEdges.BoundaryEdgesOn();
            boundaryEdges.FeatureEdgesOff();
            boundaryEdges.NonManifoldEdgesOff();
            boundaryEdges.ManifoldEdgesOff();

            vtkStripper boundaryStrips = vtkStripper.New();
            boundaryStrips.SetInputConnection(boundaryEdges.GetOutputPort());
            boundaryStrips.Update();

            // Change the polylines into polygons
            vtkPolyData boundaryPoly = vtkPolyData.New();
            boundaryPoly.SetPoints(boundaryStrips.GetOutput().GetPoints());
            boundaryPoly.SetPolys(boundaryStrips.GetOutput().GetLines());

            vtkPolyDataMapper boundaryMapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
            boundaryMapper.SetInput(boundaryPoly);
#else
         boundaryMapper.SetInputData(boundaryPoly);
#endif

            vtkActor boundaryActor = vtkActor.New();
            boundaryActor.SetMapper(boundaryMapper);
            boundaryActor.GetProperty().SetColor(0.8900, 0.8100, 0.3400);
            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(.2, .3, .4);
            // add our actor to the renderer
            renderer.AddActor(clipActor);
            renderer.AddActor(boundaryActor);
            // Generate an interesting view
            //
            renderer.ResetCamera();
            renderer.GetActiveCamera().Azimuth(30);
            renderer.GetActiveCamera().Elevation(30);
            renderer.GetActiveCamera().Dolly(1.2);
            renderer.ResetCameraClippingRange();
        }


        public static void CellEdges()
        {
            vtkTriangle triangle = vtkTriangle.New();
            triangle.GetPoints().SetPoint(0, 1.0, 0.0, 0.0);
            triangle.GetPoints().SetPoint(1, 0.0, 0.0, 0.0);
            triangle.GetPoints().SetPoint(2, 0.0, 1.0, 0.0);
            triangle.GetPointIds().SetId(0, 0);
            triangle.GetPointIds().SetId(1, 1);
            triangle.GetPointIds().SetId(2, 2);

            Console.WriteLine("The cell has " + triangle.GetNumberOfEdges() + " edges.");

            for (int i = 0; i < triangle.GetNumberOfEdges(); i++)
            {
                vtkCell edge = triangle.GetEdge(i);

                vtkIdList pointIdList = edge.GetPointIds();
                Console.WriteLine("Edge " + i + " has " + pointIdList.GetNumberOfIds() + " points.");

                for (int p = 0; p < pointIdList.GetNumberOfIds(); p++)
                {
                    Console.WriteLine("Edge " + i + " uses point " + pointIdList.GetId(p));
                }
            }
        }

        public static void ClosedSurface()
        {
            // Create a sphere
            vtkSphereSource sphereSource = vtkSphereSource.New();
            sphereSource.Update();

            vtkFeatureEdges featureEdges = vtkFeatureEdges.New();
            featureEdges.FeatureEdgesOff();
            featureEdges.BoundaryEdgesOn();
            featureEdges.NonManifoldEdgesOn();
            featureEdges.SetInputConnection(sphereSource.GetOutputPort());
            featureEdges.Update();

            int numberOfOpenEdges = (int)featureEdges.GetOutput().GetNumberOfCells();

            if (numberOfOpenEdges > 0)
            {
                Console.WriteLine("Surface is not closed");
            }
            else
            {
                Console.WriteLine("Surface is closed");
            }
            // nothing to show graphically
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }


        public static void ColorDisconnectedRegions(RenderWindowControl renderWindowControl1)
        {
            // Create some spheres
            vtkSphereSource sphereSource1 = vtkSphereSource.New();
            sphereSource1.Update();

            vtkSphereSource sphereSource2 = vtkSphereSource.New();
            sphereSource2.SetCenter(5, 0, 0);
            sphereSource2.Update();

            vtkSphereSource sphereSource3 = vtkSphereSource.New();
            sphereSource3.SetCenter(10, 0, 0);
            sphereSource3.Update();

            vtkAppendPolyData appendFilter = vtkAppendPolyData.New();
            appendFilter.AddInputConnection(sphereSource1.GetOutputPort());
            appendFilter.AddInputConnection(sphereSource2.GetOutputPort());
            appendFilter.AddInputConnection(sphereSource3.GetOutputPort());

            vtkPolyDataConnectivityFilter connectivityFilter = vtkPolyDataConnectivityFilter.New();
            connectivityFilter.SetInputConnection(appendFilter.GetOutputPort());
            connectivityFilter.SetExtractionModeToAllRegions();
            connectivityFilter.ColorRegionsOn();
            connectivityFilter.Update();

            // Visualize
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(connectivityFilter.GetOutputPort());
            double[] range = connectivityFilter.GetOutput().GetPointData().GetArray("RegionId").GetRange();
            mapper.SetScalarRange(range[0], range[1]);

            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.0, 0.0, 0.0);
            // add our actor to the renderer
            renderer.AddActor(actor);
        }

        public static void ColoredElevationMap(RenderWindowControl renderWindowControl1)
        {
            // Create a grid of points (height/terrian map)
            vtkPoints points = vtkPoints.New();

            uint GridSize = 20;
            double xx, yy, zz;
            for (uint x = 0; x < GridSize; x++)
            {
                for (uint y = 0; y < GridSize; y++)
                {
                    xx = x + vtkMath.Random(-.2, .2);
                    yy = y + vtkMath.Random(-.2, .2);
                    zz = vtkMath.Random(-.5, .5);
                    points.InsertNextPoint(xx, yy, zz);
                }
            }

            // Add the grid points to a polydata object
            vtkPolyData inputPolyData = vtkPolyData.New();
            inputPolyData.SetPoints(points);

            // Triangulate the grid points
            vtkDelaunay2D delaunay = vtkDelaunay2D.New();
#if VTK_MAJOR_VERSION_5
            delaunay.SetInput(inputPolyData);
#else
         delaunay.SetInputData(inputPolyData);
#endif
            delaunay.Update();
            vtkPolyData outputPolyData = delaunay.GetOutput();

            double[] bounds = outputPolyData.GetBounds();

            // Find min and max z
            double minz = bounds[4];
            double maxz = bounds[5];

            Debug.WriteLine("minz: " + minz);
            Debug.WriteLine("maxz: " + maxz);

            // Create the color map
            vtkLookupTable colorLookupTable = vtkLookupTable.New();
            colorLookupTable.SetTableRange(minz, maxz);
            colorLookupTable.Build();

            // Generate the colors for each point based on the color map
            vtkUnsignedCharArray colors = vtkUnsignedCharArray.New();
            colors.SetNumberOfComponents(3);
            colors.SetName("Colors");

            Debug.WriteLine("There are " + outputPolyData.GetNumberOfPoints()
                      + " points.");


#if UNSAFE // fastest way to fill color array
         colors.SetNumberOfTuples(outputPolyData.GetNumberOfPoints());
         unsafe {
            byte* pColor = (byte*)colors.GetPointer(0).ToPointer();

            for(int i = 0; i < outputPolyData.GetNumberOfPoints(); i++) {
               double[] p = outputPolyData.GetPoint(i);

               double[] dcolor = colorLookupTable.GetColor(p[2]);
               Debug.WriteLine("dcolor: "
                         + dcolor[0] + " "
                         + dcolor[1] + " "
                         + dcolor[2]);

               byte[] color = new byte[3];
               for(uint j = 0; j < 3; j++) {
                  color[j] = (byte)( 255 * dcolor[j] );
               }
               Debug.WriteLine("color: "
                         + color[0] + " "
                         + color[1] + " "
                         + color[2]);

               *( pColor + 3 * i ) = color[0];
               *( pColor + 3 * i + 1 ) = color[1];
               *( pColor + 3 * i + 2 ) = color[2];
            }
         }
#else
            for (int i = 0; i < outputPolyData.GetNumberOfPoints(); i++)
            {
                double[] p = outputPolyData.GetPoint(i);

                double[] dcolor = colorLookupTable.GetColor(p[2]);
                Debug.WriteLine("dcolor: "
                          + dcolor[0] + " "
                          + dcolor[1] + " "
                          + dcolor[2]);

                byte[] color = new byte[3];
                for (uint j = 0; j < 3; j++)
                {
                    color[j] = (byte)(255 * dcolor[j]);
                }
                Debug.WriteLine("color: "
                          + color[0] + " "
                          + color[1] + " "
                          + color[2]);
                colors.InsertNextTuple3(color[0], color[1], color[2]);
                //IntPtr pColor = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * 3);
                //Marshal.Copy(color, 0, pColor, 3);
                //colors.InsertNextTupleValue(pColor);
                //Marshal.FreeHGlobal(pColor);
            }
#endif

            outputPolyData.GetPointData().SetScalars(colors);

            // Create a mapper and actor
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
            mapper.SetInputConnection(outputPolyData.GetProducerPort());
#else
         mapper.SetInputData(outputPolyData);
#endif

            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);

            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.2, 0.3, 0.4);
            // add our actor to the renderer
            renderer.AddActor(actor);
        }

        public static void Curvature(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            // note: one should use a coarse surface mesh to achieve a good visual effect
            string filePath = System.IO.Path.Combine(root, @"Data\bunny.vtp");
            // Create a polydata
            vtkXMLPolyDataReader reader = vtkXMLPolyDataReader.New();
            reader.SetFileName(filePath);

            vtkCurvatures curvaturesFilter = vtkCurvatures.New();
            curvaturesFilter.SetInputConnection(reader.GetOutputPort());
            //curvaturesFilter.SetCurvatureTypeToGaussian();
            //curvaturesFilter.SetCurvatureTypeToMean();
            //curvaturesFilter.SetCurvatureTypeToMaximum();
            curvaturesFilter.SetCurvatureTypeToMinimum();

            // To inspect more closely, if required
            vtkXMLPolyDataWriter writer = vtkXMLPolyDataWriter.New();
            writer.SetInputConnection(curvaturesFilter.GetOutputPort());
            writer.SetFileName(System.IO.Path.Combine(root, @"Data\gauss.vtp"));
            writer.Write();

            // Create a mapper and actor
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(curvaturesFilter.GetOutputPort());
            double[] range = curvaturesFilter.GetOutput().GetScalarRange();
            mapper.SetScalarRange(range[0], range[1]);
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.0, 0.0, 0.0);
            // add our actor to the renderer
            renderer.AddActor(actor);
        }

        public static void Decimation(RenderWindowControl renderWindowControl1)
        {
            vtkSphereSource sphereSource = vtkSphereSource.New();
            sphereSource.Update();

            vtkPolyData input = vtkPolyData.New();
            input.ShallowCopy(sphereSource.GetOutput());

            Debug.WriteLine("Before decimation" + Environment.NewLine + "------------");
            Debug.WriteLine("There are " + input.GetNumberOfPoints() + " points.");
            Debug.WriteLine("There are " + input.GetNumberOfPolys() + " polygons.");

            vtkDecimatePro decimate = vtkDecimatePro.New();
#if VTK_MAJOR_VERSION_5
            decimate.SetInputConnection(input.GetProducerPort());
#else
         decimate.SetInputData(input);
#endif
            //decimate.SetTargetReduction(.99); //99% reduction (if there was 100 triangles, now there will be 1)
            decimate.SetTargetReduction(.10); //10% reduction (if there was 100 triangles, now there will be 90)
            decimate.Update();

            vtkPolyData decimated = vtkPolyData.New();
            decimated.ShallowCopy(decimate.GetOutput());

            Debug.WriteLine("After decimation" + Environment.NewLine + "------------");

            Debug.WriteLine("There are " + decimated.GetNumberOfPoints() + " points.");
            Debug.WriteLine("There are " + decimated.GetNumberOfPolys() + " polygons.");

            vtkPolyDataMapper inputMapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
            inputMapper.SetInputConnection(input.GetProducerPort());
#else
         inputMapper.SetInputData(input);
#endif
            vtkActor inputActor = vtkActor.New();
            inputActor.SetMapper(inputMapper);

            vtkPolyDataMapper decimatedMapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
            decimatedMapper.SetInputConnection(decimated.GetProducerPort());
#else
         decimatedMapper.SetInputData(decimated);
#endif
            vtkActor decimatedActor = vtkActor.New();
            decimatedActor.SetMapper(decimatedMapper);

            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            //this.Size = new System.Drawing.Size(612, 352);

            // Define viewport ranges
            // (xmin, ymin, xmax, ymax)
            double[] leftViewport = new double[] { 0.0, 0.0, 0.5, 1.0 };
            double[] rightViewport = new double[] { 0.5, 0.0, 1.0, 1.0 };

            // Setup both renderers
            vtkRenderer leftRenderer = vtkRenderer.New();
            renderWindow.AddRenderer(leftRenderer);
            leftRenderer.SetViewport(leftViewport[0], leftViewport[1], leftViewport[2], leftViewport[3]);
            leftRenderer.SetBackground(.6, .5, .4);

            vtkRenderer rightRenderer = vtkRenderer.New();
            renderWindow.AddRenderer(rightRenderer);
            rightRenderer.SetViewport(rightViewport[0], rightViewport[1], rightViewport[2], rightViewport[3]);
            rightRenderer.SetBackground(.4, .5, .6);

            // Add the sphere to the left and the cube to the right
            leftRenderer.AddActor(inputActor);
            rightRenderer.AddActor(decimatedActor);
            leftRenderer.ResetCamera();
            rightRenderer.ResetCamera();
            renderWindow.Render();
        }

        public static void DijkstraGraphGeodesicPath(RenderWindowControl renderWindowControl1)
        {
            // Create a sphere
            vtkSphereSource sphereSource = vtkSphereSource.New();
            sphereSource.Update();

            vtkDijkstraGraphGeodesicPath dijkstra = vtkDijkstraGraphGeodesicPath.New();
            dijkstra.SetInputConnection(sphereSource.GetOutputPort());
            dijkstra.SetStartVertex(0);
            dijkstra.SetEndVertex(10);
            dijkstra.Update();

            // Create a mapper and actor
            vtkPolyDataMapper pathMapper = vtkPolyDataMapper.New();
            pathMapper.SetInputConnection(dijkstra.GetOutputPort());

            vtkActor pathActor = vtkActor.New();
            pathActor.SetMapper(pathMapper);
            pathActor.GetProperty().SetColor(1, 0, 0); // Red
            pathActor.GetProperty().SetLineWidth(4);

            // Create a mapper and actor
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(sphereSource.GetOutputPort());

            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);

            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.3, 0.6, 0.3);
            // add our actor to the renderer
            renderer.AddActor(actor);
            renderer.AddActor(pathActor);
        }

        public static void ElevationFilter(RenderWindowControl renderWindowControl1)
        {
            // Created a grid of points (heigh/terrian map)
            vtkPoints points = vtkPoints.New();

            uint GridSize = 10;
            for (uint x = 0; x < GridSize; x++)
            {
                for (uint y = 0; y < GridSize; y++)
                {
                    points.InsertNextPoint(x, y, (x + y) / (y + 1));
                }
            }
            double[] bounds = points.GetBounds();

            // Add the grid points to a polydata object
            vtkPolyData inputPolyData = vtkPolyData.New();
            inputPolyData.SetPoints(points);

            // Triangulate the grid points
            vtkDelaunay2D delaunay = vtkDelaunay2D.New();
#if VTK_MAJOR_VERSION_5
            delaunay.SetInput(inputPolyData);
#else
         delaunay.SetInputData(inputPolyData);
#endif
            delaunay.Update();

            vtkElevationFilter elevationFilter = vtkElevationFilter.New();
            elevationFilter.SetInputConnection(delaunay.GetOutputPort());
            elevationFilter.SetLowPoint(0.0, 0.0, bounds[4]);
            elevationFilter.SetHighPoint(0.0, 0.0, bounds[5]);
            elevationFilter.Update();

            vtkPolyData output = vtkPolyData.New();
            output.ShallowCopy(vtkPolyData.SafeDownCast(elevationFilter.GetOutput()));

            vtkFloatArray elevation =
               vtkFloatArray.SafeDownCast(output.GetPointData().GetArray("Elevation"));

            // Create the color map
            vtkLookupTable colorLookupTable = vtkLookupTable.New();
            colorLookupTable.SetTableRange(bounds[4], bounds[5]);
            colorLookupTable.Build();

            // Generate the colors for each point based on the color map
            vtkUnsignedCharArray colors = vtkUnsignedCharArray.New();
            colors.SetNumberOfComponents(3);
            colors.SetName("Colors");

            for (int i = 0; i < output.GetNumberOfPoints(); i++)
            {
                double val = elevation.GetValue(i);
                Debug.WriteLine("val: " + val);

                double[] dcolor = colorLookupTable.GetColor(val);
                //Debug.WriteLine("dcolor: "
                //          + dcolor[0] + " "
                //          + dcolor[1] + " "
                //          + dcolor[2]);
                byte[] color = new byte[3];
                for (int j = 0; j < 3; j++)
                {
                    color[j] = (byte)(255 * dcolor[j]);
                }
                //Debug.WriteLine("color: "
                //          + color[0] + " "
                //          + color[1] + " "
                //          + color[2]);

                colors.InsertNextTuple3(color[0], color[1], color[2]);
            }

            output.GetPointData().AddArray(colors);

            // Visualize
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
            mapper.SetInputConnection(output.GetProducerPort());
#else
         mapper.SetInputData(output);
#endif

            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);

            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.2, 0.3, 0.4);
            // add our actor to the renderer
            renderer.AddActor(actor);
        }


        public static void ExtractEdges(RenderWindowControl renderWindowControl1)
        {
            vtkSphereSource sphereSource = vtkSphereSource.New();
            sphereSource.Update();

            Debug.WriteLine("Sphere" + Environment.NewLine + "----------");
            Debug.WriteLine("There are " + sphereSource.GetOutput().GetNumberOfCells() + " cells.");
            Debug.WriteLine("There are " + sphereSource.GetOutput().GetNumberOfPoints() + " points.");

            vtkExtractEdges extractEdges = vtkExtractEdges.New();
#if VTK_MAJOR_VERSION_5
            extractEdges.SetInputConnection(sphereSource.GetOutputPort());
#else
         extractEdges.SetInputData(sphereSource);
#endif
            extractEdges.Update();

            vtkCellArray lines = extractEdges.GetOutput().GetLines();
            vtkPoints points = extractEdges.GetOutput().GetPoints();

            Debug.WriteLine(Environment.NewLine + "Edges" + Environment.NewLine + "----------");
            Debug.WriteLine("There are " + lines.GetNumberOfCells() + " cells.");
            Debug.WriteLine("There are " + points.GetNumberOfPoints() + " points.");

            // Traverse all of the edges
            for (int i = 0; i < extractEdges.GetOutput().GetNumberOfCells(); i++)
            {
                //Debug.WriteLine("Type: " + extractEdges.GetOutput().GetCell(i).GetClassName() );
                vtkLine line = vtkLine.SafeDownCast(extractEdges.GetOutput().GetCell(i));
                Debug.WriteLine("Line " + i + " : " + line);
            }

            // Visualize the edges

            // Create a mapper and actor
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
            mapper.SetInputConnection(extractEdges.GetOutputPort());
#else
         mapper.SetInputData(extractEdges);
#endif
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(1, 1, 1);
            // add our actor to the renderer
            renderer.AddActor(actor);
        }

        public static void FillHoles(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            // string filePath = System.IO.Path.Combine(root, @"Data\SyntheticPolyline.vtp");
            // string filePath = System.IO.Path.Combine(root, @"Data\uniform-001371-5x5x5.vtp");
            // string filePath = System.IO.Path.Combine(root, @"Data\political.vtp");
            // string filePath = System.IO.Path.Combine(root, @"Data\filledContours.vtp");
            // string filePath = System.IO.Path.Combine(root, @"Data\disk_out_ref_surface.vtp");
            string filePath = null; // System.IO.Path.Combine(root, @"Data\cow.vtp");

            vtkPolyData input = vtkPolyData.New();
            if (filePath == null)
            {
                GenerateData(ref input);
            }
            else
            {
                vtkXMLPolyDataReader reader = vtkXMLPolyDataReader.New();
                reader.SetFileName(filePath);
                reader.Update();

                input.ShallowCopy(reader.GetOutput());
            }

            vtkFillHolesFilter fillHolesFilter = vtkFillHolesFilter.New();
#if VTK_MAJOR_VERSION_5
            fillHolesFilter.SetInputConnection(input.GetProducerPort());
#else
         fillHolesFilter.SetInputData(input);
#endif

            fillHolesFilter.Update();

            // Create a mapper and actor
            vtkPolyDataMapper originalMapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
            originalMapper.SetInputConnection(input.GetProducerPort());
#else
         originalMapper.SetInputData(input);
#endif

            vtkActor originalActor = vtkActor.New();
            originalActor.SetMapper(originalMapper);

            vtkPolyDataMapper filledMapper = vtkPolyDataMapper.New();
            filledMapper.SetInputConnection(fillHolesFilter.GetOutputPort());

            vtkActor filledActor = vtkActor.New();
            filledActor.SetMapper(filledMapper);

            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // this.Size = new System.Drawing.Size(612, 352);

            // Define viewport ranges
            // (xmin, ymin, xmax, ymax)
            double[] leftViewport = new double[] { 0.0, 0.0, 0.5, 1.0 };
            double[] rightViewport = new double[] { 0.5, 0.0, 1.0, 1.0 };

            // Setup both renderers
            vtkRenderer leftRenderer = vtkRenderer.New();
            renderWindow.AddRenderer(leftRenderer);
            leftRenderer.SetViewport(leftViewport[0], leftViewport[1], leftViewport[2], leftViewport[3]);
            leftRenderer.SetBackground(.6, .5, .4);

            vtkRenderer rightRenderer = vtkRenderer.New();
            renderWindow.AddRenderer(rightRenderer);
            rightRenderer.SetViewport(rightViewport[0], rightViewport[1], rightViewport[2], rightViewport[3]);
            rightRenderer.SetBackground(.4, .5, .6);

            // Add the sphere to the left and the cube to the right
            leftRenderer.AddActor(originalActor);
            rightRenderer.AddActor(filledActor);
            leftRenderer.ResetCamera();
            rightRenderer.ResetCamera();
            renderWindow.Render();
        }


        static void GenerateData(ref vtkPolyData input)
        {
            // Create a sphere
            vtkSphereSource sphereSource = vtkSphereSource.New();
            sphereSource.Update();

            // Remove some cells
            vtkIdTypeArray ids = vtkIdTypeArray.New();
            ids.SetNumberOfComponents(1);

            // Set values
            ids.InsertNextValue(2);
            ids.InsertNextValue(10);

            vtkSelectionNode selectionNode = vtkSelectionNode.New();

            selectionNode.SetFieldType((int)vtkSelectionNode.SelectionField.CELL);
            selectionNode.SetContentType((int)vtkSelectionNode.SelectionContent.INDICES);
            selectionNode.SetSelectionList(ids);
            selectionNode.GetProperties().Set(vtkSelectionNode.INVERSE(), 1); //invert the selection

            vtkSelection selection = vtkSelection.New();
            selection.AddNode(selectionNode);

            vtkExtractSelection extractSelection = vtkExtractSelection.New();
            extractSelection.SetInputConnection(0, sphereSource.GetOutputPort());
#if VTK_MAJOR_VERSION_5
            extractSelection.SetInput(1, selection);
#else
        extractSelection.SetInputData(1, selection);
#endif
            extractSelection.Update();

            // In selection
            vtkDataSetSurfaceFilter surfaceFilter = vtkDataSetSurfaceFilter.New();
            surfaceFilter.SetInputConnection(extractSelection.GetOutputPort());
            surfaceFilter.Update();

            input.ShallowCopy(surfaceFilter.GetOutput());
        }

        public static void GreedyTerrainDecimation(RenderWindowControl renderWindowControl1)
        {
            // Create an image
            vtkImageData image = vtkImageData.New();
            image.SetDimensions(3, 3, 1);
            image.SetNumberOfScalarComponents(1);
            image.SetScalarTypeToUnsignedChar();
            int[] dims = image.GetDimensions();
            unsafe
            {
                for (int i = 0; i < dims[0]; i++)
                {
                    for (int j = 0; j < dims[1]; j++)
                    {
                        byte* ptr = (byte*)image.GetScalarPointer(i, j, 0);
                        *ptr = (byte)vtkMath.Round(vtkMath.Random(0, 1));
                    }
                }
            }
            vtkGreedyTerrainDecimation decimation = vtkGreedyTerrainDecimation.New();
            decimation.SetInputConnection(image.GetProducerPort());
            decimation.Update();
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(decimation.GetOutputPort());
            // actor
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetInterpolationToFlat();
            actor.GetProperty().EdgeVisibilityOn();
            actor.GetProperty().SetEdgeColor(1, 0, 0);

            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.2, 0.3, 0.4);
            // add our actor to the renderer
            renderer.AddActor(actor);
        }

        public static void HighLightBadCells(RenderWindowControl renderWindowControl1)
        {
            vtkSphereSource sphereSource = vtkSphereSource.New();
            sphereSource.Update();

            vtkTriangleFilter triangleFilter = vtkTriangleFilter.New();
            triangleFilter.SetInputConnection(sphereSource.GetOutputPort());
            triangleFilter.Update();

            //Create a mapper and actor
            vtkDataSetMapper sphereMapper = vtkDataSetMapper.New();
            sphereMapper.SetInputConnection(triangleFilter.GetOutputPort());
            vtkActor sphereActor = vtkActor.New();
            sphereActor.SetMapper(sphereMapper);

            vtkPolyData mesh = triangleFilter.GetOutput();
            Debug.WriteLine("There are " + mesh.GetNumberOfCells() + " cells.");

            vtkMeshQuality qualityFilter = vtkMeshQuality.New();
#if VTK_MAJOR_VERSION_5
            qualityFilter.SetInput(mesh);
#else
         qualityFilter.SetInputData(mesh);
#endif
            qualityFilter.SetTriangleQualityMeasureToArea();
            qualityFilter.Update();

            vtkDataSet qualityMesh = qualityFilter.GetOutput();
            vtkDoubleArray qualityArray = vtkDoubleArray.SafeDownCast(qualityMesh.GetCellData().GetArray("Quality"));
            Debug.WriteLine("There are " + qualityArray.GetNumberOfTuples() + " values.");

            for (int i = 0; i < qualityArray.GetNumberOfTuples(); i++)
            {
                double val = qualityArray.GetValue(i);
                Debug.WriteLine("value " + i + ": " + val);
            }

            vtkThreshold selectCells = vtkThreshold.New();
            selectCells.ThresholdByLower(.02);
            selectCells.SetInputArrayToProcess(
               0,
               0,
               0,
               1, // POINTS = 0, CELLS = 1, NONE = 2, POINTS_THEN_CELLS = 3, VERTICES = 4, EDGES = 5, ROWS = 6
               0  // SCALARS = 0, VECTORS = 1, NORMALS = 2, TCOORDS = 3, TENSORS = 4, GLOBALIDS = 5, PEDIGREEIDS = 6, EDGEFLAG = 7
               );

#if VTK_MAJOR_VERSION_5
            selectCells.SetInput(qualityMesh);
#else
         selectCells.SetInputData(qualityMesh);
#endif
            selectCells.Update();
            vtkUnstructuredGrid ug = selectCells.GetOutput();

            // Create a mapper and actor
            vtkDataSetMapper mapper = vtkDataSetMapper.New();
#if VTK_MAJOR_VERSION_5
            mapper.SetInput(ug);
#else
         mapper.SetInputData(ug);
#endif
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetColor(1.0, 0.0, 0.0);
            actor.GetProperty().SetRepresentationToWireframe();
            actor.GetProperty().SetLineWidth(5);
            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(1.0, 1.0, 1.0);
            // add our actors to the renderer
            renderer.AddActor(actor);
            renderer.AddActor(sphereActor);
        }

        public static void vtkPolyDataConnectivityFilter_LargestRegion(RenderWindowControl renderWindowControl1)
        {
            // Small sphere
            vtkSphereSource sphereSource1 = vtkSphereSource.New();
            sphereSource1.Update();

            // Large sphere
            vtkSphereSource sphereSource2 = vtkSphereSource.New();
            sphereSource2.SetRadius(10);
            sphereSource2.SetCenter(25, 0, 0);
            sphereSource2.SetThetaResolution(10);
            sphereSource2.SetPhiResolution(10);
            sphereSource2.Update();

            vtkAppendPolyData appendFilter = vtkAppendPolyData.New();
            appendFilter.AddInputConnection(sphereSource1.GetOutputPort());
            appendFilter.AddInputConnection(sphereSource2.GetOutputPort());
            appendFilter.Update();

            vtkPolyDataConnectivityFilter connectivityFilter = vtkPolyDataConnectivityFilter.New();
            connectivityFilter.SetInputConnection(appendFilter.GetOutputPort());
            connectivityFilter.SetExtractionModeToLargestRegion();
            connectivityFilter.Update();

            // Create a mapper and actor for original data
            vtkPolyDataMapper originalMapper = vtkPolyDataMapper.New();
            originalMapper.SetInputConnection(appendFilter.GetOutputPort());
            originalMapper.Update();

            vtkActor originalActor = vtkActor.New();
            originalActor.SetMapper(originalMapper);

            // Create a mapper and actor for extracted data
            vtkPolyDataMapper extractedMapper = vtkPolyDataMapper.New();
            extractedMapper.SetInputConnection(connectivityFilter.GetOutputPort());
            extractedMapper.Update();

            vtkActor extractedActor = vtkActor.New();
            extractedActor.GetProperty().SetColor(1, 0, 0);
            extractedActor.SetMapper(extractedMapper);
            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.2, 0.3, 0.4);
            // add our actor to the renderer
            renderer.AddActor(originalActor);
            renderer.AddActor(extractedActor);
        }

        public static void MatrixMathFilter(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\tensors.vtk");
            vtkUnstructuredGridReader reader = vtkUnstructuredGridReader.New();
            reader.SetFileName(filePath);
            reader.Update();

            vtkDataSetSurfaceFilter surfaceFilter = vtkDataSetSurfaceFilter.New();
            surfaceFilter.SetInputConnection(reader.GetOutputPort());
            surfaceFilter.Update();

            vtkMatrixMathFilter matrixMathFilter = vtkMatrixMathFilter.New();
            //matrixMathFilter.SetOperationToDeterminant();
            matrixMathFilter.SetOperationToEigenvalue();
            matrixMathFilter.SetInputConnection(surfaceFilter.GetOutputPort());
            matrixMathFilter.Update();
            matrixMathFilter.GetOutput().GetPointData().SetActiveScalars("Eigenvalue");

            vtkXMLPolyDataWriter writer = vtkXMLPolyDataWriter.New();
            writer.SetInputConnection(matrixMathFilter.GetOutputPort());
            writer.SetFileName(System.IO.Path.Combine(root, @"Data\output.vtp"));
            writer.Write();

            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(matrixMathFilter.GetOutputPort());

            // actor
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.2, 0.3, 0.4);
            // add our actor to the renderer
            renderer.AddActor(actor);
        }

        public static void OBBDicer(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            // string filePath = System.IO.Path.Combine(root, @"Data\SyntheticPolyline.vtp");
            // string filePath = System.IO.Path.Combine(root, @"Data\uniform-001371-5x5x5.vtp");
            // string filePath = System.IO.Path.Combine(root, @"Data\political.vtp");
            // string filePath = System.IO.Path.Combine(root, @"Data\filledContours.vtp");
            // string filePath = System.IO.Path.Combine(root, @"Data\disk_out_ref_surface.vtp");
            string filePath = System.IO.Path.Combine(root, @"Data\cow.vtp");

            vtkPolyData inputPolyData;
            if (filePath != null)
            {
                vtkXMLPolyDataReader reader = vtkXMLPolyDataReader.New();
                reader.SetFileName(filePath);
                reader.Update();
                inputPolyData = reader.GetOutput();
            }
            else
            {
                vtkSphereSource sphereSource = vtkSphereSource.New();
                sphereSource.SetThetaResolution(30);
                sphereSource.SetPhiResolution(15);
                sphereSource.Update();
                inputPolyData = sphereSource.GetOutput();
            }

            // Create pipeline
            vtkOBBDicer dicer = vtkOBBDicer.New();
#if VTK_MAJOR_VERSION_5
            dicer.SetInput(inputPolyData);
#else
         dicer.SetInputData(inputPolyData);
#endif
            dicer.SetNumberOfPieces(4);
            dicer.SetDiceModeToSpecifiedNumberOfPieces();
            dicer.Update();

            vtkPolyDataMapper inputMapper = vtkPolyDataMapper.New();
            inputMapper.SetInputConnection(dicer.GetOutputPort());
            inputMapper.SetScalarRange(0, dicer.GetNumberOfActualPieces());

            Debug.WriteLine("Asked for: "
                      + dicer.GetNumberOfPieces() + " pieces, got: "
                      + dicer.GetNumberOfActualPieces());

            vtkActor inputActor = vtkActor.New();
            inputActor.SetMapper(inputMapper);
            inputActor.GetProperty().SetInterpolationToFlat();

            vtkOutlineCornerFilter outline = vtkOutlineCornerFilter.New();
#if VTK_MAJOR_VERSION_5
            outline.SetInput(inputPolyData);
#else
         outline.SetInputData(inputPolyData);
#endif

            vtkPolyDataMapper outlineMapper = vtkPolyDataMapper.New();
            outlineMapper.SetInputConnection(outline.GetOutputPort());

            vtkActor outlineActor = vtkActor.New();
            outlineActor.SetMapper(outlineMapper);
            outlineActor.GetProperty().SetColor(0, 0, 0);
            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(.2, .3, .4);
            // add our actor to the renderer
            renderer.AddActor(inputActor);
            renderer.AddActor(outlineActor);
        }

        public static void PolygonalSurfaceContourLineInterpolator(RenderWindowControl renderWindowControl1)
        {
            vtkPolyData polyData;
            vtkSphereSource sphereSource = vtkSphereSource.New();
            sphereSource.SetThetaResolution(40);
            sphereSource.SetPhiResolution(20);
            sphereSource.Update();

            polyData = sphereSource.GetOutput();
            // The Dijkstra interpolator will not accept cells that aren't triangles
            vtkTriangleFilter triangleFilter = vtkTriangleFilter.New();
#if VTK_MAJOR_VERSION_5
            triangleFilter.SetInput(polyData);
#else
         triangleFilter.SetInputData( polyData );
#endif
            triangleFilter.Update();

            vtkPolyData pd = triangleFilter.GetOutput();

            //Create a mapper and actor
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(triangleFilter.GetOutputPort());

            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetInterpolationToFlat();

            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.3, 0.4, 0.5);
            // add our actor to the renderer
            renderer.AddActor(actor);

            // Here comes the contour widget stuff.....
            vtkContourWidget contourWidget = vtkContourWidget.New();
            contourWidget.SetInteractor(renderWindow.GetInteractor());
            vtkOrientedGlyphContourRepresentation rep =
               vtkOrientedGlyphContourRepresentation.SafeDownCast(
                  contourWidget.GetRepresentation());
            rep.GetLinesProperty().SetColor(1, 0.2, 0);
            rep.GetLinesProperty().SetLineWidth(3.0f);

            vtkPolygonalSurfacePointPlacer pointPlacer =
               vtkPolygonalSurfacePointPlacer.New();
            pointPlacer.AddProp(actor);
            pointPlacer.GetPolys().AddItem(pd);
            rep.SetPointPlacer(pointPlacer);

            vtkPolygonalSurfaceContourLineInterpolator interpolator =
               vtkPolygonalSurfaceContourLineInterpolator.New();
            interpolator.GetPolys().AddItem(pd);
            rep.SetLineInterpolator(interpolator);

            renderWindow.Render();
            contourWidget.EnabledOn();
        }

        public static void QuadricClustering(RenderWindowControl renderWindowControl1)
        {
            vtkSphereSource sphereSource = vtkSphereSource.New();
            sphereSource.Update();

            vtkPolyData input = vtkPolyData.New();
            input.ShallowCopy(sphereSource.GetOutput());

            Debug.WriteLine("Before decimation" + Environment.NewLine + "------------");
            Debug.WriteLine("There are " + input.GetNumberOfPoints() + " points.");
            Debug.WriteLine("There are " + input.GetNumberOfPolys() + " polygons.");

            vtkQuadricClustering decimate = vtkQuadricClustering.New();
#if VTK_MAJOR_VERSION_5
            decimate.SetInputConnection(input.GetProducerPort());
#else
         decimate.SetInputData(input);
#endif
            decimate.Update();

            vtkPolyData decimated = vtkPolyData.New();
            decimated.ShallowCopy(decimate.GetOutput());

            Debug.WriteLine("After decimation" + Environment.NewLine + "------------");

            Debug.WriteLine("There are " + decimated.GetNumberOfPoints() + " points.");
            Debug.WriteLine("There are " + decimated.GetNumberOfPolys() + " polygons.");

            vtkPolyDataMapper inputMapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
            inputMapper.SetInputConnection(input.GetProducerPort());
#else
         inputMapper.SetInputData(input);
#endif
            vtkActor inputActor = vtkActor.New();
            inputActor.SetMapper(inputMapper);

            vtkPolyDataMapper decimatedMapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
            decimatedMapper.SetInputConnection(decimated.GetProducerPort());
#else
         decimatedMapper.SetInputData(decimated);
#endif
            vtkActor decimatedActor = vtkActor.New();
            decimatedActor.SetMapper(decimatedMapper);

            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            //this.Size = new System.Drawing.Size(612, 352);

            // Define viewport ranges
            // (xmin, ymin, xmax, ymax)
            double[] leftViewport = new double[] { 0.0, 0.0, 0.5, 1.0 };
            double[] rightViewport = new double[] { 0.5, 0.0, 1.0, 1.0 };

            // Setup both renderers
            vtkRenderer leftRenderer = vtkRenderer.New();
            renderWindow.AddRenderer(leftRenderer);
            leftRenderer.SetViewport(leftViewport[0], leftViewport[1], leftViewport[2], leftViewport[3]);
            leftRenderer.SetBackground(.6, .5, .4);

            vtkRenderer rightRenderer = vtkRenderer.New();
            renderWindow.AddRenderer(rightRenderer);
            rightRenderer.SetViewport(rightViewport[0], rightViewport[1], rightViewport[2], rightViewport[3]);
            rightRenderer.SetBackground(.4, .5, .6);

            // Add the sphere to the left and the cube to the right
            leftRenderer.AddActor(inputActor);
            rightRenderer.AddActor(decimatedActor);
            leftRenderer.ResetCamera();
            rightRenderer.ResetCamera();
            renderWindow.Render();
        }


        public static void QuadricDecimation(RenderWindowControl renderWindowControl1)
        {
            vtkSphereSource sphereSource = vtkSphereSource.New();
            sphereSource.Update();

            vtkPolyData input = vtkPolyData.New();
            input.ShallowCopy(sphereSource.GetOutput());

            Debug.WriteLine("Before decimation" + Environment.NewLine + "------------");
            Debug.WriteLine("There are " + input.GetNumberOfPoints() + " points.");
            Debug.WriteLine("There are " + input.GetNumberOfPolys() + " polygons.");

            vtkQuadricDecimation decimate = vtkQuadricDecimation.New();
#if VTK_MAJOR_VERSION_5
            decimate.SetInputConnection(input.GetProducerPort());
#else
         decimate.SetInputData(input);
#endif
            decimate.Update();

            vtkPolyData decimated = vtkPolyData.New();
            decimated.ShallowCopy(decimate.GetOutput());

            Debug.WriteLine("After decimation" + Environment.NewLine + "------------");

            Debug.WriteLine("There are " + decimated.GetNumberOfPoints() + " points.");
            Debug.WriteLine("There are " + decimated.GetNumberOfPolys() + " polygons.");

            vtkPolyDataMapper inputMapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
            inputMapper.SetInputConnection(input.GetProducerPort());
#else
         inputMapper.SetInputData(input);
#endif
            vtkActor inputActor = vtkActor.New();
            inputActor.SetMapper(inputMapper);

            vtkPolyDataMapper decimatedMapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
            decimatedMapper.SetInputConnection(decimated.GetProducerPort());
#else
         decimatedMapper.SetInputData(decimated);
#endif
            vtkActor decimatedActor = vtkActor.New();
            decimatedActor.SetMapper(decimatedMapper);

            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            //this.Size = new System.Drawing.Size(612, 352);

            // Define viewport ranges
            // (xmin, ymin, xmax, ymax)
            double[] leftViewport = new double[] { 0.0, 0.0, 0.5, 1.0 };
            double[] rightViewport = new double[] { 0.5, 0.0, 1.0, 1.0 };

            // Setup both renderers
            vtkRenderer leftRenderer = vtkRenderer.New();
            renderWindow.AddRenderer(leftRenderer);
            leftRenderer.SetViewport(leftViewport[0], leftViewport[1], leftViewport[2], leftViewport[3]);
            leftRenderer.SetBackground(.6, .5, .4);

            vtkRenderer rightRenderer = vtkRenderer.New();
            renderWindow.AddRenderer(rightRenderer);
            rightRenderer.SetViewport(rightViewport[0], rightViewport[1], rightViewport[2], rightViewport[3]);
            rightRenderer.SetBackground(.4, .5, .6);

            // Add the sphere to the left and the cube to the right
            leftRenderer.AddActor(inputActor);
            rightRenderer.AddActor(decimatedActor);
            leftRenderer.ResetCamera();
            rightRenderer.ResetCamera();
            renderWindow.Render();
        }

        public static void SelectPolyData(RenderWindowControl renderWindowControl1)
        {
            vtkSphereSource sphereSource = vtkSphereSource.New();
            sphereSource.Update();

            vtkPoints selectionPoints = vtkPoints.New();

            selectionPoints.InsertPoint(0, -0.16553, 0.135971, 0.451972);
            selectionPoints.InsertPoint(1, -0.0880123, -0.134952, 0.4747);
            selectionPoints.InsertPoint(2, 0.00292618, -0.134604, 0.482459);
            selectionPoints.InsertPoint(3, 0.0641941, 0.067112, 0.490947);
            selectionPoints.InsertPoint(4, 0.15577, 0.0734765, 0.469245);
            selectionPoints.InsertPoint(5, 0.166667, -0.129217, 0.454622);
            selectionPoints.InsertPoint(6, 0.241259, -0.123363, 0.420581);
            selectionPoints.InsertPoint(7, 0.240334, 0.0727106, 0.432555);
            selectionPoints.InsertPoint(8, 0.308529, 0.0844311, 0.384357);
            selectionPoints.InsertPoint(9, 0.32672, -0.121674, 0.359187);
            selectionPoints.InsertPoint(10, 0.380721, -0.117342, 0.302527);
            selectionPoints.InsertPoint(11, 0.387804, 0.0455074, 0.312375);
            selectionPoints.InsertPoint(12, 0.43943, -0.111673, 0.211707);
            selectionPoints.InsertPoint(13, 0.470984, -0.0801913, 0.147919);
            selectionPoints.InsertPoint(14, 0.436777, 0.0688872, 0.233021);
            selectionPoints.InsertPoint(15, 0.44874, 0.188852, 0.109882);
            selectionPoints.InsertPoint(16, 0.391352, 0.254285, 0.176943);
            selectionPoints.InsertPoint(17, 0.373274, 0.154162, 0.294296);
            selectionPoints.InsertPoint(18, 0.274659, 0.311654, 0.276609);
            selectionPoints.InsertPoint(19, 0.206068, 0.31396, 0.329702);
            selectionPoints.InsertPoint(20, 0.263789, 0.174982, 0.387308);
            selectionPoints.InsertPoint(21, 0.213034, 0.175485, 0.417142);
            selectionPoints.InsertPoint(22, 0.169113, 0.261974, 0.390286);
            selectionPoints.InsertPoint(23, 0.102552, 0.25997, 0.414814);
            selectionPoints.InsertPoint(24, 0.131512, 0.161254, 0.454705);
            selectionPoints.InsertPoint(25, 0.000192443, 0.156264, 0.475307);
            selectionPoints.InsertPoint(26, -0.0392091, 0.000251724, 0.499943);
            selectionPoints.InsertPoint(27, -0.096161, 0.159646, 0.46438);

            vtkSelectPolyData loop = vtkSelectPolyData.New();
            loop.SetInputConnection(sphereSource.GetOutputPort());
            loop.SetLoop(selectionPoints);
            loop.GenerateSelectionScalarsOn();
            loop.SetSelectionModeToSmallestRegion(); //negative scalars inside

            vtkClipPolyData clip = vtkClipPolyData.New(); //clips out positive region

            clip.SetInputConnection(loop.GetOutputPort());

            vtkPolyDataMapper clipMapper = vtkPolyDataMapper.New();
            clipMapper.SetInputConnection(clip.GetOutputPort());

            vtkLODActor clipActor = vtkLODActor.New();
            clipActor.SetMapper(clipMapper);

            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(.1, .2, .4);
            renderWindow.SetSize(500, 250);
            // add our actor to the renderer
            renderer.AddActor(clipActor);
        }

        public static void SimpleElevationFilter(RenderWindowControl renderWindowControl1)
        {
            // Created a grid of points (heigh/terrian map)
            vtkPoints points = vtkPoints.New();

            uint GridSize = 10;
            for (uint x = 0; x < GridSize; x++)
            {
                for (uint y = 0; y < GridSize; y++)
                {
                    points.InsertNextPoint(x, y, (x + y) / (y + 1));
                }
            }
            double[] bounds = points.GetBounds();

            // Add the grid points to a polydata object
            vtkPolyData inputPolyData = vtkPolyData.New();
            inputPolyData.SetPoints(points);

            // Triangulate the grid points
            vtkDelaunay2D delaunay = vtkDelaunay2D.New();
#if VTK_MAJOR_VERSION_5
            delaunay.SetInput(inputPolyData);
#else
         delaunay.SetInputData(inputPolyData);
#endif
            delaunay.Update();

            vtkSimpleElevationFilter elevationFilter = vtkSimpleElevationFilter.New();
            elevationFilter.SetInputConnection(delaunay.GetOutputPort());
            elevationFilter.SetVector(0.0, 0.0, 1.0);
            elevationFilter.Update();

            vtkPolyData output = vtkPolyData.New();
            output.ShallowCopy(vtkPolyData.SafeDownCast(elevationFilter.GetOutput()));

            vtkFloatArray elevation =
               vtkFloatArray.SafeDownCast(output.GetPointData().GetArray("Elevation"));

            // Create the color map
            vtkLookupTable colorLookupTable = vtkLookupTable.New();
            colorLookupTable.SetTableRange(bounds[4], bounds[5]);
            colorLookupTable.Build();

            // Generate the colors for each point based on the color map
            vtkUnsignedCharArray colors = vtkUnsignedCharArray.New();
            colors.SetNumberOfComponents(3);
            colors.SetName("Colors");

            for (int i = 0; i < output.GetNumberOfPoints(); i++)
            {
                double val = elevation.GetValue(i);
                Debug.WriteLine("val: " + val);

                double[] dcolor = colorLookupTable.GetColor(val);
                //Debug.WriteLine("dcolor: "
                //          + dcolor[0] + " "
                //          + dcolor[1] + " "
                //          + dcolor[2]);
                byte[] color = new byte[3];
                for (int j = 0; j < 3; j++)
                {
                    color[j] = (byte)(255 * dcolor[j]);
                }
                //Debug.WriteLine("color: "
                //          + color[0] + " "
                //          + color[1] + " "
                //          + color[2]);

                colors.InsertNextTuple3(color[0], color[1], color[2]);
            }

            output.GetPointData().AddArray(colors);

            // Visualize
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
            mapper.SetInputConnection(output.GetProducerPort());
#else
         mapper.SetInputData(output);
#endif

            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);

            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.2, 0.3, 0.4);
            // add our actor to the renderer
            renderer.AddActor(actor);
        }

        public static void SolidClip(RenderWindowControl renderWindowControl1)
        {
            // Create a superquadric
            vtkSuperquadricSource superquadricSource = vtkSuperquadricSource.New();
            superquadricSource.SetPhiRoundness(3.1);
            superquadricSource.SetThetaRoundness(2.2);

            // Define a clipping plane
            vtkPlane clipPlane = vtkPlane.New();
            clipPlane.SetNormal(1.0, -1.0, -1.0);
            clipPlane.SetOrigin(0.0, 0.0, 0.0);

            // Clip the source with the plane
            vtkClipPolyData clipper = vtkClipPolyData.New();
#if VTK_MAJOR_VERSION_5
            clipper.SetInputConnection(superquadricSource.GetOutputPort());
#else
         clipper.SetInputData(superquadricSource);
#endif
            clipper.SetClipFunction(clipPlane);

            //Create a mapper and actor
            vtkPolyDataMapper superquadricMapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
            superquadricMapper.SetInputConnection(clipper.GetOutputPort());
#else
         superquadricMapper.SetInputData(clipper);
#endif
            vtkActor superquadricActor = vtkActor.New();
            superquadricActor.SetMapper(superquadricMapper);

            // Create a property to be used for the back faces. Turn off all
            // shading by specifying 0 weights for specular and diffuse. Max the
            // ambient.
            vtkProperty backFaces = vtkProperty.New();
            backFaces.SetSpecular(0.0);
            backFaces.SetDiffuse(0.0);
            backFaces.SetAmbient(1.0);
            backFaces.SetAmbientColor(1.0000, 0.3882, 0.2784);

            superquadricActor.SetBackfaceProperty(backFaces);
            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // add our actor to the renderer
            renderer.AddActor(superquadricActor);
        }

        public static void vtkPolyDataConnectivityFilter_SpecifiedRegion(RenderWindowControl renderWindowControl1)
        {
            // Small sphere (first region)
            vtkSphereSource sphereSource1 = vtkSphereSource.New();
            sphereSource1.Update();

            // Large sphere (second region)
            vtkSphereSource sphereSource2 = vtkSphereSource.New();
            sphereSource2.SetRadius(10);
            sphereSource2.SetCenter(25, 0, 0);
            sphereSource2.Update();

            vtkAppendPolyData appendFilter = vtkAppendPolyData.New();
            appendFilter.AddInputConnection(sphereSource1.GetOutputPort());
            appendFilter.AddInputConnection(sphereSource2.GetOutputPort());
            appendFilter.Update();

            vtkPolyDataConnectivityFilter connectivityFilter = vtkPolyDataConnectivityFilter.New();
            connectivityFilter.SetInputConnection(appendFilter.GetOutputPort());
            connectivityFilter.SetExtractionModeToSpecifiedRegions();
            connectivityFilter.AddSpecifiedRegion(1); //select the region to extract here
            connectivityFilter.Update();

            // Create a mapper and actor for original data
            vtkPolyDataMapper originalMapper = vtkPolyDataMapper.New();
            originalMapper.SetInputConnection(appendFilter.GetOutputPort());
            originalMapper.Update();

            vtkActor originalActor = vtkActor.New();
            originalActor.SetMapper(originalMapper);

            // Create a mapper and actor for extracted data
            vtkPolyDataMapper extractedMapper = vtkPolyDataMapper.New();
            extractedMapper.SetInputConnection(connectivityFilter.GetOutputPort());
            extractedMapper.Update();

            vtkActor extractedActor = vtkActor.New();
            extractedActor.GetProperty().SetColor(1, 0, 0);
            extractedActor.SetMapper(extractedMapper);
            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.2, 0.3, 0.4);
            // add our actor to the renderer
            renderer.AddActor(originalActor);
            renderer.AddActor(extractedActor);
        }

        public static void Subdivision(RenderWindowControl renderWindowControl1)
        {

            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            // string filePath = System.IO.Path.Combine(root, @"Data\SyntheticPolyline.vtp");
            // string filePath = System.IO.Path.Combine(root, @"Data\uniform-001371-5x5x5.vtp");
            // string filePath = System.IO.Path.Combine(root, @"Data\political.vtp");
            // string filePath = System.IO.Path.Combine(root, @"Data\filledContours.vtp");
            // string filePath = System.IO.Path.Combine(root, @"Data\disk_out_ref_surface.vtp");
            string filePath = System.IO.Path.Combine(root, @"Data\cow.vtp");

            vtkPolyData originalMesh;
            if (filePath != null)
            {
                vtkXMLPolyDataReader reader = vtkXMLPolyDataReader.New();
                reader.SetFileName(filePath);
                // Subdivision filters only work on triangles
                vtkTriangleFilter triangles = vtkTriangleFilter.New();
                triangles.SetInputConnection(reader.GetOutputPort());
                triangles.Update();
                originalMesh = triangles.GetOutput();
            }
            else
            {
                vtkSphereSource sphereSource = vtkSphereSource.New();
                sphereSource.Update();
                originalMesh = sphereSource.GetOutput();
            }
            Debug.WriteLine("Before subdivision");
            Debug.WriteLine("    There are " + originalMesh.GetNumberOfPoints()
               + " points.");
            Debug.WriteLine("    There are " + originalMesh.GetNumberOfPolys()
               + " triangles.");

            int numberOfViewports = 3;

            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            //this.Size = new System.Drawing.Size(200 * numberOfViewports + 12, 252);
            //this.Text += " - Subdivision";
            Random rnd = new Random(2);
            int numberOfSubdivisions = 2;

            // Create one text property for all
            vtkTextProperty textProperty = vtkTextProperty.New();
            textProperty.SetFontSize(14);
            textProperty.SetJustificationToCentered();

            for (int i = 0; i < numberOfViewports; i++)
            {
                // Note: Here we create a superclass pointer (vtkPolyDataAlgorithm) so that we can easily instantiate different
                // types of subdivision filters. Typically you would not want to do this, but rather create the pointer to be the type
                // filter you will actually use, e.g. 
                // <vtkLinearSubdivisionFilter>  subdivisionFilter = <vtkLinearSubdivisionFilter>.New();
                vtkPolyDataAlgorithm subdivisionFilter;
                switch (i)
                {
                    case 0:
                        subdivisionFilter = vtkLinearSubdivisionFilter.New();
                        ((vtkLinearSubdivisionFilter)subdivisionFilter).SetNumberOfSubdivisions(numberOfSubdivisions);
                        break;
                    case 1:
                        subdivisionFilter = vtkLoopSubdivisionFilter.New();
                        ((vtkLoopSubdivisionFilter)subdivisionFilter).SetNumberOfSubdivisions(numberOfSubdivisions);
                        break;
                    case 2:
                        subdivisionFilter = vtkButterflySubdivisionFilter.New();
                        ((vtkButterflySubdivisionFilter)subdivisionFilter).SetNumberOfSubdivisions(numberOfSubdivisions);
                        break;
                    default:
                        subdivisionFilter = vtkLinearSubdivisionFilter.New();
                        ((vtkLinearSubdivisionFilter)subdivisionFilter).SetNumberOfSubdivisions(numberOfSubdivisions);
                        break;
                }
#if VTK_MAJOR_VERSION_5
                subdivisionFilter.SetInputConnection(originalMesh.GetProducerPort());
#else
            subdivisionFilter.SetInputData(originalMesh);
#endif
                subdivisionFilter.Update();
                vtkRenderer renderer = vtkRenderer.New();
                renderWindow.AddRenderer(renderer);
                renderer.SetViewport((float)i / numberOfViewports, 0, (float)(i + 1) / numberOfViewports, 1);
                renderer.SetBackground(.2 + rnd.NextDouble() / 8, .3 + rnd.NextDouble() / 8, .4 + rnd.NextDouble() / 8);

                vtkTextMapper textMapper = vtkTextMapper.New();
                vtkActor2D textActor = vtkActor2D.New();
                textMapper.SetInput(subdivisionFilter.GetClassName());
                textMapper.SetTextProperty(textProperty);

                textActor.SetMapper(textMapper);
                textActor.SetPosition(100, 16);

                //Create a mapper and actor
                vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
                mapper.SetInputConnection(subdivisionFilter.GetOutputPort());
                vtkActor actor = vtkActor.New();
                actor.SetMapper(mapper);
                renderer.AddActor(actor);
                renderer.AddActor(textActor);
                renderer.ResetCamera();
            }
            renderWindow.Render();
        }

        public static void Triangulate(RenderWindowControl renderWindowControl1)
        {
            vtkRegularPolygonSource polygonSource = vtkRegularPolygonSource.New();
            polygonSource.Update();

            vtkTriangleFilter triangleFilter = vtkTriangleFilter.New();
#if VTK_MAJOR_VERSION_5
            triangleFilter.SetInputConnection(polygonSource.GetOutputPort());
#else
         triangleFilter.SetInputData(polygonSource);
#endif
            triangleFilter.Update();

            vtkPolyDataMapper inputMapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
            inputMapper.SetInputConnection(polygonSource.GetOutputPort());
#else
         inputMapper.SetInputData(polygonSource);
#endif
            vtkActor inputActor = vtkActor.New();
            inputActor.SetMapper(inputMapper);
            inputActor.GetProperty().SetRepresentationToWireframe();

            vtkPolyDataMapper triangleMapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
            triangleMapper.SetInputConnection(triangleFilter.GetOutputPort());
#else
         triangleMapper.SetInputData(triangleFilter);
#endif
            vtkActor triangleActor = vtkActor.New();
            triangleActor.SetMapper(triangleMapper);
            triangleActor.GetProperty().SetRepresentationToWireframe();

            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            //this.Size = new System.Drawing.Size(612, 352);

            // Define viewport ranges
            // (xmin, ymin, xmax, ymax)
            double[] leftViewport = new double[] { 0.0, 0.0, 0.5, 1.0 };
            double[] rightViewport = new double[] { 0.5, 0.0, 1.0, 1.0 };

            // Setup both renderers
            vtkRenderer leftRenderer = vtkRenderer.New();
            renderWindow.AddRenderer(leftRenderer);
            leftRenderer.SetViewport(leftViewport[0], leftViewport[1], leftViewport[2], leftViewport[3]);
            leftRenderer.SetBackground(.6, .5, .4);

            vtkRenderer rightRenderer = vtkRenderer.New();
            renderWindow.AddRenderer(rightRenderer);
            rightRenderer.SetViewport(rightViewport[0], rightViewport[1], rightViewport[2], rightViewport[3]);
            rightRenderer.SetBackground(.4, .5, .6);

            // Add the sphere to the left and the cube to the right
            leftRenderer.AddActor(inputActor);
            rightRenderer.AddActor(triangleActor);
            leftRenderer.ResetCamera();
            rightRenderer.ResetCamera();
            renderWindow.Render();
        }

        public static void WeightedTransformFilter(RenderWindowControl renderWindowControl1)
        {
            // Use a sphere as a basis of the shape
            vtkSphereSource sphere = vtkSphereSource.New();
            sphere.SetPhiResolution(40);
            sphere.SetThetaResolution(40);
            sphere.Update();

            vtkPolyData sphereData = sphere.GetOutput();

            // Create a data array to hold the weighting coefficients
            vtkFloatArray tfarray = vtkFloatArray.New();
            int npoints = (int)sphereData.GetNumberOfPoints();
            tfarray.SetNumberOfComponents(2);
            tfarray.SetNumberOfTuples(npoints);

            // Parameterize the sphere along the z axis, and fill the weights
            // with (1.0-a, a) to linearly interpolate across the shape
            IntPtr pPoint = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * 3);
            double[] point = new double[3];
            for (int i = 0; i < npoints; i++)
            {
                sphereData.GetPoint(i, pPoint);
                Marshal.Copy(pPoint, point, 0, 3);
                double x = point[0];
                double y = point[1];
                double z = point[2];

                double zn = z + 0.5;
                double zn1 = 1.0 - zn;
                if (zn > 1.0)
                    zn = 1.0;
                if (zn1 < 0.0)
                    zn1 = 0.0;

                tfarray.SetComponent(i, 0, zn1);
                tfarray.SetComponent(i, 1, zn);
            }
            Marshal.FreeHGlobal(pPoint);

            // Create field data to hold the array, and bind it to the sphere
            vtkFieldData fd = vtkFieldData.New();
            tfarray.SetName("weights");
            sphereData.GetPointData().AddArray(tfarray);

            // Use an ordinary transform to stretch the shape
            vtkTransform stretch = vtkTransform.New();
            stretch.Scale(1, 1, 3.2);

            vtkTransformFilter stretchFilter = vtkTransformFilter.New();
            stretchFilter.SetInputConnection(sphereData.GetProducerPort());
            stretchFilter.SetTransform(stretch);

            // Now, for the weighted transform stuff
            vtkWeightedTransformFilter weightedTrans = vtkWeightedTransformFilter.New();

            // Create two transforms to interpolate between
            vtkTransform identity = vtkTransform.New();
            identity.Identity();

            vtkTransform rotated = vtkTransform.New();
            double rotatedAngle = 45;
            rotated.RotateX(rotatedAngle);

            weightedTrans.SetNumberOfTransforms(2);
            weightedTrans.SetTransform(identity, 0);
            weightedTrans.SetTransform(rotated, 1);
            // which data array should the filter use ?
            weightedTrans.SetWeightArray("weights");

            weightedTrans.SetInputConnection(stretchFilter.GetOutputPort());

            vtkPolyDataMapper weightedTransMapper = vtkPolyDataMapper.New();
            weightedTransMapper.SetInputConnection(weightedTrans.GetOutputPort());
            vtkActor weightedTransActor = vtkActor.New();
            weightedTransActor.SetMapper(weightedTransMapper);
            weightedTransActor.GetProperty().SetDiffuseColor(0.8, 0.8, 0.1);
            weightedTransActor.GetProperty().SetRepresentationToSurface();

            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.2, 0.3, 0.4);
            // add our actor to the renderer
            renderer.AddActor(weightedTransActor);

            renderer.ResetCamera();
            renderer.GetActiveCamera().Azimuth(90);
            renderer.GetActiveCamera().Dolly(1);
        }

        public static void WindowedSincPolyDataFilter(RenderWindowControl renderWindowControl1)
        {
            vtkSphereSource sphereSource = vtkSphereSource.New();
            sphereSource.Update();

            vtkWindowedSincPolyDataFilter smoother = vtkWindowedSincPolyDataFilter.New();
            smoother.SetInputConnection(sphereSource.GetOutputPort());
            smoother.SetNumberOfIterations(15);
            smoother.BoundarySmoothingOff();
            smoother.FeatureEdgeSmoothingOff();
            smoother.SetFeatureAngle(120.0);
            smoother.SetPassBand(.001);
            smoother.NonManifoldSmoothingOn();
            smoother.NormalizeCoordinatesOn();
            smoother.Update();

            vtkPolyDataMapper smoothedMapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
            smoothedMapper.SetInputConnection(smoother.GetOutputPort());
#else
         smoothedMapper.SetInputData(smoother);
#endif
            vtkActor smoothedActor = vtkActor.New();
            smoothedActor.SetMapper(smoothedMapper);

            vtkPolyDataMapper inputMapper = vtkPolyDataMapper.New();
#if VTK_MAJOR_VERSION_5
            inputMapper.SetInputConnection(sphereSource.GetOutputPort());
#else
         inputMapper.SetInputData(sphereSource);
#endif
            vtkActor inputActor = vtkActor.New();
            inputActor.SetMapper(inputMapper);

            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            //this.Size = new System.Drawing.Size(612, 352);

            // Define viewport ranges
            // (xmin, ymin, xmax, ymax)
            double[] leftViewport = new double[] { 0.0, 0.0, 0.5, 1.0 };
            double[] rightViewport = new double[] { 0.5, 0.0, 1.0, 1.0 };

            // Setup both renderers
            vtkRenderer leftRenderer = vtkRenderer.New();
            renderWindow.AddRenderer(leftRenderer);
            leftRenderer.SetViewport(leftViewport[0], leftViewport[1], leftViewport[2], leftViewport[3]);
            leftRenderer.SetBackground(.6, .5, .4);

            vtkRenderer rightRenderer = vtkRenderer.New();
            renderWindow.AddRenderer(rightRenderer);
            rightRenderer.SetViewport(rightViewport[0], rightViewport[1], rightViewport[2], rightViewport[3]);
            rightRenderer.SetBackground(.4, .5, .6);

            // Add the sphere to the left and the cube to the right
            leftRenderer.AddActor(inputActor);
            rightRenderer.AddActor(smoothedActor);
            leftRenderer.ResetCamera();
            rightRenderer.ResetCamera();
            renderWindow.Render();
        }

    }
}
