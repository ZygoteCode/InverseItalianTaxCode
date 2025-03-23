using System;
using System.Diagnostics;
using System.Windows.Forms;

public partial class MainForm : MetroSuite.MetroForm
{
    public MainForm()
    {
        InitializeComponent();
    }

    private void MainForm_MouseClick(object sender, MouseEventArgs e)
    {
        SendKeys.Send("{TAB}");
    }

    private void guna2TextBox1_MouseClick(object sender, MouseEventArgs e)
    {
        guna2TextBox1.Focus();
        guna2TextBox1.SelectAll();
    }

    private void guna2GradientButton1_Click(object sender, EventArgs e)
    {
        try
        {
            guna2TextBox4.Text = FiscalCodeSharp.GetGender(guna2TextBox1.Text).ToString();
            guna2TextBox5.Text = FiscalCodeSharp.GetMostProbableDateOfBirth(guna2TextBox1.Text);
            guna2TextBox6.Text = FiscalCodeSharp.GetBirthPlace(guna2TextBox1.Text);

            Tuple<string, string> info = NetworkUtils.GetTaxCodeInfo(guna2TextBox1.Text);

            guna2TextBox3.Text = info.Item1;
            guna2TextBox2.Text = info.Item2;
        }
        catch
        {
            MessageBox.Show("You have inserted an invalid Italian Tax Code. Please, type it correctly and try again.", "InverseItalianTaxCode", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        Process.Start("https://github.com/ZygoteCode/InverseItalianTaxCode/");
    }

    private void guna2TextBox2_MouseClick(object sender, MouseEventArgs e)
    {
        guna2TextBox2.SelectAll();
    }

    private void guna2TextBox3_MouseClick(object sender, MouseEventArgs e)
    {
        guna2TextBox3.SelectAll();
    }

    private void guna2TextBox4_MouseClick(object sender, MouseEventArgs e)
    {
        guna2TextBox4.SelectAll();
    }

    private void guna2TextBox5_MouseClick(object sender, MouseEventArgs e)
    {
        guna2TextBox5.SelectAll();
    }

    private void guna2TextBox6_MouseClick(object sender, MouseEventArgs e)
    {
        guna2TextBox6.SelectAll();
    }
}