using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace GraphEditor
{
    
    public enum EdgeOrientation { None, Direct, Inverted }

    [Serializable]
     public class Graph : NotifyPropertyChanged
    {
        private List<Vertex> vertices;
        private List<Edge> edges;
        private string name;
        private int vertexCount;
        private int edgeCount;

        public Graph()
        {
            vertices = new List<Vertex>();
            edges = new List<Edge>();
            name = "";
        }
        public Graph(string name, IEnumerable<Vertex> vertices, IEnumerable<Edge> edges)
        {
            this.vertices = new List<Vertex>(vertices);
            this.edges = new List<Edge>(edges);
            this.name = name;
        }

        public List<Vertex> Vertices
        {
            get { return vertices; }
            set
            {
                vertices = value;
                OnPropertyChanged();
            }
        }
        public List<Edge> Edges
        {
            get { return edges; }
            set
            {
                edges = value;
                OnPropertyChanged();
            }
        }
        public string Name
        {
           get { return name; }
           set
           {
                name = value;
                OnPropertyChanged();
           }
        }

        //Vertices
        public Vertex AddVertex(double x, double y, string name = "")
        {
            if (!ValidateVertexName(name))
                return null;
            vertexCount++;
            if (name == "")
                name = $"V{vertexCount}";
            Vertex v = new Vertex(x, y, name);
            vertices.Add(v);
            OnPropertyChanged();
            return v;
        }
        public Vertex AddVertex(Point coordinates, string name = "")
        {
            if (!ValidateVertexName(name))
                return null;
            vertexCount++;
            if (name == "")
                name = $"V{vertexCount}";
            Vertex v = new Vertex(coordinates, name);
            vertices.Add(v);
            OnPropertyChanged("Vertices");
            return v;
        }
        public Vertex FindVertex(string name)
        {
            foreach (Vertex v in vertices)
                if (v.Name == name)
                    return v;
            return null;
        }
        private bool ValidateVertexName(string name)
        {
            foreach (Vertex v in vertices)
                if (v.Name == name)
                    return false;
            return true;
        }
        public void RemoveVertex(Vertex vertex)
        {
            for (int i = 0; i < edges.Count; i++)
                if (edges[i].Belongs(vertex))
                {
                    RemoveEdge(edges[i]);
                    i--;
                }
            vertices.Remove(vertex);
        }
        public void ValidateVertexNames()
        {
            foreach(Vertex v in vertices)
            {
                List<string> names = new List<string>();
                for(int i = 0; i < names.Count; i++)
                {
                    if(!names[i].Equals(v.Name))
                        names.Add(v.Name);
                    else
                    {
                        Console.WriteLine("Name error");
                        MessageBox.Show("Vertex names must be unique!", "Oops!", MessageBoxButton.OK, MessageBoxImage.Error);
                        v.Name = $"V{vertexCount}";
                        names.Add(v.Name);
                    }
                }
            }
        }

        //Edges
        public void AddEdge(Vertex firstVertex, Vertex secondVertex, EdgeOrientation orient = EdgeOrientation.None, int weight = 0)
        {
            edgeCount++;
            edges.Add(new Edge(firstVertex, secondVertex, orient, weight));
        }
        public Edge FindEdge(string vName1, string vName2)
        {
            foreach (Edge e in edges)
                if (e.FirstVertex.Name == vName1 && e.SecondVertex.Name == vName2)
                    return e;
            return null;

        }
        public Edge FindEdge(Point p1, Point p2)
        {
            foreach (Edge e in edges)
                if (e.FirstVertex.Position == p1 && e.SecondVertex.Position == p2)
                    return e;
            return null;
        }
        public Edge FindEdge(Vertex v1, Vertex v2, bool oriented)
        {
            if (!oriented)
                foreach (Edge e in edges)
                {
                    if ((e.FirstVertex == v1 && e.SecondVertex == v2) || (e.FirstVertex == v2 && e.SecondVertex == v1))
                        return e;
                }
            else
                foreach (Edge e in edges)
                    if (e.FirstVertex == v1 && e.SecondVertex == v2)
                        return e;
            return null;
        }
        public void RemoveEdge(Edge edge)
        {
            edge.FirstVertex.Adjacent.Remove(edge.SecondVertex);
            edge.SecondVertex.Adjacent.Remove(edge.FirstVertex);
            edges.Remove(edge);
        }

        public bool IsOriented()
        {
            foreach (Edge e in edges)
                if (e.Orientation == EdgeOrientation.None)
                    return false;
            return true;
        }

        public string Dijkstra(Vertex source)
        {
            foreach (Edge e in edges)
                if (e.Weight < 0)
                    return "Weights must be greater then 0";

            bool oriented = IsOriented();

            int[] dist = new int[vertices.Count];
            bool[] sptSet = new bool[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
            {
                dist[i] = int.MaxValue;
                sptSet[i] = false;
            }

            dist[vertices.IndexOf(source)] = 0;
            for(int i = 0; i < vertices.Count - 1; i++)
            {
                int u = MinDistance(dist, sptSet);
                sptSet[u] = true;
                for (int j = 0; j < vertices.Count; j++)
                {
                    if (vertices[j].AdjecentWith(vertices[u]))
                    {
                        Edge edge = FindEdge(vertices[u], vertices[j], oriented);
                        if (edge != null)
                        {
                            int weight = dist[u] + edge.Weight;

                            if (!sptSet[j] && dist[u] != int.MaxValue && weight < dist[j])
                                dist[j] = weight;
                        }
                    }
                }
            }
            return FormatDijkstraOutput(dist, source);
        }

        private int MinDistance(int[] dist, bool[] sptSet)
        {
            int min = int.MaxValue;
            int minIndex = -1;
            for(int i = 0; i < vertices.Count; i++)
                if (!sptSet[i] && dist[i] <= min)
                {
                    min = dist[i];
                    minIndex = i;
                }
            return minIndex;
        }

        private string FormatDijkstraOutput(int[] dist,Vertex source)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Source:    " + source.Name + '\n');
            for (int i = 0; i < dist.Length; i++)
            {
                sb.Append(vertices[i].Name + ":    ");
                sb.Append(dist[i]);
                sb.Append('\n');
            }
            return sb.ToString();
        }

        public Vertex this[string key]
        {
            get { return FindVertex(key); }
            set
            {
                Vertex v = FindVertex(key);
                v = value;
                OnPropertyChanged("Vertices");
            }
        }

        public Edge this[string key1, string key2]
        {
            get { return FindEdge(key1, key2); }
        }

    }

    [Serializable]
    public class Vertex : NotifyPropertyChanged
    {
        private Point location;
        private string name;

        public Vertex(double x, double y, string name)
        {
            this.name = name;
            location = new Point(x, y);
            Adjacent = new List<Vertex>();
        }
        public Vertex(Point coordinates, string name)
        {
            this.name = name;
            location = coordinates;
            Adjacent = new List<Vertex>();

        }

        public List<Vertex> Adjacent { get; set; }
        public Point Position
        {
            get { return location; }
            set
            {
                location = value;
                OnPropertyChanged();
            }
        }


        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        public bool AdjecentWith(Vertex vertex)
        {
            foreach (Vertex v in Adjacent)
                if (v == vertex)
                    return true;
            return false;
        }


    }

    [Serializable]
    public class Edge : NotifyPropertyChanged
    {
        private Vertex firstVertex;
        private Vertex secondVertex;
        private EdgeOrientation orientation;
        private int weight;

        public Edge(Vertex vertex1, Vertex vertex2, EdgeOrientation orient = EdgeOrientation.None, int weight = 0)
        {
            firstVertex = vertex1;
            secondVertex = vertex2;
            firstVertex.Adjacent.Add(secondVertex);
            secondVertex.Adjacent.Add(firstVertex);
            this.weight = weight;
            orientation = orient;
        }

        public Vertex FirstVertex
        {
            get { return firstVertex; }
            set
            {
                firstVertex = value;
                OnPropertyChanged();
            }
        }
        public Vertex SecondVertex
        {
            get { return secondVertex; }
            set
            {
                secondVertex = value;
                OnPropertyChanged();
            }
        }
        public int Weight
        {
            get { return weight; }
            set
            {
                weight = value;
                OnPropertyChanged();
            }

        }
        public string StrWeight
        {
            get { return weight.ToString(); }
            set
            {
                int res = 0;
                if (Int32.TryParse(value, out res))
                    Weight = res;
            }
        }
        public EdgeOrientation Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;
                OnPropertyChanged();
            }
        }

        public bool Belongs(Vertex vertex)
        {
            return firstVertex == vertex || secondVertex == vertex ? true : false;
        }
    }



}
