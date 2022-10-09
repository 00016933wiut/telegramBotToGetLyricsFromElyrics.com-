using HtmlAgilityPack;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
string htmlLink = "";
string lyricsText = "";
var botClient = new TelegramBotClient(YOUR TOKEN);
using var cts = new CancellationTokenSource();
var receiverOptions = new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() };
botClient.StartReceiving(updateHandler: HandleUpdateAsync, pollingErrorHandler: HandlePollingErrorAsync, receiverOptions: receiverOptions, cancellationToken: cts.Token);
var me = await botClient.GetMeAsync();
GetHTML();
Console.ReadLine();
cts.Cancel();
async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Message is not { } message) return;
    if (message.Text is not { } messageText) return;
    //user sending link to lyrics from elyrics.com
    htmlLink = message.Text;
    Message showLyrics = await botClient.SendTextMessageAsync(message.Chat.Id, text: lyricsText, cancellationToken: cancellationToken);
}
async void GetHTML()
{
    var httpClient = new HttpClient();
    var html = await httpClient.GetStringAsync(htmlLink);
    var htmldoc = new HtmlDocument();
    htmldoc.LoadHtml(html);
    var lyricsList = htmldoc.DocumentNode.Descendants("div").Where(node => node.GetAttributeValue("class", "").Equals("row text-center")).ToList();
    var lyrics = lyricsList[0].Descendants().Where(node => node.GetAttributeValue("class", "").Equals("col-xl-6 inner_right p-0 mb-2")).ToList();
    foreach (var lyricsItem in lyricsList) { lyricsText = lyricsItem.Descendants().FirstOrDefault(node => node.GetAttributeValue("id", "").Equals("inlyr")).InnerText; }
}
Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch { ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}", _ => exception.ToString() };
    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}