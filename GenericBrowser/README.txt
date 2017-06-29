INSTRUCTIONS:
===============================================================================
No installation required. Just compile and run. It's a fully self-contained
EXE file, with no external dependencies. This project was built using
Visual Studio 2015.

DEBUG USING LOCAL FILES:
===============================================================================
I've included a local file "webtester.html" in this project. It's here, next
to this README file. To run debugging with local files, use "debug:" in the URL
bar, and specify the path to your local file. For example, to use the included
file, you can type the following:

debug:../../webtester.html
(Backslashes also work)

A fully-qualified path should also work just fine (forward or backslashes):
debug:C:\Temp\webtester.html

By using the "debug" ability, it will render the HTML as if it were an online
web page, but you'll also get console output, explaining all processed tags,
and what the contents of each tag were.

It also saves this information as "debug.txt" in the same folder as the EXE.

Worth mentioning: The test file includes all required tags, as requested in
the challenge description.

NOTES:
===============================================================================
To get clickable links working, the code generates a TextBox at runtime, and
drops it on top of whatever text was supposed to be a clickable link.
Unfortunately, it's kind of "free floating" so if you scroll at all, the buttons
don't go with the scrolling. I partially got it working, but not well enough to
like it. As such, there's a function near the end (called "scroller") for 
handling MouseWheel events. Inside is just a comment explaining what it should do.

It doesn't handle nested tags. Like, a "div" inside another "div". Could probably
be extended to support it (like the classic "parse a string of parentheses and
brackets to see if it's balanced") but for now, I left it basic.

Drawing a table is very rudimentary. I couldn't come up with a decent, meaningful
way to render lined-up columns, without moving into a whole world of graphics or
on-the-fly text box creation and placement. Which, if I'd done, would've had the
same problem as the clickable links -- not really attached to the page.

The code also assumes that the input HTML has properly paired open and close 
tags, unless they're tags which don't have a close, like <br> or something. So, 
if there's a <span> it needs to have a </span> to close it out.

And, even though it wasn't in the initial challege spec, I've also set up the
code to handle line break <br> tags.
