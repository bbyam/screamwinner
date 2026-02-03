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
        }

        private HostWindow _host;
        private ScreamViewLib.ScreamView _screamView = null!;

        private void ScreamControl_Load(object sender, EventArgs e)
        {
            var controlScreenSize = Screen.FromControl(this).Bounds;
            var startupX = (controlScreenSize.Width / 2) - (this.Width / 2);
            var startupY = (controlScreenSize.Height / 2) - (this.Height / 2);
            this.Location = new Point(startupX, startupY);

            // TODO: Fit to second monitor, add debug mode
            _host.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            _host.Left = 0;
            _host.Top = 0;
            _host.Width = 400;
            _host.Height = 200;
            _host.Show();
            _screamView.Background = new SolidColorBrush(Colors.Green);
        }

        private void ScreamControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            _host.Close();
        }
    }
}
