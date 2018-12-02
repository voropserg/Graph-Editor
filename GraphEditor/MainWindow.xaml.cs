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
        bool dragOn;
        bool multSelect;
        private ViewModel vm;

        public MainWindow()
        {
            InitializeComponent();

            VertexTool.Background = new SolidColorBrush(Colors.LightSlateGray);
            vm = new ViewModel();
            DataContext = vm;

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
                    b.MouseMove += Vertex_MouseMove;
                    Canvas.SetLeft(b, position.X - 25);
                    Canvas.SetTop(b, position.Y - 25);
                    GraphCanvas.Children.Add(b);
                    break;
                case ToolMode.Point:
                    ResetSelection();
                    break;

            }
        }

        private void Vertex_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (vm.ToolMode == ToolMode.Hand)
            {
                dragOn = false;
                Border b = sender as Border;
                b.ReleaseMouseCapture();
            }
        }

        private void Vertex_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Border b = sender as Border;

            if (vm.ToolMode == ToolMode.Hand)
            {
                dragOn = true;
                b.CaptureMouse();
            }
            if (vm.ToolMode == ToolMode.Point)
            {
                if (!multSelect)
                    ResetSelection();
                var bCh = b.Child as Border;
                bCh.Background = vm.SelectedVertexBrush;
                var tb = bCh.Child as TextBlock;
                vm.AddSelectedVertex(tb.Text);
            }
            e.Handled = true;
        }

        private void GraphCanvas_Key(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
                switch (vm.ToolMode)
                {
                    case ToolMode.Point:
                        if (e.IsDown)
                        {
                            this.Cursor = Cursors.Hand;
                            multSelect = true;
                        }
                        else if (e.IsUp)
                        {
                            this.Cursor = Cursors.Arrow;
                            multSelect = false;
                        }
                        break;
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
        }

        private void Vertex_MouseMove(object sender, MouseEventArgs e)
        {
            if (vm.ToolMode == ToolMode.Hand && dragOn)
            {
                Point point = e.GetPosition(GraphCanvas);
                Border b = sender as Border;

                Border bCh = b.Child as Border;
                TextBlock tb = bCh.Child as TextBlock;
                Vertex v = vm.Graph.FindVertexByName(tb.Text);

                Canvas.SetLeft(b, point.X - 25);
                Canvas.SetTop(b, point.Y - 25);
                v.Position = point;
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

    }
}
