using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Input;

namespace BasicCaptureSample
{
    class DoubleTapHelper
    {
        public DoubleTapHelper(CoreWindow window)
        {
            _window = window;

            _gestureRecognizer = new GestureRecognizer();
            _gestureRecognizer.GestureSettings = GestureSettings.DoubleTap;

            window.PointerMoved += Window_PointerMoved;
            window.PointerPressed += Window_PointerPressed;
            window.PointerReleased += Window_PointerReleased;

            _gestureRecognizer.Tapped += OnTapped;
        }

        public event EventHandler DoubleTapped;

        private void OnTapped(GestureRecognizer sender, TappedEventArgs args)
        {
            if (args.TapCount == 2)
            {
                _gestureRecognizer.CompleteGesture();
                DoubleTapped?.Invoke(_window, new EventArgs());
            }
        }

        private void Window_PointerReleased(CoreWindow sender, PointerEventArgs args)
        {
            _gestureRecognizer.ProcessUpEvent(args.CurrentPoint);
        }

        private void Window_PointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            _gestureRecognizer.ProcessDownEvent(args.CurrentPoint);
        }

        private void Window_PointerMoved(CoreWindow sender, PointerEventArgs args)
        {
            _gestureRecognizer.ProcessMoveEvents(args.GetIntermediatePoints());
        }

        private CoreWindow _window;
        private GestureRecognizer _gestureRecognizer;
    }
}
