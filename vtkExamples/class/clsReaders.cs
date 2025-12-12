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
            
            // 添加调试信息
            Console.WriteLine($"点云范围: xMin={xMin}, xMax={xMax}, yMin={yMin}, yMax={yMax}, zMin={zMin}, zMax={zMax}");
            
            // 确保xStep合理，避免因范围过小导致步长为0
            int numSteps = 100;
            double xStep = (xMax - xMin) / numSteps;
            if (xStep <= 0)
            {
                xStep = 0.1; // 设置最小步长
                Console.WriteLine($"自动调整xStep为最小步长: {xStep}");
            }
            Console.WriteLine($"扫查参数: numSteps={numSteps}, xStep={xStep}");

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

            //// 坐标轴和标签
            //vtkAssembly axesAssembly = vtkAssembly.New();

            //// X轴
            //vtkLineSource xAxis = vtkLineSource.New();
            //xAxis.SetPoint1(xMin, yMin, zMin);
            //xAxis.SetPoint2(xMax, yMin, zMin);
            //vtkPolyDataMapper xMapper = vtkPolyDataMapper.New();
            //xMapper.SetInputConnection(xAxis.GetOutputPort());
            //vtkActor xActor = vtkActor.New();
            //xActor.SetMapper(xMapper);
            //xActor.GetProperty().SetColor(1, 0, 0);
            //xActor.GetProperty().SetLineWidth(2);
            //axesAssembly.AddPart(xActor);

            //// Y轴
            //vtkLineSource yAxis = vtkLineSource.New();
            //yAxis.SetPoint1(xMin, yMin, zMin);
            //yAxis.SetPoint2(xMin, yMax, zMin);
            //vtkPolyDataMapper yMapper = vtkPolyDataMapper.New();
            //yMapper.SetInputConnection(yAxis.GetOutputPort());
            //vtkActor yActor = vtkActor.New();
            //yActor.SetMapper(yMapper);
            //yActor.GetProperty().SetColor(0, 1, 0);
            //yActor.GetProperty().SetLineWidth(2);
            //axesAssembly.AddPart(yActor);

            //// Z轴
            //vtkLineSource zAxis = vtkLineSource.New();
            //zAxis.SetPoint1(xMin, yMin, zMin);
            //zAxis.SetPoint2(xMin, yMin, zMax);
            //vtkPolyDataMapper zMapper = vtkPolyDataMapper.New();
            //zMapper.SetInputConnection(zAxis.GetOutputPort());
            //vtkActor zActor = vtkActor.New();
            //zActor.SetMapper(zMapper);
            //zActor.GetProperty().SetColor(0, 0, 1);
            //zActor.GetProperty().SetLineWidth(2);
            //axesAssembly.AddPart(zActor);

            //// X轴标签
            //for (double x = Math.Ceiling(xMin); x <= xMax; x += 1.0)
            //{
            //    vtkVectorText label = vtkVectorText.New();
            //    label.SetText($"{x:0}");
            //    vtkPolyDataMapper labelMapper = vtkPolyDataMapper.New();
            //    labelMapper.SetInputConnection(label.GetOutputPort());
            //    vtkFollower labelActor = vtkFollower.New();
            //    labelActor.SetMapper(labelMapper);
            //    labelActor.SetScale(0.5, 0.5, 0.5);
            //    labelActor.SetPosition(x, yMin - (yMax - yMin) * 0.03, zMin - (zMax - zMin) * 0.03);
            //    labelActor.GetProperty().SetColor(1, 0, 0);
            //    axesAssembly.AddPart(labelActor);
            //}
            //// Y轴标签
            //for (double y = Math.Ceiling(yMin); y <= yMax; y += 1.0)
            //{
            //    vtkVectorText label = vtkVectorText.New();
            //    label.SetText($"{y:0}");
            //    vtkPolyDataMapper labelMapper = vtkPolyDataMapper.New();
            //    labelMapper.SetInputConnection(label.GetOutputPort());
            //    vtkFollower labelActor = vtkFollower.New();
            //    labelActor.SetMapper(labelMapper);
            //    labelActor.SetScale(0.5, 0.5, 0.5);
            //    labelActor.SetPosition(xMin - (xMax - xMin) * 0.03, y, zMin - (zMax - zMin) * 0.03);
            //    labelActor.GetProperty().SetColor(0, 1, 0);
            //    axesAssembly.AddPart(labelActor);
            //}
            //// Z轴标签
            //for (double z = Math.Ceiling(zMin); z <= zMax; z += 1.0)
            //{
            //    vtkVectorText label = vtkVectorText.New();
            //    label.SetText($"{z:0}");
            //    vtkPolyDataMapper labelMapper = vtkPolyDataMapper.New();
            //    labelMapper.SetInputConnection(label.GetOutputPort());
            //    vtkFollower labelActor = vtkFollower.New();
            //    labelActor.SetMapper(labelMapper);
            //    labelActor.SetScale(0.5, 0.5, 0.5);
            //    labelActor.SetPosition(xMin - (xMax - xMin) * 0.03, yMin - (yMax - yMin) * 0.03, z);
            //    labelActor.GetProperty().SetColor(0, 0, 1);
            //    axesAssembly.AddPart(labelActor);
            //}
            ////严重影响速度，禁用
            //renderer.AddActor(axesAssembly);

            // 设置相机位置，确保能看到完整的点云
            vtkCamera camera = renderer.GetActiveCamera();

            // 计算点云中心
            double centerX = xMin + (xMax - xMin) / 2;
            double centerY = yMin + (yMax - yMin) / 2;
            double centerZ = zMin + (zMax - zMin) / 2;

            // 设置相机位置在点云前方上方，确保能看到完整范围
            double distance = Math.Max(xMax - xMin, Math.Max(yMax - yMin, zMax - zMin)) * 2;
            camera.SetPosition(centerX, centerY - distance, centerZ + distance * 0.5);

            // 设置焦点为点云中心
            camera.SetFocalPoint(centerX, centerY, centerZ);

            // 设置视图方向
            camera.SetViewUp(0, 0, 1);

            // 重置相机以适应点云范围
            renderer.ResetCamera();
            renderer.ResetCameraClippingRange();

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
                Console.WriteLine($"开始扫查: xMin={xMin}, xMax={xMax}, xStep={xStep}");
                int totalSteps = 0;
                int maxPointsPerStep = 0;
                
                // 预计算点云的完整Z范围（用于颜色映射）
                double fullMinZ = double.MaxValue;
                double fullMaxZ = double.MinValue;
                for (int i = 0; i < nPoints; i++)
                {
                    double[] point = pts.GetPoint(i);
                    double z = point[2] * zScale;
                    if (z < fullMinZ) fullMinZ = z;
                    if (z > fullMaxZ) fullMaxZ = z;
                }
                
                for (double xThreshold = xMin; xThreshold <= xMax + xStep; xThreshold += xStep)
                {
                    totalSteps++;
                    int currentStepPoints = 0;
                    
                    // 重置当前步骤的点云数据
                    scannedPoints.Reset();
                    scannedVertices.Reset();
                    scalars.Reset();
                    
                    // 处理所有点，累积显示到当前阈值
                    for (int i = 0; i < nPoints; i++)
                    {
                        xyz = pts.GetPoint(i);
                        
                        // 累积显示：只显示x坐标小于等于当前阈值的点
                        if (xyz[0] <= xThreshold + xStep)
                        {
                            xyz[2] *= zScale; // 调整 Z 轴幅度
                            scannedPoints.InsertNextPoint(xyz[0], xyz[1], xyz[2]);
                            scannedVertices.InsertNextCell(1);
                            scannedVertices.InsertCellPoint(scannedPoints.GetNumberOfPoints() - 1);

                            double z = xyz[2];
                            scalars.InsertNextValue((float)z);
                            
                            currentStepPoints++;
                        }
                    }

                    maxPointsPerStep = Math.Max(maxPointsPerStep, currentStepPoints);
                    Console.WriteLine($"步骤 {totalSteps}: xThreshold={xThreshold:F2}, 点数={currentStepPoints}, 累计点数={scannedPoints.GetNumberOfPoints()}");
                    
                    // 更新标量范围和渲染 - 使用完整Z范围以保持颜色一致性
                    scannedPolyData.SetPoints(scannedPoints);
                    scannedPolyData.SetVerts(scannedVertices);
                    scannedPolyData.GetPointData().SetScalars(scalars);
                    scannedPolyData.Modified();
                    
                    // 使用完整Z范围确保颜色映射一致
                    lut.SetTableRange(fullMinZ, fullMaxZ);
                    lut.Build();
                    mapper.SetScalarRange(fullMinZ, fullMaxZ);
                    
                    // 在UI线程中渲染
                    renderWindowControl1.Invoke((MethodInvoker)(() =>
                    {
                        renderWindow.Render();
                    }));

                    // 控制扫查速度
                    await Task.Delay(delayMs);
                }
                
                Console.WriteLine($"扫查完成: 总步骤={totalSteps}, 每步最大点数={maxPointsPerStep}");
                Console.WriteLine("点云扫查动画完成。总共显示点数: " + scannedPoints.GetNumberOfPoints());
            });
        }
    }
}
