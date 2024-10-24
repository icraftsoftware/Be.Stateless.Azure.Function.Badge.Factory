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

using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Be.Stateless.Azure.Function.Badge.Factory.Model;

/// <summary>Package Artifact Details.</summary>
/// <seealso href="https://docs.microsoft.com/en-us/rest/api/azure/devops/artifacts/artifact-details/get-packages#package">Package</seealso>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class PackageArtifact
{
	public PackageArtifactVersion LatestVersion => Versions.Single(v => v.IsLatest);

	public string Name { get; init; }

	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	public PackageArtifactVersion[] Versions { get; init; }
}
