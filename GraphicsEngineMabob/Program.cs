using System.Windows.Forms;
namespace GraphicsEngineMabob
{
    public static class Program
    {
        public struct vector3d
        {
            public float x, y, z;
            public vector3d(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }
        public struct triangle
        {
            public vector3d[] p;

            public triangle(int n)
            {
                p = new vector3d[3];
            }

            public triangle(vector3d v1, vector3d v2, vector3d v3)
            {
                p = new vector3d[] { v1, v2, v3 };
            }
        }
        static void Main(string[] args)
        {
            Application.Run(new RenderForm());
        }


    }
}
