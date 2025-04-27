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
    class clsImageFormats
    {
        public static void ImageReader2Factory(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\foot\foot.mha");

            vtkImageReader2 reader = vtkImageReader2Factory.CreateImageReader2(filePath);
            if (reader.CanReadFile(filePath) == 0)
            {
                MessageBox.Show("Cannot read file \"" + filePath + "\"", "Error", MessageBoxButtons.OK);
                return;
            }
            reader.SetFileName(filePath);
            reader.Update();
            vtkImageActor actor = vtkImageActor.New();
            actor.SetInput(reader.GetOutput());

            // Visualize
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.2, 0.3, 0.4);
            renderer.AddActor(actor);
        }

        public static void ReadJPEG(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\beach.jpg");
            //Read the image
            vtkJPEGReader reader = vtkJPEGReader.New();
            if (reader.CanReadFile(filePath) == 0)
            {
                MessageBox.Show("Cannot read file \"" + filePath + "\"", "Error", MessageBoxButtons.OK);
                return;
            }
            reader.SetFileName(filePath);
            reader.Update();

            // Visualize
            vtkImageViewer2 imageViewer = vtkImageViewer2.New();
            imageViewer.SetInputConnection(reader.GetOutputPort());
            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.2, 0.3, 0.4);
            imageViewer.SetRenderer(renderer);
        }

        public static void ReadMetaImage(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\foot\foot.mha");

            vtkMetaImageReader reader = vtkMetaImageReader.New();
            if (reader.CanReadFile(filePath) == 0)
            {
                MessageBox.Show("Cannot read file \"" + filePath + "\"", "Error", MessageBoxButtons.OK);
                return;
            }
            reader.SetFileName(filePath);
            reader.Update();
            vtkImageActor actor = vtkImageActor.New();
            actor.SetInput(reader.GetOutput());

            // Visualize
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.2, 0.3, 0.4);
            renderer.AddActor(actor);
        }

        public static void ReadPNG(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\camscene.png");
            //Read the image
            vtkPNGReader reader = vtkPNGReader.New();
            if (reader.CanReadFile(filePath) == 0)
            {
                MessageBox.Show("Cannot read file \"" + filePath + "\"", "Error", MessageBoxButtons.OK);
                return;
            }
            reader.SetFileName(filePath);
            reader.Update();
            // Visualize
            vtkImageViewer2 imageViewer = vtkImageViewer2.New();
            imageViewer.SetInputConnection(reader.GetOutputPort());
            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.2, 0.3, 0.4);
            imageViewer.SetRenderer(renderer);
        }

        public static void ReadBMP(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\masonry.bmp");
            //Read the image
            vtkBMPReader reader = vtkBMPReader.New();
            if (reader.CanReadFile(filePath) == 0)
            {
                MessageBox.Show("Cannot read file \"" + filePath + "\"", "Error", MessageBoxButtons.OK);
                return;
            }
            reader.SetFileName(filePath);
            reader.Update();

            // Visualize
            vtkImageViewer2 imageViewer = vtkImageViewer2.New();
            imageViewer.SetInputConnection(reader.GetOutputPort());
            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.2, 0.3, 0.4);
            imageViewer.SetRenderer(renderer);
            renderer.ResetCamera();
        }

        public static void ReadTIFF(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\beach.tif");
            //Read the image
            vtkTIFFReader reader = vtkTIFFReader.New();
            if (reader.CanReadFile(filePath) == 0)
            {
                MessageBox.Show("Cannot read file \"" + filePath + "\"", "Error", MessageBoxButtons.OK);
                return;
            }
            reader.SetFileName(filePath);
            reader.Update();

            // Visualize
            vtkImageViewer2 imageViewer = vtkImageViewer2.New();
            imageViewer.SetInputConnection(reader.GetOutputPort());
            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.2, 0.3, 0.4);
            imageViewer.SetRenderer(renderer);
        }
    }
}
