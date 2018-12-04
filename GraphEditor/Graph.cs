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

        public void AddVertex(Vertex vertex)
        {
            if (vertex != null)
            {
                vertexCount++;
                vertices.Add(vertex);
            }
        }
        public void AddVertex(double x, double y, string name = "")
        {
            vertexCount++;
            if (name == "")
                name = $"Vertex{vertexCount}";
            vertices.Add(new Vertex(x, y, name));
        }
        public void AddVertex(Point coordinates, string name = "")
        {
            vertexCount++;
            if (name == "")
                name = $"Vetrex{vertexCount}";
            vertices.Add(new Vertex(coordinates, name));
        }

        public void AddEdge(Edge edge)
        {
            if (edge != null)
            {
                edgeCount++;
                edges.Add(edge);
            }
        }
        public void AddEdge(Vertex firstVertex, Vertex secondVertex, EdgeOrientation orient = EdgeOrientation.None, int weight = 0)
        {
            edgeCount++;
            edges.Add(new Edge(firstVertex, secondVertex, orient, weight));
        }

        public void RemoveVertex(Vertex vertex)
        {
            foreach (Edge ed in edges)
                if (ed.Belongs(vertex))
                    RemoveEdge(ed);
            vertices.Remove(vertex);
        }
        public void RemoveEdge(Edge edge)
        {
            edges.Remove(edge);
        }

    }

    class Vertex : NotifyPropertyChanged
    {
        private Point location;
        private string name;

        public Vertex(double x = 0, double y = 0, string name = "NewVertex")
        {
            this.name = name;
            location = new Point(x, y);
        }
        public Vertex(Point coordinates, string name = "NewVertex")
        {
            this.name = name;
            location = coordinates;
        }

        public Point Location
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

    class Edge : NotifyPropertyChanged
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
