using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Web;

namespace TopicosXML
{
    public partial class Form1 : Form
    {
        XmlDocument xmlDecoratorDoc = null;
        XmlDocument xmlFuenteDoc = null;
        XmlDocument xmlResultadoDoc = null;
        string pathXmlDecorator = "";
        string pathXmlFuente = "";

        public Form1()
        {
            InitializeComponent();
            xmlDecoratorDoc = new XmlDocument();
            xmlFuenteDoc = new XmlDocument();
            xmlResultadoDoc = new XmlDocument();
        }

        private void btnCargarDecorador_Click(object sender, EventArgs e)
        {
            cargarDecorador();
        }

        private void btnCargarFuente_Click(object sender, EventArgs e)
        {
            cargarFuente();
        }

        private void btnProcesar_Click(object sender, EventArgs e)
        {
            if (rtxtDecorador.Text.Equals(""))
            {
                MessageBox.Show("Ingrese Decorador");
            }
            else if (rtxtFuente.Text.Equals(""))
            {
                MessageBox.Show("Ingrese Fuente");
            }
            else
            {
                validadorAccion();
            }
        }

        private void validadorAccion()
        {
            XPathDocument document = new XPathDocument(pathXmlDecorator);
            XPathNavigator navigator = document.CreateNavigator();
            XmlNamespaceManager nameSpaceDecorator = new XmlNamespaceManager(navigator.NameTable);
            nameSpaceDecorator.AddNamespace("cora", "http://www.xmlstairways.com/cora");

            XmlNodeList listaNodoDecorator = xmlDecoratorDoc.SelectNodes("/cora:set/cora:decorator", nameSpaceDecorator);
            IEnumerator iEnumDecorator = listaNodoDecorator.GetEnumerator();

            while (iEnumDecorator.MoveNext())
            {
                XmlNode nodoDecorator = (XmlNode)iEnumDecorator.Current;
                XmlElement elemDecorator = (XmlElement)nodoDecorator;
                string atributoRef = (elemDecorator.GetAttribute("ref"));

                XmlNodeList hijosDecorador = nodoDecorator.ChildNodes;
                IEnumerator iEnumHijosDecorador = hijosDecorador.GetEnumerator();

                while (iEnumHijosDecorador.MoveNext())
                {
                    XmlNode nodoHijo = (XmlNode)iEnumHijosDecorador.Current;
                    string tipoModificacion = nodoHijo.LocalName;

                    XmlNodeList nietosDecorador = nodoHijo.ChildNodes;
                    IEnumerator iEnumNientosDecorador = nietosDecorador.GetEnumerator();

                    while (iEnumNientosDecorador.MoveNext())
                    {
                        XmlNode nodoNieto = (XmlNode)iEnumNientosDecorador.Current;

                        string elem = nodoNieto.Name;
                        string ContElem = nodoNieto.InnerText;

                        XmlElement newElement = xmlFuenteDoc.CreateElement(elem);
                        newElement.InnerText = ContElem;

                        XmlNode nodeFuente = xmlFuenteDoc.SelectSingleNode(atributoRef);
                        XmlNode nodoPadre = xmlFuenteDoc.SelectSingleNode(atributoRef + "/..");

                        if (tipoModificacion.Equals("append"))
                        {
                            nodeFuente.AppendChild(newElement);
                        }
                        if (tipoModificacion.Equals("insert"))
                        {
                            nodoPadre.InsertAfter(newElement, nodeFuente.FirstChild);
                        }
                        if (tipoModificacion.Equals("replace"))
                        {
                            nodoPadre.ReplaceChild(newElement, nodeFuente);
                        }
                    }
                }
            }

            //Guardo el resultado y lo muestro!
            string resul = "newDoc.xml";
            xmlFuenteDoc.Save(resul);
            xmlResultadoDoc.Load(resul);
            rtxtResultado.LoadFile(resul, RichTextBoxStreamType.PlainText);
        }

        public void cargarDecorador()
        {
            ofdXMLSelector.Title = "Buscar Archivo XML DECORATOR";
            ofdXMLSelector.Filter = "Archivos XML | decorator.xml";
            if (ofdXMLSelector.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pathXmlDecorator = ofdXMLSelector.FileName;
                rtxtDecorador.LoadFile(pathXmlDecorator, RichTextBoxStreamType.PlainText);
                xmlDecoratorDoc.Load(pathXmlDecorator);
            }
        }

        public void cargarFuente() 
        {
            ofdXMLSelector.Title = "Buscar Archivo XML SOURCE";
            ofdXMLSelector.Filter = "Archivos XML | source.xml";
            if (ofdXMLSelector.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pathXmlFuente = ofdXMLSelector.FileName;
                rtxtFuente.LoadFile(pathXmlFuente, RichTextBoxStreamType.PlainText);
                xmlFuenteDoc.Load(pathXmlFuente);
            } 
        }
    }
}