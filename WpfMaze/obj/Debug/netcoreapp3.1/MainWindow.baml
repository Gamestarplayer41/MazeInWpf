   M S B A M L   `   `   `  ����  WpfMaze    WpfMaze.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	     � Title$  MazeGame V1��5   	   $	��450��6   $	��800��6*   $	��500��66   $	��250��6E   .��5      �  � ColumnDefinitions 5	   
   �� 5
      $��* �6   �� 5      $	��200 �6    � RowDefinitions 5   
   �� 5      $��* �6   .+�5   
   �� -   5         ��Name$ BitmapCanvas��5      z�5      u� 5      -   5        MatrixTransform�� u� Matrix 5      � 5       � OffsetX$ 0Q�6!    � OffsetY$ 0Q�6-   5      .+�5      �� -   5         ��Name$ Bitmap��5      $��UniformToFill=�6$   5      �� 5   
   -   5         ��Name$ MouseEventRectangle��5   F   $��0��6   	 �� Stretch$
	 Fill=�6$   
������ $��0��6c   �� 5   
   $��1��6   $��0��6$   
�������61   �� 5   
   $��0��6   $��1��6"   .+�5      �� -   5        
 ��Name$
 CreateNewGame��5      $��10q�6,   .��68   Neues Spiel erstellen6O   �� 5      -   5        $��10q�5   1   .��6=   Neu Ausrichten6M   �� 5      $��
10 10 10 0q�6   Width:64   �� 5       -   5         ��Name$ GameWidthInput��5       $��	10 0 10 0q�6.   �� 5!      $��
10 10 10 0q�6   	Height:65   �� 5"      -   5        $ GameHeightInput��5"      $��	10 0 10 0q�6/   �� 5#      -	   5         ��Name$ SolvingButtons��5#      $��10 10 10 10q�61   .+�5$      �� -
   5        $
 WallfollowerAlgorithm��5$      .��68   Wallfollower6F   �� 5%      -   5        $ 
 RandomBacktrackerAlgorithm��5%      .��6=   Random Backtracker6Q   �� 5&      -   5        $
 WavePropagationAlgorithm��5&      .��6;   Wave Propagation6M   5'      �� 5(      -   5        $��	10 0 10 0q�5(   :   .��6M   Open Devtools6\   5)      5*      5,      