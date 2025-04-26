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
            eXGMLReader
        };

        private void renderWindowControl1_Load(object sender, EventArgs e)
        {
            try
            {
                eExample   ex = (eExample)8;
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
                       clsXGMLReader.XGMLReader(renderWindowControl);
                        break;
                }

               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK);
            }
        }

    }
}
