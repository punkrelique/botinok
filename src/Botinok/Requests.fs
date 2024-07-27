module Botinok.Requests

open Funogram.Telegram
open Funogram.Telegram.Bot
open Funogram.Telegram.Types

let private getMessageId (ctx: UpdateContext) =
        match ctx.Update.Message with
        | Some msg -> msg.MessageId
        | _ ->
            match ctx.Update.CallbackQuery.Value.Message.Value with
            | Message m -> m.MessageId
            | InaccessibleMessage im -> im.MessageId

let sendReply (chatId: int64) msg ctx =
    Req.SendMessage.Make(chatId, msg, replyParameters = ReplyParameters.Create(messageId = getMessageId ctx))
    
let sendReplyMarkup (chatId: int64) msg ctx markup =
    Req.SendMessage.Make(chatId, msg, replyParameters = ReplyParameters.Create(messageId = getMessageId ctx), replyMarkup = markup)
    
let editMessageReplyMarkup (chatId: int64) (ctx: UpdateContext) markup =
    Req.EditMessageReplyMarkup.Make(chatId = ChatId.Int chatId, messageId = getMessageId ctx, replyMarkup = markup)
    
let sendDocument (chatId: int64) filename file ctx =
    Req.SendDocument.Make(chatId, InputFile.File(filename, file), replyParameters = ReplyParameters.Create(messageId = getMessageId ctx))