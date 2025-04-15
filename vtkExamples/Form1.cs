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
            eDistanceBetweenPoints
        };

        private void renderWindowControl1_Load(object sender, EventArgs e)
        {
            try
            {
                eExample   ex = (eExample)1;
                switch (ex) {
                    case eExample.eReadPlainText:
                        clsReadPlainText.ReadPlainText(renderWindowControl);
                        break;
                    case eExample.eDistanceBetweenPoints:
                        DistanceBetweenPoints();
                        break;

                }

               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK);
            }
        }


        private void DistanceBetweenPoints()
        {
            // Create two points.
            double[] p0 = new double[] { 0.0, 0.0, 0.0 };
            double[] p1 = new double[] { 1.0, 1.0, 1.0 };

            IntPtr pP0 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * 3);
            IntPtr pP1 = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * 3);
            Marshal.Copy(p0, 0, pP0, 3);
            Marshal.Copy(p1, 0, pP1, 3);

            // Find the squared distance between the points.
            double squaredDistance = vtkMath.Distance2BetweenPoints(pP0, pP1);

            // Take the square root to get the Euclidean distance between the points.
            double distance = Math.Sqrt(squaredDistance);

            // Output the results.
            Console.WriteLine("SquaredDistance = " + squaredDistance);
            Console.WriteLine("Distance = " + distance);
            Marshal.FreeHGlobal(pP0);
            Marshal.FreeHGlobal(pP1);
        }

    }
}
