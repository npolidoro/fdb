using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using FDB_BOT.Models;
using Microsoft.Bot.Builder.FormFlow;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System.Configuration;
using System.Collections.Generic;

namespace FDB_BOT.Dialogs
{

    [Serializable]
    public class RootDialog : IDialog<object>
    {

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        internal static IDialog<Insert> MakeRootDialog()
        {
            return Chain.From(() => FormDialog.FromForm(Insert.BuildForm,FormOptions.PromptInStart));
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            if (message.Text.Contains("insert")) {
                context.Call(MakeRootDialog(), EndInsertDialog);
            }
            else if (message.Text.Contains("lista"))
            {
                var reply = context.MakeMessage();

                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                reply.Attachments = GetCardsAttachments();

                await context.PostAsync(reply);

                context.Wait(this.MessageReceivedAsync);
            } 


        }

        private IList<Microsoft.Bot.Connector.Attachment> GetCardsAttachments()
        {

            InsertDB insertDB = new InsertDB();
            try
            {

            var list=insertDB.GetInsertList();
            List<Microsoft.Bot.Connector.Attachment> attachments = new List<Microsoft.Bot.Connector.Attachment>();
            foreach (var item in list)
            {
                attachments.Add(GetHeroCard(item));
            }
            return attachments;
            }
            catch (Exception e)
            {

                e.Message.ToString();return null;
            }
        }

        private static Microsoft.Bot.Connector.Attachment GetHeroCard(Insert insert)
        {

            var heroCard = new HeroCard
            {
                Title = Enum.GetName(typeof(Categoria),insert.categoria),
                Subtitle = insert.scoreDa1a10.ToString(),
                Text = "Luogo di avvistamento:"+insert.luogoAvvistamento+"\n"+
                            "Capelli:"+insert.capelli+"\n"+
                            "Altezza:"+insert.altezza+"\n"+
                            "Occhi:"+insert.occhi+"\n"+
                            "Data Avvistamento:"+insert.avvistamento.ToShortDateString()
            };

            if (insert.categoria.Equals(Categoria.Cougar))
            {
                heroCard.Images = new List<CardImage>() { new CardImage(url: "http://broadbandandsocialjustice.org/wp-content/uploads/2013/09/Woman-Silhouette-300x300.png") };
            }
            else if (insert.categoria.Equals(Categoria.Milf))
            {
                heroCard.Images = new List<CardImage>() { new CardImage(url: "http://artgonewild.com/media/catalog/product/cache/1/image/800x800/9df78eab33525d08d6e5fb8d27136e95/s/u/suzanne-carillo-j3-2503-silhouette-woman-with-bag.png") };
            }
            else if (insert.categoria.Equals(Categoria.Young))
            {
                heroCard.Images = new List<CardImage>() { new CardImage(url: "https://wallpaper-house.com/data/out/8/wallpaper2you_231895.jpg") };
            }

            return heroCard.ToAttachment();
        }

        private async Task EndInsertDialog(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("Grazie per la insert, "+context.Activity.From.Name);
        }

    }
}