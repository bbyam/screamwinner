//#define DEBUG_ONE_SCREEN

using ScreamViewLib;
using System.Diagnostics;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace ScreamControl
{
    public partial class ScreamControl : Form
    {
        public ScreamControl()
        {
            InitializeComponent();

            btnStartStop1.Enabled = false;
            btnStartStop2.Enabled = false;
            btnStartStop3.Enabled = false;

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

        private ScreamOff _screamOff = null!;

        private readonly List<IAnimation> _animations = [];

        private void ScreamControl_Load(object sender, EventArgs e)
        {
            ScreamOffConfig config = new();
            try
            {
                config.LoadFromFile("screamoff.config");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load screamoff.config:" + Environment.NewLine + ex.Message, "Configuration Load Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

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

            _screamView.Background = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20));

            var audioSource = new AudioSource(config);
            _screamOff = new(audioSource, _screamView, config);

            _animations.Add(_screamOff);
            CompositionTarget.Rendering += Animate;

            btnStartStop1.Text = "Start" + Environment.NewLine + config.Option1.Text;
            btnStartStop2.Text = "Start" + Environment.NewLine + config.Option2.Text;
            btnStartStop3.Text = "Start" + Environment.NewLine + config.Option3.Text;
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
            var startEvent = ScreamEvents.EndScream;
            Button? targetButton = sender as Button;
            if (targetButton == null)
                return;
            Button? otherBtn1 = null;
            Button? otherBtn2 = null;
    
            if (sender == btnStartStop1)
            {
                startEvent = ScreamEvents.StartScream1;
                otherBtn1 = btnStartStop2;
                otherBtn2 = btnStartStop3;
            }
            else if (sender == btnStartStop2)
            {
                startEvent = ScreamEvents.StartScream2;
                otherBtn1 = btnStartStop1;
                otherBtn2 = btnStartStop3;
            }
            else if (sender == btnStartStop3)
            {
                startEvent = ScreamEvents.StartScream3;
                otherBtn1 = btnStartStop1;
                otherBtn2 = btnStartStop2;
            }
            else
            {
                return;
            }

            if (!_running)
            {
                _screamOff.ReceiveEvent(startEvent);
                _running = true;
                targetButton.Text = targetButton.Text.Replace("Start", "Stop");
                otherBtn1.Enabled = false;
                otherBtn2.Enabled = false;
            }
            else
            {
                _screamOff.ReceiveEvent(ScreamEvents.EndScream);
                _running = false;
                targetButton.Text = targetButton.Text.Replace("Stop", "Start");
                otherBtn1.Enabled = true;
                otherBtn2.Enabled = true;
            }
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            _screamOff.ReceiveEvent(ScreamEvents.Begin);
            btnShow.Enabled = false;
            btnStartStop1.Enabled = true;
            btnStartStop2.Enabled = true;
            btnStartStop3.Enabled = true;

        }
    }
}
