namespace XmlReader
{
    /// <summary>
    /// Clase que representa un atributo XML con su nodo asociado.
    /// </summary>
    public class XAtributo
    {
        /// <summary>
        /// Constructor que crea un nuevo atributo XML.
        /// </summary>
        /// <param name="nodo">Expresión XPath del nodo que contiene el atributo</param>
        /// <param name="nombreAtributo">Nombre del atributo</param>
        public XAtributo(XPath nodo, string nombreAtributo)
        {
            NodoXPath = nodo.Expresion;
            NombreAtributo = nombreAtributo;
        }

        /// <summary>
        /// Obtiene o establece la expresión XPath del nodo que contiene el atributo.
        /// </summary>
        public string NodoXPath { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del atributo.
        /// </summary>
        public string NombreAtributo { get; set; }

        /// <summary>
        /// Convierte este atributo en una expresión XPath.
        /// </summary>
        /// <returns>Expresión XPath que apunta al atributo</returns>
        public XPath ToXPath()
        {
            return new XPath($"{NodoXPath}/@{NombreAtributo}");
        }
    }
}