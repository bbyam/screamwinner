//#define DEBUG_ONE_SCREEN

using ScreamView;
using ScreamViewLib;
using System.Windows.Forms;
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

        private HostWindow _host;
        private ScreamViewLib.ScreamView _screamView = null!;

        private AudioSource _audioSource = new();

        private Meter _meter = null!;

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
            // TODO: Fit to second monitor
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

            _meter = new(0.2, 0.8, 0.8, .1, .9);
            _meter.AddToCanvas(_screamView);

            CompositionTarget.Rendering += Animate;
        }

        private void ScreamControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            _host.Close();
        }

        private void Animate(object? sender, EventArgs e)
        {
            // TEMPORARY: Simulate audio input values for testing
            if (_audioRunning)
            {
                var nextValue = _meter.MeterValue + 0.01;
                if (nextValue > 1.0)
                    nextValue = 0.0;
                _meter.MeterValue = nextValue;
            }
        }

        private bool _audioRunning = false;
        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if (!_audioRunning)
            {
                _audioSource.Start();
                _audioRunning = true;
                btnStartStop.Text = "Stop";
            }
            else
            {
                _audioSource.Stop();
                _audioRunning = false;
                btnStartStop.Text = "Start";
            }
        }
    }
}
