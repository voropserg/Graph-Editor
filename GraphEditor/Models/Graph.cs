using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace GraphEditor
{
    public enum EdgeOrientation { None, Direct, Inverted }

    class Graph : NotifyPropertyChanged
    {
        private ObservableCollection<Vertex> vertices;
        private ObservableCollection<Edge> edges;
        private string name;
        private int vertexCount;
        private int edgeCount;

        public Graph()
        {
            vertices = new ObservableCollection<Vertex>();
            edges = new ObservableCollection<Edge>();
            name = "";
        }
        public Graph(string name, IEnumerable<Vertex> vertices, IEnumerable<Edge> edges)
        {
            this.vertices = new ObservableCollection<Vertex>(vertices);
            this.edges = new ObservableCollection<Edge>(edges);
            this.name = name;
        }

        public ObservableCollection<Vertex> Vertices
        {
            get { return vertices; }
            set
            {
                vertices = value;
                OnPropertyChanged("Vertices");
            }
        }
        public ObservableCollection<Edge> Edges
        {
            get { return edges; }
            set
            {
                edges = value;
                OnPropertyChanged("Edges");
            }
        }
        public string Name
        {
           get { return name; }
           set
           {
                name = value;
                OnPropertyChanged("Name");
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
            OnPropertyChanged("Vertices");
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
        public Vertex FindVertexByName(string name)
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
            foreach (Edge ed in edges)
                if (ed.Belongs(vertex))
                    RemoveEdge(ed);
            vertices.Remove(vertex);
        }

        //Edges
        public void AddEdge(Vertex firstVertex, Vertex secondVertex, EdgeOrientation orient = EdgeOrientation.None, int weight = 0)
        {
            edgeCount++;
            edges.Add(new Edge(firstVertex, secondVertex, orient, weight));
        }
        public void RemoveEdge(Edge edge)
        {
            edges.Remove(edge);
        }

        public bool IsOriented()
        {
            foreach (Edge e in edges)
                if (e.Orientation == EdgeOrientation.None)
                    return false;
            return true;
        }

    }

    public class Vertex : NotifyPropertyChanged
    {
        private Point location;
        private string name;

        public Vertex(double x, double y, string name)
        {
            this.name = name;
            location = new Point(x, y);
            Adjacent = new ObservableCollection<Vertex>();
        }
        public Vertex(Point coordinates, string name)
        {
            this.name = name;
            location = coordinates;
            Adjacent = new ObservableCollection<Vertex>();

        }

        public ObservableCollection<Vertex> Adjacent { get; set; }
        public Point Position
        {
            get { return location; }
            set
            {
                location = value;
                OnPropertyChanged("Location");
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }


    }

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
            this.weight = weight;
            orientation = orient;
        }

        public Vertex FirstVertex
        {
            get { return firstVertex; }
            set
            {
                firstVertex = value;
                OnPropertyChanged("FirstVertex");
            }
        }
        public Vertex SecondVertex
        {
            get { return secondVertex; }
            set
            {
                secondVertex = value;
                OnPropertyChanged("SecondVertex");
            }
        }
        public int Weight
        {
            get { return weight; }
            set
            {
                weight = value;
                OnPropertyChanged("Weight");
            }

        }
        public EdgeOrientation Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;
                OnPropertyChanged("Orientation");
            }
        }

        public bool Belongs(Vertex vertex)
        {
            return firstVertex == vertex || secondVertex == vertex ? true : false;
        }
    }



}
