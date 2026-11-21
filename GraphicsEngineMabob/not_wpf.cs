using System.Numerics;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using static GraphicsEngineMabob.Program;

namespace GraphicsEngineMabob;

public sealed class RenderApp {
    private const int ScreenWidth = 1200;
    private const int ScreenHeight = 1000;
    private float angleX;
    private float angleY;
    private float cameraDistance = 8.0f;
    private GL gl = null!;
    private ImGuiController imgui = null!;
    private IInputContext input = null!;
    private bool isAutoSpinning;

    private float hue;
    private Vector3 lineColor = new(0f, 0.5f, 0f);
    private float lineThickness = 1.5f;

    private mesh towerMesh = null!;

    private IWindow window = null!;

    public void Run() {
        towerMesh = CreateTower();

        WindowOptions options = WindowOptions.Default;
        options.Size = new Vector2D<int>(ScreenWidth, ScreenHeight);
        options.Title = "GraphicsEngineMabob";

        window = Window.Create(options);
        window.Load += OnLoad;
        window.Render += OnRender;
        window.FramebufferResize += size => gl.Viewport(size);
        window.Closing += OnClosing;
        window.Run();
        window.Dispose();
    }

    private static mesh Combine(params mesh[] parts) {
        mesh m = new();
        foreach (mesh part in parts) m.tris.AddRange(part.tris);
        return m;
    }

    private static mesh CreateTower() {
        mesh baseSlab = mesh.CreateBox(new vector3d(-1.5f, -2.4f, -1.5f), new vector3d(1.5f, -2.0f, 1.5f));
        mesh shaft = mesh.CreateCylinder(4f, 1f, 16);
        mesh topRing = mesh.Transform(new vector3d(0, 2.2f, 0), mesh.CreateCylinder(0.4f, 1.3f, 16));
        mesh roof = mesh.CreateCone(new vector3d(0, 2.4f, 0), new vector3d(0, 3.8f, 0), 1.3f, 0f, 16);

        return mesh.Transform(new vector3d(0, -0.7f, 0), Combine(baseSlab, shaft, topRing, roof));
    }

    private void OnLoad() {
        gl = GL.GetApi(window);
        input = window.CreateInput();
        imgui = new ImGuiController(gl, window, input);
    }

    private void OnClosing() {
        imgui?.Dispose();
        input?.Dispose();
        gl?.Dispose();
    }

    private void OnRender(double deltaTime) {
        float dt = (float)deltaTime;
        imgui.Update(dt);
        ImGuiIOPtr io = ImGui.GetIO();

        HandleKeys();

        hue = (hue + 60f * dt) % 360f;
        lineColor = HslToRgb(hue, 1f, 0.5f);

        if (isAutoSpinning) {
            angleX += 1.0f * dt;
            angleY += 1.5f * dt;
        }

        gl.ClearColor(0f, 0f, 0f, 1f);
        gl.Clear((uint)ClearBufferMask.ColorBufferBit);

        DrawScene(io.DisplaySize);

        ImGui.Begin("Line");
        ImGui.SliderFloat("Thickness", ref lineThickness, 0.5f, 5.0f);
        ImGui.End();
        imgui.Render();
    }
    
    private static Vector3 HslToRgb(float h, float s, float l) {
        float c = (1 - Math.Abs(2 * l - 1)) * s;
        float x = c * (1 - Math.Abs((h / 60) % 2 - 1));
        float m = l - c / 2;

        float r, g, b;
        if      (h < 60)  { r = c; g = x; b = 0; }
        else if (h < 120) { r = x; g = c; b = 0; }
        else if (h < 180) { r = 0; g = c; b = x; }
        else if (h < 240) { r = 0; g = x; b = c; }
        else if (h < 300) { r = x; g = 0; b = c; }
        else              { r = c; g = 0; b = x; }
        return new Vector3(r + m, g + m, b + m);
    }

    private void HandleKeys() {
        if (ImGui.GetIO().WantTextInput) return;

        if (ImGui.IsKeyPressed(ImGuiKey.Equal) || ImGui.IsKeyPressed(ImGuiKey.KeypadAdd))
            cameraDistance = MathF.Max(3.0f, cameraDistance - 0.5f);
        else if (ImGui.IsKeyPressed(ImGuiKey.Minus) || ImGui.IsKeyPressed(ImGuiKey.KeypadSubtract))
            cameraDistance += 0.5f;
        else if (ImGui.IsKeyPressed(ImGuiKey.Q, false))
            isAutoSpinning = !isAutoSpinning;
        else if (ImGui.IsKeyPressed(ImGuiKey.W) || ImGui.IsKeyPressed(ImGuiKey.UpArrow))
            angleX -= 0.1f;
        else if (ImGui.IsKeyPressed(ImGuiKey.S) || ImGui.IsKeyPressed(ImGuiKey.DownArrow))
            angleX += 0.1f;
        else if (ImGui.IsKeyPressed(ImGuiKey.A) || ImGui.IsKeyPressed(ImGuiKey.LeftArrow))
            angleY -= 0.1f;
        else if (ImGui.IsKeyPressed(ImGuiKey.D) || ImGui.IsKeyPressed(ImGuiKey.RightArrow))
            angleY += 0.1f;
    }

    private vector3d MultiplyVectorMatrix(vector3d inputVertex, float angleX, float angleY, float screenW,
        float screenH) {
        vector3d outputVertex = new();

        float rotY_x = inputVertex.x * MathF.Cos(angleY) + inputVertex.z * MathF.Sin(angleY);
        float rotY_y = inputVertex.y;
        float rotY_z = inputVertex.x * -MathF.Sin(angleY) + inputVertex.z * MathF.Cos(angleY);

        float rotX_x = rotY_x;
        float rotX_y = rotY_y * MathF.Cos(angleX) - rotY_z * MathF.Sin(angleX);
        float rotX_z = rotY_y * MathF.Sin(angleX) + rotY_z * MathF.Cos(angleX);

        rotX_z += cameraDistance;

        float fieldOfView = 90.0f;
        float aspectRatio = screenH / screenW;
        float fovRad = 1.0f / MathF.Tan(fieldOfView * 0.5f / 180.0f * MathF.PI);

        outputVertex.x = rotX_x * fovRad * aspectRatio;
        outputVertex.y = rotX_y * fovRad;

        if (rotX_z != 0) {
            outputVertex.x /= rotX_z;
            outputVertex.y /= rotX_z;
        }

        outputVertex.x += 1.0f;
        outputVertex.y = 1.0f - outputVertex.y;
        outputVertex.x *= 0.5f * screenW;
        outputVertex.y *= 0.5f * screenH;
        outputVertex.z = rotX_z;

        return outputVertex;
    }

    private void DrawScene(Vector2 size) {
        ImDrawListPtr drawList = ImGui.GetBackgroundDrawList();
        uint color = ImGui.ColorConvertFloat4ToU32(new Vector4(lineColor, 1f));

        foreach (triangle tri in towerMesh.tris) {
            vector3d p1 = MultiplyVectorMatrix(tri.p[0], angleX, angleY, size.X, size.Y);
            vector3d p2 = MultiplyVectorMatrix(tri.p[1], angleX, angleY, size.X, size.Y);
            vector3d p3 = MultiplyVectorMatrix(tri.p[2], angleX, angleY, size.X, size.Y);

            drawList.AddTriangle(
                new Vector2(p1.x, p1.y),
                new Vector2(p2.x, p2.y),
                new Vector2(p3.x, p3.y),
                color, lineThickness);
        }
    }
}