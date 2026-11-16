using static GraphicsEngineMabob.Program;
public class mesh
{
    public List<triangle> tris;

    public mesh()
    {
        tris = new List<triangle>();
    }
    public static mesh CreateCube(float scale)
    {
        mesh m = new mesh();

        // south
        m.tris.Add(new triangle(new vector3d(-scale, -scale, -scale), new vector3d(-scale, scale, -scale), new vector3d(scale, scale, -scale)));
        m.tris.Add(new triangle(new vector3d(-scale, -scale, -scale), new vector3d(scale, scale, -scale), new vector3d(scale, -scale, -scale)));
        // east
        m.tris.Add(new triangle(new vector3d(scale, -scale, -scale), new vector3d(scale, scale, -scale), new vector3d(scale, scale, scale)));
        m.tris.Add(new triangle(new vector3d(scale, -scale, -scale), new vector3d(scale, scale, scale), new vector3d(scale, -scale, scale)));
        // north
        m.tris.Add(new triangle(new vector3d(scale, -scale, scale), new vector3d(scale, scale, scale), new vector3d(-scale, scale, scale)));
        m.tris.Add(new triangle(new vector3d(scale, -scale, scale), new vector3d(-scale, scale, scale), new vector3d(-scale, -scale, scale)));
        // west
        m.tris.Add(new triangle(new vector3d(-scale, -scale, scale), new vector3d(-scale, scale, scale), new vector3d(-scale, scale, -scale)));
        m.tris.Add(new triangle(new vector3d(-scale, -scale, scale), new vector3d(-scale, scale, -scale), new vector3d(-scale, -scale, -scale)));
        // top
        m.tris.Add(new triangle(new vector3d(-scale, scale, -scale), new vector3d(-scale, scale, scale), new vector3d(scale, scale, scale)));
        m.tris.Add(new triangle(new vector3d(-scale, scale, -scale), new vector3d(scale, scale, scale), new vector3d(scale, scale, -scale)));
        // bottom
        m.tris.Add(new triangle(new vector3d(-scale, -scale, scale), new vector3d(-scale, -scale, -scale), new vector3d(scale, -scale, -scale)));
        m.tris.Add(new triangle(new vector3d(-scale, -scale, scale), new vector3d(scale, -scale, -scale), new vector3d(scale, -scale, scale)));

        return m;
    }

    public static mesh CreateBox(vector3d minVector, vector3d maxVector)
    {
        mesh m = new mesh();
        vector3d v1 = new vector3d(minVector.x, maxVector.y, minVector.z);
        vector3d v2 = new vector3d(minVector.x, minVector.y, maxVector.z);
        vector3d v3 = new vector3d(minVector.x, maxVector.y, maxVector.z);

        triangle t1 = new triangle(minVector, v1, v2);
        triangle t2 = new triangle(v1, v2, v3);

        vector3d v4 = new vector3d(maxVector.x, minVector.y, minVector.z);
        vector3d v5 = new vector3d(maxVector.x, minVector.y, maxVector.z);
        vector3d v6 = new vector3d(maxVector.x, maxVector.y, minVector.z);

        triangle t3 = new triangle(minVector, v1, v4);
        triangle t4 = new triangle(v1, v4, v6);
        triangle t5 = new triangle(v1, v6, maxVector);
        triangle t6 = new triangle(v6, v5, maxVector);

        triangle t7 = new triangle(minVector, v2, v4);
        triangle t8 = new triangle(v2, v4, v5);
        triangle t9 = new triangle(v1, v3, v6);
        triangle t10 = new triangle(v3, v6, maxVector);
        triangle t11 = new triangle(v2, v3, maxVector);
        // m.tris.Add(t1);
        m.tris.Add(t2);
        m.tris.Add(t3);
        m.tris.Add(t4);
        m.tris.Add(t5);
        m.tris.Add(t6);
        m.tris.Add(t7);
        m.tris.Add(t8);
        m.tris.Add(t9);
        m.tris.Add(t10);
        m.tris.Add(t11);
        return m;
    }

    public static mesh Transform(vector3d transformationVector, mesh meshIn)
    {
        mesh m = new mesh();
        for (int i = 0; i < meshIn.tris.Count; i++)
        {
            float newXv1 = meshIn.tris[i].p[0].x + transformationVector.x;
            float newYv1 = meshIn.tris[i].p[0].y + transformationVector.y;
            float newZv1 = meshIn.tris[i].p[0].z + transformationVector.z;
            vector3d v1 = new vector3d(newXv1, newYv1, newZv1);
            float newXv2 = meshIn.tris[i].p[1].x + transformationVector.x;
            float newYv2 = meshIn.tris[i].p[1].y + transformationVector.y;
            float newZv2 = meshIn.tris[i].p[1].z + transformationVector.z;
            vector3d v2 = new vector3d(newXv2, newYv2, newZv2);
            float newXv3 = meshIn.tris[i].p[2].x + transformationVector.x;
            float newYv3 = meshIn.tris[i].p[2].y + transformationVector.y;
            float newZv3 = meshIn.tris[i].p[2].z + transformationVector.z;
            vector3d v3 = new vector3d(newXv3, newYv3, newZv3);
            m.tris.Add(new triangle(v1, v2, v3));
        }
        return m;
    }
}