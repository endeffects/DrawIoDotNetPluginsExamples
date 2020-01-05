﻿using OpenApiDocumentSchemaPlugin.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace OpenApiDocumentSchemaPlugin.Model
{
    public class DiagramXmlBuilder : IDiagramXmlBuilder
    {
        private readonly XmlDocument _document;
        private readonly XmlElement _graphModel;
        private readonly XmlElement _root;
        private readonly string _docGuid;
        private int nextId = 1;

        private string GetNextId() => $"{_docGuid}_{nextId++}";


        public DiagramXmlBuilder(string diagramGuid)
            : this()
        {
            _docGuid = diagramGuid;
        }

        public DiagramXmlBuilder()
        {
            _docGuid = Guid.NewGuid().ToString();
            _document = new XmlDocument();
            _graphModel = _document.CreateElement("mxGraphModel");
            _graphModel.SetAttribute("grid", "1");
            _graphModel.SetAttribute("gridSize", "10");
            _graphModel.SetAttribute("guides", "1");
            _graphModel.SetAttribute("tooltips", "1");
            _graphModel.SetAttribute("connect", "1");
            _graphModel.SetAttribute("arrows", "1");
            _graphModel.SetAttribute("fold", "1");
            _graphModel.SetAttribute("page", "1");
            _graphModel.SetAttribute("pageScale", "1");
            _graphModel.SetAttribute("math", "0");
            _graphModel.SetAttribute("shadow", "0");

            _document.AppendChild(_graphModel);
            _root = _document.CreateElement("root");
            _graphModel.AppendChild(_root);

            AddMxCell(new Dictionary<string, string> {
                { "id", "0" }
            });

            AddMxCell(new Dictionary<string, string> {
                { "id", "1" },
                { "parent", "0" },
            });
        }

        public string AddVertex(string value)
        {
            var id = GetNextId();
            AddMxCell(new Dictionary<string, string> {
                { "id", id },
                { "vertex", "1" },
                { "parent", "1" },
                { "style", "rounded=0;whiteSpace=wrap;html=1;" }
            }
            .AddIfNotNull("value", value), 120, 60);

            return id;
        }

        public string AddEdge(string value, string sourceId, string targetId)
        {
            var id = GetNextId();

            var cell = AddMxCell(new Dictionary<string, string> {
                { "id", id },
                { "edge", "1" },
                { "parent", "1" },
                { "style", "edgeStyle=orthogonalEdgeStyle;rounded=0;orthogonalLoop=1;jettySize=auto;html=1;" }
            }
            .AddIfNotNull("value", value)
            .AddIfNotNull("target", targetId)
            .AddIfNotNull("source", sourceId));

            cell.AppendChild(CreateRelativeGeometry());

            return id;
        }

        private XmlElement AddMxCell(Dictionary<string, string> attributes)
        {
            var cell = _document.CreateElement("mxCell");
            foreach (var key in attributes)
            {
                cell.SetAttribute(key.Key, key.Value);
            }

            _root.AppendChild(cell);

            return cell;
        }

        private XmlElement AddMxCell(Dictionary<string, string> attributes, int width, int height)
        {
            var cell = AddMxCell(attributes);
            cell.AppendChild(CreateGeometry(width, height));

            return cell;
        }

        private XmlElement CreateGeometry(int width, int height)
        {
            var geometry = _document.CreateElement("mxGeometry");
            geometry.SetAttribute("width", width.ToString());
            geometry.SetAttribute("height", height.ToString());
            geometry.SetAttribute("as", "geometry");
            return geometry;
        }

        private XmlElement CreateRelativeGeometry()
        {
            var geometry = _document.CreateElement("mxGeometry");
            geometry.SetAttribute("relative", "1");
            geometry.SetAttribute("as", "geometry");
            return geometry;
        }

        public string GetDiagramXml()
        {
            return _document.InnerXml;
        }
    }
}
