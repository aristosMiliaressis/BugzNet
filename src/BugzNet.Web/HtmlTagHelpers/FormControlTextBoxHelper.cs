using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.IO;

namespace BugzNet.Web.HtmlTagHelpers
{
    [HtmlTargetElement("form-control-text-box")]
    public class FormControlTextBoxHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        [HtmlAttributeName("Name")]
        public string Name { get; set; }

        [HtmlAttributeName("Readonly")]
        public string Readonly { get; set; }

        private readonly IHtmlGenerator _generator;

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public FormControlTextBoxHelper(IHtmlGenerator generator)
        {
            _generator = generator;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            using (var writer = new StringWriter())
            {
                writer.Write(@"<div class=""field"">");

                var label = _generator.GenerateLabel(
                                ViewContext,
                                For.ModelExplorer,
                                For.Name, null,
                                new { @class = "label" });

                label.WriteTo(writer, NullHtmlEncoder.Default);

                object htmlAttr = ((Readonly == null ? false : Readonly.ToLower().Equals("true"))
                         ? (object) new { @class = "input", @readonly = true }
                         : (object) new { @class = "input" });

                var textArea = _generator.GenerateTextBox(ViewContext,
                                    For.ModelExplorer,
                                    string.IsNullOrEmpty(Name) ? For.Name : Name,
                                    For.Model,
                                    null,
                                    htmlAttr);

                textArea.WriteTo(writer, NullHtmlEncoder.Default);

                var validationMsg = _generator.GenerateValidationMessage(
                                        ViewContext,
                                        For.ModelExplorer,
                                        For.Name,
                                        null,
                                        ViewContext.ValidationMessageElement,
                                        new { @class = "is-danger" });

                validationMsg.WriteTo(writer, NullHtmlEncoder.Default);

                writer.Write(@"</div>");

                output.Content.SetHtmlContent(writer.ToString());
            }
        }
    }
}
