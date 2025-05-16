using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace XmlReader
{
    /// <summary>
    /// Clase que representa una expresión XPath para consultar documentos XML.
    /// </summary>
    public class XPath
    {
        /// <summary>
        /// Constructor que crea una nueva expresión XPath.
        /// </summary>
        /// <param name="expresion">Expresión XPath</param>
        public XPath(string expresion)
        {
            Expresion = expresion;
        }

        /// <summary>
        /// Obtiene o establece la expresión XPath.
        /// </summary>
        public string Expresion { get; internal set; }

        /// <summary>
        /// Concatena otra expresión XPath a esta expresión.
        /// </summary>
        /// <param name="expresion">Expresión XPath a concatenar</param>
        /// <returns>Nueva instancia de XPath con la expresión concatenada</returns>
        public XPath Concatenar(string expresion)
        {
            return new XPath($"{Expresion}/{expresion}");
        }

        /// <summary>
        /// Convierte los nombres de nodos en la expresión XPath a camelCase.
        /// </summary>
        /// <returns>Nueva instancia de XPath con los nombres en camelCase</returns>
        public XPath ACamelCase()
        {
            var camelCase = new List<string>();

            var splitted = Expresion.Split('/');
            foreach (var node in splitted)
            {
                if (string.IsNullOrEmpty(node))
                {
                    camelCase.Add(string.Empty);
                    continue;
                }

                var dotted = node.Split(':');
                if (dotted.Length == 1)
                {
                    var nodeName = dotted[0];
                    var camelCaseNodeName = nodeName.StartsWith("@")
                                            ? $"@{ConvertirACamelCase(nodeName.Substring(1))}"
                                            : ConvertirACamelCase(nodeName);
                    camelCase.Add(camelCaseNodeName);
                }
                else
                {
                    var ns = dotted[0];
                    var nodeName = dotted[1];
                    camelCase.Add($"{ns}:{ConvertirACamelCase(nodeName)}");
                }
            }

            var expresion = string.Join("/", camelCase);
            return new XPath(expresion);
        }

        /// <summary>
        /// Convierte una cadena a formato camelCase.
        /// </summary>
        /// <param name="texto">Texto a convertir</param>
        /// <returns>Texto en formato camelCase</returns>
        private static string ConvertirACamelCase(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return texto;
            
            // Primero convertimos todo a minúsculas
            string lowerCase = texto.ToLower();
            
            // Usamos expresión regular para encontrar palabras y convertir la primera letra a mayúscula
            string camelCase = Regex.Replace(lowerCase, @"(^|\s|_|-|\.)[a-z]", match => match.Value.ToUpper());
            
            // Eliminamos todos los espacios, guiones y subrayados
            camelCase = Regex.Replace(camelCase, @"[\s_\-\.]", "");
            
            // Asegurarnos que la primera letra sea minúscula
            if (camelCase.Length > 0)
            {
                camelCase = char.ToLower(camelCase[0]) + camelCase.Substring(1);
            }
            
            return camelCase;
        }
    }
}