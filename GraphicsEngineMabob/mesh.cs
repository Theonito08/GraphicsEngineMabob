using static GraphicsEngineMabob.Program;

public class mesh {
    public List<triangle> tris;

    public mesh() {
        tris = new List<triangle>();
    }

    public static mesh CreateCube(float size) {
        float scaleFactor = size * 0.5f;
        mesh m = new();
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

        AddQuad(m, ref v[4], ref v[5], ref v[6], ref v[7]); // front
        AddQuad(m, ref v[1], ref v[0], ref v[3], ref v[2]); // back
        AddQuad(m, ref v[0], ref v[4], ref v[7], ref v[3]); // left
        AddQuad(m, ref v[5], ref v[1], ref v[2], ref v[6]); // right
        AddQuad(m, ref v[3], ref v[2], ref v[6], ref v[7]); // top
        AddQuad(m, ref v[4], ref v[0], ref v[1], ref v[5]); // bottom
        //rub mah 
        return m;
    }

    public static mesh CreateBox(vector3d minVector, vector3d maxVector) {
        mesh m = new();
        m.tris = new List<triangle>();
        //min and max bounds
        vector3d p0 = new(minVector.x, minVector.y, minVector.z);
        vector3d p1 = new(maxVector.x, minVector.y, minVector.z);
        vector3d p2 = new(maxVector.x, maxVector.y, minVector.z);
        vector3d p3 = new(minVector.x, maxVector.y, minVector.z);
        vector3d p4 = new(minVector.x, minVector.y, maxVector.z);
        vector3d p5 = new(maxVector.x, minVector.y, maxVector.z);
        vector3d p6 = new(maxVector.x, maxVector.y, maxVector.z);
        vector3d p7 = new(minVector.x, maxVector.y, maxVector.z);

        // front
        AddQuad(m, ref p4, ref p5, ref p6, ref p7);
        // back
        AddQuad(m, ref p1, ref p0, ref p3, ref p2);
        // left
        AddQuad(m, ref p0, ref p4, ref p7, ref p3);
        // right
        AddQuad(m, ref p5, ref p1, ref p2, ref p6);
        // top
        AddQuad(m, ref p3, ref p2, ref p6, ref p7);
        // bottom
        AddQuad(m, ref p4, ref p0, ref p1, ref p5);

        return m;
    }

    public static mesh CreateCylinder(float height, float radius, int segments) {
        mesh m = new();
        float halfHeight = height * 0.5f;
        //centers
        vector3d topCenter = new(0, halfHeight, 0);
        vector3d bottomCenter = new(0, -halfHeight, 0);

        for (int i = 0; i < segments; i++) {
            //radians bcs vectors
            float angle1 = i / (float)segments * (2.0f * MathF.PI);
            float angle2 = (i + 1) / (float)segments * (2.0f * MathF.PI);

            float cos1 = MathF.Cos(angle1);
            float sin1 = MathF.Sin(angle1);
            float cos2 = MathF.Cos(angle2);
            float sin2 = MathF.Sin(angle2);

            //top and bottom vertices
            vector3d topV1 = new(radius * cos1, halfHeight, radius * sin1);
            vector3d topV2 = new(radius * cos2, halfHeight, radius * sin2);
            vector3d botV1 = new(radius * cos1, -halfHeight, radius * sin1);
            vector3d botV2 = new(radius * cos2, -halfHeight, radius * sin2);

            //wall triangles (quad per segment)
            m.tris.Add(new triangle(ref topV1, ref botV1, ref botV2));
            m.tris.Add(new triangle(ref topV1, ref botV2, ref topV2));

            //top cap
            m.tris.Add(new triangle(ref topCenter, ref topV1, ref topV2));

            //bottom cap
            m.tris.Add(new triangle(ref bottomCenter, ref botV2, ref botV1));
        }

        return m;
    }

