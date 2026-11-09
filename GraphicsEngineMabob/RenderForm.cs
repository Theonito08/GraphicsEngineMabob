using static GraphicsEngineMabob.Program;

namespace GraphicsEngineMabob
{
    public partial class RenderForm : Form
    {
        private Bitmap frameBuffer;
        private System.Windows.Forms.Timer renderTimer;
        private mesh cubeMesh1;
        private mesh cubeMesh2;
        private mesh cubeMesh3;
        private mesh customMesh;
        private const int ScreenWidth = 1200;
        private const int ScreenHeight = 1000;
        private float cameraDistance = 8.0f;
        private float angleX = 0.0f;
        private float angleY = 0.0f;
        private bool isAutoSpinning = false;
        public RenderForm()
        {
            this.Width = ScreenWidth;
            this.Height = ScreenHeight;
            this.Text = "GraphicsEngineMabob";
            this.DoubleBuffered = true;
            this.KeyPreview = true;

            this.KeyDown += RenderForm_KeyDown;

            frameBuffer = new Bitmap(ScreenWidth, ScreenHeight);
            cubeMesh1 = mesh.CreateCube(1);
            cubeMesh2 = mesh.CreateCube(1.5f);



            vector3d v1 = new vector3d { x = -2f, y = -2f, z = -2f };
            vector3d v2 = new vector3d { x = -2f, y = 2f, z = -2f };
            vector3d v3 = new vector3d { x = 2f, y = 2f, z = -2f };
            vector3d v4 = new vector3d { x = 2f, y = -2f, z = -2f };
            vector3d v5 = new vector3d { x = 0f, y = 0f, z = 2f };

            triangle t1 = new triangle(v1, v2, v3);
            triangle t2 = new triangle(v1, v2, v4);
            triangle t3 = new triangle(v1, v3, v4);
            triangle t4 = new triangle(v1, v2, v5);
            triangle t5 = new triangle(v1, v3, v5);
            triangle t6 = new triangle(v2, v4, v5);
            triangle[] triangles = { t1, t2, t3, t4, t5, t6 };

            customMesh = new mesh();
            foreach (triangle t in triangles)
            {
                customMesh.tris.Add(t);
            }

            List<mesh> meshes = new List<mesh>
            {
                cubeMesh1,
                cubeMesh2,
                customMesh
            };

            renderTimer = new System.Windows.Forms.Timer();
            renderTimer.Interval = 2;
            renderTimer.Tick += (sender, e) =>
            {
                if (isAutoSpinning)
                {
                    angleX += 0.015f;
                    angleY += 0.025f;
                }
                UpdateAndRender(meshes);
                this.Invalidate();
            };
            renderTimer.Start();
        }

        private void RenderForm_KeyDown(object sender, KeyEventArgs e)
        {
            // Zoom Controls (+ / -)
            if (e.KeyCode == Keys.Oemplus || e.KeyCode == Keys.Add)
            {
                cameraDistance -= 0.5f;
                if (cameraDistance < 3.0f) cameraDistance = 3.0f;
            }
            else if (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract)
            {
                cameraDistance += 0.5f;
            }
            // Toggle Auto-Spin with 'q'
            else if (e.KeyCode == Keys.Q)
            {
                isAutoSpinning = !isAutoSpinning;
            }
            // Rotation Controls (WASD & Arrow Keys)
            else if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up)
            {
                angleX -= 0.1f; // Pitch Up
            }
            else if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down)
            {
                angleX += 0.1f; // Pitch Down
            }
            else if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left)
            {
                angleY -= 0.1f; // Yaw Left
            }
            else if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right)
            {
                angleY += 0.1f; // Yaw Right
            }
        }

        private vector3d MultiplyVectorMatrix(vector3d inputVertex, float angleX, float angleY)
        {
            vector3d outputVertex = new vector3d();

            // Rotate around Y axis
            float rotY_x = inputVertex.x * (float)Math.Cos(angleY) + inputVertex.z * (float)Math.Sin(angleY);
            float rotY_y = inputVertex.y;
            float rotY_z = inputVertex.x * -(float)Math.Sin(angleY) + inputVertex.z * (float)Math.Cos(angleY);

            // Rotate around X axis
            float rotX_x = rotY_x;
            float rotX_y = rotY_y * (float)Math.Cos(angleX) - rotY_z * (float)Math.Sin(angleX);
            float rotX_z = rotY_y * (float)Math.Sin(angleX) + rotY_z * (float)Math.Cos(angleX);

            // Apply camera distance offset
            rotX_z += cameraDistance;

            float fieldOfView = 90.0f;
            float aspectRatio = (float)ScreenHeight / (float)ScreenWidth;
            float fovRad = 1.0f / (float)Math.Tan(fieldOfView * 0.5f / 180.0f * Math.PI);

            outputVertex.x = rotX_x * fovRad * aspectRatio;
            outputVertex.y = rotX_y * fovRad;

            if (rotX_z != 0)
            {
                outputVertex.x /= rotX_z;
                outputVertex.y /= rotX_z;
            }

            outputVertex.x += 1.0f;
            outputVertex.y += 1.0f;
            outputVertex.x *= 0.5f * ScreenWidth;
            outputVertex.y *= 0.5f * ScreenHeight;
            outputVertex.z = rotX_z;

            return outputVertex;
        }

        private void UpdateAndRender(List<mesh> meshes)
        {
            Random rng = new Random();
            using (Graphics g = Graphics.FromImage(frameBuffer))
            {
                g.Clear(Color.Blue);

                Pen pen = new Pen(Color.DarkRed, 1.5f);

                foreach (mesh m in meshes)
                {
                    foreach (triangle tri in m.tris)
                    {
                        vector3d p1 = MultiplyVectorMatrix(tri.p[0], angleX, angleY);
                        vector3d p2 = MultiplyVectorMatrix(tri.p[1], angleX, angleY);
                        vector3d p3 = MultiplyVectorMatrix(tri.p[2], angleX, angleY);

                        g.DrawLine(pen, p1.x, p1.y, p2.x, p2.y);
                        g.DrawLine(pen, p2.x, p2.y, p3.x, p3.y);
                        g.DrawLine(pen, p3.x, p3.y, p1.x, p1.y);

                    }


                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawImage(frameBuffer, 0, 0);
        }
    }
}