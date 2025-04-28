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


    }
}
