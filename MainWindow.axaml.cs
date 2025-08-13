using Avalonia.Controls;
using Avalonia.Interactivity;
using HelixToolkit.Avalonia;
using HelixToolkit.Avalonia.Core;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Avalonia3DSpace;

public partial class MainWindow : Window
{
    private readonly List<ModelVisual3D> _objects = new();
    private readonly Timer _rotationTimer;
    private int _objectCounter = 0;
    private bool _isRotating = false;

    public MainWindow()
    {
        InitializeComponent();
        
        // 初始化旋转定时器
        _rotationTimer = new Timer(50); // 20 FPS
        _rotationTimer.Elapsed += OnRotationTimerElapsed;
        
        UpdateInfo();
    }

    private void OnResetView(object? sender, RoutedEventArgs e)
    {
        Viewport3D.Reset();
        UpdateInfo("视角已重置");
    }

    private void OnTopView(object? sender, RoutedEventArgs e)
    {
        Viewport3D.Camera.Position = new System.Windows.Media.Media3D.Point3D(0, 10, 0);
        Viewport3D.Camera.LookDirection = new System.Windows.Media.Media3D.Vector3D(0, -1, 0);
        Viewport3D.Camera.UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 0, 1);
        UpdateInfo("切换到俯视图");
    }

    private void OnSideView(object? sender, RoutedEventArgs e)
    {
        Viewport3D.Camera.Position = new System.Windows.Media.Media3D.Point3D(10, 0, 0);
        Viewport3D.Camera.LookDirection = new System.Windows.Media.Media3D.Vector3D(-1, 0, 0);
        Viewport3D.Camera.UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0);
        UpdateInfo("切换到侧视图");
    }

    private void OnFrontView(object? sender, RoutedEventArgs e)
    {
        Viewport3D.Camera.Position = new System.Windows.Media.Media3D.Point3D(0, 0, 10);
        Viewport3D.Camera.LookDirection = new System.Windows.Media.Media3D.Vector3D(0, 0, -1);
        Viewport3D.Camera.UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0);
        UpdateInfo("切换到前视图");
    }

    private void OnAddCube(object? sender, RoutedEventArgs e)
    {
        var cube = new CubeVisual3D
        {
            SideLength = 1.0,
            Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue),
            Center = new System.Windows.Media.Media3D.Point3D(_objectCounter * 2 - 2, 0.5, 0)
        };
        
        _objects.Add(cube);
        Viewport3D.Children.Add(cube);
        _objectCounter++;
        
        UpdateInfo($"已添加立方体 #{_objectCounter}");
    }

    private void OnAddSphere(object? sender, RoutedEventArgs e)
    {
        var sphere = new SphereVisual3D
        {
            Radius = 0.5,
            Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red),
            Center = new System.Windows.Media.Media3D.Point3D(_objectCounter * 2 - 2, 0.5, 0)
        };
        
        _objects.Add(sphere);
        Viewport3D.Children.Add(sphere);
        _objectCounter++;
        
        UpdateInfo($"已添加球体 #{_objectCounter}");
    }

    private void OnAddCylinder(object? sender, RoutedEventArgs e)
    {
        var cylinder = new CylinderVisual3D
        {
            Height = 1.0,
            Diameter = 0.8,
            Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green),
            Center = new System.Windows.Media.Media3D.Point3D(_objectCounter * 2 - 2, 0.5, 0)
        };
        
        _objects.Add(cylinder);
        Viewport3D.Children.Add(cylinder);
        _objectCounter++;
        
        UpdateInfo($"已添加圆柱体 #{_objectCounter}");
    }

    private void OnClearObjects(object? sender, RoutedEventArgs e)
    {
        foreach (var obj in _objects)
        {
            Viewport3D.Children.Remove(obj);
        }
        _objects.Clear();
        _objectCounter = 0;
        
        UpdateInfo("已清除所有对象");
    }

    private void OnStartRotation(object? sender, RoutedEventArgs e)
    {
        if (!_isRotating)
        {
            _rotationTimer.Start();
            _isRotating = true;
            UpdateInfo("开始旋转动画");
        }
    }

    private void OnStopRotation(object? sender, RoutedEventArgs e)
    {
        if (_isRotating)
        {
            _rotationTimer.Stop();
            _isRotating = false;
            UpdateInfo("停止旋转动画");
        }
    }

    private void OnRotationTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (_isRotating)
        {
            Dispatcher.InvokeAsync(() =>
            {
                foreach (var obj in _objects)
                {
                    var transform = obj.Transform as System.Windows.Media.Media3D.RotateTransform3D;
                    if (transform == null)
                    {
                        transform = new System.Windows.Media.Media3D.RotateTransform3D();
                        obj.Transform = transform;
                    }
                    
                    transform.Rotation = new System.Windows.Media.Media3D.AxisAngleRotation3D(
                        new System.Windows.Media.Media3D.Vector3D(0, 1, 0), 
                        DateTime.Now.Millisecond * 0.36); // 每秒旋转360度
                }
            });
        }
    }

    private void UpdateInfo(string? message = null)
    {
        if (message != null)
        {
            InfoText.Text = message;
        }
        
        var info = $"场景信息:\n";
        info += $"对象数量: {_objects.Count}\n";
        info += $"相机位置: {Viewport3D.Camera.Position}\n";
        info += $"旋转状态: {(_isRotating ? "运行中" : "已停止")}";
        
        InfoText.Text = info;
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        _rotationTimer?.Stop();
        _rotationTimer?.Dispose();
        base.OnUnloaded(e);
    }
}