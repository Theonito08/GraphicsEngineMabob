using Silk.NET.Maths;

namespace GraphicsEngineMabob;

public static class Program {
    private static void Main(string[] args) {
        new RenderApp().Run();
    }

    public struct vector3d {
        public float x, y, z;

        public vector3d(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public struct triangle {
        public vector3d[] p;

        public triangle(int n) {
            p = new vector3d[3];
        }

        public triangle(ref vector3d v1, ref vector3d v2, ref vector3d v3) {
            p = new[] { v1, v2, v3 };
        }
    }
}