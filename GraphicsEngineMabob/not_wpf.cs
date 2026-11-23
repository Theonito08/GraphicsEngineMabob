using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using System.Numerics;
using static GraphicsEngineMabob.Program;

namespace GraphicsEngineMabob;

public sealed class RenderApp
{
    private bool isRainbowActive = false;

    private readonly List<mesh> meshes = new();

    private const int ScreenWidth = 1200;
    private const int ScreenHeight = 1000;
    private const float MouseSensitivity = 0.005f; // radians per pixel
    private float angleX; // mouse movement angles
    private float angleY;
    private float cameraDistance = 8.0f;
    private GL gl = null!;
    private ImGuiController imgui = null!;
    private IInputContext input = null!;
    private bool isAutoSpinning;
    
    private float hue;
    private Vector3 lineColor = new(0f, 1f, 0f);
    private float lineThickness = 0.25f;

    private IWindow window = null!;

    public void Run()
    {
        WindowOptions options = WindowOptions.Default;
        options.Size = new Vector2D<int>(ScreenWidth, ScreenHeight);
        options.Title = "GraphicsEngineMabob";

        /* MESH CREATION SPACE
        Example below:
        mesh myMesh = mesh.CreateCube(0.5f);
        meshes.Add(myMesh);
        */
        mesh myMesh = mesh.CreateCube(0.5f);
        meshes.Add(myMesh);
        
        window = Window.Create(options);
        window.Load += OnLoad;
        window.Render += OnRender;
        window.FramebufferResize += size => gl.Viewport(size);
        window.Closing += OnClosing;
        window.Run();
        window.Dispose();
    }

    private void OnLoad()
    {
        gl = GL.GetApi(window);
        input = window.CreateInput();
        imgui = new ImGuiController(gl, window, input);
    }

    private void OnClosing()
    {
        imgui?.Dispose();
        input?.Dispose();
        gl?.Dispose();
    }

    private void OnRender(double deltaTime)
    {
        float dt = (float)deltaTime;
        imgui.Update(dt);
        ImGuiIOPtr io = ImGui.GetIO();


        HandleKeys();
        if (ImGui.IsMouseDown(ImGuiMouseButton.Middle)) MouseMove(io);
        if (io.MouseWheel != 0f) MouseZoom(io);

        if (isRainbowActive)
        {
            hue = (hue + 60f * dt) % 360f;
            lineColor = HslToRgb(hue, 1f, 0.5f);
        }
        if (isAutoSpinning)
        {
            angleX += 1.0f * dt;
            angleY += 1.5f * dt;
        }

        gl.ClearColor(0f, 0f, 0f, 1f);
        gl.Clear((uint)ClearBufferMask.ColorBufferBit);

        DrawScene(io.DisplaySize, meshes);

        ImGui.Begin("Line");
        ImGui.SliderFloat("Thickness", ref lineThickness, 0.25f, 5.0f);
        ImGui.Checkbox("Rainbow", ref isRainbowActive);
        if (!isRainbowActive)
        {
            ImGui.ColorEdit3("Colour", ref lineColor);
        }
        ImGui.End();
        imgui.Render();
    }

    private static Vector3 HslToRgb(float h, float s, float l)
    {
        float c = (1 - Math.Abs(2 * l - 1)) * s;
        float x = c * (1 - Math.Abs((h / 60) % 2 - 1));
        float m = l - c / 2;

        float r, g, b;
        if (h < 60) { r = c; g = x; b = 0; }
        else if (h < 120) { r = x; g = c; b = 0; }
        else if (h < 180) { r = 0; g = c; b = x; }
        else if (h < 240) { r = 0; g = x; b = c; }
        else if (h < 300) { r = x; g = 0; b = c; }
        else { r = c; g = 0; b = x; }
        return new Vector3(r + m, g + m, b + m);
    }

    private void HandleKeys()
    {
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

    private void MouseMove(ImGuiIOPtr io)
    {
        if (io.WantCaptureMouse) return;
        
        angleY += io.MouseDelta.X * MouseSensitivity;
        angleX += io.MouseDelta.Y * MouseSensitivity;
    }
    
    private void MouseZoom(ImGuiIOPtr io)
    {
        if (io.WantCaptureMouse) return;
        
        cameraDistance = MathF.Max(3.0f, cameraDistance - io.MouseWheel * 0.5f);
    }

    private vector3d MultiplyVectorMatrix(vector3d inputVertex, float angleX, float angleY, float screenW,
        float screenH)
    {
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

        if (rotX_z != 0)
        {
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

    private void DrawScene(Vector2 size, List<mesh> parts)
    {
        ImDrawListPtr drawList = ImGui.GetBackgroundDrawList();
        uint color = ImGui.ColorConvertFloat4ToU32(new Vector4(lineColor, 1f));

        foreach (mesh part in parts)
        {
            foreach (var tri in part.tris)
            {
                vector3d p1 = MultiplyVectorMatrix(tri.p[0], angleX, angleY, size.X, size.Y);
                vector3d p2 = MultiplyVectorMatrix(tri.p[1], angleX, angleY, size.X, size.Y);
                vector3d p3 = MultiplyVectorMatrix(tri.p[2], angleX, angleY, size.X, size.Y);

                Vector2 v1 = new Vector2((float)p1.x, (float)p1.y);
                Vector2 v2 = new Vector2((float)p2.x, (float)p2.y);
                Vector2 v3 = new Vector2((float)p3.x, (float)p3.y);

                drawList.AddLine(v1, v2, color, lineThickness);
                drawList.AddLine(v2, v3, color, lineThickness);
                drawList.AddLine(v3, v1, color, lineThickness);
            }
        }
    }
}
