using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Avalonia3DControls;

public partial class MainWindow : Window
{
    private readonly List<Control> _controls = new();
    private int _controlCounter = 0;
    private double _currentSize = 1.0;
    private double _currentSpacing = 3.0;

    public MainWindow()
    {
        InitializeComponent();
        
        // 绑定滑块事件
        SizeSlider.ValueChanged += OnSizeChanged;
        SpacingSlider.ValueChanged += OnSpacingChanged;
        
        UpdateInfo();
    }

    private void OnSizeChanged(object? sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        _currentSize = e.NewValue;
        SizeValue.Text = _currentSize.ToString("F1");
        UpdateControlSizes();
    }

    private void OnSpacingChanged(object? sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        _currentSpacing = e.NewValue;
        SpacingValue.Text = _currentSpacing.ToString("F1");
        RearrangeControls();
    }

    private void OnAdd3DButton(object? sender, RoutedEventArgs e)
    {
        var button = Create3DButton();
        _controls.Add(button);
        Canvas3D.Children.Add(button);
        _controlCounter++;
        
        RearrangeControls();
        UpdateInfo($"已添加3D按钮 #{_controlCounter}");
    }

    private void OnAdd3DSlider(object? sender, RoutedEventArgs e)
    {
        var slider = Create3DSlider();
        _controls.Add(slider);
        Canvas3D.Children.Add(slider);
        _controlCounter++;
        
        RearrangeControls();
        UpdateInfo($"已添加3D滑块 #{_controlCounter}");
    }

    private void OnAdd3DSwitch(object? sender, RoutedEventArgs e)
    {
        var switchControl = Create3DSwitch();
        _controls.Add(switchControl);
        Canvas3D.Children.Add(switchControl);
        _controlCounter++;
        
        RearrangeControls();
        UpdateInfo($"已添加3D开关 #{_controlCounter}");
    }

    private void OnAdd3DProgressBar(object? sender, RoutedEventArgs e)
    {
        var progressBar = Create3DProgressBar();
        _controls.Add(progressBar);
        Canvas3D.Children.Add(progressBar);
        _controlCounter++;
        
        RearrangeControls();
        UpdateInfo($"已添加3D进度条 #{_controlCounter}");
    }

    private void OnAdd3DTextBox(object? sender, RoutedEventArgs e)
    {
        var textBox = Create3DTextBox();
        _controls.Add(textBox);
        Canvas3D.Children.Add(textBox);
        _controlCounter++;
        
        RearrangeControls();
        UpdateInfo($"已添加3D文本框 #{_controlCounter}");
    }

    private void OnClearAll(object? sender, RoutedEventArgs e)
    {
        foreach (var control in _controls)
        {
            Canvas3D.Children.Remove(control);
        }
        _controls.Clear();
        _controlCounter = 0;
        
        UpdateInfo("已清除所有3D控件");
    }

    private Control Create3DButton()
    {
        var button = new Button
        {
            Content = "3D按钮",
            Width = 120 * _currentSize,
            Height = 60 * _currentSize,
            Background = new SolidColorBrush(Colors.DodgerBlue),
            Foreground = new SolidColorBrush(Colors.White),
            FontWeight = FontWeight.Bold,
            FontSize = 14 * _currentSize
        };

        // 添加3D效果
        button.Effect = new DropShadowEffect
        {
            Color = Colors.Black,
            BlurRadius = 10,
            OffsetX = 5,
            OffsetY = 5
        };

        button.Click += (s, e) => UpdateInfo($"点击了按钮 #{_controls.IndexOf(button) + 1}");

        return button;
    }

    private Control Create3DSlider()
    {
        var slider = new Slider
        {
            Width = 150 * _currentSize,
            Height = 40 * _currentSize,
            Minimum = 0,
            Maximum = 100,
            Value = 50
        };

        // 添加3D效果
        slider.Effect = new DropShadowEffect
        {
            Color = Colors.Black,
            BlurRadius = 8,
            OffsetX = 3,
            OffsetY = 3
        };

        slider.ValueChanged += (s, e) => UpdateInfo($"滑块值: {e.NewValue:F0}");

        return slider;
    }

