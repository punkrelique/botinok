module Botinok.Commands.Libgen

open Botinok.Libgen.Types
open Botinok.LibgenClient
open Funogram.Telegram
open Funogram.Telegram.Bot
open Funogram.Telegram.Types
open Botinok.Core

let private bookListMarkup (books: Book seq) book page =
    let page = int page
    let startIndex = (page - 1) * 25

    let bookButtons =
        books
        |> Seq.toArray
        |> Array.mapi (fun i book ->
            [| InlineKeyboardButton.Create(
                   $"{startIndex + i + 1}. {book.Name} | {book.Year}y | {book.Extension} | {book.Size}",
                   callbackData = $"book/get/{book.Id}"
               ) |])

    let navButtons =
        [| [| if page <> 1 then
                  InlineKeyboardButton.Create(
                      "пред",
                      callbackData = $"prev/{book}/{if page <> 1 then page - 1 else page}")
              else ()
              InlineKeyboardButton.Create("след", callbackData = $"next/{book}/{page + 1}") |] |]

    let keyboard = Array.append bookButtons navButtons

    keyboard
    
let private bookInfoTemplate (book: Book) =
    let markup = Markup.InlineKeyboardMarkup { InlineKeyboard = [|
        [| 
            InlineKeyboardButton.Create("скачать", callbackData = $"book/download/{book.Id}") 
        |]
    |] }

    (book.ToString(), markup)
        
let private callbackMessageId (ctx: UpdateContext) = ctx.Update.CallbackQuery.Value.Message.Value.MessageId
let private commandMessageId (ctx: UpdateContext) = ctx.Update.Message.Value.MessageId

let private libgenClient = LibgenClient()

let searchBooks (book: string) (ctx: UpdateContext) config (chatId: int64) =
    if book.Length < 3 then Req.SendMessage.Make(chatId, "длина запроса должна быть больше 2 символов", replyToMessageId = commandMessageId ctx) |> bot config
    
    let booksResult =
        libgenClient.Search(SearchQuery.createDefaultQuery(book, 1))
        |> Async.AwaitTask
        |> Async.RunSynchronously

    match booksResult with
    | Ok books ->
        Req.SendMessage.Make(
            chatId,
            $"\"{book}\"",
            replyMarkup = Markup.InlineKeyboardMarkup { InlineKeyboard = bookListMarkup books book 1 },
            replyToMessageId = commandMessageId ctx
        )
        |> bot config
    | Error error ->
        Req.SendMessage.Make(chatId, error, replyToMessageId = commandMessageId ctx)
        |> bot config

let downloadBook id (ctx: UpdateContext) config (chatId: int64) =
    let bookResult =
        libgenClient.DownloadBook id |> Async.AwaitTask |> Async.RunSynchronously

    match bookResult with
    | Ok(file, name) ->
        Req.SendDocument.Make(chatId, InputFile.File(name, file), replyToMessageId = callbackMessageId ctx)
        |> bot config
    | Error error ->
        Req.SendMessage.Make(chatId, error, replyToMessageId = callbackMessageId ctx)
        |> bot config
    
let getBook id (ctx: UpdateContext) config (chatId: int64) =
    let bookResult =
        libgenClient.GetBook id |> Async.AwaitTask |> Async.RunSynchronously

    match bookResult with
    | Ok book ->
        let info, markup = bookInfoTemplate book
        Req.SendMessage.Make(chatId, info, replyMarkup = markup, replyToMessageId = callbackMessageId ctx)
        |> bot config
    | Error error ->
        Req.SendMessage.Make(chatId, error, replyToMessageId = callbackMessageId ctx)
        |> bot config
        
let getAnotherPages book (page: string) (ctx: UpdateContext) config (chatId: int64) =
    let page = int page

    let booksResult =
        libgenClient.Search(SearchQuery.createDefaultQuery(book, page))
        |> Async.AwaitTask
        |> Async.RunSynchronously

    match booksResult with
    | Ok books ->
        Req.EditMessageReplyMarkup.Make(
            chatId = chatId,
            messageId = callbackMessageId ctx,
            replyMarkup = InlineKeyboardMarkup.Create(bookListMarkup books book page)
        )
        |> bot config
    | Error error ->
        Req.SendMessage.Make(chatId = chatId, text = error, replyToMessageId = callbackMessageId ctx)
        |> bot config