    public static mesh CreateCone(vector3d basePoint, vector3d capPoint, float baseRadius, float capRadius,
        int segments) {
        mesh m = new();
        for (int i = 0; i < segments; i++) {
            float angle1 = i / (float)segments * (2.0f * MathF.PI);
            float angle2 = (i + 1) / (float)segments * (2.0f * MathF.PI);

            float cos1 = MathF.Cos(angle1);
            float sin1 = MathF.Sin(angle1);
            float cos2 = MathF.Cos(angle2);
            float sin2 = MathF.Sin(angle2);
            vector3d capV1 = new(capPoint.x + capRadius * cos1, capPoint.y, capPoint.z + capRadius * sin1);
            vector3d capV2 = new(capPoint.x + capRadius * cos2, capPoint.y, capPoint.z + capRadius * sin2);
            vector3d baseV1 = new(basePoint.x + baseRadius * cos1, basePoint.y, basePoint.z + baseRadius * sin1);
            vector3d baseV2 = new(basePoint.x + baseRadius * cos2, basePoint.y, basePoint.z + baseRadius * sin2);

            m.tris.Add(new triangle(ref capV1, ref baseV1, ref baseV2));
            m.tris.Add(new triangle(ref capV1, ref baseV2, ref capV2));

            m.tris.Add(new triangle(ref capPoint, ref capV1, ref capV2));

            m.tris.Add(new triangle(ref basePoint, ref baseV2, ref baseV1));
        }

        return m;
    }

    public static mesh CreateSphere(vector3d center, float radius, int segments) {
        mesh m = new();
        vector3d[,] vertices = new vector3d[segments + 1, segments + 1];
        for (int i = 0; i <= segments; i++) {
            float angle1 = i * (float)Math.PI / segments;
            float sin1 = (float)Math.Sin(angle1);
            float cos1 = (float)Math.Cos(angle1);
            for (int j = 0; j <= segments; j++) {
                float angle2 = j / (float)segments * (2.0f * MathF.PI);
                float sin2 = (float)Math.Sin(angle2);
                float cos2 = (float)Math.Cos(angle2);
                vertices[i, j] = new vector3d(center.x + radius * sin1 * cos2, center.y + radius * cos1,
                    center.z + radius * sin1 * sin2);
            }
        }

        for (int i = 0; i < segments; i++)
        for (int j = 0; j < segments; j++) {
            vector3d v00 = vertices[i, j];
            vector3d v01 = vertices[i, j + 1];
            vector3d v10 = vertices[i + 1, j];
            vector3d v11 = vertices[i + 1, j + 1];
            AddQuad(m, v00, v01, v11, v10);
        }

        {
            for (int j = 0; j < segments; j++)
            {
                vector3d v00 = vertices[i, j];
                vector3d v01 = vertices[i, j + 1];
                vector3d v10 = vertices[i + 1, j];
                vector3d v11 = vertices[i + 1, j + 1];
                AddQuad(m, ref v00, ref v01, ref v11, ref v10);
            }
        }
        return m;
    }
    private static void AddQuad(mesh m, ref vector3d a, ref vector3d b, ref vector3d c, ref vector3d d)
    {
        m.tris.Add(new triangle(a, b, c));
        m.tris.Add(new triangle(a, c, d));
    }

    public static mesh Transform(vector3d transformationVector, mesh meshIn) {
        mesh m = new();
        for (int i = 0; i < meshIn.tris.Count; i++) {
            float newXv1 = meshIn.tris[i].p[0].x + transformationVector.x;
            float newYv1 = meshIn.tris[i].p[0].y + transformationVector.y;
            float newZv1 = meshIn.tris[i].p[0].z + transformationVector.z;
            vector3d v1 = new(newXv1, newYv1, newZv1);
            float newXv2 = meshIn.tris[i].p[1].x + transformationVector.x;
            float newYv2 = meshIn.tris[i].p[1].y + transformationVector.y;
            float newZv2 = meshIn.tris[i].p[1].z + transformationVector.z;
            vector3d v2 = new(newXv2, newYv2, newZv2);
            float newXv3 = meshIn.tris[i].p[2].x + transformationVector.x;
            float newYv3 = meshIn.tris[i].p[2].y + transformationVector.y;
            float newZv3 = meshIn.tris[i].p[2].z + transformationVector.z;
            vector3d v3 = new(newXv3, newYv3, newZv3);
            m.tris.Add(new triangle(ref v1, ref v2, ref v3));
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
            vector3d v1 = new(newXv1, newYv1, newZv1);
            float newXv2 = meshIn.tris[i].p[1].x * scale;
            float newYv2 = meshIn.tris[i].p[1].y * scale;
            float newZv2 = meshIn.tris[i].p[1].z * scale;
            vector3d v2 = new(newXv2, newYv2, newZv2);
            float newXv3 = meshIn.tris[i].p[2].x * scale;
            float newYv3 = meshIn.tris[i].p[2].y * scale;
            float newZv3 = meshIn.tris[i].p[2].z * scale;
            vector3d v3 = new(newXv3, newYv3, newZv3);
            m.tris.Add(new triangle(ref v1, ref v2, ref v3));
        }

        return m;
    }

