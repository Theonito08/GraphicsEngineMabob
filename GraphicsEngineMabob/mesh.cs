using static GraphicsEngineMabob.Program;
public class mesh
{
    public List<triangle> tris;

    public mesh()
    {
        tris = new List<triangle>();
    }
    public static mesh CreateCube(float size)
    {
        float scaleFactor = size * 0.5f;
        mesh m = new mesh();
        m.tris = new List<triangle>();
        vector3d[] v = new vector3d[8];
        v[0] = new vector3d(-scaleFactor, -scaleFactor, -scaleFactor); //bottom left rear
        v[1] = new vector3d(scaleFactor, -scaleFactor, -scaleFactor); //bottom right rear
        v[2] = new vector3d(scaleFactor, scaleFactor, -scaleFactor); //top right rear
        v[3] = new vector3d(-scaleFactor, scaleFactor, -scaleFactor); //top right rear
        v[4] = new vector3d(-scaleFactor, -scaleFactor, scaleFactor); //bottom left front
        v[5] = new vector3d(scaleFactor, -scaleFactor, scaleFactor); //bottom right front
        v[6] = new vector3d(scaleFactor, scaleFactor, scaleFactor); //top right ront
        v[7] = new vector3d(-scaleFactor, scaleFactor, scaleFactor); //top left front

        AddQuad(m, v[4], v[5], v[6], v[7]); // front
        AddQuad(m, v[1], v[0], v[3], v[2]); // back
        AddQuad(m, v[0], v[4], v[7], v[3]); // left
        AddQuad(m, v[5], v[1], v[2], v[6]); // right
        AddQuad(m, v[3], v[2], v[6], v[7]); // top
        AddQuad(m, v[4], v[0], v[1], v[5]); // bottom
        //rub mah 
        return m;
    }

    public static mesh CreateBox(vector3d minVector, vector3d maxVector)
    {
        mesh m = new mesh();
        m.tris = new List<triangle>();
        //min and max bounds
        vector3d p0 = new vector3d(minVector.x, minVector.y, minVector.z);
        vector3d p1 = new vector3d(maxVector.x, minVector.y, minVector.z);
        vector3d p2 = new vector3d(maxVector.x, maxVector.y, minVector.z);
        vector3d p3 = new vector3d(minVector.x, maxVector.y, minVector.z);
        vector3d p4 = new vector3d(minVector.x, minVector.y, maxVector.z);
        vector3d p5 = new vector3d(maxVector.x, minVector.y, maxVector.z);
        vector3d p6 = new vector3d(maxVector.x, maxVector.y, maxVector.z);
        vector3d p7 = new vector3d(minVector.x, maxVector.y, maxVector.z);

        // front
        AddQuad(m, p4, p5, p6, p7);
        // back
        AddQuad(m, p1, p0, p3, p2);
        // left
        AddQuad(m, p0, p4, p7, p3);
        // right
        AddQuad(m, p5, p1, p2, p6);
        // top
        AddQuad(m, p3, p2, p6, p7);
        // bottom
        AddQuad(m, p4, p0, p1, p5);

        return m;
    }

    public static mesh CreateCylinder(float height, float radius, int segments)
    {
        mesh m = new mesh();
        float halfHeight = height * 0.5f;
        //centers
        vector3d topCenter = new vector3d(0, halfHeight, 0);
        vector3d bottomCenter = new vector3d(0, -halfHeight, 0);

        for (int i = 0; i < segments; i++)
        {
            //radians bcs vectors
            float angle1 = (i / (float)segments) * (2.0f * MathF.PI);
            float angle2 = ((i + 1) / (float)segments) * (2.0f * MathF.PI);

            float cos1 = MathF.Cos(angle1);
            float sin1 = MathF.Sin(angle1);
            float cos2 = MathF.Cos(angle2);
            float sin2 = MathF.Sin(angle2);

            //top and bottom vertices
            vector3d topV1 = new vector3d(radius * cos1, halfHeight, radius * sin1);
            vector3d topV2 = new vector3d(radius * cos2, halfHeight, radius * sin2);
            vector3d botV1 = new vector3d(radius * cos1, -halfHeight, radius * sin1);
            vector3d botV2 = new vector3d(radius * cos2, -halfHeight, radius * sin2);

            //wall triangles (quad per segment)
            m.tris.Add(new triangle(topV1, botV1, botV2));
            m.tris.Add(new triangle(topV1, botV2, topV2));

            //top cap
            m.tris.Add(new triangle(topCenter, topV1, topV2));

            //bottom cap
            m.tris.Add(new triangle(bottomCenter, botV2, botV1));
        }

        return m;
    }

    private static void AddQuad(mesh m, vector3d a, vector3d b, vector3d c, vector3d d)
    {
        m.tris.Add(new triangle(a, b, c));
        m.tris.Add(new triangle(a, c, d));
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

    public static mesh Enlarge(float scale, mesh meshIn)
    {
        mesh m = new mesh();
        for (int i = 0; i < meshIn.tris.Count; i++)
        {
            float newXv1 = meshIn.tris[i].p[0].x * scale;
            float newYv1 = meshIn.tris[i].p[0].y * scale;
            float newZv1 = meshIn.tris[i].p[0].z * scale;
            vector3d v1 = new vector3d(newXv1, newYv1, newZv1);
            float newXv2 = meshIn.tris[i].p[1].x * scale;
            float newYv2 = meshIn.tris[i].p[1].y * scale;
            float newZv2 = meshIn.tris[i].p[1].z * scale;
            vector3d v2 = new vector3d(newXv2, newYv2, newZv2);
            float newXv3 = meshIn.tris[i].p[2].x * scale;
            float newYv3 = meshIn.tris[i].p[2].y * scale;
            float newZv3 = meshIn.tris[i].p[2].z * scale;
            vector3d v3 = new vector3d(newXv3, newYv3, newZv3);
            m.tris.Add(new triangle(v1, v2, v3));
        }
        return m;
    }

    public static mesh Stretch(float scaleX, float scaleY, float scaleZ, mesh meshIn)
    {
        mesh m = new mesh();
        for (int i = 0; i < meshIn.tris.Count; i++)
        {
            float newXv1 = meshIn.tris[i].p[0].x * scaleX;
            float newYv1 = meshIn.tris[i].p[0].y * scaleY;
            float newZv1 = meshIn.tris[i].p[0].z * scaleZ;
            vector3d v1 = new vector3d(newXv1, newYv1, newZv1);
            float newXv2 = meshIn.tris[i].p[1].x * scaleX;
            float newYv2 = meshIn.tris[i].p[1].y * scaleY;
            float newZv2 = meshIn.tris[i].p[1].z * scaleZ;
            vector3d v2 = new vector3d(newXv2, newYv2, newZv2);
            float newXv3 = meshIn.tris[i].p[2].x * scaleX;
            float newYv3 = meshIn.tris[i].p[2].y * scaleY;
            float newZv3 = meshIn.tris[i].p[2].z * scaleZ;
            vector3d v3 = new vector3d(newXv3, newYv3, newZv3);
            m.tris.Add(new triangle(v1, v2, v3));
        }
        return m;
    }
}
