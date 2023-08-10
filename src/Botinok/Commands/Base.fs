module Botinok.Commands.Base

open Funogram.Telegram.Bot
open Funogram.Telegram.Types
open Botinok.Commands.Libgen
open Botinok.Commands.Help

let processCallbackCommand (ctx: UpdateContext)  =
    match ctx.Update.CallbackQuery.Value.Data.Value.Split('/') with
    | [| "book"; "download"; bookId |] -> downloadBook bookId ctx
    | [| "book"; "get"; bookId |] -> getBook bookId ctx
    | [| "next"; book; page |] -> getAnotherPages book page ctx
    | [| "prev"; book; page |] -> getAnotherPages book page ctx
    | _ -> iDunno

let wrap ctx fn =
    let fromId () =
        match ctx.Update.Message with
        | Some msg -> msg.From.Value.Id
        | None _ -> ctx.Update.CallbackQuery.Value.Message.Value.Chat.Id
    fn ctx.Config (fromId())

let updateArrived (ctx: UpdateContext) =
    match ctx.Update.CallbackQuery.IsSome with
    | true ->
        processCallbackCommand ctx |> wrap ctx
    | _ ->
        processCommands ctx [| 
            cmd "/start"  (fun _ -> help |> wrap ctx) 
            cmdScan "/search %s" (fun book _ -> searchBooks book ctx |> wrap ctx)
        |] |> ignore
