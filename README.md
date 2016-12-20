# THE SharpDCIIDE is inspired by Trygve Reenskaug and Jim Coplien DCI Architecture

- http://en.wikipedia.org/wiki/Trygve_Reenskaug
- http://en.wikipedia.org/wiki/Jim_Coplien
- http://fulloo.info/Examples/SqueakExamples/index.html

The IDE is inspired by Josh Varty CodeConnect and MS DebugCanvas
http://joshvarty.wordpress.com/2014/08/01/ripping-the-visual-studio-editor-apart-with-projection-buffers/comment-page-1/#comment-115

Tools: 
- GraphSharp: https://graphsharp.codeplex.com/
- https://joshsmithonwpf.wordpress.com/
- NJasmine: https://github.com/fschwiet/DreamNJasmine
 
Screenshots:
- http://goo.gl/qfh5uI
- http://goo.gl/8TjkXb

How to use:

Build the extention

1) Open the project in Visual Studio 2015 Community version

2) Build release/debug version

3) Open bin folder and run TheDCIBabyIDE.vsix to install the extension

4) Restart Visual studio, open the project again 

5) Open file with postfix _Context.cs (eg ContextFileOpeningContext.cs)

6) Right click in the editor and select 'open in DCI Baby IDE'


Debug:

1) Open the project in Visual Studio 2013 Community Version

2) Project settings:
-Start external program:
C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe
-Cmd line arguments:
/rootsuffix Exp /log

3) F5 to run and debug
