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

			// Convert any newline chars to spaces before we begin.
			// That's how a real browser would handle whitespace.
			string inputHTML = content.ReadAsStringAsync().Result;
			inputHTML = inputHTML.Replace(Environment.NewLine, " ").Trim();
			renderContent(inputHTML);
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

			// Clear out any currently-displayed web page data
			renderBox.Text = "";

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
					renderBox.Text = parseBody( body.Trim() );
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
		private string parseBody(string html)
		{
			string finalHTML = "";

			while (html.Length > 0)
			{
				html = html.Trim();
				int start = html.IndexOf("<");
				if (start == -1)
				{   // There are no tags remaining, just return the string
					finalHTML += html.Trim() + " ";
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

					// Get the tag name
					int end = html.IndexOf(">");
					string tagData = html.Substring(1, end - 1);

					// For finding the "closing" tag, we only need the
					// first part of the tag, not any ancillary data
					// ("div" instead of "div name=something").
					string tagType = tagData.Split()[0];

					// Now drop the tag we just grabbed, no longer needed
					html = html.Substring(end + 1);

					// SPECIAL CASE: The <br> tag is a singleton and doesn't
					// actually have a close-tag scenario, so we can just
					// move on with life after we process it.
					if (tagType == "br")
					{
						finalHTML += Environment.NewLine;
						continue;
					}

					// Next, get the "end" (closing flag) of the tag
					start = html.IndexOf("</" + tagType);
					if (start == -1)
					{   // Something is wrong, the tag never got closed out!
						//finalHTML = "+++ Tag " + tagType + " not properly closed +++";
						break;
					}

					// Pull all content between opening and closing HTML tags
					string tagContent = html.Substring(0, start);

					// Then drop the content and the closing tag
					end = html.IndexOf(">", start);
					html = html.Substring(end + 1);

					finalHTML += processTag(tagData, tagContent);
				}
			}

			finalHTML = finalHTML.Replace("  ", " ").Trim(); // Get rid of any double-spaces & surrounding whitespace
			return(finalHTML);
		}

		//---------------------------------------------------------------------------
		private string processTag(string tag, string content)
		{
			string retVal = parseBody(content);
			string[] tagArray = tag.Split();

			switch( tagArray[0] )
			{
				case "h1":
				case "h2":
				case "h3":
				case "h4":
				case "p":
					retVal = Environment.NewLine + retVal + Environment.NewLine;
					break;

				case "div":
					retVal += Environment.NewLine;
					break;

				case "span":
					break;

				case "table":
					retVal += Environment.NewLine;
					break;

				case "tr":
					retVal += " |" + Environment.NewLine;
					break;

				case "th":
				case "td":
					retVal = "| " + retVal;
					break;

				default:
					break;
			}

			return retVal;
		}
	}
}
