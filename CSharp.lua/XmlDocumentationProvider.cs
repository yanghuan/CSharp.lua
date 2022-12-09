/*
Copyright 2017 YANG Huan (sy.yanghuan@gmail.com).

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

using Microsoft.CodeAnalysis;

namespace CSharpLua {
  internal class XmlDocumentationProvider : DocumentationProvider {
    private string path_;
    private Dictionary<string, string> comments_;

    internal XmlDocumentationProvider(string path) {
      path_ = path;
      Load();
    }

    private void Load() {
      comments_ = XElement.Load(path_).Descendants("member").Select(e => {
        var name = e.Attribute("name");
        var comment = e.Element("summary")?.Value;
        return new { name, comment };
      }).Where(i => i.name != null && !string.IsNullOrEmpty(i.comment)).ToDictionary(i => i.name.Value, i => i.comment);
    }

    public override bool Equals(object obj) {
      if (obj is XmlDocumentationProvider other && path_ == other.path_) {
        return true;
      }
      return false;
    }

    public override int GetHashCode() {
      return path_.GetHashCode();
    }

    protected override string GetDocumentationForSymbol(string documentationMemberID, CultureInfo preferredCulture, CancellationToken cancellationToken = default) {
      return comments_.GetOrDefault(documentationMemberID);
    }
  }
}
