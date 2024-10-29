using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class cutenavanya : MonoBehaviour
{
    private Mesh mesh;
    
    public int radius;
    public int TotalPoints;
    int n;

    // Start is called before the first frame update
    void Start()
    {   
        radius =2;
        TotalPoints = 4;
        n = 3;
        List<Vector3> vertices = new List<Vector3>
        {
            new Vector3( -radius/2,  0,  0), //0
            new Vector3( radius/2,  0,  0), //1
            new Vector3( 0,  0,  0), //2
        };

        List<int> triangles = new List<int>
        {
            0,2,1
        };

        mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.MarkDynamic();
        //
        //mesh.SetNormals(normals);

        GetComponent<MeshFilter>().sharedMesh = mesh;
        //StartCoroutine(checkRoutine());
    }

    private IEnumerator checkRoutine()
    {
        while (true) {
            Debug.Log("step 1");
            yield return new WaitForSeconds(1f);
            Debug.Log("Step 2");
        }

    }

    private void Update()
    {
        bool[] f ;bool AddNewVertice = false; int k = 0;
        f= new bool[(n - 1) / 2];

        

        if (n != 3)
        {
            f[k++] = UpdateAngle(2, 0, 1);
        }

        for (int i = 0 ; i + 5 < n ; i += 2)
        {
            f[k++] = UpdateAngle(i+4, i+2, i);
        }

        if (n % 2 == 1)
        {
            f[k++] = UpdateDistance();
        }

        foreach (bool b in f) 
        {
            if (b)
            {
                AddNewVertice = true;
            }
            else
            {
               
                AddNewVertice = false;
                break;
            }
        }

        if (AddNewVertice) AddVertice();
                

        
    }
    private bool UpdateAngle(int t3, int t2, int t1)
    {
        bool done = false;
        Vector3[] vertices = mesh.vertices; 
        float ang = AngleBetween(vertices[t3], vertices[t2], vertices[t1]);

        if(ang < MathF.PI*(n-2)/n)
        { 
            Vector3 inc = new Vector3(radius * MathF.Cos(ang + 0.5f * Time.deltaTime),
                radius * MathF.Sin(ang + 0.5f * Time.deltaTime), 0);

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
            vertices[n-1].y += Time.deltaTime;
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

        Debug.Log("n is "+n);
        foreach (int i in triangle) Debug.Log(i);
        foreach (Vector3 v in vertices) Debug.Log(v);
    }

    private float AngleBetween(Vector3 v3, Vector3 v2, Vector3 v1)
    {
        float ang= MathF.Atan2(v3.y-v2.y,v3.x-v2.x)-MathF.Atan2(v1.y - v2.y, v1.x - v2.x);
        return ang;
    }
}
