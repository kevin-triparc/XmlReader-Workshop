namespace XmlReader
{
    /// <summary>
    /// Interfaz que define la capacidad de evaluar expresiones XPath.
    /// </summary>
    public interface IEvaluable
    {
        /// <summary>
        /// Evalúa una expresión XPath y devuelve el resultado convertido al tipo especificado.
        /// </summary>
        /// <typeparam name="T">Tipo de dato de retorno</typeparam>
        /// <param name="xPath">Expresión XPath a evaluar</param>
        /// <param name="default">Valor por defecto si la evaluación falla</param>
        /// <returns>Resultado de la evaluación convertido al tipo especificado</returns>
        T Evaluar<T>(XPath xPath, T @default = default);
    }
}