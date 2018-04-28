using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;

namespace BasicCaptureSample
{
    public class MainView : IFrameworkView
    {
        public void Initialize(CoreApplicationView applicationView)
        {
            _view = applicationView;
        }

        public void SetWindow(CoreWindow window)
        {
            _window = window;
        }

        public void Load(string entryPoint) { }

        public void Run()
        {
            _compositor = new Compositor();
            _target = _compositor.CreateTargetForCurrentView();
            _root = _compositor.CreateSpriteVisual();
            _content = _compositor.CreateSpriteVisual();
            _brush = _compositor.CreateSurfaceBrush();

            _root.Brush = _compositor.CreateColorBrush(Colors.White);
            _root.RelativeSizeAdjustment = Vector2.One;
            _target.Root = _root;

            _content.AnchorPoint = new Vector2(0.5f, 0.5f);
            _content.RelativeOffsetAdjustment = new Vector3(0.5f, 0.5f, 0);
            _content.RelativeSizeAdjustment = Vector2.One;
            _content.Size = new Vector2(-80, -80);
            _content.Brush = _brush;
            _brush.HorizontalAlignmentRatio = 0.5f;
            _brush.VerticalAlignmentRatio = 0.5f;
            _brush.Stretch = CompositionStretch.Uniform;
            var shadow = _compositor.CreateDropShadow();
            shadow.Mask = _brush;
            _content.Shadow = shadow;
            _root.Children.InsertAtTop(_content);

            // We can't just call the picker here, because no one is pumping messages yet.
            // By asking the dispatcher for our UI thread to run this, we ensure that the
            // message pump is pumping messages by the time this runs.
            var ignored =_window.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var ignoredTask = StartCaptureAsync();
            });

            _window.Activate();
            _window.Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessUntilQuit);
        }

        public void Uninitialize()
        {
            _compositor.Dispose();
        }

        private async Task StartCaptureAsync()
        {
            var picker = new Windows.Graphics.Capture.GraphicsCapturePicker();
            var item = await picker.PickSingleItemAsync();
            var device = new Microsoft.Graphics.Canvas.CanvasDevice();

            _capture = new Capture(device, item);

            var surface = _capture.CreateSurface(_compositor);
            _brush.Surface = surface;

            _capture.StartCapture();
        }

        private CoreWindow _window;
        private CoreApplicationView _view;

        private Compositor _compositor;
        private CompositionTarget _target;
        private SpriteVisual _root;
        private SpriteVisual _content;
        private CompositionSurfaceBrush _brush;

        private Capture _capture;
    }

    public class MainViewFactory : IFrameworkViewSource
    {
        public IFrameworkView CreateView()
        {
            return new MainView();
        }

        public static void Main(string[] args)
        {
            CoreApplication.Run(new MainViewFactory());
        }
    }
}
