

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


    }
}
