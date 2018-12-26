using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Shapes;
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
        public Edge AddEdge(Vertex firstVertex, Vertex secondVertex, EdgeOrientation orient = EdgeOrientation.None, int weight = 0)
        {
            edgeCount++;
            Edge edge = new Edge(firstVertex, secondVertex, orient, weight);
            edges.Add(edge);
            return edge;
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
        public Edge FindEdge(Vertex v1, Vertex v2, bool oriented = false)
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
        public void ChangeOrientation(EdgeOrientation orientation = EdgeOrientation.Direct)
        {
            foreach (Edge e in edges)
                e.Orientation = orientation;
        }

        //Dijkstra
        public string Dijkstra(Vertex source, Vertex destination = null)
        {
            foreach (Edge e in edges)
                if (e.Weight < 0)
                    return "Weights must be greater then 0";

            bool oriented = IsOriented();

            int[] parent = new int[vertices.Count];
            int[] dist = new int[vertices.Count];
            bool[] sptSet = new bool[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
            {
                dist[i] = int.MaxValue;
                sptSet[i] = false;
                parent[i] = -1;
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
                            {                                    
                                parent[j] = u;
                                dist[j] = weight;
                                if (destination == vertices[j])
                                    return FormatDijkstraOutput(dist, parent, source, destination);
                            }
                        }
                    }
                }
            }
            return FormatDijkstraOutput(dist, parent, source);
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
        private string FormatDijkstraOutput(int[] dist, int[] parent, Vertex source, Vertex destination = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Source:    " + source.Name + "\n\n");
            int index = vertices.IndexOf(source);
            for (int i = 0; i < dist.Length; i++)
            {
                if (destination != null && vertices[i] != destination)
                    continue;
                if (i == index)
                    continue;
                sb.Append(vertices[i].Name + "\nPath: ");

                sb.Append(FormatPath(parent, i));
                sb.Append("\n Cost:");

                if (dist[i] == int.MaxValue)
                    sb.Append("Unreachable");
                else
                    sb.Append(dist[i]);

                sb.Append("\n\n");
            }
            return sb.ToString();
        }
        private string FormatPath(int[] parents, int parent)
        {
            if (parents[parent] == -1)
                return "";

            return FormatPath(parents, parents[parent]) + $"->{vertices[parent].Name}";
        }

        //Kruskal
        public Edge[] Kruskal()
        {
            if (IsOriented())
                return null;
            Edge[] result = new Edge[vertices.Count - 1];
            int i = 0;
            int e = 0;
            edges.Sort(delegate (Edge a, Edge b)
            {
                return a.Weight.CompareTo(b.Weight);
            });

            Subset[] subsets = new Subset[vertices.Count];
            for(int v = 0; v < vertices.Count; v++)
            {
                subsets[v].Parent = v;
                subsets[v].Rank = 0;
            }
            
            while(e < vertices.Count - 1)
            {
                Edge nextEdge = edges[i++];
                int x = FindRoot(subsets, vertices.IndexOf(nextEdge.FirstVertex));
                int y = FindRoot(subsets, vertices.IndexOf(nextEdge.SecondVertex));

                if (x != y)
                {
                    result[e++] = nextEdge;
                    Union(subsets, x, y);
                }
            }
            return result;
        }
        private int FindRoot(Subset[] subsets, int i)
        {
            if (subsets[i].Parent != i)
                subsets[i].Parent = FindRoot(subsets, subsets[i].Parent);
            return subsets[i].Parent;
        }
        private void Union(Subset[] subsets, int x, int y)
        {
            int xroot = FindRoot(subsets, x);
            int yroot = FindRoot(subsets, y);

            if (subsets[xroot].Rank < subsets[yroot].Rank)
                subsets[xroot].Parent = yroot;
            else if (subsets[xroot].Rank > subsets[yroot].Rank)
                subsets[yroot].Parent = xroot;
            else
            {
                subsets[yroot].Parent = xroot;
                ++subsets[xroot].Rank;
            }
        }
        public struct Subset
        {
            public int Parent;
            public int Rank;
        }
        
        //Hamiltonian
        public bool HamitonianCircuit()
        {
            int[] path = new int[vertices.Count];
            for (int i = 0; i < path.Length; i++)
                path[i] = -1;

            path[0] = 0;

            return HamCircUtil(path, 1);

        }
        private bool HamCircUtil(int[] path, int pos)
        {
            if (pos == vertices.Count)
            {
                if (vertices[pos - 1].AdjecentWith(vertices[0]))
                    return true;
                else
                    return false;
            }

            for(int i = 1; i < vertices.Count; i++)
            {
                if(IsValidVertex(i, path, pos))
                {
                    path[pos] = i;

                    if (HamCircUtil(path, pos + 1))
                        return true;

                    path[pos] = -1;
                }
            }
            return false;
        }
        private bool IsValidVertex(int index, int[]path, int pos)
        {
            if(FindEdge(vertices[pos - 1], vertices[index], IsOriented()) == null)
                return false;
            foreach (int node in path)
                if (node == index)
                    return false;
            return true;
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

        public Edge Edge
        {
            get => default(Edge);
            set
            {
            }
        }

        public Vertex Vertex
        {
            get => default(Vertex);
            set
            {
            }
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
                foreach (Edge e in Edges)
                    e.ChangeDirection();
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


        public List<Edge> Edges = new List<Edge>();

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

            firstVertex.Edges.Add(this);
            secondVertex.Edges.Add(this);

            Weight = weight;
            Orientation = orient;
            ChangeDirection();
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
                WeightTextBlock.Text = value.ToString();
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
                ChangeDirectionVisibility();
                OnPropertyChanged();
            }
        }

        public bool Belongs(Vertex vertex)
        {
            return firstVertex == vertex || secondVertex == vertex ? true : false;
        }

        [NonSerialized]
        public Line FirstVertexWing1 = new Line()
        { Stroke = new SolidColorBrush(Colors.Crimson), StrokeThickness = 4};

        [NonSerialized]
        public Line FirstVertexWing2 = new Line()
        { Stroke = new SolidColorBrush(Colors.Crimson), StrokeThickness = 4 };

        [NonSerialized]
        public Line SecondVertexWing1 = new Line()
        { Stroke = new SolidColorBrush(Colors.Crimson), StrokeThickness = 4 };

        [NonSerialized]
        public Line SecondVertexWing2 = new Line()
        { Stroke = new SolidColorBrush(Colors.Crimson), StrokeThickness = 4 };

        [NonSerialized]
        public TextBlock WeightTextBlock = new TextBlock()
        { Foreground = new SolidColorBrush(Colors.DimGray), FontSize = 16 };



        public void ChangeDirection()
        {
            double dx = firstVertex.Position.X - secondVertex.Position.X;
            double dy = firstVertex.Position.Y - secondVertex.Position.Y;
            double norm = Math.Sqrt(dx * dx + dy * dy);
            double udx = dx / norm;
            double udy = dy / norm;
            double ax = udx * Math.Sqrt(3) / 2 - udy * 1 / 2;
            double ay = udx * 1 / 2 + udy * Math.Sqrt(3) / 2;
            double bx = udx * Math.Sqrt(3) / 2 + udy * 1 / 2;
            double by = -udx * 1 / 2 + udy * Math.Sqrt(3) / 2;

            SecondVertexWing1.X1 = secondVertex.Position.X + udx * 25;
            SecondVertexWing1.Y1 = secondVertex.Position.Y + udy * 25;
            SecondVertexWing1.X2 = SecondVertexWing1.X1 + 20 * ax;
            SecondVertexWing1.Y2 = SecondVertexWing1.Y1 + 20 * ay;

            SecondVertexWing2.X1 = secondVertex.Position.X + udx * 25;
            SecondVertexWing2.Y1 = secondVertex.Position.Y + udy * 25;
            SecondVertexWing2.X2 = SecondVertexWing2.X1 + 20 * bx;
            SecondVertexWing2.Y2 = SecondVertexWing2.Y1 + 20 * by;

            Canvas.SetLeft(WeightTextBlock, SecondVertex.Position.X + dx / 2);
            Canvas.SetTop(WeightTextBlock, SecondVertex.Position.Y + dy / 2 + 2);

            dx = secondVertex.Position.X - firstVertex.Position.X;
            dy = secondVertex.Position.Y - firstVertex.Position.Y;
            norm = Math.Sqrt(dx * dx + dy * dy);
            udx = dx / norm;
            udy = dy / norm;
            ax = udx * Math.Sqrt(3) / 2 - udy * 1 / 2;
            ay = udx * 1 / 2 + udy * Math.Sqrt(3) / 2;
            bx = udx * Math.Sqrt(3) / 2 + udy * 1 / 2;
            by = -udx * 1 / 2 + udy * Math.Sqrt(3) / 2;

            FirstVertexWing1.X1 = firstVertex.Position.X + udx * 25;
            FirstVertexWing1.Y1 = firstVertex.Position.Y + udy * 25;
            FirstVertexWing1.X2 = FirstVertexWing1.X1 + 20 * ax;
            FirstVertexWing1.Y2 = FirstVertexWing1.Y1 + 20 * ay;

            FirstVertexWing2.X1 = firstVertex.Position.X + udx * 25;
            FirstVertexWing2.Y1 = firstVertex.Position.Y + udy * 25;
            FirstVertexWing2.X2 = FirstVertexWing2.X1 + 20 * bx;
            FirstVertexWing2.Y2 = FirstVertexWing2.Y1 + 20 * by;


        }

        public void ChangeDirectionVisibility()
        {
            switch (Orientation)
            {
                case EdgeOrientation.None:
                    FirstVertexWing1.Visibility = Visibility.Hidden;
                    FirstVertexWing2.Visibility = Visibility.Hidden;
                    SecondVertexWing1.Visibility = Visibility.Hidden;
                    SecondVertexWing2.Visibility = Visibility.Hidden;
                    break;
                case EdgeOrientation.Direct:
                    FirstVertexWing1.Visibility = Visibility.Hidden;
                    FirstVertexWing2.Visibility = Visibility.Hidden;
                    SecondVertexWing1.Visibility = Visibility.Visible;
                    SecondVertexWing2.Visibility = Visibility.Visible;
                    break;
                case EdgeOrientation.Inverted:
                    FirstVertexWing1.Visibility = Visibility.Visible;
                    FirstVertexWing2.Visibility = Visibility.Visible;
                    SecondVertexWing1.Visibility = Visibility.Hidden;
                    SecondVertexWing2.Visibility = Visibility.Hidden;
                    break;

            }
        }

        public EdgeOrientation EdgeOrientation
        {
            get => default(EdgeOrientation);
            set
            {
            }
        }
    }



}
