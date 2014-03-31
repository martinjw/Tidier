#A .Net version of HTML Tidy

This is a fully managed version of HTML Tidy, based on Tidy.Net and updated to Html5.

It is a very old and not very elegant code-base, but it quickly and accurately fixes many HTML problems.

##HTML Tidy

Dave Raggett's HTML Tidy had two main purposes:

* Detect invalid HTML, including proprietary tags and attributes
* Automatically format HTML and fix many of the detected problems.

It helps find mismatched tags, duplicated attributes, and simple typos.

But the original HTML Tidy was written long ago (mostly 2000), and it does not recognise new HTML 5 tags and attributes.

This port was originally written so I could warn about presentational tags and attributes (center, b, align="right" ...)
Over time, I have also added detection for **some** HTML5.

##History

The original HTML Tidy project is http://tidy.sourceforge.net/ and has updates until 2008.

Around 2000, a Java port was created and this was in turn ported to C# around 2005 as http://sourceforge.net/projects/tidynet/

This is a port of the TidyNet project, first in .net 3.5/VS2008, now updated to VS2013.

I made this port with the following changes:

* A simple GUI (windows forms, this was written years ago!)
    - The first tab scans a directory for *.html and displays the messages (change the extension on the 2nd tab).
    - The second tab formats individual html files.
* The TidyMessageCollection is a generic list and simpler to use.
* Options.SuppressInfoMessages : You can suppress or ignore the noisy "Information" messages.
* Options.WarnPresentational : "Presentational" tags and attributes can be marked as Warnings  (center, b, align="right" ...) 
* Options.DocType = DocType.Html5
* It will recognise some common HTML5 tags and attributes, including section, nav, audio, video, data-* etc.
* Not all HTML5 is supported. SVG is not recognized.
* There's a tidy.Scan(directoryPath) method that returns the TidyMessageCollection.
* There's a tidy.Parse(html, messages):string method

Because of the origins of this code, it's not the greatest quality and certainly not modern .net. 
Even my bits (windows forms...) are old. But with the HTML5 additions, it works well enough. 

There are alternatives to this library. There are wrappers around newer versions of the Tidy binaries eg https://github.com/markbeaton/TidyManaged
There are newer HTML5 parsers such as Gumbo, which also have c# bindings: https://github.com/rgripper/GumboBindings , Nuget Gumbo.Wrappers
You could also use Html Agility Pack https://htmlagilitypack.codeplex.com/

##API

```C#
var tidy = new Tidy();
tidy.InitOptions();
var messages = new TidyMessageCollection();
var tidiedHtml = tidy.Parse(html, messages);
//check the messages.WarningMessages or messages.ErrorMessages
```

##License
w3c license http://www.w3.org/Consortium/Legal/2002/copyright-software-20021231
