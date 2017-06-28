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
		List<KeyValuePair<string, Font>> styleList = new List<KeyValuePair<string, Font>>();
		List<KeyValuePair<string, string>> weblinkList = new List<KeyValuePair<string, string>>();
		List<TextBox> generatedButtons = new List<TextBox>();

		public BrowserWindow()
		{
			InitializeComponent();
		}

		private void BrowserWindow_Load(object sender, EventArgs e)
		{

		}

		private void renderBox_TextChanged(object sender, EventArgs e)
		{

		}

		//---------------------------------------------------------------------------
		private void goButton_Click(object sender, EventArgs e)
		{
			string strURL = formatURL(inputBox.Text);
			this.Text = strURL; // Default to URL if no <title> tag found

			HttpClient client = new HttpClient();
			HttpContent content;
			try
			{
				content = client.GetAsync(strURL).Result.Content;

				// Convert any newlines and tabs to spaces before we begin.
				// That's how a real browser would handle whitespace.
				string inputHTML = content.ReadAsStringAsync().Result;
				inputHTML = inputHTML.Replace(Environment.NewLine, " ").Trim();
				inputHTML = inputHTML.Replace("\t", " ");

				// We should also normalize a handful of the tags, for ease
				inputHTML = inputHTML.Replace("<HEAD", "<head");
				inputHTML = inputHTML.Replace("/HEAD", "/head");
				inputHTML = inputHTML.Replace("<BODY", "<body");
				inputHTML = inputHTML.Replace("/BODY", "/body");
				inputHTML = inputHTML.Replace("<TITLE", "<title");
				inputHTML = inputHTML.Replace("/TITLE", "/title");

				renderContent(inputHTML);

				renderBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(scroller);
			}
			catch
			{	// Build some kind of error message, instead of crashing the program.
				renderContent("<body>Error trying to reach destination: " + strURL + "</body>");
			}
		}

		//---------------------------------------------------------------------------
		private string formatURL(string strURL)
		{
			bool hasHTTP = strURL.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase);
			bool hasHTTPS = strURL.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase);
			if (!hasHTTP && !hasHTTPS)
			{   // Correct the formatting
				strURL = string.Concat("http://", strURL);
			}

			return strURL;
		}

		//---------------------------------------------------------------------------
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

			clearOldData();

			// Now move onto the <body> processing
			//-----------------------------------------------------------------
			start = html.IndexOf("<body");
			if (start > -1)
			{   // <body*> tag located!
				int end1 = html.IndexOf(">",start)+ 1; // Add one for '>' itself
				int end2 = html.IndexOf("</body>");
				if (end2 > end1)
				{   // There's text between opening and closing tags
					string body = html.Substring(end1, end2 - end1);
					renderBox.Text = parseBody( body.Trim() );
					formatDisplay();
				}
			}
		}

		//---------------------------------------------------------------------------
		private void clearOldData()
		{
			renderBox.Text = "";
			styleList.Clear();
			weblinkList.Clear();

			// Disconnect any buttons still possibly around
			foreach (TextBox box in generatedButtons)
			{
				renderBox.Controls.Remove(box);
				box.Dispose();
			}
			generatedButtons.Clear();
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

					//---------------------------------------------------------
					// SPECIAL CASES:
					// Some tags are self-contained and have no 'closing' tag.
					if (tagType == "br" || tagType == "hr")
					{
						finalHTML += Environment.NewLine;
						continue;
					}
					else if (tagType.StartsWith("!") || tagType == "img")
					{
						continue;
					}
					//---------------------------------------------------------

					// If not a special case, find the "end" (closing flag) of the tag
					start = html.IndexOf("</" + tagType);
					if (start == -1)
					{   // Something is wrong, the tag never got closed out!
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
			string[] tagArray = tag.Split();
			string retVal = "";

			// Handle tables as their own separate thing:
			if (tagArray[0] == "table")
			{
				retVal = formatTable( content.Trim() );
				return retVal;
			}

			// Wasn't a table, so move on with everything else:
			retVal = parseBody(content);
			Font f;
			KeyValuePair<string, Font> style;

			switch ( tagArray[0] )
			{
				case "h1":
				case "H1":
					f = new Font(renderBox.Font.FontFamily, renderBox.Font.SizeInPoints + 3, FontStyle.Bold);
					style = new KeyValuePair<string, Font>(retVal, f);
					styleList.Add(style);
					retVal = Environment.NewLine + retVal + Environment.NewLine;
					break;

				case "h2":
				case "H2":
					f = new Font(renderBox.Font.FontFamily, renderBox.Font.SizeInPoints + 2, FontStyle.Bold);
					style = new KeyValuePair<string, Font>(retVal, f);
					styleList.Add(style);
					retVal = Environment.NewLine + retVal + Environment.NewLine;
					break;

				case "h3":
				case "H3":
					f = new Font(renderBox.Font.FontFamily, renderBox.Font.SizeInPoints + 1, FontStyle.Bold);
					style = new KeyValuePair<string, Font>(retVal, f);
					styleList.Add(style);
					retVal = Environment.NewLine + retVal + Environment.NewLine;
					break;

				case "h4":
				case "H4":
					f = new Font(renderBox.Font.FontFamily, renderBox.Font.SizeInPoints, FontStyle.Bold);
					style = new KeyValuePair<string, Font>(retVal, f);
					styleList.Add(style);
					retVal = Environment.NewLine + retVal + Environment.NewLine;
					break;

				case "p":
				case "P":
				case "div":
				case "DIV":
					retVal = Environment.NewLine + retVal + Environment.NewLine;
					break;

				case "span":
				case "SPAN":
					f = new Font(renderBox.Font, FontStyle.Italic);
					style = new KeyValuePair<string, Font>(retVal,f);
					styleList.Add(style);
					break;

				case "a":
				case "A":
					f = new Font(renderBox.Font, FontStyle.Underline);
					style = new KeyValuePair<string, Font>(retVal, f);
					styleList.Add(style);

					if (tagArray.Length > 1)
					{	// Ignore stuff like a bare <a> tag
						addLink(retVal, tagArray[1]);
					}
					break;

				case "script": // Ignore all "script" tags
				case "SCRIPT":
					retVal = "";
					break;

				// Table content stuff
				case "tr":
				case "TR":
					retVal += "‡"; // Special marker that's unlikely to come up normally
					break;

				case "th":
				case "TH":
					f = new Font(FontFamily.GenericMonospace, renderBox.Font.SizeInPoints, FontStyle.Bold);
					style = new KeyValuePair<string, Font>(retVal, f);
					styleList.Add(style);
					retVal = "|" + retVal;
					break;

				case "td":
				case "TD":
					f = new Font(FontFamily.GenericMonospace, renderBox.Font.SizeInPoints);
					style = new KeyValuePair<string, Font>(retVal, f);
					styleList.Add(style);
					retVal = "|" + retVal;
					break;

				default:
					break;
			}

			return retVal;
		}

		//---------------------------------------------------------------------------
		// The "innards" of a table can be handled by the normal tag parser.
		// What this tries to do is figure out the "size" of the table so
		// we can line everything up and make it look not-terrible.
		private string formatTable(string content)
		{
			// Set up some storage for "formatted table" entries
			List<List<string>> formattedRows = new List<List<string>>(); // It's a list-of-lists

			int maxLength = 0;
			int maxWidthInPx = 0;
			Font tableFont = new Font(FontFamily.GenericMonospace, renderBox.Font.SizeInPoints);

			string[] rows = parseBody(content).Split('‡');
			foreach (string row in rows)
			{
				if(row == "")
				{	// Don't do anything with empty rows
					continue;
				}

				List<string> formattedColumns = new List<string>();

				string[] columns = row.Split('|');
				foreach (string cell in columns)
				{
					string trimmedCell = cell.Trim();
					if(trimmedCell == "")
					{	// Skip empty cells
						continue;
					}

					// While we're here, track the largest cell
					if (trimmedCell.Length > maxLength)
					{
						maxLength = trimmedCell.Length;
						maxWidthInPx = TextRenderer.MeasureText(trimmedCell, tableFont).Width;
					}

					// Build a list of cells for this row
					formattedColumns.Add(trimmedCell);
				}

				// Add all formatted cells to the formatted row list
				formattedRows.Add(formattedColumns);
			}

			// Now, we take all "formatted" entries and add them back
			// into the output string in a vaguely grid-like fashion.
			string retVal = Environment.NewLine;
			foreach(List<string> rowList in formattedRows)
			{
				foreach(string col in rowList)
				{
					retVal += "| " + col;
					string filler = "";
					while (TextRenderer.MeasureText(col + filler, tableFont).Width < maxWidthInPx)
					{
						filler += '\xA0'; // &nbsp; (non-breaking space)
					}
					retVal += filler + " ";
				}

				retVal += "|" + Environment.NewLine;
			}

			return (retVal + Environment.NewLine);
		}

		//---------------------------------------------------------------------------
		private void addLink(string displaytext, string target)
		{
			string targetURL = target.Split('=')[1];
			targetURL = targetURL.Substring(1, targetURL.Length - 2); // Strip off the quotes
			KeyValuePair<string, string> newLink = new KeyValuePair<string, string>(displaytext, targetURL);
			weblinkList.Add(newLink);
		}

		//---------------------------------------------------------------------------
		private void formatDisplay()
		{
			Int32 index = 0;
			foreach (KeyValuePair<string, Font> style in styleList)
			{
				index = renderBox.Find(style.Key, index, RichTextBoxFinds.MatchCase);
				renderBox.SelectionFont = style.Value;
			}

			index = 0;
			foreach (KeyValuePair<string, string> link in weblinkList)
			{
				index = renderBox.Find(link.Key, index, RichTextBoxFinds.MatchCase);

				Point p = renderBox.GetPositionFromCharIndex(index);

				Size sz = TextRenderer.MeasureText(link.Key, renderBox.Font);
				Font fnt = new Font(renderBox.Font.FontFamily, renderBox.Font.SizeInPoints, FontStyle.Underline);

				// Here, we're building a TextBox that ACTS like a button.
				// By doing so, the button matches the rest of our text
				// window, but it becomes a thing you can click on.
				TextBox linkButton = new TextBox();
				linkButton.Location = p;
				linkButton.Size = sz;
				linkButton.Text = link.Key;
				linkButton.Tag = link.Value;
				linkButton.Font = fnt;
				linkButton.ForeColor = Color.Blue;
				linkButton.BackColor = renderBox.BackColor;
				linkButton.BorderStyle = BorderStyle.None;
				linkButton.Click += new EventHandler(linkHandler);

				generatedButtons.Add(linkButton);    // Save it to our list
				renderBox.Controls.Add(linkButton);  // Add it to the overall control list
				linkButton.BringToFront();           // Force it to draw on top
			}
		}

		//---------------------------------------------------------------------------
		private void linkHandler(object sender, EventArgs e)
		{
			TextBox btn = sender as TextBox;
			inputBox.Text = (string)btn.Tag;
			renderBox.Controls.Remove(btn); // Remove the button
			goButton.PerformClick();        // Now fake a click to "Go" there
		}

		//---------------------------------------------------------------------------
		private void scroller(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			// Do some kind of scroll handling, to keep all the web-link buttons
			// tied to the correct location and not floating in space.
		}
	}
}
