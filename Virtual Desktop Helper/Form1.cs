using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VDeskTool;

namespace Virtual_Desktop_Helper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Title
            this.Text = "Virtual Desktop Helper";

            // notifyIcon1
            notifyIcon1.Visible = true;
            notifyIcon1.Text = "Virtual Desktop Helper";

            // test the interface with VirtualDesktop
            //string[] args = new string[1];
            //args[0] = "/Count";
            //args[1] = "/Remove:1";
            //VDeskTool.CVDT program = new VDeskTool.CVDT();
            //richTextBox1.Text = program.VDTRun(args);

            // timer1
            timer1.Enabled = true;
            timer1.Interval = 100;
        }

        private void StandardExceptionHandler(Exception ex)
        {
            string caption = "You're exceptional!";

            string text = "Something went wrong with what you were trying to do.\n";
            text += "It probably wasn't our fault. Probably.\n";
            text += "\n\nException Details:\n" + ex.ToString();

            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SetCurrentDesktopIcon()
        {
            // use + 1 to get a 1-indexed number, given that Windows labels them one-indexed
            int current = VirtualDesktop.Desktop.FromDesktop(VirtualDesktop.Desktop.Current) + 1;

            switch (current)
            {
                case 1: notifyIcon1.Icon = Virtual_Desktop_Helper.Properties.Resources.Icon1; break;
                case 2: notifyIcon1.Icon = Virtual_Desktop_Helper.Properties.Resources.Icon2; break;
                case 3: notifyIcon1.Icon = Virtual_Desktop_Helper.Properties.Resources.Icon3; break;
                case 4: notifyIcon1.Icon = Virtual_Desktop_Helper.Properties.Resources.Icon4; break;
                case 5: notifyIcon1.Icon = Virtual_Desktop_Helper.Properties.Resources.Icon5; break;
                case 6: notifyIcon1.Icon = Virtual_Desktop_Helper.Properties.Resources.Icon6; break;
                case 7: notifyIcon1.Icon = Virtual_Desktop_Helper.Properties.Resources.Icon7; break;
                case 8: notifyIcon1.Icon = Virtual_Desktop_Helper.Properties.Resources.Icon8; break;
                case 9: notifyIcon1.Icon = Virtual_Desktop_Helper.Properties.Resources.Icon9; break;
                default: notifyIcon1.Icon = Virtual_Desktop_Helper.Properties.Resources.IconW; break;
            }

            // display the 10+ icon if they've gone to 10 or more virtual desktops
            if (current >= 10)
            {
                notifyIcon1.Icon = Virtual_Desktop_Helper.Properties.Resources.Icon10P;
            }
        }

        private void MenuItemExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MenuItemAdd_Click(object sender, EventArgs e)
        {
            try
            {
                VirtualDesktop.Desktop.FromDesktop(VirtualDesktop.Desktop.Create());
            }
            catch (Exception ex)
            {
                StandardExceptionHandler(ex);
            }
        }

        private void MenuItemRemoveThis_Click(object sender, EventArgs e)
        {
            // zero-indexed
            int current = VirtualDesktop.Desktop.FromDesktop(VirtualDesktop.Desktop.Current);
            try
            {
                VirtualDesktop.Desktop.FromIndex(current).Remove();
            }
            catch (Exception ex)
            {
                StandardExceptionHandler(ex);
            }
        }

        private void MenuItemRemoveOthers_Click(object sender, EventArgs e)
        {
            while (VirtualDesktop.Desktop.Count > 1)
            {
                // zero-indexed
                int current = VirtualDesktop.Desktop.FromDesktop(VirtualDesktop.Desktop.Current);

                // is the current desktop the 0th or later one?
                if (current > 0)
                {
                    // remove the 0th, since it's not the current one
                    try
                    {
                        VirtualDesktop.Desktop.FromIndex(0).Remove();
                    }
                    catch (Exception ex)
                    {
                        StandardExceptionHandler(ex);
                    }
                }
                else
                {
                    // remove the 1st, since it is the first one that isn't the current one
                    try
                    {
                        VirtualDesktop.Desktop.FromIndex(1).Remove();
                    }
                    catch (Exception ex)
                    {
                        StandardExceptionHandler(ex);
                    }
                }
            }
        }

        private void MenuItemHelp_Click(object sender, EventArgs e)
        {
            // put the line into an arguments array
            string[] args = new string[1];
            args[0] = "/Help";

            VDeskTool.CVDT program = new VDeskTool.CVDT();
            richTextBox1.Text = program.VDTRun(args);

            // display the form with the output
            this.WindowState = FormWindowState.Normal;
            Show();
        }

        private void MenuItemLoadScript_Click(object sender, EventArgs e)
        {
            System.IO.Stream stream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = @"c:\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((stream = openFileDialog1.OpenFile()) != null)
                    {
                        System.IO.StreamReader streamReader = new System.IO.StreamReader(stream);
                        string line = "";
                        List<string> list = new List<string>();

                        // empty richTextBox1
                        richTextBox1.Text = "";

                        // get the first line
                        line = streamReader.ReadLine();

                        // process the line
                        while (line != null && line.Length > 0)
                        {
                            // add the line
                            list.Add(line);

                            // get the next line
                            line = streamReader.ReadLine();
                        }

                        if (list.Count > 0)
                        {
                            // load the arguments
                            string[] args = new string[list.Count];
                            for (int i = 0; i < list.Count; i++)
                            {
                                args[i] = list[i];
                            }

                            // run the script and save the output
                            VDeskTool.CVDT program = new VDeskTool.CVDT();
                            richTextBox1.Text = program.VDTRun(args);

                            // display the form with the output
                            this.WindowState = FormWindowState.Normal;
                            Show();
                        }   
                        else
                        {
                            // something is wrong, show them the help text
                            MenuItemHelp_Click(sender, e);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string text = "There was a problem opening or running the script. Original error:\n" + ex.Message;
                    string caption = "You're exceptional!";

                    MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void PopulateContextMenu()
        {
            // one-indexed current desktop number
            int current = VirtualDesktop.Desktop.FromDesktop(VirtualDesktop.Desktop.Current) + 1;

            ContextMenu contextMenu = new ContextMenu();
            string currentOutOfTotal = "You are on Desktop: "
                + current
                + "/"
                + VirtualDesktop.Desktop.Count;
            contextMenu.MenuItems.Add(new MenuItem(currentOutOfTotal));
            contextMenu.MenuItems.Add(new MenuItem("——————————"));
            contextMenu.MenuItems.Add(new MenuItem("Add Desktop", MenuItemAdd_Click));

            // only allow removing desktops if there's more than 1
            if (VirtualDesktop.Desktop.Count > 1)
            {
                contextMenu.MenuItems.Add(new MenuItem("Remove this Desktop", MenuItemRemoveThis_Click));
                contextMenu.MenuItems.Add(new MenuItem("Remove other Desktops", MenuItemRemoveOthers_Click));
            }

            contextMenu.MenuItems.Add(new MenuItem("Load Script", MenuItemLoadScript_Click));
            contextMenu.MenuItems.Add(new MenuItem("Help", MenuItemHelp_Click));

            contextMenu.MenuItems.Add(new MenuItem("——————————"));
            contextMenu.MenuItems.Add(new MenuItem("Exit", MenuItemExit_Click));
            notifyIcon1.ContextMenu = contextMenu;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // start with the form minimized and hidden
            this.WindowState = FormWindowState.Minimized;
            Hide();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            SetCurrentDesktopIcon();
            PopulateContextMenu();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if(textBox1.Lines.Count() > 1)
            {
                string command = textBox1.Lines[0];

                richTextBox1.Text += "Input:  " + command + "\n";
                textBox1.Text = "";


                // put the line into an arguments array
                string[] args = command.Split(' ');

                VDeskTool.CVDT program = new VDeskTool.CVDT();
                richTextBox1.Text += program.VDTRun(args);

                // set the caret to the end
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                // scroll to the caret
                richTextBox1.ScrollToCaret();
            }
        }
    }
}
