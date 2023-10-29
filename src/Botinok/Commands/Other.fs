module Botinok.Commands.Help

open Funogram.Telegram
open Botinok.Core

let private text = """Доступные команды:
/search <book name> - поиск книжек
"""

let help config (chatId: int64) =
    Req.SendMessage.Make(chatId, text) |> bot config
    
let iDunno config (chatId: int64) =
    Req.SendMessage.Make(chatId, "что то пошло не так((") |> bot config