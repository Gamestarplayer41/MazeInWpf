   M S B A M L   `   `   `  ����   WpfMaze    WpfMaze.MainWindow   e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006       � Title$  MazeGame V1��$	��450��$	��800��$	��500��$	��250��.���  � ColumnDefinitions �� $��* ��� $	��200 � � RowDefinitions �� $��* �.+��� -    ��Name$ BitmapCanvas��z�u� -   MatrixTransform�� u� Matrix �  � OffsetX$ 0Q� � OffsetY$ 0Q�.+��� -    ��Name$ Bitmap��$��UniformToFill=��� -    ��Name$ MouseEventRectangle��$��0��	 �� Stretch$
	 Fill=�
������ $��0���� $��1��$��0��
��������� $��0��$��1��.+��� -   
 ��Name$
 CreateNewGame��$��10q�.��Neues Spiel erstellen�� -   $��10q�.��Neu Ausrichten�� $��
10 10 10 0q�Width:�� -    ��Name$ GameWidthInput��$��	10 0 10 0q��� $��
10 10 10 0q�	Height:�� -   $ GameHeightInput��$��	10 0 10 0q��� -	    ��Name$ SolvingButtons��$��10 10 10 10q�.+��� -
   $
 WallfollowerAlgorithm��.��Wallfollower�� -   $ 
 RandomBacktrackerAlgorithm��.��Random Backtracker�� -   $
 WavePropagationAlgorithm��.��Wave Propagation�� -   $��	10 0 10 0q�.��Open Devtools