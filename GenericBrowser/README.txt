INSTRUCTIONS:
===============================================================================
No installation required. Just compile and run. It's a fully self-contained
EXE file, with no external dependencies. This project was built using
Visual Studio 2015.

NOTES:
===============================================================================
To get clickable links working, the code generates a TextBox at runtime, and
drops it on top of whatever text was supposed to be a clickable link.
Unfortunately, it's kind of "free floating" so if you scroll at all, the buttons
don't go with the scrolling. I partially got it working, but not well enough to
like it. As such, there's a function at the very end for handling MouseWheel
events. Inside is just a comment explaining what it should do.

It doesn't handle nested tags. Like, a "div" inside another "div". Could probably
be extended to support it (like the classic "parse a string of parentheses and
brackets to see if it's balanced") but for now, I left it basic.

Drawing a table is very rudimentary. I couldn't come up with a decent, meaningful
way to render lined-up columns, without moving into a whole world of graphics or
on-the-fly text box creation and placement. Which, if I'd done, would've had the
same problem as the clickable links -- not really attached to the page.

The code also assumes that the input HTML has proper open and close paired tags,
unless they're tags which don't have a close, like <br> or something. So, if
there's a "span" it needs to have a "/span" to close it out, that kind of thing.

