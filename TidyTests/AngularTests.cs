using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TidyNet;

namespace TidyTests
{
    [TestClass]
    public class AngularTests
    {
        [TestMethod]
        public void TestAngular()
        {
            //arrange
            const string html = @"<!doctype html>
<html ng-app>
  <head>
    <script src=""https://ajax.googleapis.com/ajax/libs/angularjs/angular.min.js""></script>
    <script src=""todo.js""></script>
    <link rel=""stylesheet"" href=""todo.css"">
<title>From Angular's website</title>
  </head>
  <body>
    <h2>Todo</h2>
    <div ng-controller=""TodoCtrl"">
      <span>{{remaining()}} of {{todos.length}} remaining</span>
      [ <a href="""" ng-click=""archive()"">archive</a> ]
      <ul class=""unstyled"">
        <li ng-repeat=""todo in todos"">
          <input type=""checkbox"" ng-model=""todo.done"">
          <span class=""done-{{todo.done}}"">{{todo.text}}</span>
        </li>
      </ul>
      <form ng-submit=""addTodo()"">
        <input type=""text"" ng-model=""todoText""  size=""30""
               placeholder=""add new todo here"">
        <input class=""btn-primary"" type=""submit"" value=""add"">
      </form>
    </div>
  </body>
</html>";
            var tidy = new Tidy();
            tidy.InitOptions();
            tidy.Options.DocType = DocType.Html5;
            var messages = new TidyMessageCollection();

            //act
            tidy.Parse(html, messages);

            //assert
            Assert.AreEqual(0, messages.WarningMessages.Count, "Should be valid Html5 with angular");
        }
    }
}
