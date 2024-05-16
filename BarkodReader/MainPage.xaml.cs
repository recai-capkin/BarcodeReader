using Timer = System.Timers.Timer;
using ZXing.Net.Maui;

namespace BarkodReader;

public partial class MainPage : ContentPage
{
    private HashSet<string> processedBarcodes;
    private Timer resetTimer;
    public MainPage()
	{
		InitializeComponent();
        processedBarcodes = new HashSet<string>();
        resetTimer = new Timer(5000); // 5 saniye bekleme süresi
        resetTimer.Elapsed += ResetScanner;
        resetTimer.AutoReset = false;
    }

    public void OnBarcodeDetected(object sender, BarcodeDetectionEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            foreach (var barcode in e.Results)
            {
                if (!processedBarcodes.Contains(barcode.Value))
                {
                    processedBarcodes.Add(barcode.Value);
                    Console.WriteLine($"Barcodes: {barcode.Format} -> {barcode.Value}");
                    DisplayAlert("Barkod Algılandı", $"Barkod: {barcode.Value}", "OK");

                    // Tarayıcıyı devre dışı bırak ve zamanlayıcıyı başlat
                    cameraBarcodeReaderView.IsDetecting = false;
                    resetTimer.Start();
                }
            }
        });
    }
    private void ResetScanner(object sender, System.Timers.ElapsedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            processedBarcodes.Clear();
            cameraBarcodeReaderView.IsDetecting = true;
        });
    }
}

