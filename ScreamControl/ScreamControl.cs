#define DEBUG_ONE_SCREEN

using ScreamViewLib;
using System.Diagnostics;
using System.Windows.Media;

namespace ScreamControl
{
    public partial class ScreamControl : Form
    {
        public ScreamControl()
        {
            InitializeComponent();

            _host = new();
            var viewChildren = _host.RootGrid.Children.OfType<global::ScreamViewLib.ScreamView>();
            var view = viewChildren.FirstOrDefault();
            if (view == null)
            {
                MessageBox.Show("Unable to create view window", "Application Load Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            _screamView = view;
            _host.Background = System.Windows.Media.Brushes.Black;
        }

        private readonly HostWindow _host;
        private ScreamViewLib.ScreamView _screamView = null!;

        private readonly AudioSource _audioSource = new();
        private ScreamOff _screamOff = null!;

        private readonly List<IAnimation> _animations = [];

        private void ScreamControl_Load(object sender, EventArgs e)
        {
            var primaryScreen = Screen.PrimaryScreen ?? Screen.FromControl(this);

            var controlScreenSize = primaryScreen.Bounds;
            var startupX = (controlScreenSize.Width / 2) - (this.Width / 2);
            var startupY = (controlScreenSize.Height / 2) - (this.Height / 2);

#if DEBUG_ONE_SCREEN
            startupX = controlScreenSize.Width / 2 + 10;
            _host.Left = 0;
            _host.Top = 0;
            _host.Width = controlScreenSize.Width / 2;
            _host.Height = (int)(_host.Width * 9 / 16);

            var viewWidth = _host.Width;
            var viewHeight = _host.Height;
#else
            var secondaryScreen = Screen.AllScreens.FirstOrDefault(s => s != primaryScreen);
            if (secondaryScreen == null)
            {
                MessageBox.Show("Unable to find second monitor, please connect the second monitor and try again", "Application Load Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            _host.Left = secondaryScreen.Bounds.Left;
            _host.Top = secondaryScreen.Bounds.Top;
            _host.Height = secondaryScreen.Bounds.Height;
            _host.Width = secondaryScreen.Bounds.Width;

            var viewWidth = _host.Width;
            var viewHeight = (int)(_host.Width * 9 / 16);
#endif

            _host.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            this.Location = new Point(startupX, startupY);
            _screamView.Width = viewWidth;
            _screamView.Height = viewHeight;
            _host.Show();

            _screamView.Background = new SolidColorBrush(Colors.Gray);

            _screamOff = new(_audioSource, _screamView);

            _animations.Add(_screamOff);
            CompositionTarget.Rendering += Animate;
        }

        private void ScreamControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            _host.Close();
        }

        private void Animate(object? sender, EventArgs e)
        {
            foreach (var animation in _animations)
            {
                animation.OnAnimate();
            }
        }

        // TEMPORARY: Start/Stop ScreamOff
        private bool _running = false;
        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (!_running)
            {
                _screamOff.Start();
                _running = true;
                btnStartStop.Text = "Stop";
            }
            else
            {
                _screamOff.Stop();
                _running = false;
                btnStartStop.Text = "Start";
            }
        }
    }
}
