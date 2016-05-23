using System.Net;
using System.Windows.Forms;
using PalringoInformationScraper.Core.Providers;

namespace PalringoInformationScraper.UI.Forms
{
    public partial class MainForm : Form
    {
        private readonly PalringoProvider _palringoProvider;

        public MainForm()
        {
            _palringoProvider = new PalringoProvider();
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, System.EventArgs e)
        {
            var credential = new NetworkCredential(tbUsername.Text, tbPassword.Text);
            if (!await _palringoProvider.Login(credential))
            {
                lblLoginStatus.Text = "Login Status: Failure";
                return;
            }                      

            lblLoginStatus.Text = "Login Status: Successful";

            var information = await _palringoProvider.GetInformationAsync();
            pictureBox1.Image = await information.GetAvatarAsync();
        }  
    }
}