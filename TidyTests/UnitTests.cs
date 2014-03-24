using Microsoft.VisualStudio.TestTools.UnitTesting;
using TidyNet;

namespace TidyTests
{
    [TestClass]
    public class UnitTests
    {


        [TestMethod]
        public void TestUnclosedTags()
        {
            //arrange
            const string html = @"<html>
<title>my title
<body>
<ul>
<li>First
<li>Second
</ul>";
            var tidy = new Tidy();
            tidy.InitOptions(); //xhtml, so enforces closing
            var messages = new TidyMessageCollection();

            //act
            var result = tidy.Parse(html, messages);

            //assert
            Assert.IsFalse(string.IsNullOrEmpty(result));
            StringAssert.Contains(result, "</title>");
            StringAssert.Contains(result, "</li>");
            Assert.AreEqual(0, messages.ErrorMessages.Count);
        }

        [TestMethod]
        public void TestInvalidInputType()
        {
            //arrange
            const string html = @"<html>
<title>my title</title>
<body>
<input type=invalid>
</body>
</html>";
            var tidy = new Tidy();
            tidy.InitOptions();
            var messages = new TidyMessageCollection();

            //act
            tidy.Parse(html, messages);

            //assert
            var found = FindWarningMessage(messages, "<input> has attribute type with unrecognized value");
            Assert.IsTrue(found, "Warning message should be created");
        }

        private bool FindWarningMessage(TidyMessageCollection messages, string text)
        {
            foreach (var message in messages.WarningMessages)
            {
                if (message.Message.Contains(text))
                    return true;
            }
            return false;
        }


        [TestMethod]
        public void TestWarnDepractedAttributes()
        {
            //arrange
            const string html = @"<html>
<title>my title</title>
<body>
<font size=12>Header</font>
<table align=center summary=""My table"" border=1>
<tr><td>Hello</td></tr>
</table>
<marquee>Scrolling!</marquee>
</body>
</html>";
            var tidy = new Tidy();
            tidy.InitOptions(); //includes warn presentational
            var messages = new TidyMessageCollection();

            //act
            var result = tidy.Parse(html, messages);

            //assert
            var found = FindWarningMessage(messages, "Use CSS for presentation instead of <font> tags");
            Assert.IsTrue(found, "Warning message should be created");
            found = FindWarningMessage(messages, "Use CSS for presentation instead of <table> with attribute align");
            Assert.IsTrue(found, "Warning message should be created");
        }

        [TestMethod]
        public void TestYouTubeParams()
        {
            //arrange
            const string html = @"<html>
<title>my title</title>
<body>
<object width=""560"" height=""315"">
<param name=""movie"" value=""https://youtube.googleapis.com/v/xxx""></param>
<param name=""allowScriptAccess"" value=""always""></param>
<embed src=""https://youtube.googleapis.com/v/xxx""
  type=""application/x-shockwave-flash""
  allowfullscreen=""true""
  allowscriptaccess=""always""
  width=""560"" height=""315"">
</embed>
</object>
</body>
</html>";
            var tidy = new Tidy();
            tidy.InitOptions();
            var messages = new TidyMessageCollection();

            //act
            var result = tidy.Parse(html, messages);

            //assert
            Assert.IsFalse(messages.NeedsAuthorIntervention);
            //WarningMessages will complain about width and height
        }
    }
}
