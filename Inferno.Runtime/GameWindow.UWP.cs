#if WINDOWS_UWP

using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Inferno.Runtime
{
    /// <summary>
    /// UWP Specific GameWindow code
    /// </summary>
    internal class PlatformGameWindow
    {
        internal CanvasControl Canvas;
        private string _title;
        private int _width;
        private int _height;

        public PlatformGameWindow(string title, int width, int height)
        {
            //Store data for when the canvas is set
            _title = title;
            _width = width;
            _height = height;
        }

        public void SetCanvas(CanvasControl canvas)
        {
            Canvas = canvas;

            var appView = ApplicationView.GetForCurrentView();
            appView.Title = _title;

            ApplicationView.GetForCurrentView().TryResizeView(new Size(_width, _height));
        }

        public bool AllowResize
        {
            get { return false; }
            set
            {

            }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(0, 0, 0, 0);
            }
            set
            {
            }
        }

        public int Width
        {
            get => 0;
            set
            {
            }
        }

        public int Height
        {
            get => 0;
            set
            {
                
            }
        }

        public Point Position
        {
            get { return new Point(0, 0); }
            set
            {

            }
        }

        public bool AllowAltF4
        {
            get => false;
            set
            {

            }
        }

        public string Title
        {
            get
            {
                var appView = ApplicationView.GetForCurrentView();
                return appView.Title;
            }
            set
            {
                var appView = ApplicationView.GetForCurrentView();
                appView.Title = value;
            }
        }

        public void Exit()
        {
            
        }
    }
}

#endif