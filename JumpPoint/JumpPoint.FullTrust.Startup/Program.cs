using System.Diagnostics;

File.AppendAllText(@"D:\jumppoint-error-showwindow.txt", $"{args.Length} Args: {args}");
Process.Start(new ProcessStartInfo
{
    FileName =
#if JPBETA
        "jumppoint-beta://",
#else
        "jumppoint://",
#endif
    UseShellExecute = true
});