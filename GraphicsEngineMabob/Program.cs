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

            public triangle()
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
            vector3d v1 = new vector3d { x = 0.0f, y = 1.0f, z = 0.0f };
            vector3d v2 = new vector3d { x = -1.0f, y = -1.0f, z = 0.0f };
            vector3d v3 = new vector3d { x = 1.0f, y = -1.0f, z = 0.0f };

            triangle tri = new triangle(v1, v2, v3);
            mesh myMesh = new mesh();
            myMesh.tris.Add(tri);
            Application.Run(new RenderForm());
        }


    }
}
