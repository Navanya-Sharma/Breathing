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

    }

    private void Update()
    {
        Vector3[] vertices=mesh.vertices;

        if (n == 4)
        {
            //vertices[n-1].x += 0.5f * Time.deltaTime;
            //vertices[n-2].x -= 0.5f * Time.deltaTime;
            float ang = AngleBetween(vertices[2], vertices[0], vertices[1]);
            if (ang < MathF.PI / 2)
            {
                float y = radius * MathF.Sin(ang + 0.5f * Time.deltaTime);
                float x = radius * MathF.Cos(ang + 0.5f * Time.deltaTime);
                Vector3 inc = new Vector3(x, y, 0);
                vertices[2]=vertices[0]+inc;

                inc.x = -inc.x;
                vertices[3]=vertices[1]+inc;

            }
            else
            {
                foreach (Vector3 v in vertices) Debug.Log(v);
                AddVertice();
            }
            mesh.SetVertices(vertices);
        }
        else if (n == 3)
        {   
            
            if (Vector3.Distance(vertices[0], vertices[2]) < radius)
            {
                vertices[2].y += 0.5f * Time.deltaTime; 
                mesh.SetVertices(vertices);
            }
            else if (n == 3)
            {
                AddVertice();
            }
           
        }
        else if(n == 5)
        {
            if (Vector3.Distance(vertices[4], vertices[2]) < radius)
            {
                vertices[4].y += 0.7f * Time.deltaTime;  
            }
            float ang = AngleBetween(vertices[2], vertices[0], vertices[1]);
            if (ang < MathF.PI *0.6f)
            {
                float y = radius * MathF.Sin(ang + 0.5f * Time.deltaTime);
                float x = radius * MathF.Cos(ang + 0.5f * Time.deltaTime);
                Vector3 inc = new Vector3(x, y, 0);
                vertices[2] = vertices[0] + inc;

                inc.x = -inc.x;
                vertices[3] = vertices[1] + inc;

            }
            mesh.SetVertices(vertices);
        }
        
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

    private float AngleBetween(Vector3 v3, Vector3 v2, Vector3 v1)
    {
        float ang= MathF.Atan2(v3.y-v2.y,v3.x-v2.x)-MathF.Atan2(v1.y - v2.y, v1.x - v2.x);
        return ang;
    }
}
