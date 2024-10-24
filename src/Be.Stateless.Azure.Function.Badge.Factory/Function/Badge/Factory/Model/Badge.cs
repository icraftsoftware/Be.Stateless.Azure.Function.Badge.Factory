#region Copyright & License

// Copyright © 2012 - 2024 François Chabot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.IO;

namespace Be.Stateless.Azure.Function.Badge.Factory.Model;

public class Badge
{
	public Badge(string label, string message, string color, string style, string logo)
	{
		// see https://shields.io/
		var builder = new UriBuilder(Uri.UriSchemeHttps, "img.shields.io") {
			Path = "static/v1",
			Query = $"label={label}&message={message}&color={color}&style={style}&logo={logo}"
		};
		EntityTag = $"{label}:{message}:{color}:{style}:{logo}";
		Uri = builder.Uri;
	}

	public string ContentType => "image/svg+xml";

	public string EntityTag { get; }

	public Stream Stream { get; set; }

	public Uri Uri { get; }
}
