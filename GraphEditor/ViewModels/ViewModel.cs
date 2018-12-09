﻿using System;
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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;



namespace GraphEditor
{
    enum ToolMode { Point, Vertex, Edge, Hand, Zoom}
    class ViewModel : NotifyPropertyChanged
    {
        public double VERTEX_WIDTH = 50;
        public double VERTEX_HEIGHT = 50;
        public SolidColorBrush VertexBrush;
        public SolidColorBrush EdgeBrush;
        public SolidColorBrush SelectedVertexBrush;
        public SolidColorBrush SelectedEdgeBrush;

        private Stack<Graph> undoStack;
        private Stack<Graph> redoStack;

        private Graph prevChange;
        private Graph redoGraph;

        private bool redoState;

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
            SelectedEdgeBrush = new SolidColorBrush(Colors.Blue);

            undoStack = new Stack<Graph>();
            redoStack = new Stack<Graph>();

            redoState = false;

            prevChange = DeepClone(graph);
            SaveGraphState();
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

        public List<Vertex> SelecteVertices
        {
            get { return selectedVertices; }
        }
        public List<Edge> SelectedEdges
        {
            get { return selectedEdges; }
        }

        public Border NewVertex(Point p, string name = "")
        {
            Vertex v = Graph.AddVertex(p, name);
            SaveGraphState();
            return RenderVertex(v);
        }

        public void RemoveVertex(Vertex v)
        {
            Graph.RemoveVertex(v);
            SaveGraphState();
        }

        public void RemoveEdge(Edge e)
        {
            Graph.RemoveEdge(e);
            SaveGraphState();
        }

        public void AddEdge(Vertex v1, Vertex v2)
        {
            Graph.AddEdge(v1, v2);
            SaveGraphState();
        }

        public Border RenderVertex(Vertex v)
        {
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
            if (!selectedVertices.Contains(vertex))
                selectedVertices.Add(vertex);
            else
            {
                selectedVertices.Remove(vertex);
                return false;
            }
            return true;
        }

        public bool AddSelectedEdge(Edge edge)
        {
            if (!selectedEdges.Contains(edge))
                selectedEdges.Add(edge);
            else
            {
                selectedEdges.Remove(edge);
                return false;
            }
            return true;
        }

        public bool AddSelectedVertex(string name)
        {
            Vertex v = graph.FindVertex(name);
            return AddSelectedVertex(v);
        }

        public void SaveGraphState()
        {
            redoStack.Clear();
            undoStack.Push(prevChange);
            prevChange = DeepClone(graph);
            redoState = false;
        }

        public void UndoGraph()
        {
            if (undoStack.Count != 0)
            {
                redoStack.Push(DeepClone(Graph));
                Graph = undoStack.Pop();
                prevChange = DeepClone(Graph);
                redoState = false;
            }
        }

        public void RedoGraph()
        {
            if (redoStack.Count != 0)
            {
                if (!redoState)
                {
                    redoState = true;
                    prevChange = DeepClone(Graph);
                    undoStack.Push(prevChange);
                }
                Graph = redoStack.Pop();
                prevChange = DeepClone(Graph);
                undoStack.Push(prevChange);

            }
        }

        public static T DeepClone<T> (T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formater = new BinaryFormatter();
                formater.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formater.Deserialize(ms);
            }
        }

    }
}
