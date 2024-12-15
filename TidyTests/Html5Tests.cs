using TidyNet;

namespace TidyTests
{
    [TestClass]
    public class Html5Tests
    {
        [TestMethod]
        public void TestOptionHtml5()
        {
            //arrange
            const string html = @"<html>
<title>my title</title>
<body>
</body>
</html>";
            var tidy = new Tidy();
            tidy.InitOptions();
            tidy.Options.DocType = DocType.Html5;
            var messages = new TidyMessageCollection();

            //act
            var result = tidy.Parse(html, messages);

            //assert
            Assert.IsFalse(string.IsNullOrEmpty(result));
            StringAssert.StartsWith(result, "<!DOCTYPE html>"); //html5
        }

        [TestMethod]
        public void TestInputAttributes()
        {
            //arrange
            const string html = @"<html>
<title>my title</title>
<body>
<input autocomplete autofocus placeholder=Something>
</body>
</html>";
            var tidy = new Tidy();
            tidy.InitOptions();
            tidy.Options.DocType = DocType.Html5;
            tidy.Options.Xhtml = false; //no XHtml
            var messages = new TidyMessageCollection();

            //act
            var result = tidy.Parse(html, messages);

            //assert
            StringAssert.Contains(result, "<input autocomplete autofocus placeholder=\"Something\">");
        }

        [TestMethod]
        public void TestHtml5Range()
        {
            //arrange
            const string html = @"<html>
<title>my title</title>
<body>
<input type=range min=0 max=100 step=10 >
</body>
</html>";
            var tidy = new Tidy();
            tidy.InitOptions();
            tidy.Options.DocType = DocType.Html5;
            var messages = new TidyMessageCollection();

            //act
            var result = tidy.Parse(html, messages);

            //assert
            StringAssert.Contains(result, "<input type=\"range\" min=\"0\" max=\"100\" step=\"10\" />");
        }


        [TestMethod]
        public void TestHtml5DataList()
        {
            //arrange
            const string html = @"<html>
<title>my title</title>
<body>
<datalist id=""datalist"">
<option value=First></option>
<option value=Second>
</datalist></body>
</html>";
            var tidy = new Tidy();
            tidy.InitOptions();
            tidy.Options.DocType = DocType.Html5;
            var messages = new TidyMessageCollection();

            //act
            var result = tidy.Parse(html, messages);

            //assert
            StringAssert.Contains(result, "<option value=\"First\"");
        }

        [TestMethod]
        public void TestHtml5Audio()
        {
            //arrange
            const string html = @"<html>
<title>my title</title>
<body>
<audio controls autoplay loop>
You are missing an annoying audio!
<source src=foo.wav type=audio/wav>
</audio>
</body>
</html>";
            var tidy = new Tidy();
            tidy.InitOptions(); //xhtml so boolean attributes are correct
            tidy.Options.DocType = DocType.Html5;
            var messages = new TidyMessageCollection();

            //act
            var result = tidy.Parse(html, messages);

            //assert
            Assert.AreEqual(0, messages.WarningMessages.Count);
            StringAssert.Contains(result, "<audio controls=\"controls\" autoplay=\"autoplay\" loop=\"loop\">");
        }

        [TestMethod]
        public void TestHtml5Video()
        {
            //arrange
            const string html = @"<html>
<title>my title</title>
<body>
<video src=sample.ogv type=video/ogv>
<track kind=subtitles src=sample.srt srclang=en>
</video>
</body>
</html>";
            var tidy = new Tidy();
            tidy.InitOptions(); //xhtml so boolean attributes are correct
            tidy.Options.DocType = DocType.Html5;
            var messages = new TidyMessageCollection();

            //act
            var result = tidy.Parse(html, messages);

            //assert
            Assert.AreEqual(0, messages.WarningMessages.Count);
        }

        [TestMethod]
        public void TestScriptTypeIsOptional()
        {
            //arrange
            const string html = @"<html>
<title>my title</title>
<body>
<script>
//alert(""Hi"");
</script>
</body>
</html>";
            var tidy = new Tidy();
            tidy.InitOptions();
            tidy.Options.DocType = DocType.Html5;
            var messages = new TidyMessageCollection();

            //act
            var result = tidy.Parse(html, messages);

            //assert
            StringAssert.Contains(result, "<script>");
        }

    }
}
