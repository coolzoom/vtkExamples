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
    class clsXGMLReader
    {

        public static void XGMLReader(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\Infovis\fsm.gml");

            vtkXGMLReader reader = vtkXGMLReader.New();
            reader.SetFileName(filePath);
            reader.Update();

            vtkUndirectedGraph g = reader.GetOutput();

            vtkGraphLayoutView graphLayoutView = vtkGraphLayoutView.New();
            graphLayoutView.SetRenderWindow(renderWindowControl1.RenderWindow);
            graphLayoutView.AddRepresentationFromInput(g);
            graphLayoutView.SetLayoutStrategy("Simple 2D");
            graphLayoutView.ResetCamera();
            graphLayoutView.Render();
        }

    }
}
