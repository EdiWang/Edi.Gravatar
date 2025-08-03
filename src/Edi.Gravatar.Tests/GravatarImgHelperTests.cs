using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace Edi.Gravatar.Tests;

[TestClass]
public class GravatarImgHelperTests
{
    private GravatarImgHelper _tagHelper;
    private TagHelperContext _context;
    private TagHelperOutput _output;

    [TestInitialize]
    public void Setup()
    {
        _tagHelper = new GravatarImgHelper();
        _context = new TagHelperContext(
            tagName: "gravatar",
            allAttributes: [],
            items: new Dictionary<object, object>(),
            uniqueId: "test");
        
        _output = new TagHelperOutput(
            tagName: "gravatar",
            attributes: [],
            getChildContentAsync: (useCachedResult, encoder) => 
                Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
    }

    [TestMethod]
    public void Process_WithValidEmail_GeneratesCorrectImageTag()
    {
        // Arrange
        _tagHelper.Email = "test@example.com";

        // Act
        _tagHelper.Process(_context, _output);

        // Assert
        Assert.AreEqual("img", _output.TagName);
        Assert.AreEqual("Gravatar image", _output.Attributes["alt"].Value);
        Assert.AreEqual("lazy", _output.Attributes["loading"].Value);
        
        var srcValue = _output.Attributes["src"].Value?.ToString();
        Assert.IsNotNull(srcValue);
        Assert.Contains("https://secure.gravatar.com/avatar/", srcValue);
        Assert.Contains("s=58", srcValue); // Default size
        Assert.Contains("r=g", srcValue); // Default rating
    }

    [TestMethod]
    public void Process_WithNullEmail_GeneratesImageTagWithEmptyEmailHash()
    {
        // Arrange
        _tagHelper.Email = null;

        // Act
        _tagHelper.Process(_context, _output);

        // Assert
        var srcValue = _output.Attributes["src"].Value?.ToString();
        Assert.IsNotNull(srcValue);
        // MD5 hash of empty string is d41d8cd98f00b204e9800998ecf8427e
        Assert.Contains("d41d8cd98f00b204e9800998ecf8427e", srcValue);
    }

    [TestMethod]
    public void Process_WithWhitespaceEmail_GeneratesImageTagWithEmptyEmailHash()
    {
        // Arrange
        _tagHelper.Email = "   ";

        // Act
        _tagHelper.Process(_context, _output);

        // Assert
        var srcValue = _output.Attributes["src"].Value?.ToString();
        Assert.IsNotNull(srcValue);
        // MD5 hash of empty string is d41d8cd98f00b204e9800998ecf8427e
        Assert.Contains("d41d8cd98f00b204e9800998ecf8427e", srcValue);
    }

    [TestMethod]
    public void Process_WithMixedCaseEmail_NormalizesToLowerCase()
    {
        // Arrange
        _tagHelper.Email = "TEST@EXAMPLE.COM";

        // Act
        _tagHelper.Process(_context, _output);

        // Assert
        var srcValue = _output.Attributes["src"].Value?.ToString();
        Assert.IsNotNull(srcValue);
        // Should contain the same hash as "test@example.com"
        Assert.Contains("55502f40dc8b7c769880b10874abc9d0", srcValue);
    }

    [TestMethod]
    public void Process_WithEmailWithSpaces_TrimsSpaces()
    {
        // Arrange
        _tagHelper.Email = "  test@example.com  ";

        // Act
        _tagHelper.Process(_context, _output);

        // Assert
        var srcValue = _output.Attributes["src"].Value?.ToString();
        Assert.IsNotNull(srcValue);
        // Should contain the same hash as "test@example.com"
        Assert.Contains("55502f40dc8b7c769880b10874abc9d0", srcValue);
    }

    [TestMethod]
    public void Process_WithCustomSize_IncludesSizeInUrl()
    {
        // Arrange
        _tagHelper.Email = "test@example.com";
        _tagHelper.Size = 128;

        // Act
        _tagHelper.Process(_context, _output);

        // Assert
        var srcValue = _output.Attributes["src"].Value?.ToString();
        Assert.IsNotNull(srcValue);
        Assert.Contains("s=128", srcValue);
    }

    [TestMethod]
    public void Process_WithCustomAlt_SetsAltAttribute()
    {
        // Arrange
        _tagHelper.Email = "test@example.com";
        _tagHelper.Alt = "Custom alt text";

        // Act
        _tagHelper.Process(_context, _output);

        // Assert
        Assert.AreEqual("Custom alt text", _output.Attributes["alt"].Value);
    }

    [TestMethod]
    public void Process_WithDefaultImageUrl_IncludesEncodedUrlInGravatarUrl()
    {
        // Arrange
        _tagHelper.Email = "test@example.com";
        _tagHelper.DefaultImageUrl = "https://example.com/default-avatar.png";

        // Act
        _tagHelper.Process(_context, _output);

        // Assert
        var srcValue = _output.Attributes["src"].Value?.ToString();
        Assert.IsNotNull(srcValue);
        var expectedEncodedUrl = UrlEncoder.Default.Encode("https://example.com/default-avatar.png");
        Assert.Contains($"&d={expectedEncodedUrl}", srcValue);
    }

    [TestMethod]
    public void Process_WithForceDefaultImage_IncludesForceParameter()
    {
        // Arrange
        _tagHelper.Email = "test@example.com";
        _tagHelper.ForceDefaultImage = true;

        // Act
        _tagHelper.Process(_context, _output);

        // Assert
        var srcValue = _output.Attributes["src"].Value?.ToString();
        Assert.IsNotNull(srcValue);
        Assert.Contains("&f=y", srcValue);
    }

    [TestMethod]
    public void Process_WithPreferHttpsFalse_UsesHttpProtocol()
    {
        // Arrange
        _tagHelper.Email = "test@example.com";
        _tagHelper.PreferHttps = false;

        // Act
        _tagHelper.Process(_context, _output);

        // Assert
        var srcValue = _output.Attributes["src"].Value?.ToString();
        Assert.IsNotNull(srcValue);
        Assert.StartsWith("http://www.gravatar.com/avatar/", srcValue);
    }

    [TestMethod]
    public void Process_WithPreferHttpsTrue_UsesHttpsProtocol()
    {
        // Arrange
        _tagHelper.Email = "test@example.com";
        _tagHelper.PreferHttps = true;

        // Act
        _tagHelper.Process(_context, _output);

        // Assert
        var srcValue = _output.Attributes["src"].Value?.ToString();
        Assert.IsNotNull(srcValue);
        Assert.StartsWith("https://secure.gravatar.com/avatar/", srcValue);
    }

    [TestMethod]
    public void Process_WithAllOptions_GeneratesCompleteUrl()
    {
        // Arrange
        _tagHelper.Email = "test@example.com";
        _tagHelper.Size = 256;
        _tagHelper.DefaultImageUrl = "https://example.com/default.jpg";
        _tagHelper.ForceDefaultImage = true;
        _tagHelper.PreferHttps = false;
        _tagHelper.Alt = "Test avatar";

        // Act
        _tagHelper.Process(_context, _output);

        // Assert
        var srcValue = _output.Attributes["src"].Value?.ToString();
        Assert.IsNotNull(srcValue);
        
        Assert.StartsWith("http://www.gravatar.com/avatar/", srcValue);
        Assert.Contains("55502f40dc8b7c769880b10874abc9d0", srcValue); // MD5 of test@example.com
        Assert.Contains("s=256", srcValue);
        Assert.Contains("r=g", srcValue);
        Assert.Contains("&f=y", srcValue);
        
        var expectedEncodedUrl = UrlEncoder.Default.Encode("https://example.com/default.jpg");
        Assert.Contains($"&d={expectedEncodedUrl}", srcValue);
        
        Assert.AreEqual("Test avatar", _output.Attributes["alt"].Value);
        Assert.AreEqual("lazy", _output.Attributes["loading"].Value);
    }

    [TestMethod]
    public void Process_WithNullContext_ThrowsArgumentNullException()
    {
        // Arrange
        _tagHelper.Email = "test@example.com";

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => _tagHelper.Process(null!, _output));
    }

