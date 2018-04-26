using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;
using todo.Models;
using System.Configuration;
using Microsoft.Azure.Documents.Client;

namespace FDB_BOT.Models
{

    [Serializable]
    public class Item
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "isComplete")]
        public bool Completed { get; set; }

        [JsonProperty(PropertyName = "insert")]
        public Insert insert { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string type = "INSERT";


    }

    public enum Categoria
{
        [Prompt("Young")]
        Young,
        [Prompt("Milf")]
        Milf,
        [Prompt("Cougar")]
        Cougar
}

[Serializable]
public class Insert
{
        
        [JsonProperty(PropertyName = "data_avvistamento")]
        [Prompt("inserisci la data di avvistamento")]
        public DateTime avvistamento { get; set; }

        [JsonProperty(PropertyName = "categoria")]
        [Prompt("inserisci la categoria.{||}")]
        public Categoria? categoria { get; set; }

        [JsonProperty(PropertyName = "capelli")]
        [Prompt("descrivi i capelli")]
        public string capelli { get; set; }

        [JsonProperty(PropertyName = "occhi")]
        [Prompt("inserisci il colore degli occhi")]
        public string occhi { get; set; }

        [JsonProperty(PropertyName = "altezza")]
        [Prompt("inserisci l'altezza")]
        public string altezza { get; set; }

        [JsonProperty(PropertyName = "luogo_avvistamento")]
        [Prompt("inserisci il luogo di avvistamento")]
        public string luogoAvvistamento { get; set; }

        [JsonProperty(PropertyName = "score")]
        [Prompt("inserisci uno score da 1 a 10")]
        [Numeric(1,10)]
        public int? scoreDa1a10 { get; set; }

        public static IForm<Insert> BuildForm()
        {
            return new FormBuilder<Insert>()
                .Message("Insert! Procediamo!")
                .OnCompletion(async (context, insertForm) => {
                    await SalvaDati(context,insertForm);
            }).Build();
        }

        private async static Task SalvaDati(IDialogContext context, Insert insertForm)
        {
            InsertDB insertDB = new InsertDB();
             try
            {
                insertDB.CreateInsert(insertForm);
                await context.PostAsync("insert comletata.");

            }
            catch (Exception e)
            {
                await context.PostAsync(e.Message);

            }
        }
    }
}