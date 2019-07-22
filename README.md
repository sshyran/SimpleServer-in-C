# Deprecation Notice
SimpleServer has been superseded by Project Spirit, also made by Ultz. Please use that instead.

- 4pm UTC+1, 22nd July 2019

# SimpleServer
SimpleServer is new server environment written in C# for .NET Standard. We aim to make SimpleServer an easy-to-use platform in which developers can make advanced web applications on.

Our core values for SimpleServer are:
- **Be Expandable**: SimpleServer can be shaped to do anything as we make abstractions at the lowest level of request handling, so if you wanted to you could implement a completely unrelated protocol to HTTP(S).
- **Be Efficient**: We want SimpleServer to use system resources as sparingly as possible, so running an application built on top of SimpleServer is as cheap as possible.
- **Be Simple**: We're shaping SimpleServer so that developers can integrate it easily, but can hack at it to great extents if they felt like it.

Current features include:
- [HTTP/2](https://github.com/Matthias247/http2dotnet) support (with upgrading)
- SSL support
- Virtual Hosts
