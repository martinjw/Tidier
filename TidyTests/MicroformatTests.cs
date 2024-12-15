using TidyNet;

namespace TidyTests
{
    [TestClass]
    public class MicroformatTests
    {

        [TestMethod]
        public void TestMicroFormats()
        {
            //arrange
            const string html = @"<html>
<title>my title</title>
<body>
<div class=""vevent"">
  <a href=""/spinaltap"" class=""url summary"">Spinal Tap</a>
  When:
  <span class=""dtstart"">
    Oct 15, 7:00PM<span class=""value-title"" title=""2015-10-15T19:00-08:00""></span>
  </span>
</div>
</body>
</html>";
            var tidy = new Tidy();
            tidy.InitOptions();
            var messages = new TidyMessageCollection();

            //act
            var result = tidy.Parse(html, messages);

            //assert
            Assert.AreEqual(0, messages.WarningMessages.Count);
        }

        [TestMethod]
        public void TestSchemaMicroData()
        {
            //arrange
            const string html = @"<html>
<title>my title</title>
<body>
<div itemscope itemtype=""http://schema.org/Movie"">
  <h1 itemprop=""name"">Avatar</h1>
  <div itemprop=""director"" itemscope itemtype=""http://schema.org/Person"">
  Director: <span itemprop=""name"">James Cameron</span> (born <time itemprop=""birthDate"" datetime=""1954-08-16T19:30"">August 16, 1954</time>)
  </div>
  <span itemprop=""genre"">Science fiction</span>
  <a href=""../movies/avatar-theatrical-trailer.html"" itemprop=""trailer"">Trailer</a>
</div>
</body>
</html>";
            //NB: doesn't support <link> which schema.org uses for hidden canonical references.
            //To do so, change TagTable definition from ContentModel.Head
            var tidy = new Tidy();
            tidy.InitOptions();
            var messages = new TidyMessageCollection();

            //act
            var result = tidy.Parse(html, messages);

            //assert
            Assert.AreEqual(0, messages.WarningMessages.Count);
        }
    }
}
