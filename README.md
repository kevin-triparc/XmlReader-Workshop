# XmlReader

Este proyecto es un lector de XML basado en el proyecto [BaseXml](https://github.com/canro91/BaseXml) de [canro91](https://github.com/canro91).

XmlReader permite:
- Leer y manipular documentos XML
- Evaluar expresiones XPath
- Agregar o modificar nodos y atributos
- Validar documentos XML

## Características

- Interfaz sencilla para trabajar con archivos XML
- Soporte para namespaces
- Manipulación de documentos XML
- Métodos para evaluar expresiones XPath
- Capacidad para agregar o modificar nodos y atributos

## Uso

```csharp
using XmlReader;

// Crear un documento XML
var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<nota>
  <de>Bob</de>
  <para>Alice</para>
  <asunto>Recordatorio</asunto>
  <cuerpo>¡No olvides nuestra reunión mañana!</cuerpo>
</nota>";

var documento = new XmlDocumento(xml);

// Leer valores usando XPath
string de = documento.Evaluar(new XPath("/nota/de"));
string para = documento.Evaluar(new XPath("/nota/para"));
string asunto = documento.Evaluar(new XPath("/nota/asunto"));
string cuerpo = documento.Evaluar(new XPath("/nota/cuerpo"));

// Modificar el documento
documento.AgregarNodoHijo("<ps>¡Es importante!</ps>", new XPath("/nota"));

// Obtener el XML modificado
string xmlModificado = documento.Xml;
```

## Licencia

Este proyecto está licenciado bajo la [Licencia MIT](LICENSE).

## Agradecimientos

Este proyecto está inspirado en [BaseXml](https://github.com/canro91/BaseXml) de [canro91](https://github.com/canro91).