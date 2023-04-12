# BadAppleCMD
BadApplePrompt utulises pInvoke to disable maximising, minimising and Quick-Edit in the console for a better experience. Unfortunatley, it can be unreliable as it 
sometimes will not work. The code to disable and enable the close button had to be removed as it was simply not working.
It is recommended you keep the console window open at all times when using it to prevent errors like these

## Options
There is an option to convert to grayscale as this *might* improve viewing.

## Afterword
This was a fun little side project that I decided to do when I first started learning C#. However, I admit that C# is not the language for this and that C++
might have been better due. As Console.Write() is the fastest way to write to console without dealing with buffers from StreamWriter or 
corrupted and garbage data from WriteConsoleW. This means 60FPS and colour support is not possible.