using OpenTK.Graphics.OpenGL4;

namespace Egc;

public sealed class Triangle : IDisposable
{
    private const int FloatsPerVertex = 6;

    public float[] Vertices { get; } = new float[]
    {
        -0.5f, -0.5f, 0.0f,      1.0f, 0.0f, 0.0f,
         0.5f, -0.5f, 0.0f,      0.0f, 1.0f, 0.0f,
         0.0f,  0.5f, 0.0f,      0.0f, 0.0f, 1.0f
    };

    private readonly uint[] _indices = new uint[] { 0, 1, 2 };

    private readonly int _vao;
    private readonly int _vbo;
    private readonly int _ebo;

    public Triangle()
    {
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.DynamicDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

        int stride = FloatsPerVertex * sizeof(float);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
    }

    public void SetPosition(int vertex, float x, float y, float z = 0.0f)
    {
        int i = vertex * FloatsPerVertex;
        Vertices[i] = x;
        Vertices[i + 1] = y;
        Vertices[i + 2] = z;
    }

    public void SetColor(int vertex, float r, float g, float b)
    {
        int i = vertex * FloatsPerVertex + 3;
        Vertices[i] = r;
        Vertices[i + 1] = g;
        Vertices[i + 2] = b;
    }

    public void UpdateVertices()
    {
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, Vertices.Length * sizeof(float), Vertices);
    }

    public void Draw()
    {
        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(_vbo);
        GL.DeleteBuffer(_ebo);
        GL.DeleteVertexArray(_vao);
    }
}