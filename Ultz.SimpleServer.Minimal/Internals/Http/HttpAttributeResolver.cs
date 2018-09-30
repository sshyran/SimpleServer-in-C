// HttpAttributeResolver.cs - Ultz.SimpleServer.Minimal
// 
// Copyright (C) 2018 Ultz Limited
// 
// This file is part of SimpleServer.
// 
// SimpleServer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// SimpleServer is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with SimpleServer. If not, see <http://www.gnu.org/licenses/>.

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Ultz.SimpleServer.Common;
using Ultz.SimpleServer.Handlers;

#endregion

namespace Ultz.SimpleServer.Internals.Http
{
    /// <inheritdoc />
    public class HttpAttributeResolver : IAttributeHandlerResolver
    {
        /// <summary>
        ///     A list of
        /// </summary>
        public List<Type> Attributes { get; } = new List<Type>
        {
            typeof(HttpGetAttribute),
            typeof(HttpPostAttribute),
            typeof(HttpPutAttribute),
            typeof(HttpPatchAttribute),
            typeof(HttpDeleteAttribute),
            typeof(HttpTraceAttribute),
            typeof(HttpConnectAttribute),
            typeof(HttpHeadAttribute),
            typeof(HttpOptionsAttribute)
        };

        /// <inheritdoc />
        public IEnumerable<IHandler> GetHandlers(object instance)
        {
            var type = instance.GetType();
            var methods = type.GetTypeInfo().DeclaredMethods;
            foreach (var method in methods)
                if (method.ReturnType == typeof(void))
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == 0)
                        continue;
                    if (parameters.Length == 1 && parameters[0].ParameterType == typeof(HttpContext))
                        foreach (var attr in Attributes.Where(x =>
                            typeof(HttpAttribute).GetTypeInfo().IsAssignableFrom(x.GetTypeInfo())))
                        foreach (var attribute in method.GetCustomAttributes(attr))
                            yield return new HttpAttributeHandler(method, (HttpAttribute) attribute, instance);
                    else
                        foreach (var attr in Attributes)
                        foreach (var attribute in method.GetCustomAttributes(attr))
                        {
                            var selectedParams = parameters.ToDictionary(x => x.Name, x => x.ParameterType);
                            var regex = new Regex(selectedParams.Keys.Aggregate(
                                    ((HttpAttribute) attribute).Route.Replace("/", "\\/").Replace("[", "\\[")
                                    .Replace("^", "\\^").Replace("$", "\\$").Replace(".", "\\.").Replace("|", "\\|")
                                    .Replace("?", "\\?").Replace("*", "\\*").Replace("+", "\\+").Replace("(", "\\(")
                                    .Replace(")", "\\)"),
                                    (current, argument) =>
                                        current.Replace("{" + argument + "}", "(?<" + argument + ">.*)"))
                                .Replace("{", "\\{").Replace("}", "\\}"));
                            yield return new HttpAttributeHandler(method, (HttpAttribute) attribute, instance, regex);
                        }
                }
        }
    }

    internal class HttpAttributeHandler : IHandler
    {
        private readonly HttpAttribute _attribute;
        private readonly MethodInfo _handler;
        private readonly object _instance;
        private readonly Regex _regex;

        public HttpAttributeHandler(MethodInfo methodInfo, HttpAttribute attribute, object instance, Regex regex = null)
        {
            _attribute = attribute;
            _handler = methodInfo;
            _instance = instance;
            _regex = regex;
        }

        public bool CanHandle(IRequest request)
        {
            if (_regex != null)
                return _regex.IsMatch(_attribute is HttpConnectAttribute
                           ? ((HttpRequest) request).RawUrl
                           : ((HttpRequest) request).RawUrl.ToUrlFormat()) &&
                       _attribute.Method == (HttpMethod) request.Method;

            if (_attribute is HttpConnectAttribute)
                return ((HttpRequest) request).RawUrl == _attribute.Route;
            return ((HttpRequest) request).RawUrl.ToUrlFormat() == _attribute.Route &&
                   _attribute.Method == (HttpMethod) request.Method;
        }

        public void Handle(IContext context)
        {
            if (_regex != null)
            {
                var parameters = _handler.GetParameters().ToDictionary(x => x.Name, x => x.ParameterType);
                var match = _regex.Match(((HttpRequest) context.Request).RawUrl.ToUrlFormat());
                var invocationArgs = new List<object>();
                for (var i = 0; i < parameters.Count; i++) // foreach var param in parameters
                {
                    var param = parameters.ElementAt(i);
                    if (_regex.GetGroupNames().Contains(param.Key))
                    {
                        if (param.Value == typeof(string))
                        {
                            invocationArgs.Add(match.Groups[param.Key].Value);
                        }
                        else if (param.Value == typeof(byte[]))
                        {
                            invocationArgs.Add(Encoding.UTF8.GetBytes(match.Groups[param.Key].Value));
                        }
                        else if (param.Value == typeof(int))
                        {
                            if (int.TryParse(match.Groups[param.Key].Value, out var value))
                                invocationArgs.Add(value);
                            else
                                invocationArgs.Add(null);
                        }
                        else if (param.Value == typeof(long))
                        {
                            if (long.TryParse(match.Groups[param.Key].Value, out var value))
                                invocationArgs.Add(value);
                            else
                                invocationArgs.Add(null);
                        }
                        else if (param.Value == typeof(uint))
                        {
                            if (uint.TryParse(match.Groups[param.Key].Value, out var value))
                                invocationArgs.Add(value);
                            else
                                invocationArgs.Add(null);
                        }
                        else if (param.Value == typeof(ulong))
                        {
                            if (ulong.TryParse(match.Groups[param.Key].Value, out var value))
                                invocationArgs.Add(value);
                            else
                                invocationArgs.Add(null);
                        }
                        else if (param.Value == typeof(short))
                        {
                            if (short.TryParse(match.Groups[param.Key].Value, out var value))
                                invocationArgs.Add(value);
                            else
                                invocationArgs.Add(null);
                        }
                        else if (param.Value == typeof(ushort))
                        {
                            if (ushort.TryParse(match.Groups[param.Key].Value, out var value))
                                invocationArgs.Add(value);
                            else
                                invocationArgs.Add(null);
                        }
                        else if (param.Value == typeof(float))
                        {
                            if (float.TryParse(match.Groups[param.Key].Value, out var value))
                                invocationArgs.Add(value);
                            else
                                invocationArgs.Add(null);
                        }
                        else if (param.Value == typeof(decimal))
                        {
                            if (decimal.TryParse(match.Groups[param.Key].Value, out var value))
                                invocationArgs.Add(value);
                            else
                                invocationArgs.Add(null);
                        }
                        else if (param.Value == typeof(char[]))
                        {
                            invocationArgs.Add(match.Groups[param.Key].Value.ToCharArray());
                        }
                        else
                        {
                            invocationArgs.Add(null);
                        }
                    } // end if there's regex group matching the parameter name
                    else
                    {
                        if (param.Value == typeof(HttpContext))
                            invocationArgs.Add(context);
                        else if (param.Value == typeof(IContext))
                            invocationArgs.Add(context);
                        else
                            invocationArgs.Add(null);
                    }
                } // end foreach parameter

                _handler.Invoke(_instance, invocationArgs.ToArray());
            } // end if there's more than one parameter
            else
            {
                _handler.Invoke(_instance, new object[] {(HttpContext) context});
            }
        }
    }
}