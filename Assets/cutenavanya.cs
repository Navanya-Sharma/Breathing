using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class cutenavanya : MonoBehaviour
{
    private Mesh mesh;
    
    float radius;
    int TotalPoints=8, n;
    public float speed =2f, angSpeed=0.9f, t=1f;

    // Start is called before the first frame update
    void Start()
    {   
        radius =2;
        n = 3;

        List<Vector3> vertices = new List<Vector3> {
            new Vector3( -radius/2,  0,  0), //0
            new Vector3( radius/2,  0,  0), //1
            new Vector3( 0,  0,  0), //2
        };

        List<int> triangles = new List<int> {0,2,1};

        mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.MarkDynamic();

        GetComponent<MeshFilter>().sharedMesh = mesh;
        
        StartCoroutine(Expand());
    }

    private IEnumerator Expand()
    {
        while (true) {
            bool[] f; bool AddNewVertice = false; int k = 0;
            f = new bool[(n - 1) / 2];

            if (n != 3) f[k++] = UpdateAngle(2, 0, 1);

            for (int i = 0; i + 5 < n; i += 2)
                f[k++] = UpdateAngle(i + 4, i + 2, i);

            if (n % 2 == 1) f[k++] = UpdateDistance();

            foreach (bool b in f){
                if (b) AddNewVertice = true;
                else { AddNewVertice = false; break; }
            }

            if (AddNewVertice) { 
                if (n == TotalPoints) break;
                AddVertice(); }
            
            yield return new WaitForSeconds(0.00005f);
        }

        Invoke("st",t);
        
    }

    void st()
    {
        StartCoroutine(Shrink());
    }

    private IEnumerator Shrink()
    {
        while (true) {
            bool[] f; bool RemoveVertice = false; int k = 0;
            f = new bool[(n - 1) / 2];
            
            if (n != 3) f[k++] = ReduceAngle(2, 0, 1);

            for (int i = 0; i + 5 < n; i += 2)
                f[k++] = ReduceAngle(i + 4, i + 2, i);

            if (n % 2 == 1) f[k++] = ReduceDistance();

            foreach (bool b in f)
            {
                if (b) RemoveVertice = true;
                else { RemoveVertice = false; break; }
            }

            if (RemoveVertice)
            {   
                
                if (n == 3) break;
                ReduceVertice();
            }

            yield return new WaitForSeconds(0.00005f);
        }
        Invoke("stt", t);
        
    }
    void stt()
    {
        StartCoroutine(Expand());
    }
    private bool UpdateAngle(int t3, int t2, int t1)
    {
        bool done = false;
        Vector3[] vertices = mesh.vertices; 
        

        if(AngleBetween(vertices[t3], vertices[t2], vertices[t1]) < MathF.PI*(n-2)/n)
        { 
            Vector3 Base = vertices[t2];
            Base.x += radius;
            float ang = AngleBetween(vertices[t3], vertices[t2], Base);
            Vector3 inc = new Vector3(radius * MathF.Cos(ang + angSpeed * Time.deltaTime),
                radius * MathF.Sin(ang + angSpeed * Time.deltaTime), 0);

            vertices[t3] = vertices[t2] + inc; inc.x = -inc.x;
            vertices[t3 + 1] = vertices[t2 + 1] + inc;
            mesh.SetVertices(vertices);
        }
        else
        {
            done = true;
        }
        return done;
    }

    private bool UpdateDistance()
    {
        bool done = false;
        Vector3[] vertices = mesh.vertices;
        if (Vector3.Distance(vertices[n-3], vertices[n-1]) < radius)
        {
            vertices[n-1].y += speed*Time.deltaTime;
            mesh.SetVertices(vertices);
        }
        else
        {
            done = true;
        }
        return done;
    }

    private void AddVertice()
    {
        n += 1;
        
        
        Vector3[] VerticesArray = mesh.vertices;
        int[] TriangleArray = mesh.triangles;

        List<Vector3> vertices = VerticesArray.ToList();
        List<int> triangle=TriangleArray.ToList();
        
        if (n % 2 == 0) // Even - Add a new vertice at the same place
        {
            vertices.Add(vertices[n - 2]); // as n has been increased at the start
            
            triangle.Add(n - 2);
            triangle.Add(n - 1);
            triangle.Add(n - 3);
        }
        else // Odd - Add vertice in the middle of the 2 points
        {
            vertices.Add((vertices[n - 2]+ vertices[n - 3])/2);
            triangle.Add(n - 3);
            triangle.Add(n - 1);
            triangle.Add(n - 2);
        }
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangle, 0);

    }

    private bool ReduceAngle(int t3, int t2, int t1)
    {
        bool done = false;
        Vector3[] vertices = mesh.vertices;


        if (AngleBetween(vertices[t3], vertices[t2], vertices[t1]) > MathF.PI * (n - 3) / (n-1))
        {
            Vector3 Base = vertices[t2];
            Base.x += radius;
            float ang = AngleBetween(vertices[t3], vertices[t2], Base);
            Vector3 inc = new Vector3(radius * MathF.Cos(ang - angSpeed * Time.deltaTime),
                radius * MathF.Sin(ang - angSpeed * Time.deltaTime), 0);

            vertices[t3] = vertices[t2] + inc; inc.x = -inc.x;
            vertices[t3 + 1] = vertices[t2 + 1] + inc;
            mesh.SetVertices(vertices);
        }
        else
        {
            done = true;
        }
        return done;
    }

    private bool ReduceDistance()
    {
        bool done = false;
        Vector3[] vertices = mesh.vertices;
        if (vertices[n - 3].y < vertices[n - 1].y)
        {
            vertices[n - 1].y -= speed * Time.deltaTime;
            mesh.SetVertices(vertices);
        }
        else
        {
            done = true;
        }
        return done;
    }

    private void ReduceVertice()
    {
        n -= 1;
        Vector3[] VerticesArray = mesh.vertices;
        int[] TriangleArray = mesh.triangles;

        List<Vector3> vertices = VerticesArray.ToList();
        List<int> triangle = TriangleArray.ToList();

        vertices.RemoveAt(n);
        
        int l=triangle.Count;
        triangle.RemoveAt(l-1);
        triangle.RemoveAt(l-2);
        triangle.RemoveAt(l-3);

        mesh.SetTriangles(triangle, 0);
        mesh.SetVertices(vertices);
        

    }

    private float AngleBetween(Vector3 v3, Vector3 v2, Vector3 v1)
    {
        float ang= MathF.Atan2(v3.y-v2.y,v3.x-v2.x)-MathF.Atan2(v1.y - v2.y, v1.x - v2.x);
        return ang;
    }
}
