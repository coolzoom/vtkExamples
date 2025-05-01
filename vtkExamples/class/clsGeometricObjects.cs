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
    class clsGeometricObjects
    {
        public static void Arrow(RenderWindowControl renderWindowControl1)
        {
            // Create two arrows.  
            vtkArrowSource arrowSource01 = vtkArrowSource.New();
            vtkArrowSource arrowSource02 = vtkArrowSource.New();
            arrowSource02.SetShaftResolution(24);   // default = 6
            arrowSource02.SetTipResolution(36);     // default = 6

            // Visualize
            vtkPolyDataMapper mapper01 = vtkPolyDataMapper.New();
            vtkPolyDataMapper mapper02 = vtkPolyDataMapper.New();
            mapper01.SetInputConnection(arrowSource01.GetOutputPort());
            mapper02.SetInputConnection(arrowSource02.GetOutputPort());
            vtkActor actor01 = vtkActor.New();
            vtkActor actor02 = vtkActor.New();
            actor01.SetMapper(mapper01);
            actor02.SetMapper(mapper02);
            actor01.SetPosition(0.0, 0.25, 0.0);
            actor02.SetPosition(0.0, -0.25, 0.0);
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.2, 0.3, 0.4);
            renderer.AddActor(actor01);
            renderer.AddActor(actor02);
            renderer.ResetCamera();
        }

        public static void Axes(RenderWindowControl renderWindowControl1)
        {
            vtkSphereSource sphereSource = vtkSphereSource.New();
            sphereSource.SetCenter(0.0, 0.0, 0.0);
            sphereSource.SetRadius(0.5);

            //create a mapper
            vtkPolyDataMapper sphereMapper = vtkPolyDataMapper.New();
            sphereMapper.SetInputConnection(sphereSource.GetOutputPort());

            // create an actor
            vtkActor sphereActor = vtkActor.New();
            sphereActor.SetMapper(sphereMapper);

            // a renderer and render window
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.2, 0.3, 0.4);

            // add the actors to the scene
            renderer.AddActor(sphereActor);

            vtkAxesActor axes = vtkAxesActor.New();
            // The axes are positioned with a user transform
            vtkTransform transform = vtkTransform.New();
            transform.Translate(0.75, 0.0, 0.0);
            axes.SetUserTransform(transform);
            // properties of the axes labels can be set as follows
            // this sets the x axis label to red
            // axes.GetXAxisCaptionActor2D().GetCaptionTextProperty().SetColor(1,0,0);

            // the actual text of the axis label can be changed:
            // axes.SetXAxisLabelText("test");

            renderer.AddActor(axes);
            // we need to call Render() for the whole renderWindow, 
            // because vtkAxesActor uses an overlayed renderer for the axes label
            // in total we have now two renderer
            renderWindow.Render();
        }

        public static void ColoredLines(RenderWindowControl renderWindowControl1)
        {
            // Create three points. Join (Origin and P0) with a red line and
            // (Origin and P1) with a green line
            double[] origin = new double[] { 0.0, 0.0, 0.0 };
            double[] p0 = new double[] { 1.0, 0.0, 0.0 };
            double[] p1 = new double[] { 0.0, 1.0, 0.0 };

            // Create a vtkPoints object and store the points in it
            vtkPoints pts = vtkPoints.New();
            pts.InsertNextPoint(origin[0], origin[1], origin[2]);
            pts.InsertNextPoint(p0[0], p0[1], p0[2]);
            pts.InsertNextPoint(p1[0], p1[1], p1[2]);

            // Setup two colors - one for each line
            byte[] red = new byte[] { 255, 0, 0 };
            byte[] green = new byte[] { 0, 255, 0 };

            // Setup the colors array
            vtkUnsignedCharArray colors = vtkUnsignedCharArray.New();
            colors.SetNumberOfComponents(3);
            colors.SetName("Colors");

            // Add the colors we created to the colors array
            colors.InsertNextValue(red[0]);
            colors.InsertNextValue(red[1]);
            colors.InsertNextValue(red[2]);

            colors.InsertNextValue(green[0]);
            colors.InsertNextValue(green[1]);
            colors.InsertNextValue(green[2]);

            // Create the first line (between Origin and P0)
            vtkLine line0 = vtkLine.New();
            line0.GetPointIds().SetId(0, 0); //the second 0 is the index of the Origin in the vtkPoints
            line0.GetPointIds().SetId(1, 1); //the second 1 is the index of P0 in the vtkPoints

            // Create the second line (between Origin and P1)
            vtkLine line1 = vtkLine.New();
            line1.GetPointIds().SetId(0, 0); //the second 0 is the index of the Origin in the vtkPoints
            line1.GetPointIds().SetId(1, 2); //2 is the index of P1 in the vtkPoints

            // Create a cell array to store the lines in and add the lines to it
            vtkCellArray lines = vtkCellArray.New();
            lines.InsertNextCell(line0);
            lines.InsertNextCell(line1);

            // Create a polydata to store everything in
            vtkPolyData linesPolyData = vtkPolyData.New();

            // Add the points to the dataset
            linesPolyData.SetPoints(pts);

            // Add the lines to the dataset
            linesPolyData.SetLines(lines);

            // Color the lines - associate the first component (red) of the
            // colors array with the first component of the cell array (line 0)
            // and the second component (green) of the colors array with the
            // second component of the cell array (line 1)
            linesPolyData.GetCellData().SetScalars(colors);

            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInput(linesPolyData);
            // create an actor
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);

            // a renderer and render window
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.2, 0.3, 0.4);

            // add the actors to the scene
            renderer.AddActor(actor);
        }


        public static void Cone(RenderWindowControl renderWindowControl1)
        {
            // Create a cone.  
            vtkConeSource coneSource = vtkConeSource.New();
            // coneSource.SetCapping(1);
            // coneSource.SetRadius(0.5);
            // coneSource.SetResolution(32);

            // Visualize
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(coneSource.GetOutputPort());
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.3, 0.2, 0.1);
            renderer.AddActor(actor);
            renderer.ResetCamera();
        }

        public static void Cube(RenderWindowControl renderWindowControl1)
        {
            // Create a cube.  
            vtkCubeSource cubeSource = vtkCubeSource.New();

            // Visualize
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(cubeSource.GetOutputPort());
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.3, 0.2, 0.1);
            renderer.AddActor(actor);
            renderer.ResetCamera();
        }

        public static void Cylinder(RenderWindowControl renderWindowControl1)
        {
            // Create a cylinder.  
            vtkCylinderSource cylinderSource = vtkCylinderSource.New();
            cylinderSource.SetCenter(0.0, 0.0, 0.0);
            cylinderSource.SetRadius(5.0);
            cylinderSource.SetHeight(7.0);
            cylinderSource.SetResolution(36);
            // Visualize
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(cylinderSource.GetOutputPort());
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.1, 0.3, 0.2);
            renderer.AddActor(actor);
            renderer.ResetCamera();
        }

        public static void Disk(RenderWindowControl renderWindowControl1)
        {
            // Create a disk.  
            vtkDiskSource diskSource = vtkDiskSource.New();
            //diskSource.SetCircumferentialResolution(16);
            //diskSource.SetRadialResolution(16);
            //diskSource.SetInnerRadius(0.25);
            //diskSource.SetOuterRadius(1.25);
            // Visualize
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(diskSource.GetOutputPort());
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.2, 0.3, 0.4);
            renderer.AddActor(actor);
            renderer.ResetCamera();
        }


        public static void Frustum(RenderWindowControl renderWindowControl1)
        {
            // Create a frustum.  
            // in this example we need the renderer first to retrieve the active camera
            // in order to get camera's frustum planes
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            vtkCamera camera = renderer.GetActiveCamera();
            double[] aspect = renderer.GetAspect();
            double aspectRatio = aspect[0] / aspect[1];

            // allocate memory for 24 unmanaged doubles
            int size = Marshal.SizeOf(typeof(double)) * 24;
            IntPtr ptr = Marshal.AllocHGlobal(size);
            camera.GetFrustumPlanes(aspectRatio, ptr);
            // in case we would need this values directly we could copy 
            // the unmanaged double array to a managed array like so:

            // double[] planesArray = new double[24];
            // Marshal.Copy(ptr, planesArray, 0, 24);

            // but fortunately we can forward the IntPtr directly to the function 
            // SetFrustumPlanes()
            vtkPlanes planes = vtkPlanes.New();
            planes.SetFrustumPlanes(ptr);
            // free unmanaged memory
            Marshal.FreeHGlobal(ptr);

            vtkFrustumSource frustumSource = vtkFrustumSource.New();
            frustumSource.SetPlanes(planes);
            frustumSource.Update();

            vtkPolyData frustum = frustumSource.GetOutput();
            // Visualize
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInput(frustum);
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);

            renderer.SetBackground(.2, .1, .3); // Background color dark purple
            renderer.AddActor(actor);
            renderer.ResetCamera();
        }

        public static void GeometricObjectsDemo(RenderWindowControl renderWindowControl1)
        {
            // we create a matrix of 3x3 renderer in our renderwindow
            // each renderer can be interacted with independently from one another
            int rendererSize = 234; // width per renderer
            int gridDimensions = 3;
            //this.Size = new System.Drawing.Size(756, 756);
            Random rnd = new Random(2); // for background color variation

            List<vtkPolyDataAlgorithm> geometricObjectSources = new List<vtkPolyDataAlgorithm>();
            geometricObjectSources.Add(vtkArrowSource.New());
            geometricObjectSources.Add(vtkConeSource.New());
            geometricObjectSources.Add(vtkCubeSource.New());
            geometricObjectSources.Add(vtkCylinderSource.New());
            geometricObjectSources.Add(vtkDiskSource.New());
            geometricObjectSources.Add(vtkLineSource.New());
            geometricObjectSources.Add(vtkRegularPolygonSource.New());
            geometricObjectSources.Add(vtkSphereSource.New());
            geometricObjectSources.Add(vtkEarthSource.New());

            List<vtkRenderer> renderers = new List<vtkRenderer>();
            List<vtkPolyDataMapper> mappers = new List<vtkPolyDataMapper>();
            List<vtkActor> actors = new List<vtkActor>();
            List<vtkTextMapper> textMappers = new List<vtkTextMapper>();
            List<vtkActor2D> textActors = new List<vtkActor2D>();

            // Create one text property for all
            vtkTextProperty textProperty = vtkTextProperty.New();
            textProperty.SetFontSize(18);
            textProperty.SetJustificationToCentered();

            // Create a source, renderer, mapper, and actor
            // for each object 
            for (int i = 0; i < geometricObjectSources.Count; i++)
            {
                geometricObjectSources[i].Update();
                mappers.Add(vtkPolyDataMapper.New());
                mappers[i].SetInputConnection(geometricObjectSources[i].GetOutputPort());

                actors.Add(vtkActor.New());
                actors[i].SetMapper(mappers[i]);

                textMappers.Add(vtkTextMapper.New());
                textMappers[i].SetInput(geometricObjectSources[i].GetClassName());
                textMappers[i].SetTextProperty(textProperty);

                textActors.Add(vtkActor2D.New());
                textActors[i].SetMapper(textMappers[i]);
                textActors[i].SetPosition(rendererSize / 2, 16);

                renderers.Add(vtkRenderer.New());
            }

            // Need a renderer even if there is no actor
            for (int i = geometricObjectSources.Count; i < gridDimensions * gridDimensions; i++)
            {
                renderers.Add(vtkRenderer.New());
            }

            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            renderWindow.SetSize(rendererSize * gridDimensions, rendererSize * gridDimensions);

            for (int row = 0; row < gridDimensions; row++)
            {
                for (int col = 0; col < gridDimensions; col++)
                {
                    int index = row * gridDimensions + col;

                    // (xmin, ymin, xmax, ymax)
                    double[] viewport = new double[] {
                  (col) * rendererSize / (double)(gridDimensions * rendererSize),
                  (gridDimensions - (row+1)) * rendererSize / (double)(gridDimensions * rendererSize),
                  (col+1)*rendererSize / (double)(gridDimensions * rendererSize),
                  (gridDimensions - row) * rendererSize / (double)(gridDimensions * rendererSize)};

                    Debug.WriteLine(viewport[0] + " " + viewport[1] + " " + viewport[2] + " " + viewport[3]);
                    renderWindow.AddRenderer(renderers[index]);
                    IntPtr pViewport = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * 4);
                    Marshal.Copy(viewport, 0, pViewport, 4);
                    renderers[index].SetViewport(pViewport);
                    Marshal.FreeHGlobal(pViewport);
                    if (index > geometricObjectSources.Count - 1)
                        continue;

                    renderers[index].AddActor(actors[index]);
                    renderers[index].AddActor(textActors[index]);
                    renderers[index].SetBackground(.2 + rnd.NextDouble() / 8, .3 + rnd.NextDouble() / 8, .4 + rnd.NextDouble() / 8);
                }
            }
        }

        public static void Hexahedron(RenderWindowControl renderWindowControl1)
        {
            // Setup the coordinates of eight points 
            // (faces must be in counter clockwise order as viewed from the outside)
            double[,] p = new double[,] {
            { 0.0, 0.0, 0.0 },
            { 1.0, 0.0, 0.0 },
            { 1.0, 1.0, 0.0 },
            { 0.0, 1.0, 0.0 },
            { 0.0, 0.0, 1.0 },
            { 1.0, 0.0, 1.0 },
            { 1.0, 1.0, 1.0 },
            { 0.0, 1.0, 1.0 }
         };

            // Create the points
            vtkPoints points = vtkPoints.New();
            for (int i = 0; i < 8; i++)
                points.InsertNextPoint(p[i, 0], p[i, 1], p[i, 2]);

            // Create a hexahedron from the points
            vtkHexahedron hex = vtkHexahedron.New();
            for (int i = 0; i < 8; i++)
                hex.GetPointIds().SetId(i, i);

            // Add the hexahedron to a cell array
            vtkCellArray hexs = vtkCellArray.New();
            hexs.InsertNextCell(hex);

            // Add the points and hexahedron to an unstructured grid
            vtkUnstructuredGrid uGrid = vtkUnstructuredGrid.New();
            uGrid.SetPoints(points);
            uGrid.InsertNextCell(hex.GetCellType(), hex.GetPointIds());

            // Visualize
            vtkDataSetMapper mapper = vtkDataSetMapper.New();
            mapper.SetInput(uGrid);
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetLineWidth(4);
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.2, 0.3, 0.4);
            renderer.AddActor(actor);
            renderer.ResetCamera();
        }

        public static void Line(RenderWindowControl renderWindowControl1)
        {
            // Create a line.  
            vtkLineSource lineSource = vtkLineSource.New();
            // Create two points, P0 and P1
            double[] p0 = new double[] { 1.0, 0.0, 0.0 };
            double[] p1 = new double[] { 0.0, 1.0, 0.0 };

            lineSource.SetPoint1(p0[0], p0[1], p0[2]);
            lineSource.SetPoint2(p1[0], p1[1], p1[2]);

            // Visualize
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(lineSource.GetOutputPort());
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetLineWidth(4);
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.2, 0.3, 0.4);
            renderer.AddActor(actor);
            renderer.ResetCamera();
        }

        public static void LongLine(RenderWindowControl renderWindowControl1)
        {
            // Create five points 
            double[,] p = new double[,] {
            { 0.0, 0.0, 0.0 },
            { 1.0, 0.0, 0.0 },
            { 0.0, 1.0, 0.0 },
            { 0.0, 1.0, 2.0 },
            { 1.0, 2.0, 3.0 }
             };

            // Create a vtkPoints object and store the points in it
            vtkPoints points = vtkPoints.New();
            for (int i = 0; i < 5; i++)
                points.InsertNextPoint(p[i, 0], p[i, 1], p[i, 2]);

            // Create a cell array to store the lines in and add the lines to it
            vtkCellArray lines = vtkCellArray.New();

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
            // Visualize
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInput(linesPolyData);
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetLineWidth(4);
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.2, 0.3, 0.4);
            renderer.AddActor(actor);
            renderer.ResetCamera();
        }

        public static void OrientedArrow(RenderWindowControl renderWindowControl1)
        {
            //Create an arrow.
            vtkArrowSource arrowSource = vtkArrowSource.New();

            // Generate a random start and end point
            vtkMath.RandomSeed(8775070);
            double[] startPoint = new double[]{
            vtkMath.Random(-10,10),
            vtkMath.Random(-10,10),
            vtkMath.Random(-10,10)
         };

            double[] endPoint = new double[]{
            vtkMath.Random(-10,10),
            vtkMath.Random(-10,10),
            vtkMath.Random(-10,10)
         };

            // Compute a basis
            double[] normalizedX = new double[3];
            double[] normalizedY = new double[3];
            double[] normalizedZ = new double[3];

            // The X axis is a vector from start to end
            myMath.Subtract(endPoint, startPoint, ref normalizedX);
            double length = myMath.Norm(normalizedX);
            myMath.Normalize(ref normalizedX);

            // The Z axis is an arbitrary vector cross X
            double[] arbitrary = new double[]{
            vtkMath.Random(-10,10),
            vtkMath.Random(-10,10),
            vtkMath.Random(-10,10)
         };
            myMath.Cross(normalizedX, arbitrary, ref normalizedZ);
            myMath.Normalize(ref normalizedZ);
            // The Y axis is Z cross X
            myMath.Cross(normalizedZ, normalizedX, ref normalizedY);
            vtkMatrix4x4 matrix = vtkMatrix4x4.New();

            // Create the direction cosine matrix
            matrix.Identity();
            for (int i = 0; i < 3; i++)
            {
                matrix.SetElement(i, 0, normalizedX[i]);
                matrix.SetElement(i, 1, normalizedY[i]);
                matrix.SetElement(i, 2, normalizedZ[i]);
            }

            // Apply the transforms
            vtkTransform transform = vtkTransform.New();
            transform.Translate(startPoint[0], startPoint[1], startPoint[2]);
            transform.Concatenate(matrix);
            transform.Scale(length, length, length);


            //Create a mapper and actor for the arrow
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            vtkActor actor = vtkActor.New();
#if USER_MATRIX
         mapper.SetInputConnection(arrowSource.GetOutputPort());
         actor.SetUserMatrix(transform.GetMatrix());
#else
            // Transform the polydata
            vtkTransformPolyDataFilter transformPD = vtkTransformPolyDataFilter.New();
            transformPD.SetTransform(transform);
            transformPD.SetInputConnection(arrowSource.GetOutputPort());
            mapper.SetInputConnection(transformPD.GetOutputPort());
#endif
            actor.SetMapper(mapper);

            // Create spheres for start and end point
            vtkSphereSource sphereStartSource = vtkSphereSource.New();
            sphereStartSource.SetCenter(startPoint[0], startPoint[1], startPoint[2]);
            vtkPolyDataMapper sphereStartMapper = vtkPolyDataMapper.New();
            sphereStartMapper.SetInputConnection(sphereStartSource.GetOutputPort());
            vtkActor sphereStart = vtkActor.New();
            sphereStart.SetMapper(sphereStartMapper);
            sphereStart.GetProperty().SetColor(1.0, 1.0, .3);

            vtkSphereSource sphereEndSource = vtkSphereSource.New();
            sphereEndSource.SetCenter(endPoint[0], endPoint[1], endPoint[2]);
            vtkPolyDataMapper sphereEndMapper = vtkPolyDataMapper.New();
            sphereEndMapper.SetInputConnection(sphereEndSource.GetOutputPort());
            vtkActor sphereEnd = vtkActor.New();
            sphereEnd.SetMapper(sphereEndMapper);
            sphereEnd.GetProperty().SetColor(1.0, .3, .3);

            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.2, 0.3, 0.4);
            renderer.AddActor(actor);
            renderer.AddActor(sphereStart);
            renderer.AddActor(sphereEnd);
            renderer.ResetCamera();
        }

        public static void Plane(RenderWindowControl renderWindowControl1)
        {
            // Create a plane
            vtkPlaneSource planeSource = vtkPlaneSource.New();
            planeSource.SetCenter(1.0, 0.0, 0.0);
            planeSource.SetNormal(1.0, 0.0, 1.0);
            planeSource.Update();

            // Visualize
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(planeSource.GetOutputPort());
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetLineWidth(4);
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.2, 0.3, 0.4);
            renderer.AddActor(actor);
            renderer.ResetCamera();
        }

        public static void Planes(RenderWindowControl renderWindowControl1)
        {
            // in this example we need the renderer first to retrieve the active camera
            // in order to get camera's frustum planes and renderer's aspectratio
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = vtkRenderer.New();
            renderer = renderWindow.GetRenderers().GetFirstRenderer();
            vtkCamera camera = renderer.GetActiveCamera();
            double[] aspect = renderer.GetAspect();
            double aspectRatio = aspect[0] / aspect[1];

            vtkPlanes planes = vtkPlanes.New();
            // one way
            {
                // allocate memory for 24 unmanaged doubles
                int size = Marshal.SizeOf(typeof(double)) * 24;
                IntPtr ptr = Marshal.AllocHGlobal(size);
                camera.GetFrustumPlanes(aspectRatio, ptr);
                // in case we would need this values diectly we could copy 
                // the unmanaged double array to a managed array like so:

                // double[] planesArray = new double[24];
                // Marshal.Copy(ptr, planesArray, 0, 24);

                // but fortunately we can forward the IntPtr directly to the function 
                // SetFrustumPlanes()
                planes.SetFrustumPlanes(ptr);
                // free unmanaged memory
                Marshal.FreeHGlobal(ptr);
            }
            // another way
            {
                vtkSphereSource sphereSource = vtkSphereSource.New();
                sphereSource.Update();
                double[] bounds = new double[6];
                bounds = sphereSource.GetOutput().GetBounds();
                planes.SetBounds(bounds[0], bounds[1], bounds[2], bounds[3], bounds[4], bounds[5]);
            }
            // nothing to visualize
        }

        public static void PlanesIntersection(RenderWindowControl renderWindowControl1)
        {
            vtkSphereSource sphereSource = vtkSphereSource.New();
            sphereSource.Update();

            double[] bounds = new double[6];
            bounds = sphereSource.GetOutput().GetBounds();

            vtkPoints box = vtkPoints.New();

            box.SetNumberOfPoints(8);

            double xMin, xMax, yMin, yMax, zMin, zMax;
            xMin = bounds[0]; xMax = bounds[1];
            yMin = bounds[2]; yMax = bounds[3];
            zMin = bounds[4]; zMax = bounds[5];

            box.SetPoint(0, xMax, yMin, zMax);
            box.SetPoint(1, xMax, yMin, zMin);
            box.SetPoint(2, xMax, yMax, zMin);
            box.SetPoint(3, xMax, yMax, zMax);
            box.SetPoint(4, xMin, yMin, zMax);
            box.SetPoint(5, xMin, yMin, zMin);
            box.SetPoint(6, xMin, yMax, zMin);
            box.SetPoint(7, xMin, yMax, zMax);

            vtkPlanesIntersection planesIntersection = vtkPlanesIntersection.New();
            planesIntersection.SetBounds(bounds[0], bounds[1], bounds[2], bounds[3], bounds[4], bounds[5]);
            int intersects = planesIntersection.IntersectsRegion(box);
            Console.WriteLine("Intersects? " + ((intersects == 1) ? true : false).ToString());
            // nothing to visualize
        }

        public static void PlatonicSolid(RenderWindowControl renderWindowControl1)
        {
            vtkPlatonicSolidSource platonicSolidSource = vtkPlatonicSolidSource.New();
            platonicSolidSource.SetSolidTypeToOctahedron();

            // Each face has a different cell scalar
            vtkLookupTable lut = vtkLookupTable.New();
            lut.SetNumberOfTableValues(8);
            lut.SetTableRange(0.0, 7.0);
            lut.Build();
            lut.SetTableValue(0, 0, 0, 0, 1);
            lut.SetTableValue(1, 0, 0, 1, 1);
            lut.SetTableValue(2, 0, 1, 0, 1);
            lut.SetTableValue(3, 0, 1, 1, 1);
            lut.SetTableValue(4, 1, 0, 0, 1);
            lut.SetTableValue(5, 1, 0, 1, 1);
            lut.SetTableValue(6, 1, 1, 0, 1);
            lut.SetTableValue(7, 1, 1, 1, 1);

            // Visualize
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(platonicSolidSource.GetOutputPort());
            mapper.SetLookupTable(lut);
            mapper.SetScalarRange(0, 7);

            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.2, 0.3, 0.4);
            renderer.AddActor(actor);
        }

        public static void Point(RenderWindowControl renderWindowControl1)
        {
            // Create the geometry of the points (the coordinate)
            vtkPoints points = vtkPoints.New();
            double[,] p = new double[,] {
                {1.0, 2.0, 3.0},
                {3.0, 1.0, 2.0},
                {2.0, 3.0, 1.0}
             };

            // Create topology of the points (a vertex per point)
            vtkCellArray vertices = vtkCellArray.New();
            int nPts = 3;

            int[] ids = new int[nPts];
            for (int i = 0; i < nPts; i++)
                ids[i] =(int)points.InsertNextPoint(p[i, 0], p[i, 1], p[i, 2]);

            int size = Marshal.SizeOf(typeof(long)) * nPts;
            IntPtr pIds = Marshal.AllocHGlobal(size);
            Marshal.Copy(ids, 0, pIds, nPts);
            vertices.InsertNextCell(nPts, pIds);

            // Create a polydata object
            vtkPolyData pointPoly = vtkPolyData.New();

            // Set the points and vertices we created as the geometry and topology of the polydata
            pointPoly.SetPoints(points);
            pointPoly.SetVerts(vertices);

            // Visualize
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInput(pointPoly);

            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetPointSize(20);
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.3, 0.2, 0.1);
            renderer.AddActor(actor);

            Marshal.FreeHGlobal(pIds);
        }

        public static void PolyLine(RenderWindowControl renderWindowControl1)
        {
            // Create five points
            double[,] p = new double[,] {
                { 0.0, 0.0, 0.0 },
                { 1.0, 0.0, 0.0 },
                { 0.0, 1.0, 0.0 },
                { 0.0, 1.0, 2.0 },
                { 0.0, 3.0, 3.0 }
             };

            // Create the points
            vtkPoints points = vtkPoints.New();
            for (int i = 0; i < 5; i++)
                points.InsertNextPoint(p[i, 0], p[i, 1], p[i, 2]);


            vtkPolyLine polyLine = vtkPolyLine.New();
            polyLine.GetPointIds().SetNumberOfIds(5);
            for (int i = 0; i < 5; i++)
                polyLine.GetPointIds().SetId(i, i);

            // Create a cell array to store the lines in and add the lines to it
            vtkCellArray cells = vtkCellArray.New();
            cells.InsertNextCell(polyLine);

            // Create a polydata to store everything in
            vtkPolyData polyData = vtkPolyData.New();

            // Add the points to the dataset
            polyData.SetPoints(points);

            // Add the lines to the dataset
            polyData.SetLines(cells);
            //Create an actor and mapper
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInput(polyData);
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.2, 0.3, 0.4);
            renderer.AddActor(actor);
        }

        public static void Polygon(RenderWindowControl renderWindowControl1)
        {
            // Setup four points
            vtkPoints points = vtkPoints.New();
            double c = Math.Cos(Math.PI / 6); // helper variable

            points.InsertNextPoint(0.0, -1.0, 0.0);
            points.InsertNextPoint(c, -0.5, 0.0);
            points.InsertNextPoint(c, 0.5, 0.0);
            points.InsertNextPoint(0.0, 1.0, 0.0);
            points.InsertNextPoint(-c, 0.5, 0.0);
            points.InsertNextPoint(-c, -0.5, 0.0);

            // Create the polygon
            vtkPolygon polygon = vtkPolygon.New();
            polygon.GetPointIds().SetNumberOfIds(6); //make a six-sided figure
            polygon.GetPointIds().SetId(0, 0);
            polygon.GetPointIds().SetId(1, 1);
            polygon.GetPointIds().SetId(2, 2);
            polygon.GetPointIds().SetId(3, 3);
            polygon.GetPointIds().SetId(4, 4);
            polygon.GetPointIds().SetId(5, 5);

            // Add the polygon to a list of polygons
            vtkCellArray polygons = vtkCellArray.New();
            polygons.InsertNextCell(polygon);

            // Create a PolyData
            vtkPolyData polygonPolyData = vtkPolyData.New();
            polygonPolyData.SetPoints(points);
            polygonPolyData.SetPolys(polygons);

            // Create a mapper and actor
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInput(polygonPolyData);
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.2, 0.3, 0.4);
            renderer.AddActor(actor);
            renderer.ResetCamera();
        }

        public static void PolygonIntersection()
        {
            // Create a square in the XY plane
            vtkPoints points = vtkPoints.New();
            points.InsertNextPoint(0.0, 0.0, 0.0);
            points.InsertNextPoint(1.0, 0.0, 0.0);
            points.InsertNextPoint(1.0, 1.0, 0.0);
            points.InsertNextPoint(0.0, 1.0, 0.0);

            // Create the polygon
            vtkPolygon polygon = vtkPolygon.New();
            polygon.GetPoints().DeepCopy(points);
            polygon.GetPointIds().SetNumberOfIds(4); // 4 corners of the square
            polygon.GetPointIds().SetId(0, 0);
            polygon.GetPointIds().SetId(1, 1);
            polygon.GetPointIds().SetId(2, 2);
            polygon.GetPointIds().SetId(3, 3);

            // our line to intersect the polygon with
            double[] p1 = new double[] { 0.1, 0, -1.0 };
            double[] p2 = new double[] { 0.1, 0, 1.0 };
            double tolerance = 0.001;
            // Outputs
            // t must be initalized cause it is passed by reference (that's a c# convention)
            double t = 0.0; // Parametric coordinate of intersection (0 (corresponding to p1) to 1 (corresponding to p2))
            double[] x = new double[] { 0.0, 0.0, 0.0 };
            double[] coords = new double[] { 0.0, 0.0, 0.0 };
            // subId must be initialized cause it is passed by reference (that's a c# convention)
            int subId = 0;

            IntPtr pP1 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * 3);
            IntPtr pP2 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * 3);
            IntPtr pX = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * 3);
            IntPtr pCoords = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * 3);
            Marshal.Copy(p1, 0, pP1, 3);
            Marshal.Copy(p2, 0, pP2, 3);
            // next two lines are not necessarely needed, but I prefer to initialize ref parameter (in those a result is passed back)
            Marshal.Copy(x, 0, pX, 3);
            Marshal.Copy(coords, 0, pCoords, 3);
            // see vtkCell API for a detailed description of this function
            int iD = polygon.IntersectWithLine(pP1, pP2, tolerance, ref t, pX, pCoords, ref subId);
            // Copy result back to our managed arrays
            Marshal.Copy(pX, x, 0, 3);
            Marshal.Copy(pCoords, coords, 0, 3);
            Console.WriteLine("intersected? " + iD); ;
            Console.WriteLine("intersection: " + x[0] + " " + x[1] + " " + x[2]);
            Marshal.FreeHGlobal(pP1);
            Marshal.FreeHGlobal(pP2);
            Marshal.FreeHGlobal(pX);
            Marshal.FreeHGlobal(pCoords);
            // nothing to visualize
        }

        public static void Pyramid(RenderWindowControl renderWindowControl1)
        {
            vtkPoints points = vtkPoints.New();
            double[,] p = new double[,] {
            { 1.0,  1.0, 1.0 },
            {-1.0,  1.0, 1.0 },
            {-1.0, -1.0, 1.0 },
            { 1.0, -1.0, 1.0 },
            { 0.0,  0.0, 0.0 }};

            for (int i = 0; i < 5; i++)
                points.InsertNextPoint(p[i, 0], p[i, 1], p[i, 2]);

            vtkPyramid pyramid = vtkPyramid.New();
            for (int i = 0; i < 5; i++)
                pyramid.GetPointIds().SetId(i, i);

            vtkCellArray cells = vtkCellArray.New();
            cells.InsertNextCell(pyramid);

            vtkUnstructuredGrid ug = vtkUnstructuredGrid.New();
            ug.SetPoints(points);
            ug.InsertNextCell(pyramid.GetCellType(), pyramid.GetPointIds());

            //Create an actor and mapper
            vtkDataSetMapper mapper = vtkDataSetMapper.New();
            mapper.SetInput(ug);
            vtkActor actor = vtkActor.New();
            actor.RotateX(105.0);
            actor.RotateZ(-36.0);
            actor.SetMapper(mapper);
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.2, 0.3, 0.4);
            renderer.AddActor(actor);
            renderer.ResetCamera();
        }

        public static void Quad(RenderWindowControl renderWindowControl1)
        {
            double[,] p = new double[,] {
                { 0.0, 0.0, 0.0 },
                { 1.0, 0.0, 0.0 },
                { 1.0, 1.0, 0.0 },
                { 0.0, 1.0, 0.0 }
             };

            // Create the points
            vtkPoints points = vtkPoints.New();
            for (int i = 0; i < 4; i++)
                points.InsertNextPoint(p[i, 0], p[i, 1], p[i, 2]);


            vtkQuad quad = vtkQuad.New();
            quad.GetPointIds().SetNumberOfIds(4);
            for (int i = 0; i < 4; i++)
                quad.GetPointIds().SetId(i, i);

            // Create a cell array to store the quad in and add the quad to it
            vtkCellArray cells = vtkCellArray.New();
            cells.InsertNextCell(quad);

            // Create a polydata to store everything in
            vtkPolyData polyData = vtkPolyData.New();

            // Add the points to the dataset
            polyData.SetPoints(points);

            // Add the quad to the dataset
            polyData.SetPolys(cells);

            //Create an actor and mapper
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInput(polyData);
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.2, 0.3, 0.4);
            renderer.AddActor(actor);
        }

        public static void RegularPolygon(RenderWindowControl renderWindowControl1)
        {
            // Create a pentagon
            vtkRegularPolygonSource polygonSource = vtkRegularPolygonSource.New();

            //polygonSource.GeneratePolygonOff();
            polygonSource.SetNumberOfSides(5);
            polygonSource.SetRadius(5);
            polygonSource.SetCenter(0, 0, 0);
            //polygonSource.Update(); // not necessary

            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(polygonSource.GetOutputPort());
            vtkActor actor = vtkActor.New();
            actor.GetProperty().SetLineWidth(4);
            actor.SetMapper(mapper);
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.2, 0.3, 0.4);
            renderer.AddActor(actor);
        }

        public static void Sphere(RenderWindowControl renderWindowControl1)
        {
            // Create a sphere.  
            vtkSphereSource sphereSource = vtkSphereSource.New();
            sphereSource.SetRadius(0.5);
            // a more smoother sphere
            //sphereSource.SetPhiResolution(36);
            //sphereSource.SetThetaResolution(36);

            //not a complete sphere, only a spherical shell
            //sphereSource.SetEndPhi(120);
            //sphereSource.SetEndTheta(90);

            // Visualize
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(sphereSource.GetOutputPort());
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.3, 0.2, 0.1);
            renderer.AddActor(actor);
        }

        public static void Tetrahedron(RenderWindowControl renderWindowControl1)
        {
            vtkPoints points = vtkPoints.New();
            points.InsertNextPoint(0, 0, 0);
            points.InsertNextPoint(1, 0, 0);
            points.InsertNextPoint(1, 1, 0);
            points.InsertNextPoint(0, 1, 1);
            points.InsertNextPoint(5, 5, 5);
            points.InsertNextPoint(6, 5, 5);
            points.InsertNextPoint(6, 6, 5);
            points.InsertNextPoint(5, 6, 6);

            // Method 1
            vtkUnstructuredGrid unstructuredGrid1 = vtkUnstructuredGrid.New();
            unstructuredGrid1.SetPoints(points);

            int[] ptIds = new int[] { 0, 1, 2, 3 };
            IntPtr ptIdsPointer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)) * 4);
            Marshal.Copy(ptIds, 0, ptIdsPointer, 4);
            unstructuredGrid1.InsertNextCell(10, 4, ptIdsPointer);
            Marshal.FreeHGlobal(ptIdsPointer);

            // Method 2
            vtkUnstructuredGrid unstructuredGrid2 = vtkUnstructuredGrid.New();
            unstructuredGrid2.SetPoints(points);

            vtkTetra tetra = vtkTetra.New();

            tetra.GetPointIds().SetId(0, 4);
            tetra.GetPointIds().SetId(1, 5);
            tetra.GetPointIds().SetId(2, 6);
            tetra.GetPointIds().SetId(3, 7);

            vtkCellArray cellArray = vtkCellArray.New();
            cellArray.InsertNextCell(tetra);
            unstructuredGrid2.SetCells(10, cellArray);

            // Create a mapper and actor
            vtkDataSetMapper mapper1 = vtkDataSetMapper.New();
            mapper1.SetInputConnection(unstructuredGrid1.GetProducerPort());

            vtkActor actor1 = vtkActor.New();
            actor1.SetMapper(mapper1);

            // Create a mapper and actor
            vtkDataSetMapper mapper2 = vtkDataSetMapper.New();
            mapper2.SetInputConnection(unstructuredGrid2.GetProducerPort());

            vtkActor actor2 = vtkActor.New();
            actor2.SetMapper(mapper2);

            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.2, 0.3, 0.4);
            // Add the actor to the scene
            renderer.AddActor(actor1);
            renderer.AddActor(actor2);
            renderer.SetBackground(.3, .6, .3); // Background color green
        }

        public static void Triangle(RenderWindowControl renderWindowControl1)
        {
            // Create a triangle
            vtkPoints points = vtkPoints.New();
            points.InsertNextPoint(1.0, 0.0, 0.0);
            points.InsertNextPoint(0.0, 0.0, 0.0);
            points.InsertNextPoint(0.0, 1.0, 0.0);

            vtkTriangle triangle = vtkTriangle.New();
            triangle.GetPointIds().SetId(0, 0);
            triangle.GetPointIds().SetId(1, 1);
            triangle.GetPointIds().SetId(2, 2);

            // Create a cell array to store the triangle in and add the triangle to it
            vtkCellArray cells = vtkCellArray.New();
            cells.InsertNextCell(triangle);

            // Create a polydata to store everything in
            vtkPolyData polyData = vtkPolyData.New();

            // Add the points to the dataset
            polyData.SetPoints(points);

            // Add the quad to the dataset
            polyData.SetPolys(cells);

            //Create an actor and mapper
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInput(polyData);
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.2, 0.3, 0.4);
            renderer.AddActor(actor);
        }

        public static void TriangleStrip(RenderWindowControl renderWindowControl1)
        {
            vtkPoints points = vtkPoints.New();
            points.InsertNextPoint(0, 0, 0);
            points.InsertNextPoint(0, 1, 0);
            points.InsertNextPoint(1, 0, 0);
            points.InsertNextPoint(1.5, 1, 0);

            vtkTriangleStrip triangleStrip = vtkTriangleStrip.New();
            triangleStrip.GetPointIds().SetNumberOfIds(4);
            triangleStrip.GetPointIds().SetId(0, 0);
            triangleStrip.GetPointIds().SetId(1, 1);
            triangleStrip.GetPointIds().SetId(2, 2);
            triangleStrip.GetPointIds().SetId(3, 3);

            vtkCellArray cells = vtkCellArray.New();
            cells.InsertNextCell(triangleStrip);
            // Create a polydata to store everything in
            vtkPolyData polyData = vtkPolyData.New();

            // Add the points to the dataset
            polyData.SetPoints(points);
            // Add the strip to the dataset
            polyData.SetStrips(cells);

            //Create an actor and mapper
            vtkDataSetMapper mapper = vtkDataSetMapper.New();
            mapper.SetInput(polyData);
            vtkActor actor = vtkActor.New();
            actor.GetProperty().SetRepresentationToWireframe();

            actor.SetMapper(mapper);
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.2, 0.3, 0.4);
            renderer.AddActor(actor);
        }

        public static void Vertex(RenderWindowControl renderWindowControl1)
        {
            vtkPoints points = vtkPoints.New();
            points.InsertNextPoint(0, 0, 0);

            vtkVertex vertex = vtkVertex.New();
            vertex.GetPointIds().SetId(0, 0);

            vtkCellArray vertices = vtkCellArray.New();
            vertices.InsertNextCell(vertex);

            vtkPolyData polydata = vtkPolyData.New();
            polydata.SetPoints(points);
            polydata.SetVerts(vertices);

            // Visualize
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(polydata.GetProducerPort());
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetPointSize(10);
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.2, 0.3, 0.4);
            // Add the actor to the scene
            renderer.AddActor(actor);
        }

    }



    public class myMath
    {
        public static void Subtract(double[] a, double[] b, ref double[] c)
        {
            c[0] = a[0] - b[0];
            c[1] = a[1] - b[1];
            c[2] = a[2] - b[2];
        }


        public static double Norm(double[] x)
        {
            return Math.Sqrt(x[0] * x[0] + x[1] * x[1] + x[2] * x[2]);
        }


        public static void Normalize(ref double[] x)
        {
            double length = Norm(x);
            x[0] /= length;
            x[1] /= length;
            x[2] /= length;
        }

        public static void Cross(double[] x, double[] y, ref double[] z)
        {
            z[0] = (x[1] * y[2]) - (x[2] * y[1]);
            z[1] = (x[2] * y[0]) - (x[0] * y[2]);
            z[2] = (x[0] * y[1]) - (x[1] * y[0]);
        }
    }

}
