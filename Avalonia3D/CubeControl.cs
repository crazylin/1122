using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Avalonia3D;

public sealed class CubeControl : OpenGlControlBase
{
    private bool _bindingsLoaded;
    private bool _initialized;
    private int _shaderProgram;
    private int _vao;
    private int _vbo;
    private int _ebo;

    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    private sealed class AvaloniaBindingsContext : OpenTK.IBindingsContext
    {
        private readonly GlInterface _gl;
        public AvaloniaBindingsContext(GlInterface gl) => _gl = gl;
        public IntPtr GetProcAddress(string procName) => _gl.GetProcAddress(procName);
    }

    protected override void OnOpenGlRender(GlInterface gl, int fb)
    {
        if (!_bindingsLoaded)
        {
            OpenTK.Graphics.OpenGL.GL.LoadBindings(new AvaloniaBindingsContext(gl));
            _bindingsLoaded = true;
        }

        if (!_initialized)
        {
            InitializeGlResources();
            _initialized = true;
        }

        var size = Bounds.Size;
        int width = Math.Max(1, (int)Math.Round(size.Width));
        int height = Math.Max(1, (int)Math.Round(size.Height));

        GL.Viewport(0, 0, width, height);
        GL.ClearColor(0.1f, 0.12f, 0.15f, 1f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.UseProgram(_shaderProgram);

        float t = (float)_stopwatch.Elapsed.TotalSeconds;
        var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60f), width / (float)height, 0.1f, 100f);
        var view = Matrix4.LookAt(new Vector3(1.8f, 1.6f, 2.2f), Vector3.Zero, Vector3.UnitY);
        var model = Matrix4.CreateRotationY(t * 0.8f) * Matrix4.CreateRotationX(t * 0.4f);
        var mvp = model * view * projection;

        int mvpLoc = GL.GetUniformLocation(_shaderProgram, "uMVP");
        GL.UniformMatrix4(mvpLoc, false, ref mvp);

        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);

        RequestNextFrameRendering();
    }

    private static void CheckShader(int shader, string name)
    {
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
        if (status == (int)All.False)
        {
            string info = GL.GetShaderInfoLog(shader);
            throw new InvalidOperationException($"Failed to compile {name} shader: {info}");
        }
    }

    private static void CheckProgram(int program)
    {
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int status);
        if (status == (int)All.False)
        {
            string info = GL.GetProgramInfoLog(program);
            throw new InvalidOperationException($"Failed to link program: {info}");
        }
    }

    private void InitializeGlResources()
    {
        GL.Enable(EnableCap.DepthTest);

        const string vertexShaderSource = @"#version 330 core\nlayout(location = 0) in vec3 aPosition;\nlayout(location = 1) in vec3 aColor;\nuniform mat4 uMVP;\nout vec3 vColor;\nvoid main(){ vColor = aColor; gl_Position = uMVP * vec4(aPosition,1.0); }";
        const string fragmentShaderSource = @"#version 330 core\nin vec3 vColor;\nout vec4 FragColor;\nvoid main(){ FragColor = vec4(vColor,1.0); }";

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);
        CheckShader(vertexShader, "vertex");

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);
        CheckShader(fragmentShader, "fragment");

        _shaderProgram = GL.CreateProgram();
        GL.AttachShader(_shaderProgram, vertexShader);
        GL.AttachShader(_shaderProgram, fragmentShader);
        GL.LinkProgram(_shaderProgram);
        CheckProgram(_shaderProgram);

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        float[] vertices =
        {
            -0.5f, -0.5f,  0.5f,   1f, 0f, 0f,
             0.5f, -0.5f,  0.5f,   0f, 1f, 0f,
             0.5f,  0.5f,  0.5f,   0f, 0f, 1f,
            -0.5f,  0.5f,  0.5f,   1f, 1f, 0f,
            -0.5f, -0.5f, -0.5f,   1f, 0f, 1f,
             0.5f, -0.5f, -0.5f,   0f, 1f, 1f,
             0.5f,  0.5f, -0.5f,   1f, 1f, 1f,
            -0.5f,  0.5f, -0.5f,   0.3f, 0.3f, 0.3f,
        };

        uint[] indices =
        {
            0, 1, 2, 2, 3, 0,
            1, 5, 6, 6, 2, 1,
            7, 6, 5, 5, 4, 7,
            4, 0, 3, 3, 7, 4,
            4, 5, 1, 1, 0, 4,
            3, 2, 6, 6, 7, 3
        };

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        int stride = 6 * sizeof(float);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
        GL.EnableVertexAttribArray(1);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));

        GL.BindVertexArray(0);
    }
}