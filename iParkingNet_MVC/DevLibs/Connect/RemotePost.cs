using System.Collections.Specialized;
using System.Web;


/// <summary>
/// Represents a RemotePost helper class
/// </summary>
public partial class RemotePost
{
    private readonly HttpContext httpContext;
    private NameValueCollection valuePair = new NameValueCollection();

    /// <summary>
    /// Gets or sets a remote URL
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets a method
    /// </summary>
    public string Method { get; set; }

    /// <summary>
    /// Gets or sets a form name
    /// </summary>
    public string FormName { get; set; }

    /// <summary>
    /// Gets or sets a form character-sets the server can handle for form-data.
    /// </summary>
    public string AcceptCharset { get; set; }



    /// <summary>
    /// Creates a new instance of the RemotePost class
    /// </summary>
    public RemotePost() : this(HttpContext.Current)
    {
    }

    /// <summary>
    /// Creates a new instance of the RemotePost class
    /// </summary>
    /// <param name="httpContext">HTTP Context</param>
    /// <param name="webHelper">Web helper</param>
    public RemotePost(HttpContext httpContext)
    {
        this.Url = "http://www.someurl.com";
        this.Method = "post";
        this.FormName = "formName";
        this.httpContext = httpContext;
    }

    /// <summary>
    /// Adds the specified key and value to the dictionary (to be posted).
    /// </summary>
    /// <param name="name">The key of the element to add</param>
    /// <param name="value">The value of the element to add.</param>
    public void Add(string name, string value)
    {
        valuePair.Add(name, value);
    }

    /// <summary>
    /// Post
    /// </summary>
    public void Post()
    {
        httpContext.Response.Clear();
        httpContext.Response.Write("<html><head></head>");
        httpContext.Response.Write(string.Format("<body onload=\"document.{0}.submit()\">", FormName));
        if (!string.IsNullOrEmpty(AcceptCharset))
        {
            //AcceptCharset specified
            httpContext.Response.Write(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" accept-charset=\"{3}\">", FormName, Method, Url, AcceptCharset));
        }
        else
        {
            //no AcceptCharset specified
            httpContext.Response.Write(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >", FormName, Method, Url));
        }

        for (int i = 0; i < valuePair.Keys.Count; i++)
            httpContext.Response.Write(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", HttpUtility.HtmlEncode(valuePair.Keys[i]), HttpUtility.HtmlEncode(valuePair[valuePair.Keys[i]])));

        httpContext.Response.Write("</form>");
        httpContext.Response.Write("</body></html>");
        httpContext.Response.End();
    }
}