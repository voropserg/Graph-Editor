   M S B A M L   `   `   `  ����F  BGraphEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:GraphEditorGraphEditor  5   	       GraphEditor.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:GraphEditor     5   	   V RMaterialDesignThemes.Wpf, Version=2.5.0.1205, Culture=neutral, PublicKeyToken=null:md1http://materialdesigninxaml.net/winfx/xaml/themes  5   	   X TXceed.Wpf.Toolkit, Version=3.4.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f42xt)http://schemas.xceed.com/wpf/xaml/toolkit  5   	   -   5           MaterialDesignBody#���   5
   	   $��Regular"�5   	   $��13(�5   	   &   System.Windows.Media.TextOptions    TextFormattingMode$  Ideal=�5   	      TextRenderingMode$
 Auto=�5   	     MaterialDesignPaper#���  5   	   $X��Rpack://application:,,,/MaterialDesignThemes.Wpf;component/Resources/Roboto/#Roboto)�5   	    � Title$ Graph Editor��5   	   $	��768��6   $
��1024��6+   .��5      � -   5         �Name$ MainGrid��5       � RowDefinitions 5   
   �� 5      $��0.05* �6   $��30��6+   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $��75 �6   �� 5      $��* �6   �� 5      $��0.20* �6   $	��250��6-   .+�5   
   �� 
���iii�$	��0.5q�6&   ._�5       �� $��Resources/logo2.png��6    � ToolTip$ by: @voropserg��5!      5"      � 5%   
   -   5        $ LeftPanelGrid��5%      $��1��5&       5'      �� 5(      $��0.25* �6    �� 5)      $
��auto �6    �� 5*      $
��auto �6    �� 5+      $
��auto �6    �� 5,      $
��auto �6    �� 5-      $
��auto �6    �� 5.      $��0.25* �6    .+�51      �� -   5         ��Name$ 	PointTool��51      $��50��52      $
��0,20q�53      $��1��54      $ 
Point tool��55        MaterialDesignFlatButton#��[ 57      .��58      '  !MaterialDesignThemes.Wpf.PackIcon  �  �ControlzEx.PackIconBase`1[[MaterialDesignThemes.Wpf.PackIconKind, MaterialDesignThemes.Wpf, Version=2.5.0.1205, Culture=neutral, PublicKeyToken=null]]   Kind$ CursorPointer=�6   $��40��59      $��40��5:      5;      �� 5=      -   5        $ 
VertexTool��5=      $��50��5>      $
��0,20q�5?      $��2��5@      $ Vertex tool��5A      #��[ 5C      .��5D        $ VectorCircleVariant=�6   $��40��5E      $��40��5F      5G      �� 5I      -   5        $ EdgeTool��5I      $��50��5J      $
��0,20q�5K      $��3��5L      $ 	Edge tool��5M      #��[ 5O      .��5P        $ 
VectorLine=�6   $��40��5Q      $��40��5R      5S      �� 5U      -   5        $ HandTool��5U      $��50��5V      $
��0,20q�5W      $��4��5X      $ 	Hand Tool��5Y      #��[ 5[      .��5\        $
 Hand=�6   $��40��5]      $��40��5^      5_      5d      �� 5f   
   -   5        	 ��Name$	 RightPanelVertex��5f      $��2��5g      $��1��6%   $��Center=�5h      $y�	Collapsed=�5i      .+�5j      �� $��Vertex��6   
 �� TextAlignment$
 Center=�6&   $��25(�6=   �� 5k      -	   5         ��Name$ 
VertexName��5k      $��	8,25,8,25q�6*   )  #MaterialDesignThemes.Wpf.HintAssist   Hint$
 Name��5m      �� 5o      $��Adjecent��6   $
 Center=�6(   $��18(�6?   �� 5p      -
   5         ��Name$ VertexAdjecent��5p      $
 Center=�60   $��	8,25,8,25q�6G   5q      �� 5t   
   -   5        $	 RightPanelEdge��5t      $��2��5u      $��1��6%   $��Center=�5v      $y�	Collapsed=�5w      .+�5x      �� $
��Edge��6   $��25(�6$   $
 Center=�62   �� 5y      -   5        $ 
EdgeWeight��5y      $��	8,25,8,25q�6*   $ Weight��6=   6Y   �� 5z      $��Orientation��6   $��18(�6+   $
 Center=�69   �� 5|      $��	8,15,8,25q�6   .+�5}      	� -   5         	�Name$ rbOrientNone��5}       	� 	GroupName$ Orientation��5~      $
��None��5      5�      	� 5�      -   5        $ rbOrintDirect��5�      $ Orientation��5�      $��Direct��5�      $	��0,5q�5�      5�      	� 5�      -   5        $ rbOrientInv��5�      $ Orientation��5�      $��Inversed��5�      5�      5�      � 5�      $��8,0,8,25q�6   .+�5�      �� $��First Vertex:��6   $��16(�61   $
 Center=�6?   �� 5�      -   5        $ EdgeFirstVertexName��5�      $��10,0,0,0q�69   $
 Center=�6K   5�      � 5�      $��8,0,8,0q�6   .+�5�      �� $��Second Vertex:��6   $��16(�62   $
 Center=�6@   �� 5�      -   5        $ EdgeSecondVertexName��5�      $��10,0,0,0q�6:   $
 Center=�6L   5�      5�      �� 5�   
   -   5        $	 RightPanelAlg��5�      $��2��5�      $��1��6%   $��Center=�5�      $y�	Collapsed=�5�      .+�5�      �� -   5        $ AlgTitle��5�      $
 Center=�6*   $��WrapWithOverflow=�6A   $��25(�6a   �� 5�      -   5        $ AlgRes��5�      $
 Center=�6(   $��WrapWithOverflow=�6?   $��8,25,8,0q�6_   $��16(�6q   5�      o� 5�   
   -   5         o�Name$
 Menu��5�      
���iii�$��1��5�      $��2��6   .@�5�      m� $��_File��6   .@�5�      m� -   5        $
��Open��5�       m� Icon 5�        5�      $ 
FolderOpen=�6&   5�      m� 5�      -   5        $	��New��5�       5�        5�      $
 File=�6&   5�      m� 5�      -   5        $
��Save��5�       5�        5�      $ ContentSave=�6&   5�      m� 5�      -   5        $��Save as��5�       5�        5�      $ ContentSaveOutline=�6&   5�      5�      m� 5�      $��_Graph��6   .@�5�      m� $��
Algorithms��6   .@�5�      m� -   5        $��
Dijkstra's��5�      63   m� 5�      -   5        $��Kruskal MST��5�      64   m� 5�      -   5        $��Hamiltonian Circuit��5�      6<   5�      m� 5�      -   5        $��Clear��5�      6*   m� 5�      -   5        $��	Orientade��5�      6.   m� 5�      -   5        $��Disorientade��5�      61   5�      5�      '  !Xceed.Wpf.Toolkit.Zoombox.Zoombox  5�   
   -    5          Name$ ZoomBox��5�      $��1��6&   $��1��66   $��2��5�         MinScale$	 0.5Q�5�         MaxScale$	 150Q�6$   #���   5�      .��5�      �� -!   5         ��Name$ GraphCanvas��5�      $��1��5�      $��1��5�      $
��4500��5�      $
��4500��5�      
�������z�5�      �� 5�      -"   5        	Scale��5�   %   .+�5�      ��  �� Right$ 0��6    �� Bottom$ 0��6-   
���������5�      �� *Graph.Vertices.Count+5�      5�      5�      5�      5�      