    public static mesh Stretch(float scaleX, float scaleY, float scaleZ, mesh meshIn) {
        mesh m = new();
        for (int i = 0; i < meshIn.tris.Count; i++) {
            float newXv1 = meshIn.tris[i].p[0].x * scaleX;
            float newYv1 = meshIn.tris[i].p[0].y * scaleY;
            float newZv1 = meshIn.tris[i].p[0].z * scaleZ;
            vector3d v1 = new(newXv1, newYv1, newZv1);
            float newXv2 = meshIn.tris[i].p[1].x * scaleX;
            float newYv2 = meshIn.tris[i].p[1].y * scaleY;
            float newZv2 = meshIn.tris[i].p[1].z * scaleZ;
            vector3d v2 = new(newXv2, newYv2, newZv2);
            float newXv3 = meshIn.tris[i].p[2].x * scaleX;
            float newYv3 = meshIn.tris[i].p[2].y * scaleY;
            float newZv3 = meshIn.tris[i].p[2].z * scaleZ;
            vector3d v3 = new(newXv3, newYv3, newZv3);
            m.tris.Add(new triangle(ref v1, ref v2, ref v3));
        }

        return m;
    }

    public static mesh RotateMesh(mesh meshIn, float angleXDeg, float angleYDeg, float angleZDeg) {
        float angleX = angleXDeg * (MathF.PI / 180.0f);
        float angleY = angleYDeg * (MathF.PI / 180.0f);
        float angleZ = angleZDeg * (MathF.PI / 180.0f);
        mesh m = new();
        for (int i = 0; i < meshIn.tris.Count; i++) {
            vector3d v1 = RotateVector(meshIn.tris[i].p[0], angleX, angleY, angleZ);
            vector3d v2 = RotateVector(meshIn.tris[i].p[1], angleX, angleY, angleZ);
            vector3d v3 = RotateVector(meshIn.tris[i].p[2], angleX, angleY, angleZ);

            m.tris.Add(new triangle(ref v1, ref v2, ref v3));
        }

        return m;
    }

    public static vector3d RotateVector(vector3d v, float angleX, float angleY, float angleZ) {
        float cosX = MathF.Cos(angleX);
        float sinX = MathF.Sin(angleX);
        float cosY = MathF.Cos(angleY);
        float sinY = MathF.Sin(angleY);
        float cosZ = MathF.Cos(angleZ);
        float sinZ = MathF.Sin(angleZ);

        //x axis
        float y1 = v.y * cosX - v.z * sinX;
        float z1 = v.y * sinX + v.z * cosX;
        float x1 = v.x;

        //y axis
        float x2 = x1 * cosY + z1 * sinY;
        float z2 = -x1 * sinY + z1 * cosY;
        float y2 = y1;

        //z axis
        float x3 = x2 * cosZ - y2 * sinZ;
        float y3 = x2 * sinZ + y2 * cosZ;
        float z3 = z2;

        return new vector3d(x3, y3, z3);
    }

    public static mesh Combine(params mesh[] parts)
    {
        mesh m = new();
        foreach (mesh part in parts) m.tris.AddRange(part.tris);
        return m;
    }
    public static void AppendMesh(mesh targetMesh, mesh sourceMesh)
    {
        foreach (var tri in sourceMesh.tris)
        {
            targetMesh.tris.Add(tri);
        }
    }
}
