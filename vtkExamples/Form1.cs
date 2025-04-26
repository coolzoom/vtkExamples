using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

using Kitware.VTK;

namespace vtkExamples
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.renderWindowControl = new Kitware.VTK.RenderWindowControl();
            this.renderWindowControl.AddTestActors = false;
            this.renderWindowControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderWindowControl.Location = new System.Drawing.Point(0, 0);
            this.renderWindowControl.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.renderWindowControl.Name = "renderWindowControl";
            this.renderWindowControl.Size = new System.Drawing.Size(617, 347);
            this.renderWindowControl.TabIndex = 0;
            this.renderWindowControl.TestText = null;
            this.renderWindowControl.Load += new System.EventHandler(this.renderWindowControl1_Load);
            pictureBox1.Controls.Add(this.renderWindowControl);
        }

        public Kitware.VTK.RenderWindowControl renderWindowControl;

        public enum eExample {
            eReadPlainText = 0,
            eDistanceBetweenPoints,
            eDistancePointToLine,
            eGaussianRandomNumber,
            ePerspectiveTransform,
            eProjectPointPlane,
            eRandomSequence,
            eUniformRandomNumber,
            eXGMLReader,
            eDEMReader,
            eReadDICOMSeries
        };

        private void renderWindowControl1_Load(object sender, EventArgs e)
        {
            try
            {
                eExample   ex = eExample.eReadDICOMSeries;
                switch (ex) {
                    case eExample.eReadPlainText:
                        clsReadPlainText.ReadPlainText(renderWindowControl);
                        break;
                    case eExample.eDistanceBetweenPoints:
                        clsSimpleOperations.DistanceBetweenPoints();
                        break;
                    case eExample.eDistancePointToLine:
                        clsSimpleOperations.DistancePointToLine();
                        break;
                    case eExample.eGaussianRandomNumber:
                        clsSimpleOperations.GaussianRandomNumber();
                        break;
                    case eExample.ePerspectiveTransform:
                        clsSimpleOperations.PerspectiveTransform();
                        break;
                    case eExample.eProjectPointPlane:
                        clsSimpleOperations.ProjectPointPlane();
                        break;
                    case eExample.eRandomSequence:
                        clsSimpleOperations.RandomSequence();
                        break;
                    case eExample.eUniformRandomNumber:
                        clsSimpleOperations.UniformRandomNumber();
                        break;
                    case eExample.eXGMLReader:
                        clsReaders.XGMLReader(renderWindowControl);
                        break;
                    case eExample.eDEMReader:
                        clsReaders.ReadDEM(renderWindowControl);
                        break;
                    case eExample.eReadDICOMSeries:
                        ReadDICOMSeries(renderWindowControl);
                        break;
                }

               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK);
            }
        }



        // module wide accessible variables
        private vtkImageViewer2 _ImageViewer;
        private vtkTextMapper _SliceStatusMapper;
        private int _Slice;
        private int _MinSlice;
        private int _MaxSlice;

        public void ReadDICOMSeries(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            // Read all the DICOM files in the specified directory.
            // Caution: folder "DicomTestImages" don't exists by default in the standard vtk data folder
            // sample data are available at http://www.vtk.org/Wiki/images/1/12/VTK_Examples_StandardFormats_Input_DicomTestImages.zip
            string folder = Path.Combine(root, @"Data\DicomTestImages");
            vtkDICOMImageReader reader = vtkDICOMImageReader.New();
            reader.SetDirectoryName(folder);
            reader.Update();
            // Visualize
            _ImageViewer = vtkImageViewer2.New();
            _ImageViewer.SetInputConnection(reader.GetOutputPort());
            // get range of slices (min is the first index, max is the last index)
            _ImageViewer.GetSliceRange(ref _MinSlice, ref _MaxSlice);
            Debug.WriteLine("slices range from : " + _MinSlice.ToString() + " to " + _MaxSlice.ToString());

            // slice status message
            vtkTextProperty sliceTextProp = vtkTextProperty.New();
            sliceTextProp.SetFontFamilyToCourier();
            sliceTextProp.SetFontSize(20);
            sliceTextProp.SetVerticalJustificationToBottom();
            sliceTextProp.SetJustificationToLeft();

            _SliceStatusMapper = vtkTextMapper.New();
            _SliceStatusMapper.SetInput("Slice No " + (_Slice + 1).ToString() + "/" + (_MaxSlice + 1).ToString());
            _SliceStatusMapper.SetTextProperty(sliceTextProp);

            vtkActor2D sliceStatusActor = vtkActor2D.New();
            sliceStatusActor.SetMapper(_SliceStatusMapper);
            sliceStatusActor.SetPosition(15, 10);

            // usage hint message
            vtkTextProperty usageTextProp = vtkTextProperty.New();
            usageTextProp.SetFontFamilyToCourier();
            usageTextProp.SetFontSize(14);
            usageTextProp.SetVerticalJustificationToTop();
            usageTextProp.SetJustificationToLeft();

            vtkTextMapper usageTextMapper = vtkTextMapper.New();
            usageTextMapper.SetInput("Slice with mouse wheel\nor Up/Down-Key");
            usageTextMapper.SetTextProperty(usageTextProp);

            vtkActor2D usageTextActor = vtkActor2D.New();
            usageTextActor.SetMapper(usageTextMapper);
            usageTextActor.GetPositionCoordinate().SetCoordinateSystemToNormalizedDisplay();
            usageTextActor.GetPositionCoordinate().SetValue(0.05, 0.95);

            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;

            vtkInteractorStyleImage interactorStyle = vtkInteractorStyleImage.New();
            interactorStyle.MouseWheelForwardEvt += new vtkObject.vtkObjectEventHandler(interactor_MouseWheelForwardEvt);
            interactorStyle.MouseWheelBackwardEvt += new vtkObject.vtkObjectEventHandler(interactor_MouseWheelBackwardEvt);

            renderWindow.GetInteractor().SetInteractorStyle(interactorStyle);
            renderWindow.GetRenderers().InitTraversal();
            vtkRenderer ren;
            while ((ren = renderWindow.GetRenderers().GetNextItem()) != null)
                ren.SetBackground(0.0, 0.0, 0.0);

            _ImageViewer.SetRenderWindow(renderWindow);
            _ImageViewer.GetRenderer().AddActor2D(sliceStatusActor);
            _ImageViewer.GetRenderer().AddActor2D(usageTextActor);
            _ImageViewer.SetSlice(_MinSlice);
            _ImageViewer.Render();
        }


        /// <summary>
        /// move forward to next slice
        /// </summary>
        private void MoveForwardSlice()
        {
            Debug.WriteLine(_Slice.ToString());
            if (_Slice < _MaxSlice)
            {
                _Slice += 1;
                _ImageViewer.SetSlice(_Slice);
                _SliceStatusMapper.SetInput("Slice No " + (_Slice + 1).ToString() + "/" + (_MaxSlice + 1).ToString());
                _ImageViewer.Render();
            }
        }


        /// <summary>
        /// move backward to next slice
        /// </summary>
        private void MoveBackwardSlice()
        {
            Debug.WriteLine(_Slice.ToString());
            if (_Slice > _MinSlice)
            {
                _Slice -= 1;
                _ImageViewer.SetSlice(_Slice);
                _SliceStatusMapper.SetInput("Slice No " + (_Slice + 1).ToString() + "/" + (_MaxSlice + 1).ToString());
                _ImageViewer.Render();
            }
        }


        /// <summary>
        /// eventhanndler to process keyboard input
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        {
            //Debug.WriteLine(DateTime.Now + ":" + msg.Msg + ", " + keyData);
            if (keyData == System.Windows.Forms.Keys.Up)
            {
                MoveForwardSlice();
                return true;
            }
            else if (keyData == System.Windows.Forms.Keys.Down)
            {
                MoveBackwardSlice();
                return true;
            }
            // don't forward the following keys
            // add all keys which are not supposed to get forwarded
            else if (
                  keyData == System.Windows.Forms.Keys.F
               || keyData == System.Windows.Forms.Keys.L
            )
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// event handler for mousewheel forward event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void interactor_MouseWheelForwardEvt(vtkObject sender, vtkObjectEventArgs e)
        {
            MoveForwardSlice();
        }


        /// <summary>
        /// event handler for mousewheel backward event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void interactor_MouseWheelBackwardEvt(vtkObject sender, vtkObjectEventArgs e)
        {
            MoveBackwardSlice();
        }
    }
}
