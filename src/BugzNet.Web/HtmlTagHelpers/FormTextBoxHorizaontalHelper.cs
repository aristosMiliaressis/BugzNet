using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.IO;

namespace BugzNet.Web.HtmlTagHelpers
{
    [HtmlTargetElement("form-field-horizontal")]
    public class FormTextBoxHorizaontalHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        [HtmlAttributeName("Name")]
        public string Name { get; set; }

        private readonly IHtmlGenerator _generator;

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public FormTextBoxHorizaontalHelper(IHtmlGenerator generator)
        {
            _generator = generator;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            using (var writer = new StringWriter())
            {
                var label = _generator.GenerateLabel(
                             ViewContext,
                             For.ModelExplorer,
                             For.Name, null,
                             new { @class = "label" });

                var textArea = _generator.GenerateTextBox(ViewContext,
                                  For.ModelExplorer,
                                  string.IsNullOrEmpty(Name) ? For.Name : Name,
                                  For.Model,
                                  null,
                                  new { @class = "input is-info" });

                var validationMsg = _generator.GenerateValidationMessage(
                                      ViewContext,
                                      For.ModelExplorer,
                                      For.Name,
                                      null,
                                      ViewContext.ValidationMessageElement,
                                      new { @class = "is-danger" });

                writer.Write(@"<div class=""field is-horizontal"">"); //Main Div field horizontal
                writer.Write(@"<div class=""field-label is-normal"">"); //Label Div
                label.WriteTo(writer, NullHtmlEncoder.Default);
                writer.Write(@"</div>"); // --- close label div
                writer.Write(@"<div class=""field-body"">"); //Field body
                writer.Write(@"<div class=""field"">"); //Field        
                textArea.WriteTo(writer, NullHtmlEncoder.Default);
                writer.Write(@"</div>");// --- close filed
                writer.Write(@"</div>");// --- close Field body
                //validationMsg.WriteTo(writer, NullHtmlEncoder.Default);
                writer.Write(@"</div>");// --- close Main Div

                output.Content.SetHtmlContent(writer.ToString());

            }

        }
    }
}
