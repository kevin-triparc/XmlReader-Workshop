using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace XmlReader
{
    /// <summary>
    /// Clase principal para trabajar con documentos XML.
    /// Permite evaluar expresiones XPath, agregar o modificar nodos y validar documentos XML.
    /// </summary>
    public class XmlDocumento : IEvaluable
    {
        protected XmlDocument _xmlDocument;
        protected XmlNamespaceManager _xmlNamespaceManager;
        protected XPathNavigator _navigator;

        /// <summary>
        /// Constructor que crea un documento XML a partir de una cadena XML.
        /// </summary>
        /// <param name="xml">Contenido XML</param>
        public XmlDocumento(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            _xmlDocument = doc;

            XPathNavigator navigator = _xmlDocument.CreateNavigator();
            XmlNamespaceManager ns = new XmlNamespaceManager(navigator.NameTable);
            foreach (var item in XmlNamespaces)
            {
                ns.AddNamespace(item.Prefijo, item.Uri);
            }

            _xmlNamespaceManager = ns;
            _navigator = navigator;
        }

        /// <summary>
        /// Obtiene o establece la lista de espacios de nombres XML.
        /// </summary>
        public virtual IEnumerable<XmlEspacioNombres> XmlNamespaces { get; }
            = Enumerable.Empty<XmlEspacioNombres>();

        /// <summary>
        /// Obtiene el contenido XML del documento.
        /// </summary>
        public string Xml
        {
            get
            {
                using (StringWriter sw = new UTF8StringWriter())
                {
                    _xmlDocument.Save(sw);
                    return sw.ToString();
                }
            }
        }

        /// <summary>
        /// Obtiene el nombre del elemento raíz del documento XML.
        /// </summary>
        public string Raiz
        {
            get => _xmlDocument.DocumentElement.Name;
        }

        /// <summary>
        /// Evalúa una expresión XPath y devuelve el resultado como cadena.
        /// </summary>
        /// <param name="xPath">Expresión XPath a evaluar</param>
        /// <returns>Resultado de la evaluación como cadena</returns>
        public string Evaluar(XPath xPath)
        {
            return Evaluar<string>(xPath);
        }

        /// <summary>
        /// Evalúa un atributo XML y devuelve el resultado como cadena.
        /// </summary>
        /// <param name="xAttribute">Atributo XML a evaluar</param>
        /// <returns>Resultado de la evaluación como cadena</returns>
        public string Evaluar(XAtributo xAttribute)
        {
            return Evaluar<string>(xAttribute.ToXPath());
        }

        /// <summary>
        /// Evalúa una expresión XPath y devuelve el resultado convertido al tipo especificado.
        /// </summary>
        /// <typeparam name="T">Tipo de dato de retorno</typeparam>
        /// <param name="xPath">Expresión XPath a evaluar</param>
        /// <param name="default">Valor por defecto si la evaluación falla</param>
        /// <returns>Resultado de la evaluación convertido al tipo especificado</returns>
        public T Evaluar<T>(XPath xPath, T @default = default)
        {
            var result = _navigator.Evaluate(xPath.Expresion, _xmlNamespaceManager);
            var obj = Convertidor.ObtenerValor(result);
            var converted = Convertidor.CambiarTipo(obj, typeof(T));
            return converted != null ? (T)converted : @default;
        }

        /// <summary>
        /// Evalúa múltiples nodos que coinciden con una expresión XPath.
        /// </summary>
        /// <typeparam name="T">Tipo de dato de retorno</typeparam>
        /// <param name="xPath">Expresión XPath a evaluar</param>
        /// <returns>Colección de resultados convertidos al tipo especificado</returns>
        public IEnumerable<T> EvaluarMultiple<T>(XPath xPath)
        {
            var result = _navigator.Evaluate(xPath.Expresion, _xmlNamespaceManager);
            var obj = Convertidor.ObtenerDeValoresMultiples(result);
            return obj?.Select(t => (T)Convertidor.CambiarTipo(t, typeof(T)));
        }

        /// <summary>
        /// Agrega nodos hijos al primer nodo que coincida con alguna de las expresiones XPath proporcionadas.
        /// </summary>
        /// <param name="xml">Contenido XML a agregar</param>
        /// <param name="xPaths">Expresiones XPath donde se agregarán los nodos</param>
        public void AgregarNodoHijo(string xml, params XPath[] xPaths)
        {
            var allNamespaces = string.Join(" ", this.XmlNamespaces.Select(t => $"xmlns:{t.Prefijo}=\"{t.Uri}\""));
            var dummyRoot = $@"<root {allNamespaces}>
    {xml}
</root>";
            var temp = new XmlDocument();
            temp.LoadXml(dummyRoot);

            XmlNode parentNode = xPaths.Select(t => _xmlDocument.SelectSingleNode(t.Expresion, _xmlNamespaceManager))
                                       .FirstOrDefault(t => t != null);
            if (parentNode != null)
            {
                foreach (XmlNode item in temp.DocumentElement.ChildNodes)
                {
                    XmlNode newNode = parentNode.OwnerDocument.ImportNode(item, true);
                    parentNode.AppendChild(newNode);
                }
            }
        }

        /// <summary>
        /// Agrega nodos hermanos después del último nodo que coincida con alguna de las expresiones XPath proporcionadas.
        /// </summary>
        /// <param name="xml">Contenido XML a agregar</param>
        /// <param name="xPaths">Expresiones XPath de referencia</param>
        public void AgregarNodoHermanoDespues(string xml, params XPath[] xPaths)
        {
            var allNamespaces = string.Join(" ", this.XmlNamespaces.Select(t => $"xmlns:{t.Prefijo}=\"{t.Uri}\""));
            var dummyRoot = $@"<root {allNamespaces}>
    {xml}
</root>";
            var temp = new XmlDocument();
            temp.LoadXml(dummyRoot);

            XmlNode parentNode = xPaths.Select(t => _xmlDocument.SelectNodes(t.Expresion, _xmlNamespaceManager)
                                                                .Cast<XmlNode>()
                                                                .LastOrDefault())
                                       .FirstOrDefault(t => t != null);
            if (parentNode != null)
            {
                XmlNode newNode = parentNode.OwnerDocument.ImportNode(temp.DocumentElement.FirstChild, true);
                parentNode.ParentNode.InsertAfter(newNode, parentNode);
            }
        }

        /// <summary>
        /// Agrega nodos hermanos antes del primer nodo que coincida con alguna de las expresiones XPath proporcionadas.
        /// </summary>
        /// <param name="xml">Contenido XML a agregar</param>
        /// <param name="xPaths">Expresiones XPath de referencia</param>
        public void AgregarNodoHermanoAntes(string xml, params XPath[] xPaths)
        {
            var allNamespaces = string.Join(" ", this.XmlNamespaces.Select(t => $"xmlns:{t.Prefijo}=\"{t.Uri}\""));
            var dummyRoot = $@"<root {allNamespaces}>
    {xml}
</root>";
            var temp = new XmlDocument();
            temp.LoadXml(dummyRoot);

            XmlNode parentNode = xPaths.Select(t => _xmlDocument.SelectSingleNode(t.Expresion, _xmlNamespaceManager))
                                       .FirstOrDefault(t => t != null);
            if (parentNode != null)
            {
                XmlNode newNode = parentNode.OwnerDocument.ImportNode(temp.DocumentElement.FirstChild, true);
                parentNode.ParentNode.InsertBefore(newNode, parentNode);
            }
        }

        /// <summary>
        /// Cambia el valor de un nodo que coincida con la expresión XPath proporcionada.
        /// </summary>
        /// <param name="xPath">Expresión XPath del nodo a modificar</param>
        /// <param name="value">Nuevo valor para el nodo</param>
        public void CambiarValorNodo(XPath xPath, string value)
        {
            XmlNode node = _xmlDocument.SelectSingleNode(xPath.Expresion, _xmlNamespaceManager);
            if (node != null)
            {
                node.InnerText = value;
            }
        }

        /// <summary>
        /// Agrega o cambia el valor de un atributo en el nodo que coincida con la expresión XPath proporcionada.
        /// </summary>
        /// <param name="attribute">Atributo a agregar o modificar</param>
        /// <param name="value">Nuevo valor para el atributo</param>
        public void AgregarOCambiarValorAtributo(XAtributo attribute, string value)
        {
            XmlNode node = _xmlDocument.SelectSingleNode(attribute.NodoXPath, _xmlNamespaceManager);
            if (node.Attributes[attribute.NombreAtributo] == null)
            {
                var attr = _xmlDocument.CreateAttribute(attribute.NombreAtributo);
                attr.InnerText = value;
                node.Attributes.Append(attr);
            }
            else
            {
                node.Attributes[attribute.NombreAtributo].Value = value;
            }
        }

        /// <summary>
        /// Cambia el valor de un atributo existente en el nodo que coincida con la expresión XPath proporcionada.
        /// </summary>
        /// <param name="attribute">Atributo a modificar</param>
        /// <param name="value">Nuevo valor para el atributo</param>
        public void CambiarValorAtributo(XAtributo attribute, string value)
        {
            XmlNode node = _xmlDocument.SelectSingleNode(attribute.NodoXPath, _xmlNamespaceManager);
            var attr = node.Attributes[attribute.NombreAtributo];
            if (attr != null)
            {
                attr.Value = value;
            }
        }
    }

    /// <summary>
    /// Clase auxiliar para manejar la codificación UTF-8 en la escritura de cadenas XML.
    /// </summary>
    public class UTF8StringWriter : StringWriter
    {
        public UTF8StringWriter() { }
        public UTF8StringWriter(IFormatProvider formatProvider) : base(formatProvider) { }
        public UTF8StringWriter(StringBuilder sb) : base(sb) { }
        public UTF8StringWriter(StringBuilder sb, IFormatProvider formatProvider) : base(sb, formatProvider) { }

        public override Encoding Encoding { get => Encoding.UTF8; }
    }
}