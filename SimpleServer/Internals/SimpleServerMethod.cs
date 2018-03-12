using System.Collections.Generic;

namespace SimpleServer.Internals
{
    public class SimpleServerMethod
    {
        public static SimpleServerMethod Get
        {
            get { return new SimpleServerMethod() {Name = "GET", HasInputStream = false}; }
        }

        public static SimpleServerMethod Post
        {
            get { return new SimpleServerMethod() {Name = "POST", HasInputStream = true}; }
        }

        public static SimpleServerMethod Put
        {
            get { return new SimpleServerMethod() {Name = "PUT", HasInputStream = true}; }
        }

        public static SimpleServerMethod Patch
        {
            get { return new SimpleServerMethod() {Name = "PATCH", HasInputStream = true}; }
        }

        public static SimpleServerMethod Delete
        {
            get { return new SimpleServerMethod() {Name = "DELETE", HasInputStream = false}; }
        }

        public static SimpleServerMethod Head
        {
            get { return new SimpleServerMethod() {Name = "HEAD", HasInputStream = false}; }
        }

        public static SimpleServerMethod Options
        {
            get { return new SimpleServerMethod() {Name = "OPTIONS", HasInputStream = false}; }
        }

        public static SimpleServerMethod Trace
        {
            get { return new SimpleServerMethod() {Name = "TRACE", HasInputStream = false}; }
        }

        public static SimpleServerMethod Connect
        {
            get { return new SimpleServerMethod() {Name = "CONNECT", HasInputStream = false}; }
        }

        public static readonly ICollection<SimpleServerMethod> DefaultMethods =
            new List<SimpleServerMethod>() {Get, Head, Post, Put, Delete, Options, Trace, Patch, Connect}.AsReadOnly();

        public string Name { get; set; }
        public bool HasInputStream { get; set; }
    }
}