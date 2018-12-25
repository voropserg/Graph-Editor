using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphEditor.Converters;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.Serialization.Formatters.Binary;

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
            ZoomBox.CenterContent();

            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = vm.RequestSave();
        }

        private void GraphCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(GraphCanvas);

            switch (vm.ToolMode)
            {
                case ToolMode.Vertex:
                    if (!ctrlHold)
                    {
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
                    }
                    break;
                case ToolMode.Point:
                    ResetSelection();
                    ChangeEdgePanelState(false);
                    ChangeAlgPanelState(false);
                    ChangeVertexPanelState(false);
                    break;
                case ToolMode.Edge:
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
                else if (e.Key == Key.Delete)
                {
                    if (vm.SelectedEdges.Count > 0)
                    {
                        ChangeEdgePanelState(false);
                        foreach (Edge edge in vm.SelectedEdges)
                            RemoveEdge(FindEdge(edge));
                        vm.SelectedEdges.Clear();
                    }
                    if (vm.SelectedVertices.Count > 0)
                    {
                        ChangeVertexPanelState(false);
                        foreach (Vertex v in vm.SelectedVertices)
                            RemoveVertex(v, FindVertex(v));
                        vm.SelectedVertices.Clear();
                    }

                }
                else if (e.Key == Key.A)
                {
                    ChangeEdgePanelState(false);
                    ChangeVertexPanelState(false);
                    ChangeAlgPanelState(true);
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

                ChangeEdgePanelState(false);
                ChangeAlgPanelState(false);
                if (vm.SelectedVertices.Count == 1 && vm.SelectedEdges.Count == 0)
                    ChangeVertexPanelState(true);
                else
                    ChangeVertexPanelState(false);
            }
            else if (vm.ToolMode == ToolMode.Edge)
            {
                ResetSelection();
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
                        Edge edge = vm.AddEdge(startVertex, endVertex, ctrlHold);
                        Binding b1X = new Binding($"Graph[{startVertex.Name},{endVertex.Name}].FirstVertex.Position.X");
                        Binding b1Y = new Binding($"Graph[{startVertex.Name},{endVertex.Name}].FirstVertex.Position.Y");
                        Binding b2X = new Binding($"Graph[{startVertex.Name},{endVertex.Name}].SecondVertex.Position.X");
                        Binding b2Y = new Binding($"Graph[{startVertex.Name},{endVertex.Name}].SecondVertex.Position.Y");
                        Line edgeLine = new Line();

                        Panel.SetZIndex(edgeLine, 0);
                        edgeLine.Stroke = vm.EdgeBrush;
                        edgeLine.StrokeThickness = 4;
                        edgeLine.SetBinding(Line.X1Property, b1X);
                        edgeLine.SetBinding(Line.Y1Property, b1Y);
                        edgeLine.SetBinding(Line.X2Property, b2X);
                        edgeLine.SetBinding(Line.Y2Property, b2Y);
                        edgeLine.MouseRightButtonDown += Line_MouseRightButtonDown;
                        edgeLine.MouseLeftButtonDown += Line_MouseLeftButtonDown;
                        GraphCanvas.Children.Add(edgeLine);

                        GraphCanvas.Children.Add(edge.SecondVertexWing1);
                        GraphCanvas.Children.Add(edge.SecondVertexWing2);
                        GraphCanvas.Children.Add(edge.FirstVertexWing1);
                        GraphCanvas.Children.Add(edge.FirstVertexWing2);
                        GraphCanvas.Children.Add(edge.WeightTextBlock);
                        Console.WriteLine(edge.WeightTextBlock.Text);
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
                RemoveVertex(v, b);
            }
            ResetSelection();
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
            ResetSelection();
            if (vm.ToolMode == ToolMode.Edge)
            {
                RemoveEdge(sender as Line);
            }
            e.Handled = true;
        }
        private void Line_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (vm.ToolMode == ToolMode.Point)
            {
                if (!ctrlHold)
                    ResetSelection();
                Line l = sender as Line;
                Edge edge = vm.Graph.FindEdge(new Point(l.X1, l.Y1), new Point(l.X2, l.Y2));
                if (vm.AddSelectedEdge(edge))
                {
                    l.Stroke = vm.SelectedEdgeBrush;
                    edge.FirstVertexWing1.Stroke = vm.SelectedEdgeBrush;
                    edge.FirstVertexWing2.Stroke = vm.SelectedEdgeBrush;
                    edge.SecondVertexWing1.Stroke = vm.SelectedEdgeBrush;
                    edge.SecondVertexWing2.Stroke = vm.SelectedEdgeBrush;
                    Console.WriteLine("Edge selected");
                }
                else
                {
                    l.Stroke = vm.EdgeBrush;
                    edge.FirstVertexWing1.Stroke = vm.EdgeBrush;
                    edge.FirstVertexWing2.Stroke = vm.EdgeBrush;
                    edge.SecondVertexWing1.Stroke = vm.EdgeBrush;
                    edge.SecondVertexWing2.Stroke = vm.EdgeBrush;
                    Console.WriteLine("Edge desected");
                }

                ChangeVertexPanelState(false);
                ChangeAlgPanelState(false);
                if (vm.SelectedVertices.Count == 0 && vm.SelectedEdges.Count >= 1)
                    ChangeEdgePanelState(true);
                else
                    ChangeEdgePanelState(false);

            }
            e.Handled = true;
        }


        private void Tool_Click(object sender, RoutedEventArgs e)
        {
            PointTool.Background = VertexTool.Background = EdgeTool.Background 
                = HandTool.Background = new SolidColorBrush(Colors.Transparent);
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
            }
            if (tempLine != null)
            {
                edgeStart = false;
                GraphCanvas.Children.Remove(tempLine);
                tempLine = null;
            }
        }


        private void ResetSelection()
        {
            Console.WriteLine("Reset selection");
            foreach (UIElement el in GraphCanvas.Children)
            {
                if (el is Border)
                {
                    Border b = el as Border;
                    Border bCh = b.Child as Border;
                    if (bCh!=null)
                        bCh.Background = vm.VertexBrush;
                }
                else if (el is Line)
                {
                    Line l = el as Line;
                    l.Stroke = vm.EdgeBrush;
                }
            }
            vm.ResetSelection();
        }

        private void RemoveVertex(Vertex v, Border b)
        {
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
            
            foreach (Edge edge in v.Edges)
            {
                GraphCanvas.Children.Remove(edge.FirstVertexWing1);
                GraphCanvas.Children.Remove(edge.FirstVertexWing2);
                GraphCanvas.Children.Remove(edge.SecondVertexWing1);
                GraphCanvas.Children.Remove(edge.SecondVertexWing2);
                GraphCanvas.Children.Remove(edge.WeightTextBlock);
            }

            vm.RemoveVertex(v);
            GraphCanvas.Children.Remove(b);

        }

        private void RemoveEdge(Line l)
        {
            Point p1 = new Point(l.X1, l.Y1);
            Point p2 = new Point(l.X2, l.Y2);
            Edge edge = vm.Graph.FindEdge(p1, p2);
            vm.RemoveEdge(edge);
            GraphCanvas.Children.Remove(l);
            GraphCanvas.Children.Remove(edge.FirstVertexWing1);
            GraphCanvas.Children.Remove(edge.FirstVertexWing2);
            GraphCanvas.Children.Remove(edge.SecondVertexWing1);
            GraphCanvas.Children.Remove(edge.SecondVertexWing2);
            GraphCanvas.Children.Remove(edge.WeightTextBlock);
        }

        private Border FindVertex(Vertex v)
        {
            foreach (UIElement ue in GraphCanvas.Children)
            {
                if (ue is Border)
                {
                    Border b = ue as Border;
                    Border chB = b.Child as Border;
                    TextBlock tb = chB.Child as TextBlock;
                    if (tb.Text == v.Name)
                        return b;
                }
            }
            return null;
        }

        private Line FindEdge(Edge e)
        {
            foreach (UIElement ue in GraphCanvas.Children)
            {
                if (ue is Line)
                {
                    Line l = ue as Line;
                    if (l.X1 == e.FirstVertex.Position.X && l.Y1 == e.FirstVertex.Position.Y
                        && l.X2 == e.SecondVertex.Position.X && l.Y2 == e.SecondVertex.Position.Y)
                        return l;
                }
            }
            return null;
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
                edgeLine.MouseLeftButtonDown += Line_MouseLeftButtonDown;
                GraphCanvas.Children.Add(edgeLine);
                GraphCanvas.Children.Add(e.SecondVertexWing1);
                GraphCanvas.Children.Add(e.SecondVertexWing2);
                GraphCanvas.Children.Add(e.FirstVertexWing1);
                GraphCanvas.Children.Add(e.FirstVertexWing2);
                GraphCanvas.Children.Add(e.WeightTextBlock);

            }
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            vm.Load();
            BuildCanvas();
        }
        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            vm.Save();
        }
        private void MenuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            vm.SaveAs();
        }
        private void MenuNew_Click(object sender, RoutedEventArgs e)
        {
            vm.NewGraph();
            BuildCanvas();
        }
        private void MenuClear_Click(object sender, RoutedEventArgs e)
        {
            vm.Graph = new Graph();
            BuildCanvas();
            ChangeAlgPanelState(false);
            ChangeEdgePanelState(false);
            ChangeVertexPanelState(false);
        }

        private void MenuDijkstra_Click(object sender, RoutedEventArgs e)
        {
            string res = vm.Dijkstra();
            AlgRes.Text = res;
            AlgTitle.Text = "Dijkstra's algorithm";
            ChangeEdgePanelState(false);
            ChangeVertexPanelState(false);
            ChangeAlgPanelState(true);

            if (vm.SelectedVertices.Count == 2)
            {
                string[] path = res.Split('\n')[3].Split(new char[] { '-', '>' });
                Vertex prevVertex = vm.SelectedVertices[0];
                for(int i = 2; i < path.Length; i += 2)
                {
                    Vertex vertex = vm.Graph[path[i]];
                    vm.AddSelectedVertex(vertex);
                    Border b = FindVertex(vertex).Child as Border;
                    b.Background = vm.SelectedVertexBrush;
                    Edge edge = vm.Graph.FindEdge(prevVertex, vertex, vm.Graph.IsOriented());
                    vm.AddSelectedEdge(edge);
                    edge.FirstVertexWing1.Stroke = vm.SelectedEdgeBrush;
                    edge.FirstVertexWing2.Stroke = vm.SelectedEdgeBrush;
                    edge.SecondVertexWing1.Stroke = vm.SelectedEdgeBrush;
                    edge.SecondVertexWing2.Stroke = vm.SelectedEdgeBrush;
                    FindEdge(edge).Stroke = vm.SelectedEdgeBrush;
                    prevVertex = vertex;
                }
            }

        }
        private void MenuHamiltonian_Click(object sender, RoutedEventArgs e)
        {

        }
        private void MenuEulerian_Click(object sender, RoutedEventArgs e)
        {

        }
        private void MenuKruskal_Click(object sender, RoutedEventArgs e)
        {
            Edge[] res = vm.Graph.Kruskal();
            if (res != null)
                foreach (Edge edge in res)
                {
                    vm.AddSelectedEdge(edge);
                    edge.FirstVertexWing1.Stroke = vm.SelectedEdgeBrush;
                    edge.FirstVertexWing2.Stroke = vm.SelectedEdgeBrush;
                    edge.SecondVertexWing1.Stroke = vm.SelectedEdgeBrush;
                    edge.SecondVertexWing2.Stroke = vm.SelectedEdgeBrush;
                    FindEdge(edge).Stroke = vm.SelectedEdgeBrush;

                    vm.AddSelectedVertex(edge.FirstVertex);
                    Border b = FindVertex(edge.FirstVertex).Child as Border;
                    b.Background = vm.SelectedVertexBrush;

                    vm.AddSelectedVertex(edge.SecondVertex);
                    b = FindVertex(edge.SecondVertex).Child as Border;
                    b.Background = vm.SelectedVertexBrush;

                }
            else
                AlgRes.Text = "Graph has to be not oriented";
            AlgTitle.Text = "Kruskal's MST";
            ChangeAlgPanelState(true);

        }


        private void MenuOrient_Click(object sender, RoutedEventArgs e)
        {
            vm.Graph.ChangeOrientation();
            vm.SaveGraphState();
        }
        private void MenuDisor_Click(object sender, RoutedEventArgs e)
        {
            vm.Graph.ChangeOrientation(EdgeOrientation.None);
            vm.SaveGraphState();
        }

        


        private void ChangeVertexPanelState(bool open)
        {
            if (open)
            {
                Grid.SetColumnSpan(ZoomBox, 1);
                VertexAdjecent.Text = "";
                RightPanelVertex.Visibility = Visibility.Visible;
                Binding b = new Binding("SelectedVertices[0].Name");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.Mode = BindingMode.TwoWay;
                VertexName.SetBinding(TextBox.TextProperty, b);
                foreach(Vertex v in vm.SelectedVertices[0].Adjacent)
                    VertexAdjecent.Text += v.Name + ", ";
            }
            else
            {
                Grid.SetColumnSpan(ZoomBox, 2);
                RightPanelVertex.Visibility = Visibility.Collapsed;
                BindingOperations.ClearBinding(VertexName, TextBox.TextProperty);
            }
        }

        private void ChangeEdgePanelState(bool open)
        {
            if (open)
            {
                Grid.SetColumnSpan(ZoomBox, 1);
                RightPanelEdge.Visibility = Visibility.Visible;
                Binding b = new Binding("SelectedEdges[0].StrWeight");
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                b.Mode = BindingMode.TwoWay;
                EdgeWeight.SetBinding(TextBox.TextProperty, b);

                switch (vm.SelectedEdges[0].Orientation)
                {
                    case EdgeOrientation.None:
                        rbOrientNone.IsChecked = true;
                        break;
                    case EdgeOrientation.Direct:
                        rbOrintDirect.IsChecked = true;
                        break;
                    case EdgeOrientation.Inverted:
                        rbOrientInv.IsChecked = true;
                        break;
                }

                EdgeFirstVertexName.Text = vm.SelectedEdges[0].FirstVertex.Name;
                EdgeSecondVertexName.Text = vm.SelectedEdges[0].SecondVertex.Name;

            }
            else
            {
                Grid.SetColumnSpan(ZoomBox, 2);
                RightPanelEdge.Visibility = Visibility.Collapsed;
                BindingOperations.ClearBinding(EdgeWeight, TextBox.TextProperty);
            }
        }

        private void ChangeAlgPanelState(bool open)
        {
            if (open)
            {
                Grid.SetColumnSpan(ZoomBox, 1);
                RightPanelAlg.Visibility = Visibility.Visible;
            }
            else
            {
                Grid.SetColumnSpan(ZoomBox, 2);
                RightPanelAlg.Visibility = Visibility.Collapsed;
            }
        }

        private void VertexName_KeyUp(object sender, KeyEventArgs e)
        {
            vm.Graph.ValidateVertexNames();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton check = sender as RadioButton;
            if (check == rbOrintDirect)
                foreach(Edge edge in vm.SelectedEdges)
                    edge.Orientation = EdgeOrientation.Direct;
            else if (check == rbOrientInv)
                foreach (Edge edge in vm.SelectedEdges)
                    edge.Orientation = EdgeOrientation.Inverted;
            else
                foreach (Edge edge in vm.SelectedEdges)
                    edge.Orientation = EdgeOrientation.None;
            vm.SaveGraphState();

        }

        private void EdgeWeight_KeyUp(object sender, KeyEventArgs e)
        {

        }

    }
}
