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
}