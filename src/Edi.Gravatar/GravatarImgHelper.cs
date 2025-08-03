using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Edi.Gravatar;

[HtmlTargetElement("gravatar", TagStructure = TagStructure.NormalOrSelfClosing)]
public class GravatarImgHelper : TagHelper
{
    private const string GravatarBaseUrl = "gravatar.com/avatar/";
    private const string DefaultRating = "g";

    public string? Email { get; set; }

    public int Size { get; set; } = 58;

    public string DefaultImageUrl { get; set; } = string.Empty;

    public bool PreferHttps { get; set; } = true;

    public bool ForceDefaultImage { get; set; }

    public string Alt { get; set; } = "Gravatar image";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        var normalizedEmail = NormalizeEmail(Email);
        var emailHash = GetMd5Hash(normalizedEmail);
        var gravatarUrl = BuildGravatarUrl(emailHash);

        output.TagName = "img";
        output.Attributes.SetAttribute("src", gravatarUrl);
        output.Attributes.SetAttribute("alt", Alt);
        
        // Add loading attribute for better performance
        output.Attributes.SetAttribute("loading", "lazy");
    }

    private static string NormalizeEmail(string? email)
    {
        return string.IsNullOrWhiteSpace(email) ? string.Empty : email.Trim().ToLowerInvariant();
    }

    private string BuildGravatarUrl(string emailHash)
    {
        var protocol = PreferHttps ? "https" : "http";
        var subdomain = PreferHttps ? "secure" : "www";
        
        var urlBuilder = new StringBuilder()
            .Append($"{protocol}://{subdomain}.{GravatarBaseUrl}{emailHash}")
            .Append($"?s={Size}")
            .Append($"&r={DefaultRating}");

        if (!string.IsNullOrWhiteSpace(DefaultImageUrl))
        {
            urlBuilder.Append("&d=").Append(UrlEncoder.Default.Encode(DefaultImageUrl));
        }

        if (ForceDefaultImage)
        {
            urlBuilder.Append("&f=y");
        }

        return urlBuilder.ToString();
    }

    private static string GetMd5Hash(string input)
    {
        var hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}