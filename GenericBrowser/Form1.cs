using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;

namespace GenericBrowser
{
	public partial class BrowserWindow : Form
	{
		public BrowserWindow()
		{
			InitializeComponent();
		}

		private void BrowserWindow_Load(object sender, EventArgs e)
		{

		}

		private void renderBox_Enter(object sender, EventArgs e)
		{

		}

		private void goButton_Click(object sender, EventArgs e)
		{
			string strURL = formatURL(inputBox.Text);
			this.Text = strURL; // Default to URL if no <title> tag found

			HttpClient client = new HttpClient();
			HttpContent content = client.GetAsync(strURL).Result.Content;
			renderContent(content.ReadAsStringAsync().Result);
		}

		private string formatURL(string strURL)
		{
			if(strURL.StartsWith("http://",StringComparison.InvariantCultureIgnoreCase) == false)
			{   // Correct the formatting
				strURL = string.Concat("http://", strURL);
			}

			return strURL;
		}

		private void renderContent(string html)
		{
			// Start with the <head> processing
			//-----------------------------------------------------------------
			int start = html.IndexOf("<head>");
			if(start > -1)
			{   // <head> tag located!
				start += 6; // We don't actually NEED the <head> part
				int end = html.IndexOf("</head>");
				if (end > start)
				{	// There's text between opening and closing tags
					string header = html.Substring(start, end - start);
					parseHead(header);
				}
			}

			// Now move onto the <body> processing
			//-----------------------------------------------------------------
			start = html.IndexOf("<body>");
			if (start > -1)
			{   // <body> tag located!
				start += 6; // As before, we don't need the tag part itself
				int end = html.IndexOf("</body>");
				if (end > start)
				{   // There's text between opening and closing tags
					string body = html.Substring(start, end - start);
					parseBody(body.Trim());
				}
			}
		}

		//---------------------------------------------------------------------------
		private void parseHead(string html)
		{	// Strip out the "title" and set the window's title to it
			int start = html.IndexOf("<title>");
			if (start > -1)
			{   // <title> tag located!
				start += 7; // Again, we don't need the opening tag part
				int end = html.IndexOf("</title>");
				if (end > start)
				{   // There's text between opening and closing tags
					string title = html.Substring(start, end - start);
					this.Text = title; // Set window title to HTML <title> content
				}
			}
		}

		//---------------------------------------------------------------------------
		// Valid HTML tags to parse, per instructions:
		//static readonly string[] VALID_TAGS = "h1,h2,h3,h4,div,p,span,a,table,tr,td,th".Split(',');

		//---------------------------------------------------------------------------
		private void parseBody(string html)
		{
			string finalHTML = "";

			while (html.Length > 0)
			{
				html = html.Trim();
				int start = html.IndexOf("<");
				if (start == -1)
				{   // There are no tags on this line, just return the string
					finalHTML = html.Trim() + " ";
					html = "";
				}
				else
				{   // First, keep any text prior to the first bracket
					// (for things like mid-line HTML tags)
					if (start > 0)
					{
						finalHTML += html.Substring(0, start) + " ";
						html = html.Substring(start); // Then drop the processed part
					}

					// Get the tag!
					int end = html.IndexOf(">");
					string tagContent = html.Substring(1, end - 1);
					finalHTML += processTag(tagContent);
					html = html.Substring(end + 1);
				}
			}

			finalHTML = finalHTML.Replace("  ", " ").Trim(); // Get rid of any double-spaces & leading whitespace
			renderBox.Text = finalHTML;
		}

		//---------------------------------------------------------------------------
		private string processTag(string tag)
		{
			string retVal = "";
			bool bClosing = false;

			if (tag[0] == '/')
			{
				tag = tag.Substring(1);
				bClosing = true;
			}

			string[] tagArray = tag.Split();

			switch( tagArray[0] )
			{
				case "h1":
				case "h2":
				case "h3":
				case "h4":
					retVal += "\n";
					if(bClosing)
						retVal += "\n";
					break;

				case "div":
					retVal += "\n";
					if (bClosing)
						retVal += "\n";
					break;

				case "p":
					retVal += "\n";
					if (bClosing)
						retVal += "\n";
					break;

				case "br":
					retVal += "\n";
					break;

				default:
					break;
			}

			return retVal;
		}
	}
}
