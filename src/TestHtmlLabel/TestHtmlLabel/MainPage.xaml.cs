using HtmlLabel.Forms.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestHtmlLabel
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

			LeftLabel.Text = "Left <strong>Hello</strong> world!";
			CenterLabel.Text = "Center <strong>Hello</strong> world!";
			RightLabel.Text = "Right <strong>Hello</strong> world!";
			JustifyLabel.Text = "Justify <strong>Hello</strong> world! Works only for multiline text so long that it has second line so resize window if it has only single line here!";
		}
	}
}
