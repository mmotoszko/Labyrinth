   M S B A M L   `   `   `  ����F  Bsi_labirynt, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:si_labiryntsi_labirynt  5   	       si_labirynt.NewLabyrinth   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:si_labirynt     5   	     � Title$
  Nowy��5   	   $	��170��6   $	��200��6#   $	��200��6/   $	��200��6>   $	��170��5	   	   $	��170��6    � Icon$ Media/Icon.ico��5
   	   .��5      � $��5q�6    � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      $
��auto �6    � ColumnDefinitions 5   
   �� 5      $
��auto �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $��0��6    .��6-   Liczba kolumn:6=   �� 5   
   $��0��6   $��1��6    .��6-   Liczba wierszy:6>   �� 5   
   -   5         ��Name$ 
txtColumns��5      $��1��6&   $��0��66   6E   �� 5   
   -   5        $ txtRows��5      $��1��6#   $��1��63   6B   �� 5   
   -   5         ��Name$ chkRand��5      $��0��6$   $��2��64   $��
50 10 0 10q�6A   6W   �� 5   
   $��1��6   $��2��6    $��-20 3q�6-   .��6<   Losowe ściany6K   �� 5   
   -   5         ��Name$ Dodaj��5      $��5q�6    $��0��6+   $��3��6;   .��6U   Dodaj6\   �� 5   
   -   5        $ Anuluj��5      $��5q�6!   $��1��6,   $��3��6<   .��6Z   Anuluj6b   5!      5"      