# Third-party notices

## Included in CurveReaderConfigurator.exe

CurveReaderConfigurator embeds the following component in its executable.

### Newtonsoft.Json 13.0.4

The MIT License (MIT)

Copyright (c) 2007 James Newton-King

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

## Build-time tools

CurveReaderConfigurator is built with the following MIT-licensed tools:

- Costura.Fody 6.2.0 (embeds references into the executable) - https://github.com/Fody/Costura
- Fody 6.9.3 - https://github.com/Fody/Fody

## Not included in this repository or its releases

FanControl.CurveReader.dll is loaded by Fan Control, which provides the
libraries it compiles against: FanControl.Plugins, FanControl.IPC,
Google.Protobuf (BSD-3-Clause), Grpc.Core.Api (Apache-2.0) and
Newtonsoft.Json (MIT). They remain under their own licenses.
