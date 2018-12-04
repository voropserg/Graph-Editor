using System;
using System.Collections.Generic;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;


namespace GraphEditor
{
    class ViewModel : NotifyPropertyChanged
    {
        public Graph Graph;
        private ObservableCollection<Vertex> selectedVertices;
        private ObservableCollection<Edge> selectedEdges;

        public ViewModel()
        {
            Graph = new Graph();
            Graph.AddVertex(0, 0);
            Graph.AddVertex(100, 100);
        }
    }
}