    [TestMethod]
    public void Process_WithNullOutput_ThrowsArgumentNullException()
    {
        // Arrange
        _tagHelper.Email = "test@example.com";

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => _tagHelper.Process(_context, null!));
    }

    [TestMethod]
    public void Process_DefaultValues_AreSetCorrectly()
    {
        // Arrange & Act
        var tagHelper = new GravatarImgHelper();

        // Assert
        Assert.AreEqual(58, tagHelper.Size);
        Assert.AreEqual(string.Empty, tagHelper.DefaultImageUrl);
        Assert.IsTrue(tagHelper.PreferHttps);
        Assert.IsFalse(tagHelper.ForceDefaultImage);
        Assert.AreEqual("Gravatar image", tagHelper.Alt);
        Assert.IsNull(tagHelper.Email);
    }

    [TestMethod]
    public void Process_AlwaysAddsLoadingLazyAttribute()
    {
        // Arrange
        _tagHelper.Email = "test@example.com";

        // Act
        _tagHelper.Process(_context, _output);

        // Assert
        Assert.AreEqual("lazy", _output.Attributes["loading"].Value);
    }

    [TestMethod]
    public void Process_GeneratesKnownMd5Hash()
    {
        // Arrange
        _tagHelper.Email = "MyEmailAddress@example.com";

        // Act  
        _tagHelper.Process(_context, _output);

        // Assert
        var srcValue = _output.Attributes["src"].Value?.ToString();
        Assert.IsNotNull(srcValue);
        // MD5 hash of "myemailaddress@example.com" (normalized)
        Assert.Contains("0bc83cb571cd1c50ba6f3e8a78ef1346", srcValue);
    }
}