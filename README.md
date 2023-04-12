# BadApplePrompt
![Platform](https://img.shields.io/badge/platform-windows-lightgrey)
![GitHub](https://img.shields.io/github/license/Labhatorian/BadApplePrompt)
![GitHub repo size](https://img.shields.io/github/repo-size/Labhatorian/BadApplePrompt)
![GitHub last commit](https://img.shields.io/github/last-commit/Labhatorian/BadApplePrompt)

Inspired by countless videos of [Bad Apple](https://www.youtube.com/watch?v=FtutLA63Cp8) being played on any device imaginable. Play black and white videos right into your command prompt/terminal with audio support.<br>

## Setup
Setup for BadApplePrompt is easy:
1. Create a folder anywhere
2. Drop `BadApple.exe` into it
3. [Download](https://ffmpeg.org/about.html) and drop `FFmpeg` and `FFprobe` into the folder
4. Open `BadApple.exe`, select and open a video

### Notes
Please note that while BadApplePrompt is fairly stable, you can mess up the playback by minimising, maximising and changing the size of the window. In addition, closing BadApplePrompt might not always delete the entirity of the hidden `/temp` folder located in the same folder as `BadApple.exe`.

## Arguments and options
You can run this application with arguments through a terminal window or in shortcut inside the target field. All the options that can be found in the Settings screen can be set up using arguments. The following arguments/options are available:

| Argument                             | Explanation                                                                 | Default |
|--------------------------------------|-----------------------------------------------------------------------------|---------|
| (FilePath)*                          | Required as first argument, path to videofile                               | N/A     |
| -FileExtension "(Name of extension)" | Choose which file extension will be used when the videoframes get extracted | JPEG    |
| -Factor (Number)                     | Choose by which factor the frames will be sized down for viewing            | 4       |
| -Verbose                             | Enable verbose mode to show FFmpeg and FFprobe output                       | False   |
| -AutoStart                           | Automatically start playing the video                                       | False   |
| -Resize                              | Disable resizing the videoframes                                            | True    |
| -ConvertBlackWhite                   | Convert the videoframes to grayscale as this *might* improve viewing.       | False   |
| -FPSCounter                          | Disable the FPS counter when playing the video                              | True    |

## Afterword
This was a fun 'little' side project that I decided to do when I first started learning C# that got out of control. I admit that C# is not the language for this and that C++ might have been better. I had to unfortunately drop colour support as `Console.Write()` is the fastest way to write to console without dealing with buffers from `StreamWriter` or corrupted garbage data from `WriteConsoleW()`.

Copyright &copy; 2023 Labhatorian<br>
Dit werk is gelicentieerd onder de GNU General Public License v3.0.
