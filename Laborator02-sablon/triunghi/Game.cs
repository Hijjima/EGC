using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Egc;

public class Game : GameWindow
{
    private readonly Camera _camera = new();

    private Shader _shader = null!;
    private Quad _quad = null!; // Am schimbat din Triangle în Quad

    private Color4 _clearColor = new(0.05f, 0.1f, 0.3f, 1.0f);
    private Color4 _tint = Color4.White;
    private Vector3 _offset = new Vector3(0.3f, 0.2f, 0.0f);

    private readonly string _baseTitle;
    private double _elapsed;
    private double _secondTimer;
    private int _updates;
    private int _frames;

    private bool _wireframe = false;

    public Game(int width, int height, string title)
        : base(GameWindowSettings.Default,
               new NativeWindowSettings
               {
                   ClientSize = new Vector2i(width, height),
                   Title = title,
                   APIVersion = new Version(3, 3),
                   Profile = ContextProfile.Core,
                   Flags = ContextFlags.ForwardCompatible
               })
    {
        _baseTitle = title;
        VSync = VSyncMode.On;
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        Console.WriteLine($"OpenGL  : {GL.GetString(StringName.Version)}");
        Console.WriteLine($"Vendor  : {GL.GetString(StringName.Vendor)}");
        Console.WriteLine($"Renderer: {GL.GetString(StringName.Renderer)}");

        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

        _shader = Shader.FromFiles("Shaders/shader.vert", "Shaders/shader.frag");
        _quad = new Quad(); // Inițializăm Quad
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
        _camera.Resize(e.Width, e.Height);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        float dt = (float)args.Time;
        _elapsed += dt;
        float t = (float)_elapsed; // Declarat o singură dată aici

        // --- LOGICA PENTRU PROBLEMA 1 (Fundal animat) ---
        _clearColor = new Color4(
            0.5f + 0.5f * MathF.Sin(t),
            0.5f + 0.5f * MathF.Sin(t + 2.094f),
            0.5f + 0.5f * MathF.Sin(t + 4.189f),
            1.0f);

        // --- LOGICA PENTRU PROBLEMA 2 (Nuanță taste) ---
        if (KeyboardState.IsKeyPressed(Keys.D1)) _tint = Color4.Red;
        if (KeyboardState.IsKeyPressed(Keys.D2)) _tint = Color4.Green;
        if (KeyboardState.IsKeyPressed(Keys.D3)) _tint = Color4.Blue;
        if (KeyboardState.IsKeyPressed(Keys.D0)) _tint = Color4.White;

        // --- LOGICA PENTRU PROBLEMA 3 (Wireframe) ---
        if (KeyboardState.IsKeyPressed(Keys.F)) _wireframe = !_wireframe;

        // --- LOGICA PENTRU PROBLEMA 4 (Mișcare cu săgeți) ---
        const float speed = 1.0f;
        if (KeyboardState.IsKeyDown(Keys.Left)) _offset.X -= speed * dt;
        if (KeyboardState.IsKeyDown(Keys.Right)) _offset.X += speed * dt;
        if (KeyboardState.IsKeyDown(Keys.Up)) _offset.Y += speed * dt;
        if (KeyboardState.IsKeyDown(Keys.Down)) _offset.Y -= speed * dt;

        // --- LOGICA PENTRU PROBLEMA 5 (Deformare) ---
        // Am șters codul cu _triangle pentru că acum avem _quad.
        // Dacă vrei să deformezi dreptunghiul, va trebui să adaugi metodele SetPosition în Quad.cs.

        // Tasta Escape
        if (KeyboardState.IsKeyDown(Keys.Escape)) Close();

        // Titlu
        _updates++;
        _secondTimer += args.Time;
        if (_secondTimer >= 1.0)
        {
            Title = $"{_baseTitle} | update/s: {_updates} | render/s: {_frames}";
            _updates = 0;
            _frames = 0;
            _secondTimer -= 1.0;
        }
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        _frames++;

        GL.ClearColor(_clearColor.R, _clearColor.G, _clearColor.B, _clearColor.A);
        GL.Clear(ClearBufferMask.ColorBufferBit);

        _shader.Use();
        _shader.SetMatrix4("uView", _camera.ViewMatrix);
        _shader.SetMatrix4("uProjection", _camera.ProjectionMatrix);
        _shader.SetVector3("uOffset", _offset);
        _shader.SetColor("uTint", _tint);

        // --- PROBLEMA 3: Setăm modul de desenare (Wireframe comutat cu F) ---
        GL.PolygonMode(MaterialFace.FrontAndBack,
            _wireframe ? PolygonMode.Line : PolygonMode.Fill);

        GL.PointSize(10.0f); // Pentru modul Point

        // --- Desenăm Dreptunghiul ---
        _quad.Draw();

        SwapBuffers();
    }

    protected override void OnUnload()
    {
        _quad.Dispose();
        _shader.Dispose();

        base.OnUnload();
    }
}