    private Control Create3DSwitch()
    {
        var toggleSwitch = new ToggleSwitch
        {
            Width = 80 * _currentSize,
            Height = 40 * _currentSize
        };

        // 添加3D效果
        toggleSwitch.Effect = new DropShadowEffect
        {
            Color = Colors.Black,
            BlurRadius = 6,
            OffsetX = 2,
            OffsetY = 2
        };

        toggleSwitch.IsCheckedChanged += (s, e) => UpdateInfo($"开关状态: {(toggleSwitch.IsChecked == true ? "开启" : "关闭")}");

        return toggleSwitch;
    }

    private Control Create3DProgressBar()
    {
        var progressBar = new ProgressBar
        {
            Width = 150 * _currentSize,
            Height = 30 * _currentSize,
            Value = 75,
            Maximum = 100
        };

        // 添加3D效果
        progressBar.Effect = new DropShadowEffect
        {
            Color = Colors.Black,
            BlurRadius = 8,
            OffsetX = 3,
            OffsetY = 3
        };

        return progressBar;
    }

    private Control Create3DTextBox()
    {
        var textBox = new TextBox
        {
            Width = 150 * _currentSize,
            Height = 40 * _currentSize,
            Text = "3D文本框",
            FontSize = 14 * _currentSize,
            Background = new SolidColorBrush(Colors.White),
            Foreground = new SolidColorBrush(Colors.Black)
        };

        // 添加3D效果
        textBox.Effect = new DropShadowEffect
        {
            Color = Colors.Black,
            BlurRadius = 8,
            OffsetX = 3,
            OffsetY = 3
        };

        textBox.TextChanged += (s, e) => UpdateInfo($"文本框内容: {textBox.Text}");

        return textBox;
    }

    private void RearrangeControls()
    {
        var startX = 50.0;
        var startY = 50.0;
        var controlsPerRow = 3;

        for (int i = 0; i < _controls.Count; i++)
        {
            var control = _controls[i];
            var row = i / controlsPerRow;
            var col = i % controlsPerRow;

            Canvas.SetLeft(control, startX + col * (150 * _currentSize + _currentSpacing * 20));
            Canvas.SetTop(control, startY + row * (60 * _currentSize + _currentSpacing * 20));
        }
    }

    private void UpdateControlSizes()
    {
        foreach (var control in _controls)
        {
            if (control is Button button)
            {
                button.Width = 120 * _currentSize;
                button.Height = 60 * _currentSize;
                button.FontSize = 14 * _currentSize;
            }
            else if (control is Slider slider)
            {
                slider.Width = 150 * _currentSize;
                slider.Height = 40 * _currentSize;
            }
            else if (control is ToggleSwitch toggleSwitch)
            {
                toggleSwitch.Width = 80 * _currentSize;
                toggleSwitch.Height = 40 * _currentSize;
            }
            else if (control is ProgressBar progressBar)
            {
                progressBar.Width = 150 * _currentSize;
                progressBar.Height = 30 * _currentSize;
            }
            else if (control is TextBox textBox)
            {
                textBox.Width = 150 * _currentSize;
                textBox.Height = 40 * _currentSize;
                textBox.FontSize = 14 * _currentSize;
            }
        }
        
        RearrangeControls();
    }

    private void UpdateInfo(string? message = null)
    {
        if (message != null)
        {
            InfoText.Text = message;
        }
        
        var info = $"场景信息:\n";
        info += $"3D控件数量: {_controls.Count}\n";
        info += $"控件大小: {_currentSize:F1}\n";
        info += $"控件间距: {_currentSpacing:F1}\n";
        info += $"画布尺寸: {Canvas3D.Width} x {Canvas3D.Height}";
        
        InfoText.Text = info;
    }
}