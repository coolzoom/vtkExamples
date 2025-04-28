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

        public static void WriteJPEG(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\test_jpeg.jpg");
            int[] extent = new int[] { 0, 99, 0, 99, 0, 0 };
            vtkImageCanvasSource2D imageSource = vtkImageCanvasSource2D.New();
            imageSource.SetExtent(extent[0], extent[1], extent[2], extent[3], extent[4], extent[5]);
            imageSource.SetScalarTypeToUnsignedChar();
            imageSource.SetNumberOfScalarComponents(3);
            imageSource.SetDrawColor(127, 45, 255);
            imageSource.FillBox(0, 99, 0, 99);
            imageSource.SetDrawColor(255, 255, 255);
            imageSource.FillBox(40, 70, 20, 50);
            imageSource.Update();

            vtkImageCast castFilter = vtkImageCast.New();
            castFilter.SetOutputScalarTypeToUnsignedChar();
            castFilter.SetInputConnection(imageSource.GetOutputPort());
            castFilter.Update();

            vtkJPEGWriter writer = vtkJPEGWriter.New();
            writer.SetFileName(filePath);
            writer.SetInputConnection(castFilter.GetOutputPort());
            writer.Write();
            // Read and display file for verification that it was written correctly
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
            renderer.ResetCamera();
        }

        public static void WriteMetaImage(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\test_mha.mhd");
            string filePathRaw = System.IO.Path.Combine(root, @"Data\test_mha.raw");
            // Create an image
            vtkImageMandelbrotSource source = vtkImageMandelbrotSource.New();
            source.Update();

            vtkImageCast castFilter = vtkImageCast.New();
            castFilter.SetOutputScalarTypeToUnsignedChar();
            castFilter.SetInputConnection(source.GetOutputPort());
            castFilter.Update();

            vtkMetaImageWriter writer = vtkMetaImageWriter.New();
            writer.SetInputConnection(castFilter.GetOutputPort());
            writer.SetFileName(filePath);
            writer.SetRAWFileName(filePathRaw);
            writer.Write();

            // Read and display file for verification that it was written correctly
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

        public static void WriteBMP(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\test_bmp.bmp");
            int[] extent = new int[] { 1, 10, 1, 10, 1, 10 };
            vtkImageCanvasSource2D imageSource = vtkImageCanvasSource2D.New();
            imageSource.SetScalarTypeToUnsignedChar();
            imageSource.SetExtent(0, 9, 0, 9, 0, 0);
            imageSource.SetNumberOfScalarComponents(3);
            imageSource.SetDrawColor(0, 0, 0, 0);
            imageSource.FillBox(0, 9, 0, 9);
            imageSource.SetDrawColor(255, 0, 0, 0);
            imageSource.FillBox(5, 7, 5, 7);
            imageSource.Update();

            vtkBMPWriter bmpWriter = vtkBMPWriter.New();
            bmpWriter.SetFileName(filePath);
            bmpWriter.SetInputConnection(imageSource.GetOutputPort());
            bmpWriter.Write();

            // Read and display file for verification that it was written correctly
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

        public static void WritePNG(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\test_png.png");
            int[] extent = new int[] { 0, 99, 0, 99, 0, 0 };
            vtkImageCanvasSource2D imageSource = vtkImageCanvasSource2D.New();
            imageSource.SetExtent(extent[0], extent[1], extent[2], extent[3], extent[4], extent[5]);
            imageSource.SetScalarTypeToUnsignedChar();
            imageSource.SetNumberOfScalarComponents(3);
            imageSource.SetDrawColor(127, 45, 255);
            imageSource.FillBox(0, 99, 0, 99);
            imageSource.SetDrawColor(255, 255, 255);
            imageSource.FillBox(40, 70, 20, 50);
            imageSource.Update();

            vtkImageCast castFilter = vtkImageCast.New();
            castFilter.SetOutputScalarTypeToUnsignedChar();
            castFilter.SetInputConnection(imageSource.GetOutputPort());
            castFilter.Update();

            vtkPNGWriter writer = vtkPNGWriter.New();
            writer.SetFileName(filePath);
            writer.SetInputConnection(castFilter.GetOutputPort());
            writer.Write();
            // Read and display file for verification that it was written correctly
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
            renderer.ResetCamera();
        }

        public static void WriteTIFF(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\test_tiff.tif");

            vtkImageCanvasSource2D imageSource = vtkImageCanvasSource2D.New();
            imageSource.SetScalarTypeToUnsignedChar();
            imageSource.SetExtent(0, 9, 0, 9, 0, 0);
            imageSource.SetNumberOfScalarComponents(3);
            imageSource.SetDrawColor(0, 0, 0, 0);
            imageSource.FillBox(0, 9, 0, 9);
            imageSource.SetDrawColor(255, 0, 0, 0);
            imageSource.FillBox(5, 7, 5, 7);
            imageSource.Update();

            vtkTIFFWriter tiffWriter = vtkTIFFWriter.New();
            tiffWriter.SetFileName(filePath);
            tiffWriter.SetInputConnection(imageSource.GetOutputPort());
            tiffWriter.Write();

            // Read and display file for verification that it was written correctly
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
            renderer.ResetCamera();
        }
    }
}
