﻿#pragma checksum "..\..\FancyBalloon.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "C43CA727D29ECFBD3B4C6A0497205643E8A01CFD"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace APCUPS {
    
    
    /// <summary>
    /// FancyBalloon
    /// </summary>
    public partial class FancyBalloon : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 5 "..\..\FancyBalloon.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal APCUPS.FancyBalloon me;
        
        #line default
        #line hidden
        
        
        #line 67 "..\..\FancyBalloon.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Animation.BeginStoryboard FadeIn_BeginStoryboard;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\FancyBalloon.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Animation.BeginStoryboard HighlightCloseButton_BeginStoryboard;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\FancyBalloon.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Animation.BeginStoryboard FadeCloseButton_BeginStoryboard;
        
        #line default
        #line hidden
        
        
        #line 81 "..\..\FancyBalloon.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Animation.BeginStoryboard FadeBack_BeginStoryboard1;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\FancyBalloon.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Animation.BeginStoryboard FadeOut_BeginStoryboard;
        
        #line default
        #line hidden
        
        
        #line 89 "..\..\FancyBalloon.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid grid;
        
        #line default
        #line hidden
        
        
        #line 122 "..\..\FancyBalloon.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtCountdown;
        
        #line default
        #line hidden
        
        
        #line 129 "..\..\FancyBalloon.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Documents.Run shutdownTime;
        
        #line default
        #line hidden
        
        
        #line 166 "..\..\FancyBalloon.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image imgClose;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/APCUPS;component/fancyballoon.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\FancyBalloon.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.me = ((APCUPS.FancyBalloon)(target));
            return;
            case 2:
            
            #line 53 "..\..\FancyBalloon.xaml"
            ((System.Windows.Media.Animation.Storyboard)(target)).Completed += new System.EventHandler(this.OnFadeOutCompleted);
            
            #line default
            #line hidden
            return;
            case 3:
            this.FadeIn_BeginStoryboard = ((System.Windows.Media.Animation.BeginStoryboard)(target));
            return;
            case 4:
            this.HighlightCloseButton_BeginStoryboard = ((System.Windows.Media.Animation.BeginStoryboard)(target));
            return;
            case 5:
            this.FadeCloseButton_BeginStoryboard = ((System.Windows.Media.Animation.BeginStoryboard)(target));
            return;
            case 6:
            this.FadeBack_BeginStoryboard1 = ((System.Windows.Media.Animation.BeginStoryboard)(target));
            return;
            case 7:
            this.FadeOut_BeginStoryboard = ((System.Windows.Media.Animation.BeginStoryboard)(target));
            return;
            case 8:
            this.grid = ((System.Windows.Controls.Grid)(target));
            
            #line 90 "..\..\FancyBalloon.xaml"
            this.grid.MouseEnter += new System.Windows.Input.MouseEventHandler(this.grid_MouseEnter);
            
            #line default
            #line hidden
            return;
            case 9:
            this.txtCountdown = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 10:
            this.shutdownTime = ((System.Windows.Documents.Run)(target));
            return;
            case 11:
            this.imgClose = ((System.Windows.Controls.Image)(target));
            
            #line 167 "..\..\FancyBalloon.xaml"
            this.imgClose.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.imgClose_MouseDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

