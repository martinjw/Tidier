using System.Collections.Generic;

namespace TidyNet
{
    internal class InputCheckAttribs : ICheckAttribs
    {
        public void Check(Lexer lexer, Node node)
        {
            node.CheckUniqueAttributes(lexer);

            var type = node.GetAttrByName("type");
            if (type == null) return;
            var typeValue = type.Val.ToLowerInvariant();
            if (string.IsNullOrEmpty(typeValue)) return; //implicitly "text"
            var values = new List<string>
                         {
                             "button",
                             "checkbox",
                             "color",
                             "date",
                             "datetime",
                             "datetime-local",
                             "email",
                             "file",
                             "hidden",
                             "image",
                             "month",
                             "number",
                             "password",
                             "radio",
                             "range",
                             "reset",
                             "search",
                             "submit",
                             "tel",
                             "text",
                             "time",
                             "url",
                             "week",
                         };

            if (!values.Contains(typeValue))
            {
                Report.AttrError(lexer, node, "type", Report.UNKNOWN_INPUT_TYPE);
            }
        }
    }
}