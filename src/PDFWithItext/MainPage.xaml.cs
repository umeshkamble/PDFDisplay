using System.Net;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using Image = iText.Layout.Element.Image;

namespace PDFWithItext;

public partial class MainPage : ContentPage
{
    string filePath = string.Empty;

    public MainPage()
    {
        InitializeComponent();
        Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("webview", (handler, View) =>
        {
#if ANDROID
            handler.PlatformView.Settings.AllowFileAccess = true;
            handler.PlatformView.Settings.AllowFileAccessFromFileURLs = true;
            handler.PlatformView.Settings.AllowUniversalAccessFromFileURLs = true;
#endif
        });
    }


    private async void OnGenaratePdfClicked(object sender, EventArgs e)
    {
        try
        {
            string fileName = "mauidotnet.pdf";

            filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);

            PdfWriter writer = new PdfWriter(filePath);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);
            Paragraph header = new Paragraph("MAUI PDF Sample")
               .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
               .SetFontSize(20);

            document.Add(header);
            Paragraph subheader = new Paragraph("Welcome to .NET Multi-platform App UI")
               .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
               .SetFontSize(15);
            document.Add(subheader);
            LineSeparator ls = new LineSeparator(new SolidLine());
            document.Add(ls);
            var imgStream = await ConvertImageSourceToStreamAsync("dotnet_bot.png");
            Image image = new Image(ImageDataFactory
                .Create(imgStream))
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);

            document.Add(image);

            Paragraph footer = new Paragraph("Created By \nUmesh")
              .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)
              .SetFontColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY)
              .SetFontSize(14);

            document.Add(footer);
            document.Close();

            string url = DeviceInfo.Platform == DevicePlatform.Android ? $"file:///android_asset/pdfjs/web/viewer.html?file={"file:///" + WebUtility.UrlEncode(filePath)}" : string.Empty;
            webprint.Source = DeviceInfo.Platform == DevicePlatform.Android ? url : filePath;

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
        }
    }

    void OnLoadPDFClicked(System.Object sender, System.EventArgs e)
    {
        try
        {
            string url = string.Empty;
#if ANDROID
            string path = string.Format("file:///android_asset/{0}", WebUtility.UrlEncode("applifecycle.pdf"));
             url = string.Format("file:///android_asset/pdfjs/web/viewer.html?file={0}", path);
#else
            url = LoadAssets("applifecycle.pdf");
#endif
            webprint.Source = url;

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
        }
    }

    
    async void OnExternarlBrowserClicked(System.Object sender, System.EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath))
            {
                await DisplayAlert("Alert", "File is exist!!, Please Genarate PDF", "OK");
                return;
            }
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
        }
    }

    public string LoadAssets(string path)
    {

#if IOS || MACCATALYST
        string directory = Path.GetDirectoryName(path);
        string filename = Path.GetFileNameWithoutExtension(path);
        string extension = Path.GetExtension(path).Substring(1);
        string url = Foundation.NSBundle.MainBundle.GetUrlForResource(filename, extension, directory).FilePathUrl.Path;
        return url;
#else
        return string.Empty;
#endif

    }

    public async Task<byte[]> ConvertImageSourceToStreamAsync(string imageName)
    {
        using var ms = new MemoryStream();
        using (var stream = await FileSystem.OpenAppPackageFileAsync(imageName))
            await stream.CopyToAsync(ms);
        return ms.ToArray();
    }
}


