   M S B A M L   `   `   `  ����E  AOS-Project, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:OS_Project
OS_Project  5   	   =#clr-namespace:OS_Project.ViewModelsOS_Project.ViewModels  5   	   3clr-namespace:OS_Project.ViewsOS_Project.Views  5   	       OS_Project.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:OS_Project     5   	   Z VFontAwesome.WPF, Version=4.7.0.37774, Culture=neutral, PublicKeyToken=0758b07a11a4f466-fa$http://schemas.fontawesome.io/icons/  5   	   4
viewmodels#clr-namespace:OS_Project.ViewModels  5   	   *viewsclr-namespace:OS_Project.Views  5   	   -   5          � Title$  
MainWindow��5	   	   $	��800��6   $
��1200��6)    � WindowStartupLocation$ CenterScreen=�5
   	    � WindowStyle$
 None=�5   	   
������  � AllowsTransparency .  � 	Resources 5       �� DataType-   'OS_Project.ViewModels.TreeViewViewModel   OS_Project.Views.TreeView � DataContext+   %OS_Project.ViewModels.MemberViewModel   OS_Project.Views.Members*   $OS_Project.ViewModels.AboutViewModel   OS_Project.Views.About   FocusVisual �� Property �� Value	 �� StrokeDashArray
 J� SnapsToDevicePixels  Button.Static.Background �� Color  Button.Static.Border  Button.MouseOver.Background  Button.MouseOver.Border  Button.Pressed.Background  Button.Pressed.Border  Button.Disabled.Background  Button.Disabled.Border 	 Button.Disabled.Foreground �� 
TargetType �� 
TargetType ��Name ��Name �� Triggers P� Property P� Value �� 
TargetName 
 FocusVisual1 �� BasedOn  
SideBarBtn �� CornerRadius 
 maxbtn �� Value �� Binding!  FontAwesome.WPF.FontAwesome   Icon��%�  (��       5   )   *, +)(�� M     5   '   *, +)(�� �     5   &   *, +)&	  �     &	 �    &	 �    &	 �    &	 �    &	     &	 A    &	 f    &	 �    &		 �    '�� �    7   7  7  7  7  7  7  7  7  7 	 &	
 �    &	 :    7	�&	 `
    �� 5   
   TreeView��"  6)   .R�5         6   �� 5      �� 5   
   
