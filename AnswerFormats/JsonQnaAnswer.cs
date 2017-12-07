//////////////////////////////////////////////////////////////////////////////////////////////////////
////                                 CLASE PRINCIPAL:QnaDialog.cs                                 ////
//// Proporciona los parametros que podra contener un paquete de respuesta de QnAMaker            ////
//// Autores: MIA G5                                                                              ////
//// Universidad Autonoma de Coahuila                                                             ////
//// Facultad de Sistemas                                                                         ////
//// Diciembre de 2017                                                                            ////
//////////////////////////////////////////////////////////////////////////////////////////////////////

/////////////////////////
// Paquetes a importar //
/////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MIA_Bot.AnswerFormats
{
    /////////////////////////
    //   Clase principal   //
    /////////////////////////
    public class JsonQnaAnswer
    {
        public string type { get; set; }
        public string title { get; set; }
        public string desc { get; set; }
        public string Url { get; set; }
        public string ContentType { get; set; }
        public string image { get; set; }
        public string text { get; set; }
        public string botonText { get; set; }

    }
}