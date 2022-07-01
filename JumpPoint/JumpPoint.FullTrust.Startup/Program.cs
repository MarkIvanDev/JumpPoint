using System.Diagnostics;

Process.Start(new ProcessStartInfo
{
    FileName =
#if BETA
        "jumppoint-beta://",
#else
        "jumppoint://"
#endif
    UseShellExecute = true
});