Member��"  6'   .R�5         6   �� 5      �� 5   
   	About��"  6&   .R�5         6   �� 5      �� 5!   
   .�5"      ��  � �� 5#      �� 5$      .R�5%      �� $��2q�6$   $		 1 2R�6/   #��� ��6E   
 . $��1��6�   5&      5(      5)      �� 5*   
   $ 	#FFDDDDDD��6;   �� 5+   
   $ 	#FF707070��67   �� 5,   
   $ Transparent��6>   �� 5-   
   $ Transparent��6:   �� 5.   
   $ Transparent��6<   �� 5/   
   $ Transparent��68   �� 50   
   $ 	#FFF4F4F4��6=   �� 51   
   $ 	#FFADB2B5��69   �� 52   
   $ 	#FF838383��6=   �� 53   
   " ��6   .�54      ��  �@�FocusVisualStyle8   61   �� 55       � ��8  6+   �� 56       � ��8  6,   �� 57       � ��# � ��6+   �� 58       � ��$ 1q�60   �� 59       � ��$ Center=�6;   �� 5:       � ��$ Center=�69   �� 5;       � ��$ 1q�6(   �� 5<       � �� 5=      �� 5>      " ��6&   .R�5?      �� $ border��6!   #��z��61   #��z��6[   #��z��6�   
 . ._�5@      �� $ contentPresenter��6/   |�.  #��z��6[   #��z��6�   ��. #
 z
 6�   #��z��6  5A       5B      P� 5C       �@��IsDefaulted . . �5D   "   �� $ border��6@   # � ��6T    � ��5E      P� 5F       �@J�IsMouseOver . . �5G   "   �� $ border��6?   8   � ��6S   �� 5H   "   $ border��6@   8   � ��6T   5I      P� 5J       � �� . . �5K   "   �� $ border��6?   8   � ��6S   �� 5L   "   $ border��6@   8   � ��6T   5M      P� 5N       � {� .  . �5O   "   �� $ border��6?   8   � ��6S   �� 5P   "   $ border��6@   8   � ��6T   �� 5Q   "   $ contentPresenter��6K   8 	  � ��6i   5R      5T      5V      5W      �� 5X   
   .�5Y      ��  � �� 5Z      �� 5[      .R�5\      �� $��2q�6$   $		 1 2R�6/   #��� ��6E   
 . $��1��6�   5]      5_      5`      �� 5b   
   8   " 	�6@   �5c      �� 5d       � �� 5e      �� 5f      " 	�6*   .R�5g      �� $ SideIconBorder��6%   $��80��6=   $��80��6I   $ 8��6T   ._�5h   "   �� $ SideBarIconPresenter��63   $��Center=�6Q   5i       5k      P� 5l   "    �@J�IsMouseOver . . �5m   &   �� $ SideIconBorder��6-    � �� �#D7E5FA6_   5n   #   P� 5o   "    �@d�	IsChecked$
 TrueY�6?   . �5p   &   �� $ SideIconBorder��6-    � �� �#D7E5FA6_   5q   #   5s      5u      5w      �� 5y   
   " ��6   .�5z      ��  � �� 5{      �� 5|      " ��6&   .R�5}      �� $ contentPresenter��6+   $��Center=�6E   $��Center=�6b    5      �� 5�      $
 True��6J    6*   �� *IsMaximized+.a�5�   "   �� $ contentPresenter��6)    � �� 5�   &     5�   *   
�������$��Right=�6M   $ WindowRestore=�6d   $��24(�6y   5�   #   5�      �� 5�      $ False��6J    6*   �� *IsMaximized+.a�5�   "   �� $ contentPresenter��6)    � �� 5�   &     5�   *   
�������$��Right=�6M   $ WindowMaximize=�6d   $��24(�6z   5�   #   5�      5�      5�      5�      5�      .��5�      �� $ 8 8 0 0 ��6   $	��Top=�6%   ���#091F4B._�5�   
   �� $	��800��6   ��6"   �� *ActualWidth+ �� RelativeSource ��  �� AncestorType" �.+�5�      � $��86��6    � ColumnDefinitions 5�      �� 5�      6)   �� 5�      6)   .+�5�      ]� $��0��6   $��Center=�6,   $��20 0 0 0q�6G   .+�5�      �� $��Center=�6    $��20(�6;   
�������$
��Bold"�6\   $��Poppins)�6n   NNP's Product6�   �� 5�      $��center=�6    $��20 0 0 0q�6;   $��16(�6M   
�������$��Light"�6n   $��Poppins)�6�   
USB Reader6�   5�      �� 5�      $��Right=�6    �� Orientation$ 
Horizontal=�69   $��Center=�6R   $��1��6m   .+�5�      �� $��Center=�6G   $��Center=�6d   $��40��6   $��40��6�   
������ 
������ ��6   �� *MinimizeWindowCommand+.��5�        
�������$��Right=�6=   $ WindowMinimize=�6T   $��24(�6j   5�      �� 5�      #���  6G   $��Center=�6h   $��Center=�6�   $��40��6�   $��40��6�   
������ 
������ ��6   �� *MaximizeWindowCommand+5�      �� 5�      $��0 0 10 0q�6D   $��Center=�6V   $��Center=�6s   $��40��6�   $��40��6�   
������ 
������ ��6   �� *CloseWindowCommand+.��5�        $��4q�6)   
�������$��Right=�6G   $ Close=�6^   $��24(�6k   5�      5�      5�      �� 5�      $ 0 0 8 8��6   ���#fff$
��Left=�6>   ._�5�      � $	��714��6   $
��Left=�6�   ��6$   �� *ActualWidth+ �� " � 5�      �� 5�      $��96 �6+   68   �� 5�      $��* �6+   67   .+�5�      �� $ 0 0 0 12��6   $��0��65   $��2q�6E   ���#D7E5FA���#F8FAFE._�5�      �� -   5         ��Name$ SideBarContainer��5�   %   .+�5�      	� -   5         	�Name$ 
DefaultBtn��5�   *   #��[ 6?   $��80��6d   $��80��6p   $��0 24 0 0q�6{   .��5�   "     $
 Home=�61   $��40(�6=   ���#061A496K   5�      	� 5�      -   5        #��[ 5�   *   $��80��6O   $��80��6[   $��	0 24 0 24q�6f   .��5�   "     $ 
InfoCircle=�61   $��40(�6C   ���#061A496Q   5�      	� 5�      -   5        #��[ 5�   *   $��80��6P   $��80��6\   .��5�   "     $ Group=�61   $��40(�6>   ���#061A496L   5�      5�      5�      �� 5�      -   5         ��Name$ CC��5�   %   $��1��61   5�      5�      5�      5�      5�      