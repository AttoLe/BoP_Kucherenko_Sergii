   M S B A M L   `   `   `  ����B  >WpfApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1  5   	       WpfApp1.Window1   5   	   W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:WpfApp1     5   	   -   5          �Name$  First_Window��5   	    � Title$ Window1��5   	   $	��450��6   $	��800��6&   ���	#FF79B2B2$��-2,0,0,0q�6I   $��Times New Roman)�6[   $��14(�6x   $y�Visible=�6�   .��5	      �  � RowDefinitions 5
   
   �� 5      $��1* �6   6*   �� 5      $��1* �6   6*   �� 5      $��0.75* �6   6-   �� 5      $��0.75* �6   6-   �� 5      $
��0.1* �6   6,   �� 5      $��0.75* �6   6-   �� 5      $
��0.1* �6   6,   �� 5      $��0.45* �6   6-   �� 5      $��1* �6   6*    � ColumnDefinitions 5   
   �� 5      $
��0.6* �6   6/   �� 5      $
��1.1* �6   6/   �� 5      $��0.15* �6   60   �� 5      $
��1.2* �6   6/   �� 5      $��0.05* �6   60   �� 5      $
��1.5* �6   6/   �� 5      $
��0.3* �6   6/   �� 5      $
��0.6* �6   6/   .+�5   
   �� -   5         ��Name$ InputBox��5      $��1��6$   $��3��64   $��35(�6B   $��Enter text:��6P    �� TextWrapping$
 Wrap=�6c   ���	#FF4D7C90���	#FF4D7C906�   �� 5    
   -   5         ��Name$
 Save��5       $��UltraExpanded&�6   $��Save student��6;   $��3��6S   $��3��6c   $��30(�6q   ���	#FFCE3A3A6�   �� 5!   
   -   5        $ Delete��5!      $��Delete student��6!   $��5��6<   $��3��6L   ���	#FFCE3A3A$��30(�6q   6   �� 5"   
   $��1��6   $��1��6"   $��3��62   $��30(�6F   ���	#FF4D7C90�� 6l   $�This is first window��6p   �� 6�   �� 6�   6�   �� 5#   
   $��7��6   $��Expanded&�6"   $��1��69   $��3��6I   $
��19.5(�6]   ���	#FF4D7C90�� 6�   $/�)Write like: full name +phone_number email��6�   �� 6�   �� 6�   6�   �� 5$   
   -   5        $ GoMain��5$      $��5��6!   $��5��6/   $��30(�6?   $��2��6M   $��3��6a   $��Go to main window��6r   ���	#FFCE3A3A6�   �� 5%   
   -   5         ��Name$ Output��5%      $�� ��6    $��5��6+   $��3��6:   $��1��6N   $��30(�6^   ���	#FF4D7C906n   5&      5'      