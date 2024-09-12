using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Edi.Gravatar;

[HtmlTargetElement("gravatar", TagStructure = TagStructure.NormalOrSelfClosing)]
public class GravatarImgHelper : TagHelper
{
    public string Email { get; set; }

    public int Size { get; set; } = 58;

    public string DefaultImageUrl { get; set; } = string.Empty;

    public bool PreferHttps { get; set; } = true;

    public bool ForceDefaultImage { get; set; }

    public string Alt { get; set; } = "Gravatar image";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var email = string.IsNullOrEmpty(Email) ? string.Empty : Email.Trim().ToLower();
        var emailHash = GetMd5Hash(email);

        var src = string.Format("{0}://{1}.gravatar.com/avatar/{2}?s={3}{4}{5}{6}",
            PreferHttps ? "https" : "http",
            PreferHttps ? "secure" : "www",
            emailHash,
            Size.ToString(),
            "&d=" + (!string.IsNullOrEmpty(DefaultImageUrl)
                ? UrlEncoder.Default.Encode(DefaultImageUrl)
                : string.Empty),
            ForceDefaultImage ? "&f=y" : string.Empty,
            "&r=g");

        output.TagName = "img";
        output.Attributes.SetAttribute("src", src);
        output.Attributes.SetAttribute("alt", Alt);
    }

    private static string GetMd5Hash(string input)
    {
        var data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
        var sBuilder = new StringBuilder();

        foreach (var t in data)
        {
            sBuilder.Append(t.ToString("x2"));
        }

        return sBuilder.ToString();
    }
}