using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
    public partial class MainWindow : Window
    {
        bool vertexDragOn;

        const double scaleRate = 1.1;

        //Edge
        bool edgeStart;
        Line tempLine;
        Vertex startVertex;
        
        
        bool ctrlHold;

        private ViewModel vm;

        public MainWindow()
        {
            InitializeComponent();

            VertexTool.Background = new SolidColorBrush(Colors.LightSlateGray);
            vm = new ViewModel();
            DataContext = vm;

            edgeStart = false;

            GraphCanvas.Cursor = Cursors.Pen;

        }

        private void GraphCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(GraphCanvas);

            switch (vm.ToolMode)
            {
                case ToolMode.Vertex:
                    ResetSelection();
                    Border b = vm.NewVertex(position);
                    b.MouseLeftButtonDown += Vertex_MouseLeftButtonDown;
                    b.MouseLeftButtonUp += Vertex_MouseLeftButtonUp;
                    b.MouseRightButtonDown += Vertex_MouseRightButtonDown;
                    b.MouseMove += Vertex_MouseMove;
                    Canvas.SetLeft(b, position.X - 25);
                    Canvas.SetTop(b, position.Y - 25);
                    Panel.SetZIndex(b, 1);
                    GraphCanvas.Children.Add(b);
                    break;
                case ToolMode.Point:
                    ResetSelection();
                    break;

            }
        }

        private void GraphCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (vm.ToolMode == ToolMode.Edge)
                if (tempLine != null)
                {
                    edgeStart = false;
                    GraphCanvas.Children.Remove(tempLine);
                    tempLine = null;
                }

        }


        private void GraphCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(GraphCanvas);
            if (vm.ToolMode == ToolMode.Edge && edgeStart)
            {
                tempLine.X2 = position.X - 1;
                tempLine.Y2 = position.Y - 1;
            }

        }


        private void GraphCanvas_Key(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
            {
                if (e.IsDown)
                {
                    if (vm.ToolMode == ToolMode.Point)
                        this.Cursor = Cursors.Hand;
                    ctrlHold = true;
                }
                else
                {
                    if (vm.ToolMode == ToolMode.Point)
                        this.Cursor = Cursors.Arrow;
                    ctrlHold = false;
                }
               
            }
            if (e.IsDown)
            {
                if (e.Key == Key.Z)
                {
                    vm.UndoGraph();
                    BuildCanvas();
                    Console.WriteLine("Undo graph");
                }
                else if (e.Key == Key.Y)
                {
                    vm.RedoGraph();
                    BuildCanvas();
                    Console.WriteLine("Redo Graph");
                }
                else if (e.Key == Key.L)
                {
                    RightPanel.Visibility = Visibility.Collapsed;
                    Grid.SetColumnSpan(GraphCanvas, 2);
                }
            }

        }



        private void Vertex_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (vm.ToolMode == ToolMode.Hand)
            {
                vertexDragOn = false;
                Border b = sender as Border;
                b.ReleaseMouseCapture();
                vm.SaveGraphState();
            }
        }

        private void Vertex_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border b = sender as Border;

            if (vm.ToolMode == ToolMode.Hand)
            {
                vertexDragOn = true;
                b.CaptureMouse();
            }
            else if (vm.ToolMode == ToolMode.Point)
            {
                if (!ctrlHold)
                    ResetSelection();
                var bCh = b.Child as Border;
                var tb = bCh.Child as TextBlock;
                if (vm.AddSelectedVertex(tb.Text))
                    bCh.Background = vm.SelectedVertexBrush;
                else
                    bCh.Background = vm.VertexBrush;
            }
            else if (vm.ToolMode == ToolMode.Edge)
            {
                Border bCh = b.Child as Border;
                TextBlock tb = bCh.Child as TextBlock;
                if (!edgeStart)
                {
                    tempLine = new Line();
                    startVertex = vm.Graph[tb.Text];
                    tempLine.Stroke = vm.EdgeBrush;
                    tempLine.StrokeThickness = 4;
                    tempLine.X1 = tempLine.X2 = startVertex.Position.X;
                    tempLine.Y1 = tempLine.Y2 = startVertex.Position.Y;
                    GraphCanvas.Children.Add(tempLine);
                }
                else
                {
                    GraphCanvas.Children.Remove(tempLine);
                    tempLine = null;
                    Vertex endVertex = vm.Graph[tb.Text];
                    if (!startVertex.AdjecentWith(endVertex))
                    {
                        vm.AddEdge(startVertex, endVertex);
                        Binding b1X = new Binding($"Graph[{startVertex.Name},{endVertex.Name}].FirstVertex.Position.X");
                        Binding b1Y = new Binding($"Graph[{startVertex.Name},{endVertex.Name}].FirstVertex.Position.Y");
                        Binding b2X = new Binding($"Graph[{startVertex.Name},{endVertex.Name}].SecondVertex.Position.X");
                        Binding b2Y = new Binding($"Graph[{startVertex.Name},{endVertex.Name}].SecondVertex.Position.Y");
                        Line edgeLine = new Line();
                        edgeLine.Stroke = vm.EdgeBrush;
                        Panel.SetZIndex(edgeLine, 0);
                        edgeLine.StrokeThickness = 4;
                        edgeLine.SetBinding(Line.X1Property, b1X);
                        edgeLine.SetBinding(Line.Y1Property, b1Y);
                        edgeLine.SetBinding(Line.X2Property, b2X);
                        edgeLine.SetBinding(Line.Y2Property, b2Y);
                        edgeLine.MouseRightButtonDown += Line_MouseRightButtonDown;
                        GraphCanvas.Children.Add(edgeLine);
                    }


                }
                edgeStart = !edgeStart;
            }
            e.Handled = true;
        }

        private void Vertex_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (vm.ToolMode == ToolMode.Vertex)
            {
                Border b = sender as Border;
                Border bCh = b.Child as Border;
                TextBlock tb = bCh.Child as TextBlock;
                Vertex v = vm.Graph[tb.Text];
                int len = GraphCanvas.Children.Count;
                for (int i = 0; i < len; i++)
                {
                    Line l = GraphCanvas.Children[i] as Line;
                    if (l != null)
                        if (l.X1 == v.Position.X || l.X2 == v.Position.X || l.Y1 == v.Position.Y || l.Y2 == v.Position.Y)
                        {
                            len--;
                            i--;
                            GraphCanvas.Children.Remove(l);
                        }
                }
                vm.RemoveVertex(v);
                GraphCanvas.Children.Remove(b);
            }
        }

        private void Vertex_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(GraphCanvas);
            if (vm.ToolMode == ToolMode.Hand && vertexDragOn)
            {
                Point point = e.GetPosition(GraphCanvas);
                Border b = sender as Border;

                Border bCh = b.Child as Border;
                TextBlock tb = bCh.Child as TextBlock;
                Vertex v = vm.Graph[tb.Text];

                Canvas.SetLeft(b, point.X - 25);
                Canvas.SetTop(b, point.Y - 25);
                v.Position = point;
            }
        }

        private void Line_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (vm.ToolMode == ToolMode.Edge)
            {
                Console.WriteLine("Edge");
                Line l = sender as Line;
                Point p1 = new Point(l.X1, l.Y1);
                Point p2 = new Point(l.X2, l.Y2);
                vm.RemoveEdge(vm.Graph.FindEdge(p1, p2));
                GraphCanvas.Children.Remove(l);

                e.Handled = true;
            }
        }


        private void Tool_Click(object sender, RoutedEventArgs e)
        {
            PointTool.Background = VertexTool.Background = EdgeTool.Background = HandTool.Background
                = ZoomTool.Background = new SolidColorBrush(Colors.Transparent);
            Button but = sender as Button;
            but.Background = new SolidColorBrush(Colors.LightSlateGray);
            switch (but.Name)
            {
                case "PointTool":
                    vm.ToolMode = ToolMode.Point;
                    GraphCanvas.Cursor = Cursors.Hand;
                    break;
                case "VertexTool":
                    vm.ToolMode = ToolMode.Vertex;
                    GraphCanvas.Cursor = Cursors.Pen;
                    break;
                case "EdgeTool":
                    vm.ToolMode = ToolMode.Edge;
                    GraphCanvas.Cursor = Cursors.UpArrow;
                    break;
                case "HandTool":
                    vm.ToolMode = ToolMode.Hand;
                    GraphCanvas.Cursor = Cursors.SizeAll;
                    break;
                case "ZoomTool":
                    vm.ToolMode = ToolMode.Zoom;
                    GraphCanvas.Cursor = Cursors.ScrollNS;
                    break;
            }
            if(tempLine != null)
            {
                edgeStart = false;
                GraphCanvas.Children.Remove(tempLine);
                tempLine = null;
            }
        }


        private void ResetSelection()
        {
            foreach(UIElement el in GraphCanvas.Children)
            {
                if (el is Border)
                {
                    Border b = el as Border;
                    Border bCh = b.Child as Border;
                    bCh.Background = vm.VertexBrush;
                }
            }
            vm.ResetSelection();
        }

        private void BuildCanvas()
        {
            GraphCanvas.Children.Clear();
            foreach(Vertex v in vm.Graph.Vertices)
            {
                Border b = vm.RenderVertex(v);
                b.MouseLeftButtonDown += Vertex_MouseLeftButtonDown;
                b.MouseLeftButtonUp += Vertex_MouseLeftButtonUp;
                b.MouseRightButtonDown += Vertex_MouseRightButtonDown;
                b.MouseMove += Vertex_MouseMove;
                Panel.SetZIndex(b, 1);
                Canvas.SetLeft(b, v.Position.X - vm.VERTEX_WIDTH / 2);
                Canvas.SetTop(b, v.Position.Y - vm.VERTEX_HEIGHT / 2);
                GraphCanvas.Children.Add(b);
            }
            foreach (Edge e in vm.Graph.Edges)
            {
                Line edgeLine = new Line();
                Vertex v1 = e.FirstVertex;
                Vertex v2 = e.SecondVertex;
                Binding b1X = new Binding($"Graph[{v1.Name},{v2.Name}].FirstVertex.Position.X");
                Binding b1Y = new Binding($"Graph[{v1.Name},{v2.Name}].FirstVertex.Position.Y");
                Binding b2X = new Binding($"Graph[{v1.Name},{v2.Name}].SecondVertex.Position.X");
                Binding b2Y = new Binding($"Graph[{v1.Name},{v2.Name}].SecondVertex.Position.Y");
                edgeLine.Stroke = vm.EdgeBrush;
                Panel.SetZIndex(edgeLine, 0);
                edgeLine.StrokeThickness = 4;
                edgeLine.SetBinding(Line.X1Property, b1X);
                edgeLine.SetBinding(Line.Y1Property, b1Y);
                edgeLine.SetBinding(Line.X2Property, b2X);
                edgeLine.SetBinding(Line.Y2Property, b2Y);
                edgeLine.MouseRightButtonDown += Line_MouseRightButtonDown;
                GraphCanvas.Children.Add(edgeLine);
            }
        }

    }
}
