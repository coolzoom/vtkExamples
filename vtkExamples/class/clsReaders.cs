using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

using Kitware.VTK;

namespace vtkExamples
{
    class clsReaders
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

        public static void ReadDEM(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\SainteHelens.dem");

            vtkDEMReader reader = vtkDEMReader.New();
            reader.SetFileName(filePath);
            reader.Update();

            vtkLookupTable lut = vtkLookupTable.New();
            lut.SetHueRange(0.6, 0);
            lut.SetSaturationRange(1.0, 0);
            lut.SetValueRange(0.5, 1.0);
            double[] range = reader.GetOutput().GetScalarRange();
            lut.SetTableRange(range[0], range[1]);

            // Visualize
            vtkImageMapToColors mapColors = vtkImageMapToColors.New();
            mapColors.SetLookupTable(lut);
            mapColors.SetInputConnection(reader.GetOutputPort());

            // Create an actor
            vtkImageActor actor = vtkImageActor.New();
            actor.SetInput(mapColors.GetOutput());
            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.2, 0.3, 0.4);
            // add our actor to the renderer
            renderer.AddActor(actor);
        }

        public static void ReadPDB(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\caffeine.pdb");

            vtkPDBReader pdb = vtkPDBReader.New();
            pdb.SetFileName(filePath);
            pdb.SetHBScale(1.0);
            pdb.SetBScale(1.0);
            pdb.Update();
            Debug.WriteLine("# of atoms is: " + pdb.GetNumberOfAtoms());
            // if molecule contains a lot of atoms, reduce the resolution of the sphere (represents an atom) for faster rendering
            int resolution = (int)Math.Floor(Math.Sqrt(300000.0 / pdb.GetNumberOfAtoms())); // 300000.0 is an empriric value
            if (resolution > 20)
                resolution = 20;
            else if (resolution < 4)
                resolution = 4;

            Debug.WriteLine("Resolution is: " + resolution);
            vtkSphereSource sphere = vtkSphereSource.New();
            sphere.SetCenter(0, 0, 0);
            sphere.SetRadius(1);
            sphere.SetThetaResolution(resolution);
            sphere.SetPhiResolution(resolution);

            vtkGlyph3D glyph = vtkGlyph3D.New();
            glyph.SetInputConnection(pdb.GetOutputPort());
            glyph.SetOrient(1);
            glyph.SetColorMode(1);
            // glyph.ScalingOn();
            glyph.SetScaleMode(2);
            glyph.SetScaleFactor(.25);
            glyph.SetSourceConnection(sphere.GetOutputPort());

            vtkPolyDataMapper atomMapper = vtkPolyDataMapper.New();
            atomMapper.SetInputConnection(glyph.GetOutputPort());
            atomMapper.UseLookupTableScalarRangeOff();
            atomMapper.ScalarVisibilityOn();
            atomMapper.SetScalarModeToDefault();

            vtkLODActor atom = vtkLODActor.New();
            atom.SetMapper(atomMapper);
            atom.GetProperty().SetRepresentationToSurface();
            atom.GetProperty().SetInterpolationToGouraud();
            atom.GetProperty().SetAmbient(0.15);
            atom.GetProperty().SetDiffuse(0.85);
            atom.GetProperty().SetSpecular(0.1);
            atom.GetProperty().SetSpecularPower(30);
            atom.GetProperty().SetSpecularColor(1, 1, 1);
            atom.SetNumberOfCloudPoints(30000);


            vtkTubeFilter tube = vtkTubeFilter.New();
            tube.SetInputConnection(pdb.GetOutputPort());
            tube.SetNumberOfSides(resolution);
            tube.CappingOff();
            tube.SetRadius(0.2);
            // turn off variation of tube radius with scalar values
            tube.SetVaryRadius(0);
            tube.SetRadiusFactor(10);

            vtkPolyDataMapper bondMapper = vtkPolyDataMapper.New();
            bondMapper.SetInputConnection(tube.GetOutputPort());
            bondMapper.UseLookupTableScalarRangeOff();
            bondMapper.ScalarVisibilityOff();
            bondMapper.SetScalarModeToDefault();

            vtkLODActor bond = vtkLODActor.New();
            bond.SetMapper(bondMapper);
            bond.GetProperty().SetRepresentationToSurface();
            bond.GetProperty().SetInterpolationToGouraud();
            bond.GetProperty().SetAmbient(0.15);
            bond.GetProperty().SetDiffuse(0.85);
            bond.GetProperty().SetSpecular(0.1);
            bond.GetProperty().SetSpecularPower(30);
            bond.GetProperty().SetSpecularColor(1, 1, 1);
            bond.GetProperty().SetDiffuseColor(1.0000, 0.8941, 0.70981);


            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            // renderer
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            // set background color
            renderer.SetBackground(0.2, 0.3, 0.4);
            // add our actor to the renderer
            renderer.AddActor(atom);
            renderer.AddActor(bond);
        }

        public static void ReadPLOT3D(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePathXYZ = System.IO.Path.Combine(root, @"Data\bluntfinxyz.bin");
            string filePathQ = System.IO.Path.Combine(root, @"Data\bluntfinq.bin");
            //string filePathXYZ = System.IO.Path.Combine(root, @"Data\combxyz.bin");
            //string filePathQ = System.IO.Path.Combine(root, @"Data\combq.bin");

            vtkMultiBlockPLOT3DReader reader = vtkMultiBlockPLOT3DReader.New();
            reader.SetXYZFileName(filePathXYZ);
            reader.SetQFileName(filePathQ);

            // Specify the scalar function to extract. If ==(-1), then no scalar function is extracted. 
            int scalarFctNo = reader.GetScalarFunctionNumber();
            int vectorFctNo = reader.GetVectorFunctionNumber();
            if (scalarFctNo != -1)
                reader.SetScalarFunctionNumber(scalarFctNo);
            // Specify the vector function to extract. If ==(-1), then no vector function is extracted. 
            if (vectorFctNo != -1)
                reader.SetVectorFunctionNumber(vectorFctNo);
            reader.Update();

            //// geometry filter
            //// This filter is multi-block aware and will request blocks from the
            //// input. These blocks will be processed by simple processes as if they
            //// are the whole dataset
            //vtkCompositeDataGeometryFilter geom1 = vtkCompositeDataGeometryFilter.New();
            //geom1.SetInputConnection(0, reader.GetOutputPort(0));

            vtkStructuredGridGeometryFilter geometryFilter = vtkStructuredGridGeometryFilter.New();
            geometryFilter.SetInput(reader.GetOutput().GetBlock(0));
            //geometryFilter.SetInputConnection(geom1.GetOutputPort(0));
            geometryFilter.Update();

            // Visualize
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(geometryFilter.GetOutputPort());
            //mapper.SetInputConnection(geom1.GetOutputPort());
            mapper.ScalarVisibilityOn();
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

        public static void ReadPLY(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\bunny.ply");
            vtkPLYReader reader = vtkPLYReader.New();
            reader.SetFileName(filePath);
            reader.Update();
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(reader.GetOutputPort());

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

        public static void ReadSTL(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\42400-IDGH.stl");
            vtkSTLReader reader = vtkSTLReader.New();
            reader.SetFileName(filePath);
            reader.Update();
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(reader.GetOutputPort());

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


        public static void SimplePointsReader(RenderWindowControl renderWindowControl1, double zScale = 1.0)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\points1.txt");

            vtkSimplePointsReader reader = vtkSimplePointsReader.New();
            reader.SetFileName(filePath);
            reader.Update();

            // 获取输出 polydata 和点集合
            vtkPolyData poly = reader.GetOutput();
            vtkPoints pts = poly?.GetPoints();
            long nPoints = (pts != null && pts.GetNumberOfPoints() > 0) ? pts.GetNumberOfPoints() : 0;

            // 创建标量数组（按 Z 高度）
            vtkFloatArray scalars = vtkFloatArray.New();
            scalars.SetName("ZHeight");
            scalars.SetNumberOfComponents(1);

            double minZ = double.MaxValue;
            double maxZ = double.MinValue;
            double[] xyz = new double[3];
            //放大系数
            //var zScale = 5000;
            for (int i = 0; i < nPoints; i++)
            {
                xyz = pts.GetPoint(i);
                xyz[2] *= zScale; // 调整 Z 轴幅度
                pts.SetPoint(i, xyz[0], xyz[1], xyz[2]); // 放大z轴
                double z = xyz[2];
                scalars.InsertNextValue((float)z);
                if (z < minZ) minZ = z;
                if (z > maxZ) maxZ = z;
            }

            if (nPoints == 0)
            {
                // 防护：无点时设置默认范围
                minZ = 0.0;
                maxZ = 1.0;
            }

            // 将标量赋给点数据
            poly.GetPointData().SetScalars(scalars);

            // 创建 LookupTable，根据 Z 值映射颜色（蓝->绿->黄->红）
            vtkLookupTable lut = vtkLookupTable.New();
            lut.SetNumberOfTableValues(256);
            // 使用 hue 从蓝到红（约 0.667 -> 0.0）
            lut.SetHueRange(0.667, 0.0);
            lut.SetSaturationRange(1.0, 1.0);
            lut.SetValueRange(1.0, 1.0);
            lut.SetTableRange(minZ, maxZ);
            lut.Build();

            // 可视化
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(reader.GetOutputPort());
            mapper.SetLookupTable(lut);
            mapper.SetScalarRange(minZ, maxZ);
            mapper.ScalarVisibilityOn();
            mapper.UseLookupTableScalarRangeOn();

            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetPointSize(4);
            actor.GetProperty().SetRepresentationToPoints();

            // 添加鼠标交互器以显示点信息
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            renderer.SetBackground(0.2, 0.3, 0.4);
            renderer.AddActor(actor);

            vtkRenderWindowInteractor interactor = renderWindow.GetInteractor();
            vtkInteractorStyleTrackballCamera style = vtkInteractorStyleTrackballCamera.New();
            interactor.SetInteractorStyle(style);

            vtkPointPicker pointPicker = vtkPointPicker.New();
            interactor.SetPicker(pointPicker);

            //// 添加坐标轴
            //vtkAxesActor axes = vtkAxesActor.New();
            //axes.SetTotalLength(1.0, 1.0, 1.0); // 设置坐标轴长度
            //axes.SetShaftTypeToCylinder();
            //axes.SetCylinderRadius(0.02);

            // 创建 ToolTip 控件并添加到窗体
            ToolTip toolTip = new ToolTip();
            renderWindowControl1.Parent.Controls.Add(new Control()); // 确保 ToolTip 有宿主控件

            interactor.MouseMoveEvt += (sender, args) =>
            {
                int[] eventPosition = interactor.GetEventPosition();
                if (pointPicker.Pick(eventPosition[0], eventPosition[1], 0, renderer) != 0)
                {
                    double[] pickedPosition = pointPicker.GetPickPosition();
                    long pointId = pointPicker.GetPointId();
                    if (pointId >= 0)
                    {
                        double zValue = scalars.GetValue(pointId);
                        string message = $"Point ID: {pointId}, Coordinates: ({pickedPosition[0]:F2}, {pickedPosition[1]:F2}, {pickedPosition[2]:F2}), Z: {zValue:F2}";
                        Debug.WriteLine(message);
                        renderWindowControl1.Invoke((MethodInvoker)(() =>
                        {
                            toolTip.SetToolTip(renderWindowControl1, message);
                        }));
                    }
                }
            };
        }

        public static void VRML(RenderWindowControl renderWindowControl1)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\bot2.wrl");
            // reader
            vtkVRMLImporter importer = vtkVRMLImporter.New();
            importer.SetFileName(filePath);
            importer.Update();

            // get a reference to the renderwindow of our renderWindowControl1
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            renderWindow.AddRenderer(importer.GetRenderer());
            renderWindow.Render();

            //vtkActorCollection actors = importer.GetRenderer().GetActors();
            //actors.InitTraversal();
            //vtkActor tmp;
            //while(( tmp = actors.GetNextActor()) != null) {
            //}
        }
        public static void SimplePointsReaderWithScanEffect(RenderWindowControl renderWindowControl1, double zScale = 1.0, int delayMs = 50)
        {
            // Path to vtk data must be set as an environment variable
            // VTK_DATA_ROOT = "C:\VTK\vtkdata-5.8.0"
            vtkTesting test = vtkTesting.New();
            string root = test.GetDataRoot();
            string filePath = System.IO.Path.Combine(root, @"Data\points1.txt");

            vtkSimplePointsReader reader = vtkSimplePointsReader.New();
            reader.SetFileName(filePath);
            reader.Update();

            // 获取输出 polydata 和点集合
            vtkPolyData poly = reader.GetOutput();
            vtkPoints pts = poly?.GetPoints();
            long nPoints = (pts != null && pts.GetNumberOfPoints() > 0) ? pts.GetNumberOfPoints() : 0;

            // 创建标量数组（按 Z 高度）
            vtkFloatArray scalars = vtkFloatArray.New();
            scalars.SetName("ZHeight");
            scalars.SetNumberOfComponents(1);

            double minZ = double.MaxValue;
            double maxZ = double.MinValue;
            double[] xyz = new double[3];

            // 获取点的范围
            double[] bounds = poly.GetBounds();
            double xMin = bounds[0], xMax = bounds[1];
            double yMin = bounds[2], yMax = bounds[3];
            double zMin = bounds[4], zMax = bounds[5];
            double xStep = 10;// (xMax - xMin) / 100; // 分成100步扫描

            // 创建用于扫查显示的点云数据结构
            vtkPoints scannedPoints = vtkPoints.New();
            vtkCellArray scannedVertices = vtkCellArray.New();
            vtkPolyData scannedPolyData = vtkPolyData.New();
            scannedPolyData.SetPoints(scannedPoints);
            scannedPolyData.SetVerts(scannedVertices);

            // 创建 LookupTable，根据 Z 值映射颜色（蓝->绿->黄->红）
            vtkLookupTable lut = vtkLookupTable.New();
            lut.SetNumberOfTableValues(256);
            // 使用 hue 从蓝到红（约 0.667 -> 0.0）
            lut.SetHueRange(0.667, 0.0);
            lut.SetSaturationRange(1.0, 1.0);
            lut.SetValueRange(1.0, 1.0);
            lut.Build();

            // 可视化设置
            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInput(scannedPolyData);
            mapper.SetLookupTable(lut);
            mapper.ScalarVisibilityOn();
            mapper.UseLookupTableScalarRangeOn();

            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetPointSize(4);
            actor.GetProperty().SetRepresentationToPoints();

            // 获取或创建渲染窗口和渲染器
            vtkRenderWindow renderWindow = renderWindowControl1.RenderWindow;
            vtkRenderer renderer = renderWindow.GetRenderers().GetFirstRenderer();
            if (renderer == null)
            {
                renderer = vtkRenderer.New();
                renderWindow.AddRenderer(renderer);
            }
            renderer.SetBackground(0.2, 0.3, 0.4);
            renderer.AddActor(actor);

            //// 设置相机位置，确保能看到完整的点云
            //vtkCamera camera = renderer.GetActiveCamera();
            //camera.SetPosition(xMin + (xMax - xMin) / 2, yMin - (yMax - yMin) * 1.5, zMin + (zMax - zMin) * 2);
            //camera.SetFocalPoint(xMin + (xMax - xMin) / 2, yMin + (yMax - yMin) / 2, zMin + (zMax - zMin) / 2);
            //camera.SetViewUp(0, 0, 1);
            //renderer.ResetCamera();

            // 设置交互器
            vtkRenderWindowInteractor interactor = renderWindow.GetInteractor();
            if (interactor != null)
            {
                vtkInteractorStyleTrackballCamera style = vtkInteractorStyleTrackballCamera.New();
                interactor.SetInteractorStyle(style);
            }

            // 开始扫查动画
            Task.Run(async () =>
            {
                for (double xThreshold = xMin; xThreshold <= xMax; xThreshold += xStep)
                {
                    // 处理当前扫描列的所有点
                    for (int i = 0; i < nPoints; i++)
                    {
                        xyz = pts.GetPoint(i);
                        // 检查点是否在当前扫描范围内
                        if (xyz[0] == xThreshold)
                        {
                            xyz[2] *= zScale; // 调整 Z 轴幅度
                            scannedPoints.InsertNextPoint(xyz[0], xyz[1], xyz[2]);
                            scannedVertices.InsertNextCell(1);
                            scannedVertices.InsertCellPoint(scannedPoints.GetNumberOfPoints() - 1);

                            double z = xyz[2];
                            scalars.InsertNextValue((float)z);
                            if (z < minZ) minZ = z;
                            if (z > maxZ) maxZ = z;
                        }
                    }

                    // 更新标量范围和渲染
                    scannedPolyData.GetPointData().SetScalars(scalars);
                    lut.SetTableRange(minZ, maxZ);
                    lut.Build();
                    mapper.SetScalarRange(minZ, maxZ);
                    
                    // 在UI线程中渲染
                    renderWindowControl1.Invoke((MethodInvoker)(() =>
                    {
                        renderWindow.Render();
                    }));

                    // 控制扫查速度
                    await Task.Delay(delayMs);
                }
                
                // 扫查完成后输出信息
                Console.WriteLine("点云扫查动画完成。总共显示点数: " + scannedPoints.GetNumberOfPoints());
            });
        }
    }
}
