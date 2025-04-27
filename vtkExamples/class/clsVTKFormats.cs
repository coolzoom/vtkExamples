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
    class clsVTKFormats
    {

        public static void ParticleReader(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\Particles.raw");

            // Read the file
            vtkParticleReader reader = vtkParticleReader.New();
            reader.SetFileName(filePath);
            reader.SetDataByteOrderToBigEndian();
            reader.Update();
            Debug.WriteLine("NumberOfPieces: " + reader.GetOutput().GetNumberOfPieces());

            // Visualize
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(reader.GetOutputPort());
            mapper.SetScalarRange(4, 9);
            mapper.SetPiece(1);

            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetPointSize(4);
            actor.GetProperty().SetColor(1, 0, 0);
            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.2, 0.3, 0.4);
            // add our actor to the renderer
            renderer.AddActor(actor);
        }

        public static void ReadImageData(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\vase_1comp.vti");

            // reader
            // Read all the data from the file
            vtkXMLImageDataReader reader = vtkXMLImageDataReader.New();
            if (reader.CanReadFile(filePath) == 0)
            {
                MessageBox.Show("Cannot read file \"" + filePath + "\"", "Error", MessageBoxButtons.OK);
                return;
            }
            reader.SetFileName(filePath);
            reader.Update(); // here we read the file actually

            // mapper
            vtkDataSetMapper mapper = vtkDataSetMapper.New();
            mapper.SetInputConnection(reader.GetOutputPort());

            // actor
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetRepresentationToWireframe();

            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.2, 0.3, 0.4);
            // add our actor to the renderer
            renderer.AddActor(actor);
        }

        public static void ReadOBJ(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            // caution: vtkdata-5.8.0\Data\mni-surface-mesh.obj is no valid obj file!
            //string filePath = System.IO.Path.Combine(root, @"Data\mni-surface-mesh.obj");
            string filePath = System.IO.Path.Combine(root, @"Data\ViewPoint\cow.obj");

            vtkOBJReader reader = vtkOBJReader.New();
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Cannot read file \"" + filePath + "\"", "Error", MessageBoxButtons.OK);
                return;
            }
            reader.SetFileName(filePath);
            reader.Update();
            // Visualize
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(reader.GetOutputPort());
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
            renderWindow.Render();
        }

        public static void ReadPlainText(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\teapot.xyz");

            FileStream fs = null;
            StreamReader sr = null;
            String sLineBuffer;
            String[] sXYZ;
            char[] chDelimiter = new char[] { ' ', '\t', ';' };
            double[] xyz = new double[3];
            vtkPoints points = vtkPoints.New();
            int cnt = 0;

            try
            {
                // in case file must be open in another application too use "FileShare.ReadWrite"
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                sr = new StreamReader(fs);
                while (!sr.EndOfStream)
                {
                    sLineBuffer = sr.ReadLine();
                    cnt++;
                    sXYZ = sLineBuffer.Split(chDelimiter, StringSplitOptions.RemoveEmptyEntries);
                    if (sXYZ == null || sXYZ.Length != 3)
                    {
                        MessageBox.Show("data seems to be in wrong format at line " + cnt, "Format Exception", MessageBoxButtons.OK);
                        return;
                    }
                    xyz[0] = double.Parse(sXYZ[0], CultureInfo.InvariantCulture);
                    xyz[1] = double.Parse(sXYZ[1], CultureInfo.InvariantCulture);
                    xyz[2] = double.Parse(sXYZ[2], CultureInfo.InvariantCulture);
                    points.InsertNextPoint(xyz[0], xyz[1], xyz[2]);
                }
                vtkPolyData polydata = vtkPolyData.New();
                polydata.SetPoints(points);
                vtkVertexGlyphFilter glyphFilter = vtkVertexGlyphFilter.New();
                glyphFilter.SetInputConnection(polydata.GetProducerPort());

                // Visualize
                vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
                mapper.SetInputConnection(glyphFilter.GetOutputPort());

                vtkActor actor = vtkActor.New();
                actor.SetMapper(mapper);
                actor.GetProperty().SetPointSize(4);
                actor.GetProperty().SetColor(1, 0.5, 0);
                // get a reference to the renderwindow of our renderWindowControl1
                vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
                // renderer
                vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
                // set background color
                renderer.SetBackground(0.2, 0.3, 0.4);
                // add our actor to the renderer
                renderer.AddActor(actor);
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "IOException", MessageBoxButtons.OK);
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                    sr = null;
                }
            }
        }

        public static void ReadPolyData(RenderWindowControl renderWindowControl1)
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
            // reader
            // Read all the data from the file
            vtkXMLPolyDataReader reader = vtkXMLPolyDataReader.New();
            if (reader.CanReadFile(filePath) == 0)
            {
                MessageBox.Show("Cannot read file \"" + filePath + "\"", "Error", MessageBoxButtons.OK);
                return;
            }
            reader.SetFileName(filePath);
            reader.Update(); // here we read the file actually

            // mapper
            vtkDataSetMapper mapper = vtkDataSetMapper.New();
            mapper.SetInputConnection(reader.GetOutputPort());

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

        public static void ReadRectilinearGrid(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\cth.vtr");
            // reader
            vtkXMLRectilinearGridReader reader = vtkXMLRectilinearGridReader.New();
            if (reader.CanReadFile(filePath) == 0)
            {
                MessageBox.Show("Cannot read file \"" + filePath + "\"", "Error", MessageBoxButtons.OK);
                return;
            }
            reader.SetFileName(filePath);
            reader.Update(); // here we read the file actually

            //vtkRectilinearGridGeometryFilter geometryFilter = vtkRectilinearGridGeometryFilter.New();
            //geometryFilter.SetInputConnection(reader.GetOutputPort());
            //geometryFilter.Update();


            // Visualize
            // mapper
            vtkDataSetMapper mapper = vtkDataSetMapper.New();
            //mapper.SetInputConnection(geometryFilter.GetOutputPort());
            mapper.SetInputConnection(reader.GetOutputPort());

            // actor
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetRepresentationToWireframe();
            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.2, 0.3, 0.4);
            // add our actor to the renderer
            renderer.AddActor(actor);
        }

        public static void ReadStructuredGrid(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\multicomb_0.vts");

            // reader
            vtkXMLStructuredGridReader reader = vtkXMLStructuredGridReader.New();
            if (reader.CanReadFile(filePath) == 0)
            {
                MessageBox.Show("Cannot read file \"" + filePath + "\"", "Error", MessageBoxButtons.OK);
                return;
            }
            reader.SetFileName(filePath);
            reader.Update(); // here we read the file actually

            vtkStructuredGridGeometryFilter geometryFilter = vtkStructuredGridGeometryFilter.New();
            geometryFilter.SetInputConnection(reader.GetOutputPort());
            geometryFilter.Update();

            // Visualize
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(geometryFilter.GetOutputPort());

            //// mapper
            //vtkDataSetMapper mapper = vtkDataSetMapper.New();
            //mapper.SetInputConnection(reader.GetOutputPort());

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

        public static void ReadUnknownTypeXMLFile(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\quadraticTetra01.vtu");
            //string filePath = System.IO.Path.Combine(root, @"Data\SyntheticPolyline.vtp");

            // reader
            // Read all the data from the file
            vtkXMLGenericDataObjectReader reader = vtkXMLGenericDataObjectReader.New();
            // caution: you cannot use CanReadFile with the generic reader
            //if(reader.CanReadFile(filePath) == 0) {
            //   MessageBox.Show("Cannot read file \"" + filePath + "\"", "Error", MessageBoxButtons.OK);
            //   return;
            //}
            reader.SetFileName(filePath);
            reader.Update(); // here we read the file actually

            // All of the standard data types can be checked and obtained like this:
            if (vtkPolyData.SafeDownCast(reader.GetOutput()) != null)
            {
                Debug.WriteLine("File is a polydata");
            }
            else if (vtkUnstructuredGrid.SafeDownCast(reader.GetOutput()) != null)
            {
                Debug.WriteLine("File is an unstructured grid");
            }

            // mapper
            vtkDataSetMapper mapper = vtkDataSetMapper.New();
            mapper.SetInputConnection(reader.GetOutputPort());

            // actor
            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            //actor.GetProperty().SetRepresentationToWireframe();

            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.2, 0.3, 0.4);
            // add our actor to the renderer
            renderer.AddActor(actor);
        }



    }
}
