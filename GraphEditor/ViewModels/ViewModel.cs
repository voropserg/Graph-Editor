using System;
using System.Collections.Generic;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace GraphEditor
{
    enum ToolMode { Point, Vertex, Edge, Hand, Zoom}
    class ViewModel : NotifyPropertyChanged
    {
        public  double VERTEX_WIDTH = 50;
        public  double VERTEX_HEIGHT = 50;
        public SolidColorBrush VertexBrush;
        public SolidColorBrush EdgeBrush;
        public SolidColorBrush SelectedVertexBrush;
        public SolidColorBrush SelectedEdgeBrush;



        private Graph graph;

        private ToolMode mode;


        private List<Vertex> selectedVertices;
        private List<Edge> selectedEdges;

        public ViewModel()
        {
            mode = ToolMode.Vertex;
            Graph = new Graph();

            selectedEdges = new List<Edge>();
            selectedVertices = new List<Vertex>();

            SelectedVertexBrush = new SolidColorBrush(Colors.PaleGreen);
            VertexBrush = new SolidColorBrush(Colors.Purple);
            EdgeBrush = new SolidColorBrush(Colors.Crimson);
        }

        public ToolMode ToolMode
        {
            get { return mode; }
            set
            {
                mode = value;
                OnPropertyChanged();
            }
        }

        public Graph Graph
        {
            get { return graph; }
            set
            {
                graph = value;
                OnPropertyChanged();
            }
        }

        public Border NewVertex(Point p, string name = "")
        {
            Vertex v =  Graph.AddVertex(p, name);

            Border b = new Border();
            b.CornerRadius = new CornerRadius(VERTEX_HEIGHT / 2 + 3);
            b.Width = VERTEX_WIDTH + 6;
            b.Height = VERTEX_HEIGHT + 6;
            b.Background = new SolidColorBrush(Colors.DarkSlateBlue);

            Border inerB = new Border();
            inerB.CornerRadius = new CornerRadius(VERTEX_HEIGHT / 2);
            inerB.Width = VERTEX_WIDTH;
            inerB.Height = VERTEX_HEIGHT;
            inerB.Background = new SolidColorBrush(Colors.Purple);

            Binding binding = new Binding($"Graph[{v.Name}].Name");
            TextBlock tb = new TextBlock();
            tb.SetBinding(TextBlock.TextProperty, binding);
            tb.FontWeight = FontWeights.Bold;
            tb.VerticalAlignment = VerticalAlignment.Center;
            tb.TextAlignment = TextAlignment.Center;

            inerB.Child = tb;
            b.Child = inerB;

            return b;

        }

        public void ResetSelection()
        {
            selectedEdges.Clear();
            selectedVertices.Clear();
        }

        public bool AddSelectedVertex(Vertex vertex)
        {
            if (graph.Vertices.Contains(vertex))
            {
                if (!selectedVertices.Contains(vertex))
                    selectedVertices.Add(vertex);
                else
                {
                    selectedVertices.Remove(vertex);
                    return false;
                }
            }
            else
                throw new Exception("Vertex does not belong to current graph");
            return true;
        }

        public bool AddSelectedVertex(string name)
        {
            Vertex v = graph.FindVertex(name);
            return AddSelectedVertex(v);
        }

        public void NewEdge(Vertex v1, Vertex v2)
        {
            
        }
    }
}
