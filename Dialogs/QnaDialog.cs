//////////////////////////////////////////////////////////////////////////////////////////////////////
////                                 CLASE PRINCIPAL:QnaDialog.cs                                 ////
//// Recibe una paquete desde una base de datos de QnAMaker.ai y si el paquete cumple con el      ////
//// formato de JsonQnaAnswer.cs, lo procesa para entregar una HeroCard, Respuesta multimedia     ////
//// o respuesta de solo texto.                                                                   ////
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
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Newtonsoft.Json.Linq;
using MIA_Bot.AnswerFormats;

namespace MIA_Bot.Dialogs
{
    [Serializable]
    /////////////////////////
    //   Clase principal   //
    /////////////////////////
    public class QnaDialog : QnAMakerDialog
    {
        // Establecemos conexion con QnaMaker y damos un mensaje por default 
        // en caso de que no exista una respuesta adecuada en la base de datos.
        public QnaDialog() : base(new QnAMakerService(new QnAMakerAttribute(ConfigurationManager.AppSettings["QnaSubscriptionKey"], ConfigurationManager.AppSettings["QnaKnowledgebaseId"], "Lo siento, no pude encontrar una respuesta adecuada", 0.5)))
        {
        }
        // Aqui se procesa el paquete de QnAMaker
        protected override async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {
            JsonQnaAnswer qnaAnswer = new JsonQnaAnswer(); // Un objeto JsonQnaAnswer para 
                                                           // dar formato a la respuesta
            var answer = result.Answers.First().Answer; // Tomamos el paquete de QnAMaker
            Activity reply = ((Activity)context.Activity).CreateReply(); // Creamos la respuesta que
                                                                         // vera el cliente
            var response = JObject.Parse(answer); // parsear el paquete de QnAMaker
            qnaAnswer.type= response.Value<string>("type"); // identificar el tipo de paquete
            switch (qnaAnswer.type)
            {
                case "attachment":
                    //Multimedia en general, puede ser una imagen, video o audio.
                    qnaAnswer.Url = response.Value<string>("url");
                    qnaAnswer.title = response.Value<string>("title");
                    qnaAnswer.ContentType = response.Value<string>("ContentType");
                    // Cargamos los datos a la respuesta que vera el cliente
                    reply.Attachments.Add(new Attachment()
                    {
                        ContentUrl = qnaAnswer.Url,
                        ContentType = qnaAnswer.ContentType,
                        Name = qnaAnswer.title
                    });
                    break;
                case "CustomVideoCard":
                    // Tarjeta que se utiliza para proveer un enlace a otra pagina
                    qnaAnswer.title = response.Value<string>("title");
                    qnaAnswer.botonText = response.Value<string>("botonText");
                    qnaAnswer.desc = response.Value<string>("desc");
                    qnaAnswer.Url = response.Value<string>("url");
                    qnaAnswer.image = response.Value<string>("image");
                    // Esto es para agregar la imagen a la tarjeta,
                    // tecnicamente se podria agregar mas de una imagen
                    // para formar un carrusel de imagenes
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: qnaAnswer.image));
                    HeroCard card = new HeroCard()
                    {
                        Title = qnaAnswer.title,
                        Subtitle = qnaAnswer.desc,
                        Images = cardImages,
                        Buttons = new List<CardAction>
            {
                new CardAction(ActionTypes.OpenUrl, qnaAnswer.botonText, value: qnaAnswer.Url)
            }// Aqui se creo el boton de la tarjeta
                    };
                    reply.Attachments.Add(card.ToAttachment());
                    break;
                case "text":
                    //Solo se pasa el texto simple a la respuesta para el cliente
                    qnaAnswer.text = response.Value<string>("text");
                    reply.Text = qnaAnswer.text;
                    break;
                default:
                    // En caso de que el paquete no tenga un formato adecuado, se muestra el sig mensaje
                    reply.Text = "Tipo de mensaje invalido, enviaste un: <" + qnaAnswer.type + ">";
                    break;
            }
            await context.PostAsync(reply); //Responder al cliente
        }
    }
}